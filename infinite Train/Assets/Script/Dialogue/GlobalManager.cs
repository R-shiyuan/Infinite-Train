using System.Collections.Generic;
using UnityEngine;
using System;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance { get; private set; }

    // 事件：用于广播状态改变
    public event Action<string, bool> OnWorldStateChanged;

    private Dictionary<string, bool> worldState = new Dictionary<string, bool>();
    private Dictionary<string, int> personalProgress = new Dictionary<string, int>();
    private Dictionary<string, bool> miniGameStatus = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetWorldState(string key, bool value)
    {
        worldState[key] = value;
        Debug.Log($">>> [GlobalManager] 状态已更新: {key} = {value}");

        // 关键：必须触发这个事件，PresenceController 才能收到
        if (OnWorldStateChanged != null)
        {
            OnWorldStateChanged.Invoke(key, value);
        }
    }

    public bool GetWorldState(string key, bool defaultValue = false)
        => worldState.ContainsKey(key) ? worldState[key] : defaultValue;

    // --- 这里是修复报错的关键 ---
    public void TriggerTestScenario()
    {
        Debug.Log(">>> [TestTool] 按下了 T 键，正在执行状态变更...");
        SetWorldState("RAY_C1_Finished", true);
        Debug.Log(">>> [测试] 已强制将 RAY_C1_Finished 设为 True！");
    }

    public bool IsStateFinished(string key)
    {
        return worldState.ContainsKey(key) && worldState[key];
    }

    public void SetProgress(string npcID, int stage) => personalProgress[npcID] = stage;
    public int GetProgress(string npcID)
        => personalProgress.ContainsKey(npcID) ? personalProgress[npcID] : 0;

    public void SetGameStatus(string gameID, bool isDone) => miniGameStatus[gameID] = isDone;
    public bool GetGameStatus(string gameID)
        => miniGameStatus.ContainsKey(gameID) ? miniGameStatus[gameID] : false;
}