using System;
using Godot;
using Godot.Collections;

namespace QuestSystemCSharp
{
    [GlobalClass]
    public partial class QuestPool : Node
    {
        protected Array<Quest> Quests { get; } = [];

        public QuestPool(){ }

        public QuestPool(string poolName) => Name = poolName;

        public virtual Quest AddQuest(Quest quest)
        {
            ArgumentNullException.ThrowIfNull(quest);
            if (!Quests.Contains(quest))
                Quests.Add(quest);
            return quest;
        }

        public virtual Quest RemoveQuest(Quest quest)
        {
            ArgumentNullException.ThrowIfNull(quest);
            Quests.Remove(quest);
            return quest;
        }

        public virtual Quest? GetQuestFromId(int id)
        {
            foreach (Quest quest in Quests) if (quest.Id == id) return quest;
            return null;
        }

        public virtual bool IsQuestInside(Quest quest)
        {
            return Quests.Contains(quest);
        }

        public virtual Array<int> GetIdsFromQuests()
        {
            Array<int> ids = [];
            foreach (Quest quest in Quests) ids.Add(quest.Id);
            return ids;
        }

        public virtual Array<Quest> GetAllQuests() => Quests;
        public virtual void Reset() => Quests.Clear();
    }
}

