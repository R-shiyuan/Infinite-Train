using UnityEngine;
using System;
using System.Collections.Generic;

public class DialogueBridge : MonoBehaviour
{
    public static DialogueBridge Instance { get; private set; }

    private List<DialogueRow> currentPlotRows;
    private int currentIndex = 0;
    private Action onPlotComplete;

    private bool isPlaying = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //========================================================
    // 播放剧情入口
    //========================================================
    public void PlayPlot(string plotID, Action callback)
    {
        if (isPlaying)
        {
            Debug.LogWarning("DialogueBridge: 当前正在播放剧情，忽略新请求");
            return;
        }

        isPlaying = true;
        onPlotComplete = callback;

        if (CSVManager.Instance == null)
        {
            Debug.LogError("CSVManager 不存在");
            FinishSafe();
            return;
        }

        currentPlotRows = CSVManager.Instance.GetPlot(plotID);

        if (currentPlotRows == null || currentPlotRows.Count == 0)
        {
            Debug.LogError("找不到 PlotID: " + plotID);
            FinishSafe();
            return;
        }

        currentIndex = 0;

        DisplayCurrentLine();
    }

    //========================================================
    // 下一句（由 VNDialogueUI 调用）
    //========================================================
    public void Next()
    {
        if (!isPlaying) return;

        currentIndex++;

        if (currentPlotRows == null || currentIndex >= currentPlotRows.Count)
        {
            FinishDialogue();
            return;
        }

        DisplayCurrentLine();
    }

    //========================================================
    // 显示当前行
    //========================================================
    void DisplayCurrentLine()
    {
        if (currentPlotRows == null || currentIndex < 0 || currentIndex >= currentPlotRows.Count)
        {
            FinishDialogue();
            return;
        }

        DialogueRow row = currentPlotRows[currentIndex];

        Debug.Log($"[{row.actorName}] {row.text}");

        if (VNDialogueUI.Instance != null)
        {
            VNDialogueUI.Instance.ShowDialogue(row);
        }
        else
        {
            Debug.LogError("VNDialogueUI 不存在");
        }
    }

    //========================================================
    // 正常结束
    //========================================================
    void FinishDialogue()
    {
        Debug.Log("剧情结束");

        if (VNDialogueUI.Instance != null)
        {
            VNDialogueUI.Instance.HideDialogue();
        }

        onPlotComplete?.Invoke();

        Cleanup();
    }

    //========================================================
    // 异常安全结束
    //========================================================
    void FinishSafe()
    {
        if (VNDialogueUI.Instance != null)
        {
            VNDialogueUI.Instance.HideDialogue();
        }

        onPlotComplete?.Invoke();

        Cleanup();
    }

    //========================================================
    // 清理状态
    //========================================================
    void Cleanup()
    {
        currentPlotRows = null;
        currentIndex = 0;
        onPlotComplete = null;
        isPlaying = false;
    }

    //========================================================
    // 可选：强制中断剧情（未来扩展用）
    //========================================================
    public void StopDialogue()
    {
        Debug.LogWarning("DialogueBridge: 强制中断剧情");

        Cleanup();

        if (VNDialogueUI.Instance != null)
        {
            VNDialogueUI.Instance.HideDialogue();
        }
    }
}