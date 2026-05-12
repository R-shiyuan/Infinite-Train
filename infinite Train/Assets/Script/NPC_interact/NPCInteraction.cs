using UnityEngine;
using NodeCanvas.DialogueTrees;

public class NPCInteraction : MonoBehaviour, Interactable
{
    [Header("对话树")]
    public DialogueTreeController dialogueTree;

    private NPC npc;

    void Awake()
    {
        npc = GetComponent<NPC>();
    }

    public void OnInteract()
    {
        if (DialogueSequenceController.Instance == null)
        {
            Debug.LogError("DialogueSequenceController 不存在");
            return;
        }

        if (npc == null)
        {
            Debug.LogError("NPC 不存在");
            return;
        }

        if (dialogueTree == null)
        {
            Debug.LogError("DialogueTreeController 未绑定");
            return;
        }

        if (!npc.CanInteract())
        {
            return;
        }

        npc.BeginConversation();

        Debug.Log("开始剧情序列");

        DialogueSequenceController.Instance.StartSequence(
            npc,
            dialogueTree
        );
    }
}