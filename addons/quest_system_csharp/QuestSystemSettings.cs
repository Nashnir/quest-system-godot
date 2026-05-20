using Godot;
using Godot.Collections;

namespace QuestSystemCSharp;

[GlobalClass]
public partial class QuestSystemSettings : RefCounted
{
    public const string MainCategory = "quest_system";
    public const string ConfigCategory = MainCategory + "/config";

#if TOOLS
    public static void Initialize(StringName pluginPath)
    {
        InitSetting( 
            $"{ConfigCategory}/check_for_updates_on_startup", 
            true, 
            Variant.Type.Bool);

        InitSetting(
            $"{ConfigCategory}/autoload_script_path",
            $"{pluginPath}/QuestSystemManager.cs",
            Variant.Type.String,
            PropertyHint.File
        );

        InitSetting(
            $"{ConfigCategory}/available_quest_pool_path",
            $"{pluginPath}/AvailableQuestPool.cs",
            Variant.Type.String,
            PropertyHint.File
        );

        InitSetting(
            $"{ConfigCategory}/active_quest_pool_path",
            $"{pluginPath}/ActiveQuestPool.cs",
            Variant.Type.String,
            PropertyHint.File
        );

        InitSetting(
            $"{ConfigCategory}/completed_quest_pool_path",
            $"{pluginPath}/CompletedQuestPool.cs",
            Variant.Type.String,
            PropertyHint.File
        );

        InitSetting( 
            $"{ConfigCategory}/additional_pools",
            new Array(),
            Variant.Type.Array, 
            PropertyHint.TypeString,
            $"{(int)Variant.Type.String}:");

        InitSetting(
            $"{ConfigCategory}/require_objective_completed", 
            true, 
            Variant.Type.Bool);
            
        InitSetting(
            $"{ConfigCategory}/allow_repeating_completed_quests", 
            false, 
            Variant.Type.Bool);
    }

    private static void InitSetting( string name, 
                                     Variant defaultValue, Variant.Type type,
                                     PropertyHint hint = PropertyHint.None,
                                     string hintString = "")
    {
        if (!ProjectSettings.HasSetting(name))
            ProjectSettings.SetSetting(name, defaultValue);

        ProjectSettings.SetInitialValue(name, defaultValue);

        Dictionary hintInfo = new()
        {
            ["name"] = name,
            ["type"] = (int)type,
            ["hint"] = (int)hint,
            ["hint_string"] = hintString
        };

        ProjectSettings.AddPropertyInfo(hintInfo);
    }
#endif

    public static Variant GetConfigSetting(string name, Variant defaultValue = default)
    {
        return ProjectSettings.GetSetting($"{ConfigCategory}/{name}", defaultValue);
    }
}