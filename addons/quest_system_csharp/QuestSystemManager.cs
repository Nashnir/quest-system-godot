using System;
using Godot;
using Godot.Collections;

namespace QuestSystemCSharp
{
    [GlobalClass]
    public partial class QuestSystemManager : QuestManager
    {
        [Signal] public delegate void QuestAcceptedEventHandler(Quest quest);

        [Signal] public delegate void QuestCompletedEventHandler(Quest quest);

        [Signal] public delegate void NewAvailableQuestEventHandler(Quest quest);

        private readonly QuestPoolAvailable _available = new("Available");
        private readonly QuestPoolActive _active = new("Active");
        private readonly QuestPoolCompleted _completed = new("Completed");

        public QuestPoolAvailable Available => _available;
        public QuestPoolActive Active => _active;
        public QuestPoolCompleted Completed => _completed;

        public override void _Ready()
        {
            AddChild(_available);
            AddChild(_active);
            AddChild(_completed);

            base._Ready();
        }

        public Quest StartQuest(Quest quest, Dictionary? args = null)
        {
            ArgumentNullException.ThrowIfNull(quest);
            args ??= [];

            if (_active.IsQuestInside(quest))
                return quest;

            bool isCompleted = _completed.IsQuestInside(quest);
            
            bool allowRepeating = QuestSystemSettings.GetConfigSetting("allow_repeating_completed_quests", false)
                                                    .AsBool();

            if (isCompleted && !allowRepeating)
                return quest;

            if (isCompleted && allowRepeating)
                _completed.RemoveQuest(quest);

            _available.RemoveQuest(quest);
            _active.AddQuest(quest);

            EmitSignal(SignalName.QuestAccepted, quest);
            quest.Start(args);

            return quest;
        }

        public Quest CompleteQuest(Quest quest, Dictionary? args = null)
        {
            args ??= [];

            if (!_active.IsQuestInside(quest)) return quest;

            bool requireObjectiveCompleted = QuestSystemSettings
            .GetConfigSetting("require_objective_completed", true)
            .AsBool();

            if (!quest.ObjectiveCompleted && requireObjectiveCompleted) return quest;

            quest.Complete(args);

            _active.RemoveQuest(quest);
            _completed.AddQuest(quest);

            EmitSignal(SignalName.QuestCompleted, quest);

            return quest;
        }

        public Quest UpdateQuest(Quest quest, Dictionary? args = null)
        {
            args ??= [];

            QuestPool? poolWithQuest = null;

            foreach (QuestPool pool in GetAllPools())
            {
                if (!pool.IsQuestInside(quest)) continue;

                poolWithQuest = pool;
                break;
            }

            if (poolWithQuest == null)
            {
                GD.PushWarning("Tried calling update on a Quest that is not in any pool.");
                return quest;
            }

            quest.Update(args);
            return quest;
        }

        public void MarkQuestAsAvailable(Quest quest)
        {
            if ( _available.IsQuestInside(quest) ||
                _completed.IsQuestInside(quest) ||
                _active.IsQuestInside(quest))
            {
                return;
            }

            _available.AddQuest(quest);
            EmitSignal(SignalName.NewAvailableQuest, quest);
        }

        public Array<Quest> GetAvailableQuests() => _available.GetAllQuests();

        public Array<Quest> GetActiveQuests() => _active.GetAllQuests();
        public Array<Quest> GetCompletedQuests() => _completed.GetAllQuests();

        public bool IsQuestAvailable(Quest quest) => _available.IsQuestInside(quest);
        public bool IsQuestActive(Quest quest) => _active.IsQuestInside(quest);
        public bool IsQuestCompleted(Quest quest) => _completed.IsQuestInside(quest);
    }
    
}

