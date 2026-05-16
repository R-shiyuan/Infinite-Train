using System.Collections.Generic;
using UnityEngine;

public enum PlotStepType
{
    Dialogue,
    MiniGame,
    UnlockNPC,
    UnlockItem,
    FinishNPC
}

[System.Serializable]
public class PlotStep
{
    [Header("节点ID（调试用）")]
    public string stepID;

    [Header("执行类型")]
    public PlotStepType stepType;

    [Header("内容ID")]
    public string plotID;
    public string miniGameID;

    [Header("解锁Key")]
    public string unlockKey;

    [Header("条件")]
    public List<Condition> conditions = new List<Condition>();

    [Header("效果")]
    public List<Effect> effects = new List<Effect>();
}