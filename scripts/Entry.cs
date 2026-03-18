using System;
using System.IO;
using System.Reflection;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace AscensionAdjuster.Scripts;

/// <summary>
/// Main entry point for the Ascension Adjuster mod.
///
/// This mod allows you to freely adjust the Ascension level for any character
/// without needing to unlock them sequentially by winning runs.
///
/// How it works:
/// - On load, reads config.json from the mod folder
/// - Patch 1: CharacterStats.MaxAscension getter → returns override value instead of real save data
/// - Patch 2: CharacterStats.PreferredAscension getter → defaults UI to the override level
/// - Patch 3: StartRunLobby.IsAscensionEpochRevealed → bypasses the "must beat Act 3 first" gate
///            so characters that have NEVER been played also get the ascension slider unlocked
/// - The game's existing UI and lobby logic naturally pick up the higher MaxAscension
/// - You can then select any ascension level up to the configured max in character select
///
/// Configuration (config.json):
/// {
///   "enabled": true,
///   "global_ascension_override": 10,    // Set to 0-10, or -1 to disable
///   "character_overrides": {             // Optional per-character overrides
///     "CHARACTER.IRONCLAD": 5,
///     "CHARACTER.SILENT": 10
///   }
/// }
/// </summary>
[ModInitializer("Init")]
public class Entry
{
    public static void Init()
    {
        try
        {
            // Determine the mod's directory for config file
            string? assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (assemblyDir != null)
            {
                AscensionConfig.Initialize(assemblyDir);
            }
            else
            {
                Log.Error("[AscensionAdjuster] Could not determine mod directory!");
            }

            // Apply all Harmony patches
            var harmony = new Harmony("sts2.varian.ascension_adjuster");

            // Patch each class individually with try/catch so one failure doesn't break all
            try
            {
                harmony.CreateClassProcessor(typeof(AscensionPatches.CharacterStats_MaxAscension_Getter_Patch)).Patch();
                Log.Info("[AscensionAdjuster] Patched CharacterStats.MaxAscension getter.");
            }
            catch (Exception ex)
            {
                Log.Error($"[AscensionAdjuster] Failed to patch MaxAscension: {ex.Message}");
            }

            try
            {
                harmony.CreateClassProcessor(typeof(AscensionPatches.CharacterStats_PreferredAscension_Getter_Patch)).Patch();
                Log.Info("[AscensionAdjuster] Patched CharacterStats.PreferredAscension getter.");
            }
            catch (Exception ex)
            {
                Log.Error($"[AscensionAdjuster] Failed to patch PreferredAscension: {ex.Message}");
            }

            try
            {
                harmony.CreateClassProcessor(typeof(AscensionPatches.StartRunLobby_IsAscensionEpochRevealed_Patch)).Patch();
                Log.Info("[AscensionAdjuster] Patched StartRunLobby.IsAscensionEpochRevealed.");
            }
            catch (Exception ex)
            {
                Log.Error($"[AscensionAdjuster] Failed to patch IsAscensionEpochRevealed: {ex.Message}");
            }

            // Allow Godot to load custom scripts from this assembly
            ScriptManagerBridge.LookupScriptsInAssembly(typeof(Entry).Assembly);

            Log.Info("[AscensionAdjuster] Mod initialized successfully!");
            if (AscensionConfig.Enabled)
            {
                Log.Info($"[AscensionAdjuster] Global override: {AscensionConfig.GlobalAscensionOverride}");
                foreach (var kvp in AscensionConfig.CharacterOverrides)
                {
                    Log.Info($"[AscensionAdjuster] Character override: {kvp.Key} -> {kvp.Value}");
                }
            }
            else
            {
                Log.Info("[AscensionAdjuster] Mod is disabled in config.");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"[AscensionAdjuster] Critical error during initialization: {ex}");
        }
    }
}
