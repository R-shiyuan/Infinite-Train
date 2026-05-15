
using UnityEngine;
using System;

public class DialogueSequenceController : MonoBehaviour
{
    public static DialogueSequenceController Instance;

    private NPC currentNPC;

    private string currentPlotID;

    private Action onDialogueComplete;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    //====================================================
    // 外部入口
    //====================================================

    public void StartPlot(
        NPC npc,
        string plotID,
        Action onComplete
    )
    {
        if (npc == null)
            return;

        currentNPC = npc;

        currentPlotID = plotID;

        onDialogueComplete = onComplete;

        Transform window =
            FindNearestWindow(npc);

        Sprite memory = null;

        if (MemoryDatabase.Instance != null)
        {
            memory =
                MemoryDatabase.Instance.GetMemory(npc);
        }

        //------------------------------------------------
        // 没有车窗 or 没有回忆图
        //------------------------------------------------

        if (window == null || memory == null)
        {
            StartDialogue();

            return;
        }

        //------------------------------------------------
        // 播放电影效果
        //------------------------------------------------

        CinemaTransitionManager.Instance.OnCinemaReady -=
            OnCinemaFinished;

        CinemaTransitionManager.Instance.OnCinemaReady +=
            OnCinemaFinished;

        CinemaTransitionManager.Instance.Play(
            window,
            memory,
            null
        );
    }

    //====================================================
    // 电影结束
    //====================================================

    void OnCinemaFinished()
    {
        CinemaTransitionManager.Instance.OnCinemaReady -=
            OnCinemaFinished;

        StartDialogue();
    }

    //====================================================
    // 开始对话
    //====================================================

    void StartDialogue()
    {
        DialogueBridge.Instance.PlayPlot(
            currentPlotID,
            OnDialogueFinished
        );
    }

    //====================================================
    // 对话结束
    //====================================================

    void OnDialogueFinished()
    {
        //------------------------------------------------
        // 恢复NPC状态
        //------------------------------------------------

        if (currentNPC != null)
        {
            currentNPC.EndConversation();
        }

        //------------------------------------------------
        // 通知剧情步骤结束
        //------------------------------------------------

        onDialogueComplete?.Invoke();

        onDialogueComplete = null;
    }

    //====================================================
    // 找最近车窗
    //====================================================

    private Transform FindNearestWindow(
        NPC npc
    )
    {
        GameObject[] windows =
            GameObject.FindGameObjectsWithTag(
                "Window"
            );

        Transform nearest = null;

        float minDistSqr = float.MaxValue;

        Vector3 visualPos =
            npc.GetVisualCenterPosition();

        foreach (GameObject win in windows)
        {
            BoxCollider2D col =
                win.GetComponent<BoxCollider2D>();

            if (col == null)
                continue;

            Vector3 closestPoint =
                col.bounds.ClosestPoint(
                    visualPos
                );

            float d2 =
                (
                    closestPoint -
                    visualPos
                ).sqrMagnitude;

            if (d2 < minDistSqr)
            {
                minDistSqr = d2;

                nearest = win.transform;
            }
        }

        return nearest;
    }
}

