using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

namespace QuestSystemCSharp;

public partial class QuestManager : Node
{
    public override void _Ready()
    {
        Variant setting = ProjectSettings.GetSetting("quest_system/config/additional_pools", new Array());

        if (setting.VariantType != Variant.Type.Array)
            return;

        foreach (Variant poolPathVariant in setting.AsGodotArray())
        {
            string poolPath = poolPathVariant.AsString();
            string poolName = poolPath.GetFile().Split('.')[0].ToPascalCase();

            AddNewPool(poolPath, poolName);
        }
    }

    public override void _ExitTree()
    {
        foreach (QuestPool pool in GetAllPools())
        {
            pool.Reset();
            pool.QueueFree();
        }
    }

    public bool IsQuestInPool(Quest quest, string poolName = "")
    {
        if (string.IsNullOrEmpty(poolName))
        {
            foreach (QuestPool pool in GetAllPools())
            {
                if (pool.IsQuestInside(quest)) 
                    return true;
            }
            return false;
        }

        QuestPool targetPool = GetNode<QuestPool>(poolName);
        return targetPool.IsQuestInside(quest);
    }

    public void CallQuestMethod(int questId, StringName method, Array args = null)
    {
        Quest? quest = GetQuestById(questId);
        if (quest == null) 
            return;
        if (quest.HasMethod(method)) quest.Callv(method, args);
    }

    public void SetQuestProperty(int questId, StringName property, Variant value)
    {
        Quest? quest = GetQuestById(questId);
        if (quest == null) 
            return;
        if (!QuestHasProperty(quest, property))
            return;
        quest.Set(property, value);
    }

    public Variant GetQuestProperty(int questId, StringName property)
    {
        Quest? quest = GetQuestById(questId);        
        if (quest == null) 
            return default;
        if (!QuestHasProperty(quest, property)) 
            return default;
        return quest.Get(property);
    }

    private bool QuestHasProperty(Quest quest, StringName property)
    {
        if (property == default || string.IsNullOrEmpty(property.ToString())) 
            return false;

        foreach (Dictionary propertyData in quest.GetPropertyList())
        {
            if (!propertyData.ContainsKey("name")) 
                continue;
            if (propertyData["name"].AsString() == property.ToString()) 
                return true;
        }

        return false;
    }

    private Quest? GetQuestById(int questId)
    {
        foreach (QuestPool pool in GetAllPools())
        {
            Quest quest = pool.GetQuestFromId(questId);
            if (quest != null) 
                return quest;
        }
        return null;
    }

    public void AddNewPool(string poolPath, StringName poolName)
    {
        Script poolScript = GD.Load<Script>(poolPath);
        if (poolScript == null) 
            return;

        Variant instanceVariant = poolScript.Call("new", poolName);
        Node? poolInstance = instanceVariant.AsGodotObject() as Node;

        if (poolInstance == null) 
            return;

        foreach (QuestPool existingPool in GetAllPools())
        {
            if (poolInstance.GetScript().Equals(existingPool.GetScript()) &&
                poolName.ToString() != existingPool.Name)
            {
                poolInstance.QueueFree();
                return;
            }
        }

        AddChild(poolInstance, true);
    }

    public void RemovePool(StringName poolName)
    {
        QuestPool pool = GetPool(poolName);
        pool?.QueueFree();
    }

    public QuestPool GetPool(StringName pool)
    {
        return GetNodeOrNull<QuestPool>(new NodePath(pool.ToString()));
    }

    public Array<QuestPool> GetAllPools()
    {
        Array<QuestPool> pools = new();

        foreach (Node child in GetChildren())
        {
            if (child is QuestPool pool) pools.Add(pool);
        }

        return pools;
    }

    public Quest MoveQuestToPool(Quest quest, StringName oldPool, StringName newPool)
    {
        ArgumentNullException.ThrowIfNull(quest);

        if (oldPool == newPool)
            return quest;

        QuestPool oldPoolInstance = GetPool(oldPool);
        QuestPool newPoolInstance = GetPool(newPool);

        if (oldPoolInstance == null || newPoolInstance == null)
            throw new InvalidOperationException("Both source and target quest pools must exist.");

        oldPoolInstance.RemoveQuest(quest);
        newPoolInstance.AddQuest(quest);

        return quest;
    }

    public void ResetPool(StringName? poolName = default)
    {
        if (string.IsNullOrEmpty(poolName?.ToString()))
        {
            foreach (QuestPool pool in GetAllPools()) pool.Reset();

            return;
        }

        QuestPool targetPool = GetPool(poolName);
        targetPool?.Reset();
    }

    public Dictionary PoolStateAsDict()
    {
        Dictionary questDict = [];

        foreach (QuestPool pool in GetAllPools()) questDict[pool.Name.ToString().ToLower()] = pool.GetIdsFromQuests();

        return questDict;
    }

    public void RestorePoolStateFromDict(Dictionary dict, Array<Quest> quests)
    {
        foreach (QuestPool pool in GetAllPools())
        {
            string poolKey = pool.Name.ToString().ToLower();

            if (!dict.ContainsKey(poolKey)) 
                continue;

            Array<int> poolIds = new();

            foreach (Variant idVariant in dict[poolKey].AsGodotArray()) poolIds.Add((int)idVariant.AsDouble());

            Array<Quest> questsCopy = new(quests);

            foreach (Quest quest in questsCopy)
            {
                if (!poolIds.Contains(quest.Id)) 
                    continue;

                pool.AddQuest(quest);
                quests.Remove(quest);
            }
        }
    }

    public Dictionary SerializeQuests(StringName? pool = default)
    {
        Array<Quest> quests = [];
        if (string.IsNullOrEmpty(pool?.ToString()))
        {
            foreach (QuestPool questPool in GetAllPools()) quests.AddRange(questPool.GetAllQuests());
        }
        else
        {
            QuestPool questPool = GetPool(pool);
            if (questPool == null) 
                return new ();

            quests.AddRange(questPool.GetAllQuests());
        }

        Dictionary questDictionary = new();

        foreach (Quest quest in quests) questDictionary[quest.Id.ToString()] = quest.Serialize();

        return questDictionary;
    }

    public Error DeserializeQuests(Dictionary data, StringName ?pool = default)
    {
        Array<Quest> quests = [];
        if (string.IsNullOrEmpty(pool?.ToString()))
        {
            foreach (QuestPool questPool in GetAllPools()) quests.AddRange(questPool.GetAllQuests());
        }
        else
        {
            QuestPool questPool = GetPool(pool);
            if (questPool == null) 
                return Error.DoesNotExist;
            quests.AddRange(questPool.GetAllQuests());
        }

        foreach (Quest quest in quests)
        {
            Dictionary questData = data.GetValueOrDefault( quest.Id.ToString(), new Dictionary()).AsGodotDictionary();
            quest.Deserialize(questData);
        }

        return Error.Ok;
    }
}