using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Saves;

namespace AscensionAdjuster.Scripts;

/// <summary>
/// Harmony patches for the Ascension Adjuster mod.
///
/// The game enforces ascension limits in several places:
///
/// 1. StartRunLobby.SetSingleplayerAscensionAfterCharacterChanged()
///    - Reads CharacterStats.MaxAscension to set the lobby's MaxAscension
///    - This controls what the NAscensionPanel UI allows you to select
///
/// 2. NCharacterSelectScreen.StartNewSingleplayerRun()
///    - Clamps ascensionToEmbark to Math.Min(maxAscension, ascensionToEmbark)
///    - Only for RandomCharacter (since the selected character may differ)
///
/// 3. ProgressState.ClampCharacterStatsFields()
///    - Clamps MaxAscension and PreferredAscension to [0, 10] on save load
///
/// Our strategy:
/// - Patch CharacterStats.MaxAscension getter to return the overridden value.
///   This propagates through all code paths that read from it.
/// - Patch StartRunLobby.IsAscensionEpochRevealed to return true when an
///   override is active, bypassing the "must beat Act 3 first" gate that
///   blocks ascension for characters that have never been played.
///
/// Multiplayer:
/// - Multiplayer uses separate fields: ProgressState.MaxMultiplayerAscension
///   and ProgressState.PreferredMultiplayerAscension (global, not per-character).
/// - The lobby computes MAX = MIN(all players' maxMultiplayerAscensionUnlocked).
/// - LobbyPlayer.maxMultiplayerAscensionUnlocked is populated from
///   ProgressState.MaxMultiplayerAscension during lobby join, so patching
///   the ProgressState getter is sufficient.
/// </summary>
public static class AscensionPatches
{
    /// <summary>
    /// Patch CharacterStats.MaxAscension getter.
    /// When an override is active for the character, return the override value
    /// instead of the actual stored value.
    ///
    /// This is the most elegant approach because ALL code paths that check
    /// ascension limits read from this property:
    /// - StartRunLobby reads it to set MaxAscension for the lobby
    /// - NCharacterSelectScreen reads it to clamp the embark ascension
    /// - The UI panel receives MaxAscension from the lobby
    /// </summary>
    [HarmonyPatch(typeof(CharacterStats), nameof(CharacterStats.MaxAscension), MethodType.Getter)]
    public static class CharacterStats_MaxAscension_Getter_Patch
    {
        public static void Postfix(CharacterStats __instance, ref int __result)
        {
            if (!AscensionConfig.Enabled) return;
            if (__instance.Id == null) return;

            string characterId = __instance.Id.ToString()!;
            int overrideValue = AscensionConfig.GetEffectiveAscension(characterId);

            if (overrideValue >= 0)
            {
                // Only override if the override is higher than the actual value.
                // This allows the player to still progress naturally.
                // If you want to force a lower ascension, that's handled by
                // simply selecting a lower level in the UI.
                if (overrideValue > __result)
                {
                    Log.Debug($"[AscensionAdjuster] Overriding MaxAscension for {characterId}: {__result} -> {overrideValue}");
                    __result = overrideValue;
                }
            }
        }
    }

    /// <summary>
    /// Patch StartRunLobby.IsAscensionEpochRevealed (private instance method).
    ///
    /// This is the gatekeeper that prevents ascension from being shown at all
    /// for characters that have never beaten Act 3. Even if MaxAscension is
    /// overridden to 10, the lobby checks this method first and will hard-reset
    /// MaxAscension back to 0 if it returns false.
    ///
    /// The condition in SetSingleplayerAscensionAfterCharacterChanged:
    ///   orCreateCharacterStats.MaxAscension <= 0 || !flag
    /// is an OR — so !flag alone is enough to kill ascension, regardless of MaxAscension.
    ///
    /// When an override is active for the character, we return true so the
    /// lobby proceeds to the normal path and respects the MaxAscension getter.
    /// </summary>
    [HarmonyPatch(typeof(StartRunLobby), "IsAscensionEpochRevealed")]
    public static class StartRunLobby_IsAscensionEpochRevealed_Patch
    {
        public static void Postfix(ModelId characterId, ref bool __result)
        {
            if (!AscensionConfig.Enabled) return;
            if (__result) return; // Already true, nothing to do

            string charIdStr = characterId.ToString()!;
            int overrideValue = AscensionConfig.GetEffectiveAscension(charIdStr);

            if (overrideValue >= 0)
            {
                Log.Debug($"[AscensionAdjuster] Bypassing Ascension Epoch gate for {charIdStr}");
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch CharacterStats.PreferredAscension getter.
    /// When the max ascension is overridden to a higher value,
    /// also return the override as the preferred level so the UI
    /// defaults to the new max instead of the old preferred.
    /// </summary>
    [HarmonyPatch(typeof(CharacterStats), nameof(CharacterStats.PreferredAscension), MethodType.Getter)]
    public static class CharacterStats_PreferredAscension_Getter_Patch
    {
        public static void Postfix(CharacterStats __instance, ref int __result)
        {
            if (!AscensionConfig.Enabled) return;
            if (__instance.Id == null) return;

            string characterId = __instance.Id.ToString()!;
            int overrideValue = AscensionConfig.GetEffectiveAscension(characterId);

            if (overrideValue >= 0 && overrideValue > __result)
            {
                __result = overrideValue;
            }
        }
    }

    // ==================== Multiplayer Patches ====================

    /// <summary>
    /// Patch ProgressState.MaxMultiplayerAscension getter.
    /// Multiplayer ascension is global (not per-character). This value is read
    /// when joining a lobby to populate LobbyPlayer.maxMultiplayerAscensionUnlocked,
    /// and the lobby computes its max as MIN(all players' unlocked levels).
    /// By overriding this getter, each player with the mod reports a higher max,
    /// so the MIN calculation is not restrictive.
    /// </summary>
    [HarmonyPatch(typeof(ProgressState), nameof(ProgressState.MaxMultiplayerAscension), MethodType.Getter)]
    public static class ProgressState_MaxMultiplayerAscension_Getter_Patch
    {
        public static void Postfix(ref int __result)
        {
            if (!AscensionConfig.Enabled) return;
            int overrideValue = AscensionConfig.GetEffectiveMultiplayerAscension();
            if (overrideValue >= 0 && overrideValue > __result)
            {
                Log.Debug($"[AscensionAdjuster] Overriding MaxMultiplayerAscension: {__result} -> {overrideValue}");
                __result = overrideValue;
            }
        }
    }

    /// <summary>
    /// Patch ProgressState.PreferredMultiplayerAscension getter.
    /// Syncs the preferred (default selected) multiplayer ascension to the override
    /// so the UI defaults to the overridden level.
    /// </summary>
    [HarmonyPatch(typeof(ProgressState), nameof(ProgressState.PreferredMultiplayerAscension), MethodType.Getter)]
    public static class ProgressState_PreferredMultiplayerAscension_Getter_Patch
    {
        public static void Postfix(ref int __result)
        {
            if (!AscensionConfig.Enabled) return;
            int overrideValue = AscensionConfig.GetEffectiveMultiplayerAscension();
            if (overrideValue >= 0 && overrideValue > __result)
            {
                __result = overrideValue;
            }
        }
    }
}
