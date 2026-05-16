using UnityEngine;

public enum ConditionType
{
    WorldState,
    NPCStep,
    MiniGameComplete
}

[System.Serializable]
public class Condition
{
    public ConditionType type;

    public string key;
    public int requiredInt;
}