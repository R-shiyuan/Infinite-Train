using UnityEngine;
using System.Collections.Generic;
using System;

public class DialogueBridge : MonoBehaviour
{
    public static DialogueBridge Instance { get; private set; }
    private List<DialogueRow> currentPlotRows;
    private int currentIndex = 0;
    private Action onPlotComplete;

    private void Awake() { Instance = this; }

    // 供 NodeCanvas 的 PlayCSVPlot 脚本调用
    public void PlayPlot(string plotID, Action callback)
    {
        Debug.Log("===== 开始播放剧情 =====");

        Debug.Log("请求播放 PlotID: [" + plotID + "]");

        onPlotComplete = callback;

        if (CSVManager.Instance == null)
        {
            Debug.LogError("CSVManager.Instance 为空！");
            return;
        }

        currentPlotRows = CSVManager.Instance.GetPlot(plotID);

        if (currentPlotRows == null)
        {
            Debug.LogError("找不到 PlotID: [" + plotID + "]");
        }
        else
        {
            Debug.Log("成功读取剧情，行数: " + currentPlotRows.Count);

            foreach (var row in currentPlotRows)
            {
                Debug.Log("读取到内容: " + row.text);
            }
        }

        if (currentPlotRows == null || currentPlotRows.Count == 0)
        {
            Debug.LogError("剧情为空，直接结束");
            onPlotComplete?.Invoke();
            return;
        }

        currentIndex = 0;

        Debug.Log("准备显示第一句对白");

        DisplayNextLine();
    }

    // --- 核心修复：加回 Proceed 方法，解决编译报错 ---
    public void Proceed()
    {
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        Debug.Log("DisplayNextLine 被调用");
        if (currentPlotRows != null && currentIndex < currentPlotRows.Count)
        {
            DialogueRow row = currentPlotRows[currentIndex];

            // 路径：Resources/Portraits/角色名_表情名
            Sprite portrait = Resources.Load<Sprite>("Portraits/" + row.actorID + "_" + row.express);

            // 转换参数：将 CSV 的 "Left" 字符串转为 UI 需要的 bool
            bool isLeft = row.pos.ToLower() == "left";

            if (DialogueUIController.Instance != null)
            {
                DialogueUIController.Instance.ShowDialogue(row.actorName, row.text, portrait, isLeft);
            }

            currentIndex++;
        }
        else { FinishDialogue(); }
    }

    private void FinishDialogue()
    {
        if (DialogueUIController.Instance != null)
            DialogueUIController.Instance.HideDialogue();

        onPlotComplete?.Invoke();
        onPlotComplete = null;
    }
}