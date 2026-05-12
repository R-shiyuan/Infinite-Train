using UnityEngine;
using NodeCanvas.DialogueTrees;

public class DialogueSequenceController : MonoBehaviour
{
    public static DialogueSequenceController Instance;

    private NPC currentNPC;
    private DialogueTreeController currentDialogue;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartSequence(NPC npc, DialogueTreeController dialogueTree)
    {
        if (npc == null || dialogueTree == null)
            return;

        currentNPC = npc;
        currentDialogue = dialogueTree;

        Transform window = FindNearestWindow(npc);

        Sprite memory = null;

        if (MemoryDatabase.Instance != null)
        {
            memory = MemoryDatabase.Instance.GetMemory(npc);
        }

        // 没有电影效果
        if (window == null || memory == null)
        {
            StartDialogue();
            return;
        }

        // 防止重复注册
        CinemaTransitionManager.Instance.OnCinemaReady -= OnCinemaFinished;
        CinemaTransitionManager.Instance.OnCinemaReady += OnCinemaFinished;

        CinemaTransitionManager.Instance.Play(
            window,
            memory,
            dialogueTree
        );
    }

    void OnCinemaFinished()
    {
        CinemaTransitionManager.Instance.OnCinemaReady -= OnCinemaFinished;

        StartDialogue();
    }

    void StartDialogue()
    {
        if (currentDialogue == null)
            return;

        if (!currentDialogue.graph.isRunning)
        {
            currentDialogue.StartDialogue();
        }
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