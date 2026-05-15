using UnityEngine;

public enum EffectType
{
    WorldStateOn,
    WorldStateOff
}

[System.Serializable]
public class Effect
{
    public EffectType type;
    public string key;
}