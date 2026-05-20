using Godot;

namespace QuestSystemCSharp;

[GlobalClass]
public partial class QuestPoolCompleted : QuestPool
{
    public QuestPoolCompleted()
    {
    }

    public QuestPoolCompleted(string poolName) : base(poolName)
    {
    }
}