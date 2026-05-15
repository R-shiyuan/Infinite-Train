using System.Collections.Generic;
using UnityEngine;

public class PlotStateManager : MonoBehaviour
{
    public static PlotStateManager Instance;

    private Dictionary<string, int> npcStep = new();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetStep(string npcID)
    {
        if (!npcStep.ContainsKey(npcID))
            npcStep[npcID] = 0;

        return npcStep[npcID];
    }

    public void SetStep(string npcID, int step)
    {
        npcStep[npcID] = step;
    }
}