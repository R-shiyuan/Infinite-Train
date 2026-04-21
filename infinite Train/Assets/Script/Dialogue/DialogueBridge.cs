using UnityEngine;
using NodeCanvas.DialogueTrees;
using System.Collections.Generic;

public class DialogueBridge : MonoBehaviour
{
    private List<DialogueRow> currentPlotRows;
    private int currentIndex = 0;
    private SubtitlesRequestInfo currentInfo; // 缓存当前的 NodeCanvas 请求
    private bool isWaitingForClick = false;

    void Awake() { DialogueTree.OnSubtitlesRequest += OnSubtitlesRequest; }
    void OnDestroy() { DialogueTree.OnSubtitlesRequest -= OnSubtitlesRequest; }

    void OnSubtitlesRequest(SubtitlesRequestInfo info)
    {
        currentInfo = info; // 拦截请求，暂不调用 info.Continue()
        string plotID = info.statement.text;

        if (currentPlotRows == null || currentPlotRows[0].plotID != plotID)
        {
            currentPlotRows = CSVManager.Instance.GetPlot(plotID);
            currentIndex = 0;
        }

        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (currentPlotRows != null && currentIndex < currentPlotRows.Count)
        {
            DialogueRow row = currentPlotRows[currentIndex];

            if (row.plotID == "END")
            {
                FinishDialogue();
                return;
            }

            // 更新 UI
            DialogueUIController.Instance.ShowDialogue(row.actorName, row.content, null, true);
            currentIndex++;
            isWaitingForClick = true;
        }
        else
        {
            FinishDialogue();
        }
    }

    public void Proceed() // 供 UI 按钮调用的方法
    {
        if (isWaitingForClick)
        {
            isWaitingForClick = false;
            DisplayNextLine(); // 显示下一行
        }
        else if (currentInfo != null)
        {
            currentInfo.Continue(); // 全部播完，放行
            currentInfo = null;
        }
    }

    void FinishDialogue()
    {
        DialogueUIController.Instance.HideDialogue();
        if (currentInfo != null) currentInfo.Continue();
        currentInfo = null;
    }
}