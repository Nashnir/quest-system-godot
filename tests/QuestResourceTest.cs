using GdUnit4;
using Godot.Collections;
using static GdUnit4.Assertions;

namespace QuestSystemCSharp;

[TestSuite]
[RequireGodotRuntime]
public class QuestResourceTest
{
    private Quest _quest = new();

    [Before]
    public void Before()
    {
        _quest.Id = 1;
        _quest.QuestName = "Test";
        _quest.QuestDescription = "This is a Test quest.";
        _quest.QuestObjective = "Run the tests without errors";
        _quest.ObjectiveCompleted = false;
    }

    [TestCase]
    public void TestUpdate()
    {
        AssertSignal(_quest).IsEmitted(Quest.SignalName.Updated);
        _quest.Update();
    }

    [TestCase]
    public void TestComplete()
    {
        AssertSignal(_quest).IsEmitted(Quest.SignalName.Completed);
        _quest.Complete();
    }

    [TestCase]
    public void TestStart()
    {
        AssertSignal(_quest).IsEmitted(Quest.SignalName.Started);
        _quest.Start();
    }

    [TestCase]
    public void TestObjectiveStatusUpdatedSignal()
    {
        AssertSignal(_quest).IsEmitted(Quest.SignalName.ObjectiveStatusUpdated);
        _quest.ObjectiveCompleted = true;

        AssertSignal(_quest).IsEmitted(Quest.SignalName.ObjectiveStatusUpdated);
        _quest.ObjectiveCompleted = false;
    }

    [TestCase]
    public void TestSerialize()
    {
        Dictionary expected = new()
        {
            ["QuestName"] = "Test",
            ["QuestDescription"] = "This is a Test quest.",
            ["QuestObjective"] = "Run the tests without errors",
            ["ObjectiveCompleted"] = false
        };

        AssertThat(_quest.Serialize()).IsEqual(expected);
    }

    [TestCase]
    public void TestDeserialize()
    {
        Dictionary expected = new()
        {
            ["QuestName"] = "Test",
            ["QuestDescription"] = "This is a Test quest.",
            ["QuestObjective"] = "Run the tests without errors",
            ["ObjectiveCompleted"] = false
        };

        _quest.Deserialize(expected);

        AssertThat(_quest.Serialize()).IsEqual(expected);
    }
}