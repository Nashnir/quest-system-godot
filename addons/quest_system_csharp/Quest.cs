using Godot;
using Godot.Collections;

namespace QuestSystemCSharp;

[GlobalClass]
public partial class Quest : Resource
{
    [Signal]
    public delegate void StartedEventHandler();

    [Signal]
    public delegate void UpdatedEventHandler();

    [Signal]
    public delegate void CompletedEventHandler();

    [Signal]
    public delegate void ObjectiveStatusUpdatedEventHandler(bool value);

    [Export]
    public int Id { get; set; }

    [Export]
    public string QuestName { get; set; } = "";

    [Export(PropertyHint.MultilineText)]
    public string QuestDescription { get; set; } = "";

    [Export(PropertyHint.MultilineText)]
    public string QuestObjective { get; set; } = "";

    private bool _objectiveCompleted = false;

    [Export]
    public bool ObjectiveCompleted
    {
        get => _objectiveCompleted;
        set
        {
            if (_objectiveCompleted == value)
                return;

            _objectiveCompleted = value;
            EmitSignal(SignalName.ObjectiveStatusUpdated, value);
        }
    }

    public virtual void Update(Dictionary? args = null)
    {
        EmitSignal(SignalName.Updated);
    }

    public virtual void Start(Dictionary? args = null)
    {
        EmitSignal(SignalName.Started);
    }

    public virtual void Complete(Dictionary? args = null)
    {
        EmitSignal(SignalName.Completed);
    }

    public virtual Dictionary Serialize()
    {
        return new Dictionary
        {
            ["quest_name"] = QuestName,
            ["quest_description"] = QuestDescription,
            ["quest_objective"] = QuestObjective,
            ["objective_completed"] = ObjectiveCompleted
        };
    }

    public virtual void Deserialize(Dictionary data)
    {
        if (data.ContainsKey("quest_name"))
            QuestName = data["quest_name"].AsString();

        if (data.ContainsKey("quest_description"))
            QuestDescription = data["quest_description"].AsString();

        if (data.ContainsKey("quest_objective"))
            QuestObjective = data["quest_objective"].AsString();

        if (data.ContainsKey("objective_completed"))
            ObjectiveCompleted = data["objective_completed"].AsBool();
    }
}