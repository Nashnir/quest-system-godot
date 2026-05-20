#if TOOLS
using Godot;
using Godot.Collections;

namespace QuestSystemCSharp;

[Tool]
public partial class QuestPropertyTranslationPlugin : EditorTranslationParserPlugin
{
    public override Array<string[]> _ParseFile(string path)
    {
        Resource resource = ResourceLoader.Load<Resource>(path);

        if (resource == null) return [];
        if (resource is not Quest quest) return [];

        Array<string[]> result = [];

        Script script = quest.GetScript().As<Script>();
        if (script == null) return result;

        foreach (Dictionary property in script.GetScriptPropertyList())
        {
            Variant.Type propertyType = (Variant.Type)property["type"].AsInt32();

            PropertyUsageFlags usage = (PropertyUsageFlags)property["usage"].AsInt32();

            string propertyName = property["name"].AsString();

            if (propertyType != Variant.Type.String) continue;

            bool isExportedScriptVariable = usage == ( PropertyUsageFlags.ScriptVariable | 
                                                       PropertyUsageFlags.Storage |
                                                       PropertyUsageFlags.Editor);

            if (!isExportedScriptVariable) continue;

            result.Add( new[] { $"quest_{quest.Id}/{propertyName}",
                                $"Quest ID: {quest.Id}, property: {propertyName}",
                                $"quest_{quest.Id}/{propertyName}_plural"});
        }

        return result;
    }

    public override string[] _GetRecognizedExtensions()
    {
        return ["tres"];
    }
}
#endif