
using UnityEngine;

public enum PlotStepType
{
    Dialogue,
    MiniGame,
    WaitCondition,
    UnlockNPC,
    UnlockItem,
    FinishNPC
}

[System.Serializable]
public class PlotStep
{
    [Header("≤Ω÷Ë¿‡–Õ")]
    public PlotStepType stepType;

    //====================================================
    // Dialogue
    //====================================================

    public string plotID;

    //====================================================
    // MiniGame
    //====================================================

    public string miniGameID;

    //====================================================
    // WaitCondition
    //====================================================

    public string requiredWorldState;

    //====================================================
    // Unlock
    //====================================================

    public string unlockKey;
}

