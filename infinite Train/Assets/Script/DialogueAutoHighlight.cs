using UnityEngine;
using NodeCanvas.DialogueTrees; // 必须引用这个命名空间

public class DialogueAutoHighlight : MonoBehaviour
{
    private DialogueTreeController controller;

    void Awake()
    {
        controller = GetComponent<DialogueTreeController>();
    }

    void OnEnable()
    {
        // 监听对话开始时的说话事件
        DialogueTree.OnSubtitlesRequest += OnSubtitles;
    }

    void OnDisable()
    {
        // 记得取消监听，防止内存泄露
        DialogueTree.OnSubtitlesRequest -= OnSubtitles;
    }

    // 当任何一个 Statement 节点被触发时，这个函数会自动执行
    private void OnSubtitles(SubtitlesRequestInfo info)
    {
        // info.actor 是当前说话的人
        if (info.actor != null)
        {
            string speakerName = info.actor.name;

            // 如果说话的是 System，你可以选择让所有人都暗掉
            if (speakerName == "System")
            {
                PortraitManager.Instance.UpdatePortraits("None");
            }
            else
            {
                // 通知管理器高亮这个 Actor 的名字
                PortraitManager.Instance.UpdatePortraits(speakerName);
            }
        }
    }
}