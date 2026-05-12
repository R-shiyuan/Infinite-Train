using UnityEngine;
using System.Collections.Generic;
using System;

public class DialogueBridge : MonoBehaviour
{
    public static DialogueBridge Instance { get; private set; }

    private List<DialogueRow> currentPlotRows;

    private int currentIndex = 0;

    private Action onPlotComplete;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //========================================================
    // 播放剧情
    //========================================================

    public void PlayPlot(string plotID, Action callback)
    {
        Debug.Log("===== 开始播放剧情 =====");

        onPlotComplete = callback;

        if (CSVManager.Instance == null)
        {
            Debug.LogError("CSVManager 不存在");

            callback?.Invoke();

            return;
        }

        currentPlotRows =
            CSVManager.Instance.GetPlot(plotID);

        if (
            currentPlotRows == null ||
            currentPlotRows.Count == 0
        )
        {
            Debug.LogError("找不到 PlotID : " + plotID);

            callback?.Invoke();

            return;
        }

        currentIndex = 0;

        DisplayCurrentLine();
    }

    //========================================================
    // 下一句
    //========================================================

    public void Next()
    {
        currentIndex++;

        if (currentIndex >= currentPlotRows.Count)
        {
            FinishDialogue();

            return;
        }

        DisplayCurrentLine();
    }

    //========================================================
    // 显示当前句
    //========================================================

    void DisplayCurrentLine()
    {
        DialogueRow row =
            currentPlotRows[currentIndex];

        Debug.Log($"[{row.actorName}] {row.text}");

        if (VNDialogueUI.Instance != null)
        {
            VNDialogueUI.Instance.ShowDialogue(row);
        }
    }

    //========================================================
    // 结束
    //========================================================

    void FinishDialogue()
    {
        Debug.Log("剧情结束");

        if (VNDialogueUI.Instance != null)
        {
            VNDialogueUI.Instance.HideDialogue();
        }

        onPlotComplete?.Invoke();

        onPlotComplete = null;
    }
}