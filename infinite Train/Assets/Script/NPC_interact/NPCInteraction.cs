using UnityEngine;
using NodeCanvas.DialogueTrees;

public class NPCInteraction : MonoBehaviour, Interactable
{
    [Header("¶Ô»°Ê÷")]
    public DialogueTreeController dialogueTree;

    private NPC npc;

    void Awake()
    {
        npc = GetComponent<NPC>();
    }

  
    public void OnInteract()
    {
        if (DialogueSequenceController.Instance == null)
            return;

        if (npc == null || dialogueTree == null)
            return;

       
        DialogueSequenceController.Instance.StartSequence(npc, dialogueTree);
    }
}