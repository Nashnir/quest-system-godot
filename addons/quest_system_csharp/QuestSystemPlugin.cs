#if TOOLS
using Godot;

namespace QuestSystemCSharp;

[Tool]
public partial class QuestSystemPlugin : EditorPlugin
{
    private QuestPropertyTranslationPlugin? _translationPlugin;

    public override void _EnterTree()
    {
        QuestSystemSettings.Initialize(GetPluginPath());
        string defaultManagerPath = $"{GetPluginPath()}/QuestSystemManager.cs";

        string autoloadPath = QuestSystemSettings.GetConfigSetting("autoload_script_path", defaultManagerPath)
                                                 .AsString();

        if (string.IsNullOrEmpty(autoloadPath)) autoloadPath = defaultManagerPath;

        bool isDefault = autoloadPath == "QuestSystemManager.cs" || autoloadPath == defaultManagerPath;

        if (!isDefault)
        {
            if (!ValidateManagerScript(autoloadPath))
            {
                GD.PrintRich( "[color=red][!][/color] [b]Cannot override default autoload script!\n" +
                              "[color=red]The script is not valid or does not exist.[/color]\n" +
                              "Using default script. Check QuestSystem settings.[/b]");

                autoloadPath = defaultManagerPath;
            }
        }

        AddAutoloadSingleton("QuestSystem", autoloadPath);

        if (Engine.IsEditorHint())
        {
            _translationPlugin = new QuestPropertyTranslationPlugin();
            AddTranslationParserPlugin(_translationPlugin);
        }
    }

    public override void _ExitTree()
    {
        RemoveAutoloadSingleton("QuestSystem");

        if (_translationPlugin != null) RemoveTranslationParserPlugin(_translationPlugin);

        _translationPlugin = null;
    }

    private bool ValidateManagerScript(string path)
    {
        if (path.StartsWith("uid://"))
        {
            long id = ResourceUid.TextToId(path);
            path = ResourceUid.GetIdPath(id);
        }

        if (!ResourceLoader.Exists(path)) return false;

        Script script = GD.Load<Script>(path);
        if (script == null) return false;

        Variant instanceVariant = script.Call("new");
        GodotObject instance = instanceVariant.AsGodotObject();

        bool valid = instance is QuestManager;

        if (instance is Node node) node.QueueFree();

        return valid;
    }

    private Texture2D GetPluginIcon()
    {
        return GD.Load<Texture2D>($"{GetPluginPath()}/assets/quest_resource.svg");
    }

    private string GetPluginPath()
    {
        return GetScript().As<Script>().ResourcePath.GetBaseDir();
    }

    private int VersionToInt(string version)
    {
        string[] parts = version.Split(".");

        if (parts.Length < 3) return 0;

        return parts[0].ToInt() * 10000 +
               parts[1].ToInt() * 100 +
               parts[2].ToInt();
    }
}
#endif