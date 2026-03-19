using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using MegaCrit.Sts2.Core.Logging;

namespace AscensionAdjuster.Scripts;

/// <summary>
/// Configuration file for the Ascension Adjuster mod.
/// Stores the desired ascension level override per character.
/// Config is saved to: [GameDir]/mods/AscensionAdjuster/config.json
/// </summary>
public static class AscensionConfig
{
    private const string ConfigFileName = "config.json";
    private const int MaxAscension = 10;
    private const int MinAscension = 0;

    private static string? _configPath;
    private static ConfigData _config = new();

    /// <summary>
    /// Whether the mod is enabled (master toggle).
    /// </summary>
    public static bool Enabled => _config.Enabled;

    /// <summary>
    /// The global ascension level override. Applies to all characters
    /// unless a per-character override is set.
    /// -1 means "don't override, use the character's actual max".
    /// 0-10 means "override to this level".
    /// </summary>
    public static int GlobalAscensionOverride => _config.GlobalAscensionOverride;

    /// <summary>
    /// Per-character ascension overrides. Key is the character ID string
    /// (e.g., "CHARACTER.IRONCLAD"). Value is 0-10.
    /// If a character is not in this dictionary, the GlobalAscensionOverride is used.
    /// </summary>
    public static Dictionary<string, int> CharacterOverrides => _config.CharacterOverrides;

    /// <summary>
    /// Multiplayer ascension override. Applies globally (multiplayer is not per-character).
    /// -1 means "don't override" (default). 0-10 means "override to this level".
    /// </summary>
    public static int MultiplayerAscensionOverride => _config.MultiplayerAscensionOverride;

    public static void Initialize(string modDirectory)
    {
        _configPath = Path.Combine(modDirectory, ConfigFileName);
        Load();
    }

    /// <summary>
    /// Gets the effective ascension override for a given character.
    /// Returns -1 if no override is active (use game default).
    /// Returns 0-10 for a specific override level.
    /// </summary>
    public static int GetEffectiveAscension(string characterId)
    {
        if (!Enabled)
            return -1;

        // Check per-character override first
        if (CharacterOverrides.TryGetValue(characterId, out int charOverride))
        {
            return Math.Clamp(charOverride, MinAscension, MaxAscension);
        }

        // Fall back to global override
        if (GlobalAscensionOverride >= MinAscension)
        {
            return Math.Clamp(GlobalAscensionOverride, MinAscension, MaxAscension);
        }

        return -1; // No override
    }

    /// <summary>
    /// Gets the effective multiplayer ascension override.
    /// Returns -1 if no override is active, 0-10 for a specific level.
    /// </summary>
    public static int GetEffectiveMultiplayerAscension()
    {
        if (!Enabled) return -1;
        if (_config.MultiplayerAscensionOverride >= MinAscension)
            return Math.Clamp(_config.MultiplayerAscensionOverride, MinAscension, MaxAscension);
        return -1;
    }

    public static void Load()
    {
        if (_configPath == null) return;

        try
        {
            if (File.Exists(_configPath))
            {
                string json = File.ReadAllText(_configPath);
                _config = JsonSerializer.Deserialize<ConfigData>(json) ?? new ConfigData();
                Log.Info($"[AscensionAdjuster] Config loaded. Enabled={_config.Enabled}, Global={_config.GlobalAscensionOverride}, Overrides={_config.CharacterOverrides.Count}");
            }
            else
            {
                // Create default config
                _config = new ConfigData();
                Save();
                Log.Info("[AscensionAdjuster] Default config created.");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"[AscensionAdjuster] Failed to load config: {ex.Message}");
            _config = new ConfigData();
        }
    }

    public static void Save()
    {
        if (_configPath == null) return;

        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(_config, options);
            File.WriteAllText(_configPath, json);
            Log.Info("[AscensionAdjuster] Config saved.");
        }
        catch (Exception ex)
        {
            Log.Error($"[AscensionAdjuster] Failed to save config: {ex.Message}");
        }
    }

    /// <summary>
    /// JSON-serializable config data structure.
    /// </summary>
    public class ConfigData
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = true;

        [JsonPropertyName("global_ascension_override")]
        public int GlobalAscensionOverride { get; set; } = 10;

        [JsonPropertyName("character_overrides")]
        public Dictionary<string, int> CharacterOverrides { get; set; } = new();

        [JsonPropertyName("multiplayer_ascension_override")]
        public int MultiplayerAscensionOverride { get; set; } = -1;
    }
}
