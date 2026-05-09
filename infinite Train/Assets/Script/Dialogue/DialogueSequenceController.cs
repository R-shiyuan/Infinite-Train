using UnityEngine;
using NodeCanvas.DialogueTrees;

public class DialogueSequenceController : MonoBehaviour
{
    public static DialogueSequenceController Instance;

    [Header("UI")]
    public GameObject dialogueCanvas;

    private NPC currentNPC;
    private DialogueTreeController currentDialogue;

    void Awake()
    {
        Instance = this;
    }

    // =========================
    // ⭐⭐⭐ 只保留这一种入口 ⭐⭐⭐
    // =========================
    public void StartSequence(NPC npc, DialogueTreeController dialogueTree)
    {
        currentNPC = npc;
        currentDialogue = dialogueTree;

        Transform window = FindNearestWindow(npc);
        Sprite memory = MemoryDatabase.Instance.GetMemory(npc);

        CinemaTransitionManager.Instance.Play(window, memory, dialogueTree);
    }

    private Transform FindNearestWindow(NPC npc)
    {
        GameObject[] windows = GameObject.FindGameObjectsWithTag("Window");

        Transform nearest = null;
        float min = float.MaxValue;

        Vector3 pos = npc.transform.position;

        foreach (var w in windows)
        {
            float d = (w.transform.position - pos).sqrMagnitude;
            if (d < min)
            {
                min = d;
                nearest = w.transform;
            }
        }

        return nearest;
    }
}