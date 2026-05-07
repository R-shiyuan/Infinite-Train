using UnityEngine;
using NodeCanvas.DialogueTrees;
using System.Collections.Generic;

public class DialogueBridge : MonoBehaviour
{
    private List<DialogueRow> currentPlotRows;
    private int currentIndex = 0;
    private SubtitlesRequestInfo currentInfo;
    private bool isWaitingForClick = false;

    // 立绘资源存放的基础路径（Assets/Resources/Portraits/）
    private const string PORTRAIT_PATH = "Portraits/";

    void Awake() { DialogueTree.OnSubtitlesRequest += OnSubtitlesRequest; }
    void OnDestroy() { DialogueTree.OnSubtitlesRequest -= OnSubtitlesRequest; }

    void OnSubtitlesRequest(SubtitlesRequestInfo info)
    {
        currentInfo = info;
        string plotID = info.statement.text; // 获取对话树节点里填写的 ID

        // 加载 CSV 数据
        if (currentPlotRows == null || currentPlotRows.Count == 0 || currentPlotRows[0].plotID != plotID)
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

            // --- 核心逻辑：动态加载立绘 ---
            // 拼写规则：角色ID_表情名 (例如: Ray_Normal)
            string spriteName = row.actorID + "_" + row.express;
            Sprite portraitSprite = Resources.Load<Sprite>(PORTRAIT_PATH + spriteName);

            if (portraitSprite == null)
            {
                Debug.LogWarning($"[DialogueBridge] 未找到立绘资源: {PORTRAIT_PATH + spriteName}，请检查 Resources 文件夹。");
            }

            // 判断位置：CSV 填 "Left" 则在左，否则在右
            bool isLeft = (row.pos.ToLower() == "left");

            // 更新 UI 展示
            DialogueUIController.Instance.ShowDialogue(
                row.actorName,
                row.content,
                portraitSprite,
                isLeft
            );

            currentIndex++;
            isWaitingForClick = true;
        }
        else
        {
            FinishDialogue();
        }
    }

    public void Proceed()
    {
        if (isWaitingForClick)
        {
            isWaitingForClick = false;
            DisplayNextLine();
        }
        else if (currentInfo != null)
        {
            FinishDialogue();
        }
    }

    void FinishDialogue()
    {
        DialogueUIController.Instance.HideDialogue();
        if (currentInfo != null) currentInfo.Continue();
        currentInfo = null;
        currentPlotRows = null; // 清空缓存，防止下次对话冲突
    }
}