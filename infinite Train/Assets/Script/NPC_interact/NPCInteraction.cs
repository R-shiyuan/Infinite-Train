using UnityEngine;

public class NPCInteraction : MonoBehaviour, Interactable
{
    private NPC npc;
    private NPCPlotController plotController;

    void Awake()
    {
        npc = GetComponent<NPC>();
        plotController = GetComponent<NPCPlotController>();
    }

    public void OnInteract()
    {
        if (npc == null)
        {
            Debug.LogError("NPC 不存在");
            return;
        }

        if (!npc.CanInteract())
        {
            return;
        }

        if (plotController == null)
        {
            Debug.LogError("NPCPlotController 不存在");
            return;
        }

        npc.BeginConversation();

        plotController.OnNPCClick();
    }
}