using System.Collections.Generic;
using UnityEngine;

public class NPCPlotController : MonoBehaviour
{
    [Header("NPC唯一ID")]
    public string npcID;

    [Header("剧情节点")]
    public List<PlotStep> steps = new List<PlotStep>();

    private NPC npc;

    private void Awake()
    {
        npc = GetComponent<NPC>();
    }

    public void OnNPCClick()
    {
        EvaluateState();
    }

    // =========================================================
    // 核心：状态驱动决策
    // =========================================================
    public void EvaluateState()
    {
        foreach (var step in steps)
        {
            if (PlotConditionChecker.CanExecuteStep(this, step))
            {
                ExecuteStep(step);
                return;
            }
        }

        ShowBlockedHint();
    }

    // =========================================================
    // 执行节点
    // =========================================================
    void ExecuteStep(PlotStep step)
    {
        switch (step.stepType)
        {
            case PlotStepType.Dialogue:
                DialogueSequenceController.Instance.StartPlot(
                    npc,
                    step.plotID,
                    OnStepFinished
                );
                break;

            case PlotStepType.MiniGame:
                MiniGameManager.Instance.Play(
                    step.miniGameID,
                    OnStepFinished
                );
                break;

            case PlotStepType.UnlockNPC:
                GlobalManager.Instance.SetWorldState(step.unlockKey, true);
                OnStepFinished();
                break;

            case PlotStepType.UnlockItem:
                GlobalManager.Instance.SetWorldState(step.unlockKey, true);
                OnStepFinished();
                break;

            case PlotStepType.FinishNPC:
                GlobalManager.Instance.SetWorldState(npcID + "_finished", true);
                OnStepFinished();
                break;
        }
    }

    // =========================================================
    // Step完成回调（不再 +1 index）
    // =========================================================
    void OnStepFinished()
    {
        Debug.Log($"NPC {npcID} Step 完成");
    }

    // =========================================================
    // 阻塞提示
    // =========================================================
    void ShowBlockedHint()
    {
        Debug.Log($"NPC {npcID} 当前无可执行剧情");

    }
}