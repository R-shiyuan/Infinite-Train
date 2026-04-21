using UnityEngine;
using NodeCanvas.DialogueTrees; // 这里你的程序集引用应该没问题了
using ParadoxNotion;

public class DialogueBridge : MonoBehaviour
{
    void Awake()
    {
        // 关键点：订阅事件，而不是修改源码！
        DialogueTree.OnSubtitlesRequest += OnSubtitlesRequest;
    }

    void OnDestroy()
    {
        DialogueTree.OnSubtitlesRequest -= OnSubtitlesRequest;
    }

    void OnSubtitlesRequest(SubtitlesRequestInfo info)
    {
        // 现在你的所有业务逻辑都可以写在这里，完全不用碰插件原文件
        string plotID = info.statement.text;
        var dialogueRows = CSVManager.Instance.GetPlot(plotID);

        if (dialogueRows != null && dialogueRows.Count > 0)
        {
            var row = dialogueRows[0];
            // 调用你自己的 UI 控制器
            DialogueUIController.Instance.ShowDialogue(row.actorName, row.content, null, true);
        }
    }
}