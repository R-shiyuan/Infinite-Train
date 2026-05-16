using UnityEngine;

public static class PlotConditionChecker
{
    public static bool CanExecuteStep(NPCPlotController npc, PlotStep step)
    {
        if (step == null) return false;

        foreach (var condition in step.conditions)
        {
            if (!Check(condition, npc))
                return false;
        }

        return true;
    }

    static bool Check(Condition c, NPCPlotController npc)
    {
        switch (c.type)
        {
            // =====================================================
            case ConditionType.WorldState:
                return GlobalManager.Instance.GetWorldState(c.key);

            // =====================================================
            case ConditionType.NPCStep:
                return PlotStateManager.Instance.GetStep(c.key) >= c.requiredInt;

            // =====================================================
            case ConditionType.MiniGameComplete:
                return MiniGameManager.Instance.IsCompleted(c.key);
        }

        return false;
    }
}