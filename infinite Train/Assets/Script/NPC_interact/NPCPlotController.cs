using System.Collections.Generic;
using UnityEngine;

public class NPCPlotController : MonoBehaviour
{
    [Header("NPC唯一ID")]
    public string npcID;

    [Header("剧情步骤")]
    public List<PlotStep> steps = new List<PlotStep>();

    [Header("当前步骤")]
    public int currentStepIndex = 0;

    private NPC npc;

    private void Awake()
    {
        npc = GetComponent<NPC>();
    }

    //====================================================
    // 对外入口
    //====================================================

    public void ExecuteCurrentStep()
    {
        if (currentStepIndex >= steps.Count)
        {
            Debug.Log("剧情已全部完成");
            return;
        }

        PlotStep step = steps[currentStepIndex];

        switch (step.stepType)
        {
            case PlotStepType.Dialogue:
                ExecuteDialogue(step);
                break;

            case PlotStepType.MiniGame:
                ExecuteMiniGame(step);
                break;

            case PlotStepType.WaitCondition:
                ExecuteWaitCondition(step);
                break;

            case PlotStepType.UnlockNPC:
                ExecuteUnlock(step);
                break;

            case PlotStepType.UnlockItem:
                ExecuteUnlock(step);
                break;

            case PlotStepType.FinishNPC:
                ExecuteFinishNPC(step);
                break;
        }
    }

    //====================================================
    // Dialogue
    //====================================================

    void ExecuteDialogue(PlotStep step)
    {
        Debug.Log("播放对话: " + step.plotID);

        DialogueSequenceController.Instance.StartPlot(
            npc,
            step.plotID,
            OnStepFinished
        );
    }

    //====================================================
    // MiniGame
    //====================================================

    void ExecuteMiniGame(PlotStep step)
    {
        Debug.Log("开始小游戏: " + step.miniGameID);

        MiniGameManager.Instance.Play(
            step.miniGameID,
            OnStepFinished
        );
    }

    //====================================================
    // WaitCondition
    //====================================================

    void ExecuteWaitCondition(PlotStep step)
    {
        bool ok =
            GlobalManager.Instance.GetWorldState(
                step.requiredWorldState
            );

        if (ok)
        {
            Debug.Log("条件已满足");

            OnStepFinished();
        }
        else
        {
            Debug.Log("条件未满足");

            // 不推进
            // 玩家需要之后再次点击
        }
    }

    //====================================================
    // Unlock
    //====================================================

    void ExecuteUnlock(PlotStep step)
    {
        GlobalManager.Instance.SetWorldState(
            step.unlockKey,
            true
        );

        Debug.Log("解锁: " + step.unlockKey);

        OnStepFinished();
    }

    //====================================================
    // Finish NPC
    //====================================================

    void ExecuteFinishNPC(PlotStep step)
    {
        Debug.Log("NPC剧情完成");

        PresenceController pc =
            GetComponent<PresenceController>();

        if (pc != null)
        {
            GlobalManager.Instance.SetWorldState(
                pc.finishedKey,
                true
            );
        }

        OnStepFinished();
    }

    //====================================================
    // 步骤结束
    //====================================================

    void OnStepFinished()
    {
        currentStepIndex++;

        Debug.Log(
            $"进入下一步骤: {currentStepIndex}"
        );
    }
}

