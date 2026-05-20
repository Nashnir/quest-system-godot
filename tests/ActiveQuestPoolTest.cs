using Godot;
using GdUnit4;
using static GdUnit4.Assertions;

namespace QuestSystemCSharp;

[TestSuite]
[RequireGodotRuntime]
public class QuestPoolActiveTest
{
    private Quest _quest = new();

    private QuestSystemManager QuestSystem => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<QuestSystemManager>("QuestSystem");

    [Before]
    public void Before()
    {
        _quest.Id = 1;
        _quest.QuestName = "Test";
        _quest.QuestDescription = "This is a Test quest.";
        _quest.QuestObjective = "Run the tests without errors";
    }

    [TestCase]
    public void TestUpdateObjective()
    {
        QuestSystem.GetPool("Active").AddQuest(_quest);
		AssertSignal(_quest).IsEmitted(Quest.SignalName.Updated);
        ((QuestPoolActive)QuestSystem.GetPool("Active")).UpdateObjective(_quest.Id);
    }
}