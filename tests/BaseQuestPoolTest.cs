using Godot;
using Godot.Collections;
using GdUnit4;
using static GdUnit4.Assertions;

namespace QuestSystemCSharp;

[TestSuite]
[RequireGodotRuntime]
public class QuestPoolTest
{
    private const string TestPoolName = "TestPool";

    private Quest _quest = new();
    private QuestPool? _pool;

    private QuestSystemManager QuestSystem => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<QuestSystemManager>("QuestSystem");

    [Before]
    public void Before()
    {
        _quest.Id = 1;
        _quest.QuestName = "Test";
        _quest.QuestDescription = "This is a Test quest.";
        _quest.QuestObjective = "Run the tests without errors";

        QuestPool pool = new(TestPoolName);
        QuestSystem.AddChild(pool);

        _pool = QuestSystem.GetPool(TestPoolName);
    }

    [BeforeTest]
    public void BeforeTest()
    {
        _pool?.Reset();
        _pool?.AddQuest(_quest);
    }

    [After]
    public void After()
    {
        QuestSystem.RemovePool(TestPoolName);
    }

    [TestCase]
    public void TestAddQuest()
    {
        AssertBool(QuestSystem.IsQuestInPool(_quest, TestPoolName)).IsTrue();
    }

    [TestCase]
    public void TestRemoveQuest()
    {
        AssertBool(QuestSystem.IsQuestInPool(_quest, TestPoolName)).IsTrue();
        _pool?.RemoveQuest(_quest);
        AssertBool(QuestSystem.IsQuestInPool(_quest, TestPoolName)).IsFalse();
    }

    [TestCase]
    public void TestGetQuestFromId()
    {
        AssertBool(QuestSystem.IsQuestInPool(_quest, TestPoolName)).IsTrue();
        AssertObject(_pool?.GetQuestFromId(1)).IsEqual(_quest);
    }

    [TestCase]
    public void TestIsQuestInside()
    {
		AssertBool(_pool.IsQuestInside(_quest)).IsTrue();
    }

    [TestCase]
    public void TestGetIdsFromQuests()
    {
        AssertArray(_pool?.GetIdsFromQuests()).ContainsExactly(new Array<int> { 1 });
    }

    [TestCase]
    public void TestGetAllQuests()
    {
        AssertArray(_pool?.GetAllQuests()).ContainsExactly(new Array<Quest> { _quest });
    }

    [TestCase]
    public void TestReset()
    {
        AssertBool(QuestSystem.IsQuestInPool(_quest, TestPoolName)).IsTrue();
        _pool?.Reset();
        AssertArray(_pool?.GetAllQuests()).IsEmpty();
    }
}