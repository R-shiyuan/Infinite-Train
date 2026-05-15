using UnityEngine;
using NodeCanvas.DialogueTrees;

public class DialogueSequenceController : MonoBehaviour
{
    public static DialogueSequenceController Instance;

    private NPC currentNPC;
    private DialogueTreeController currentDialogue;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartSequence(NPC npc, DialogueTreeController dialogueTree)
    {
        if (npc == null || dialogueTree == null) return;

        currentNPC = npc;
        currentDialogue = dialogueTree;

        Transform window = FindNearestWindow(npc);
        Sprite memory = null;

        if (MemoryDatabase.Instance != null)
            memory = MemoryDatabase.Instance.GetMemory(npc);

        if (window == null || memory == null)
        {
            StartDialogue();
            return;
        }

        CinemaTransitionManager.Instance.OnCinemaReady -= OnCinemaFinished;
        CinemaTransitionManager.Instance.OnCinemaReady += OnCinemaFinished;
        CinemaTransitionManager.Instance.Play(window, memory, dialogueTree);
    }

    void OnCinemaFinished()
    {
        CinemaTransitionManager.Instance.OnCinemaReady -= OnCinemaFinished;
        StartDialogue();
    }

    void StartDialogue()
    {
        if (currentDialogue != null && !currentDialogue.graph.isRunning)
            currentDialogue.StartDialogue();
    }

    // ========== 修改这里 ==========
    private Transform FindNearestWindow(NPC npc)
    {
        GameObject[] windows = GameObject.FindGameObjectsWithTag("Window");
        Transform nearest = null;
        float minDistSqr = float.MaxValue;

        // 使用 NPC 的视觉中心位置（你先前已实现 GetVisualCenterPosition）
        Vector3 visualPos = npc.GetVisualCenterPosition();

        foreach (GameObject win in windows)
        {
            BoxCollider2D col = win.GetComponent<BoxCollider2D>();
            if (col == null) continue;   // 必须有 Collider 才能计算边界

            // 计算视觉中心到车窗碰撞体表面的最近距离
            Vector3 closestPoint = col.bounds.ClosestPoint(visualPos);
            float d2 = (closestPoint - visualPos).sqrMagnitude;

            if (d2 < minDistSqr)
            {
                minDistSqr = d2;
                nearest = win.transform;
            }
        }
        return nearest;
    }
}