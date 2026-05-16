using UnityEngine;
using System;

public class DialogueSequenceController : MonoBehaviour
{
    public static DialogueSequenceController Instance;

    private NPC currentNPC;
    private string currentPlotID;
    private Action onDialogueComplete;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartPlot(NPC npc, string plotID, Action onComplete)
    {
        if (npc == null) return;

        currentNPC = npc;
        currentPlotID = plotID;
        onDialogueComplete = onComplete;

        Transform window = FindNearestWindow(npc);

        Sprite memory = MemoryDatabase.Instance != null
            ? MemoryDatabase.Instance.GetMemory(npc)
            : null;

        if (window == null || memory == null)
        {
            StartDialogue();
            return;
        }

        CinemaTransitionManager.Instance.OnCinemaReady -= OnCinemaFinished;
        CinemaTransitionManager.Instance.OnCinemaReady += OnCinemaFinished;

        CinemaTransitionManager.Instance.Play(window, memory, null);
    }

    void OnCinemaFinished()
    {
        CinemaTransitionManager.Instance.OnCinemaReady -= OnCinemaFinished;
        StartDialogue();
    }

    void StartDialogue()
    {
        DialogueBridge.Instance.PlayPlot(currentPlotID, OnDialogueFinished);
    }

    void OnDialogueFinished()
    {
        if (currentNPC != null)
            currentNPC.EndConversation();

        onDialogueComplete?.Invoke();
        onDialogueComplete = null;
    }

    private Transform FindNearestWindow(NPC npc)
    {
        GameObject[] windows = GameObject.FindGameObjectsWithTag("Window");

        Transform nearest = null;
        float minDistSqr = float.MaxValue;

        Vector3 pos = npc.GetVisualCenterPosition();

        foreach (GameObject win in windows)
        {
            BoxCollider2D col = win.GetComponent<BoxCollider2D>();
            if (col == null) continue;

            Vector3 closest = col.bounds.ClosestPoint(pos);
            float d2 = (closest - pos).sqrMagnitude;

            if (d2 < minDistSqr)
            {
                minDistSqr = d2;
                nearest = win.transform;
            }
        }

        return nearest;
    }
}