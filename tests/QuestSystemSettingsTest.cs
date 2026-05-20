using Godot;
using GdUnit4;
using static GdUnit4.Assertions;

namespace QuestSystemCSharp;

[TestSuite]
[RequireGodotRuntime]
public class QuestSystemSettingsTest
{
    [TestCase]
    public void TestGetConfigSetting()
    {
        bool setting = ProjectSettings.GetSetting("quest_system/config/require_objective_completed", true)
                                      .AsBool();

        bool result = QuestSystemSettings.GetConfigSetting("require_objective_completed", true)
                                         .AsBool();

        AssertBool(result).IsEqual(setting);
    }
}