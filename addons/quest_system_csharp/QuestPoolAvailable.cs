using Godot;

namespace QuestSystemCSharp;

[GlobalClass]
public partial class QuestPoolAvailable : QuestPool
{
    public QuestPoolAvailable()
    {
    }

    public QuestPoolAvailable(string poolName) : base(poolName)
    {
    }
}