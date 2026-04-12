using NodeCanvas.Framework;

public class CloseDialogueAction : ActionTask
{
    protected override void OnExecute()
    {
        // 1. 关闭 UI
        if (DialogueUIController.Instance != null)
        {
            DialogueUIController.Instance.HideDialogue();
        }

        // 2. 通知 NPC 结束对话状态（解锁高亮，恢复移动）
        // Agent 也就是挂载对话树控制器的 NPC 
        var npc = agent.GetComponent<NPC>();
        if (npc != null)
        {
            npc.EndConversation();
        }

        EndAction(true);
    }
}