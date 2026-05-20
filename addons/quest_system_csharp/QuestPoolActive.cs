using Godot;

namespace QuestSystemCSharp;

[GlobalClass]
public partial class QuestPoolActive : QuestPool
{
    public QuestPoolActive()
    {
    }

    public QuestPoolActive(string poolName) : base(poolName)
    {
    }

    public void UpdateObjective(int questId)
    {
        Quest? quest = GetQuestFromId(questId);
        quest?.Update();
    }
}