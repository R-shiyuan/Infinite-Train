using UnityEngine;
using NodeCanvas.DialogueTrees;

public class NPC : MonoBehaviour, Interactable
{
    [Header("引用")]
    public OutlineHandler outline;
    public DialogueTreeController dialogueController;

    [Header("交互设置")]
    public float activeDistance = 2.5f;

    [Header("剧情控制")]
    public bool canInteract = true;

    private bool isPlayerNearby = false;
    private bool isMouseOver = false;
    private bool isTalking = false;
    private Collider2D npcCollider;

    void Awake()
    {
        npcCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (PlayerController.Instance == null || npcCollider == null) return;

        // 1. 实时计算物理距离
        Collider2D playerCollider = PlayerController.Instance.GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            float currentDist = npcCollider.Distance(playerCollider).distance;
            isPlayerNearby = (currentDist <= activeDistance);
        }

        // 2. 实时同步高亮状态
        RefreshHighlightState();
    }

    private void OnMouseEnter() { isMouseOver = true; }
    private void OnMouseExit() { isMouseOver = false; }

    private void RefreshHighlightState()
    {
        if (outline != null)
        {
            bool shouldShow = isTalking || (isPlayerNearby && isMouseOver && canInteract);
            if (outline.gameObject.activeSelf != shouldShow)
            {
                outline.ShowOutline(shouldShow);
            }
        }
    }

    public void OnInteract()
    {
        // 交互判定
        if (!isPlayerNearby || !canInteract || isTalking)
        {
            return;
        }

        Debug.Log("进入对话模式");

        // 1. 状态锁定
        isTalking = true;

        // 2. 调用 UI 控制器开启全局对话 UI (仅开启面板，不再手动填参)
        if (DialogueUIController.Instance != null)
        {
            DialogueUIController.Instance.ShowDialogue("NPC名称", "这里是对话内容", null, true);
        }

        // 3. 锁定玩家移动
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.SetCanMove(false);
        }

        // 4. 触发 NodeCanvas 对话树
        // 之后由 DialogueUGUI (NodeCanvas自带) 或你自定义的接口去对接 CSV 数据
        if (dialogueController != null)
        {
            dialogueController.StartDialogue();
        }
    }

    public void EndConversation()
    {
        isTalking = false;

        // 1. 恢复控制
        if (outline != null) outline.ShowOutline(false);

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.SetCanMove(true);
        }

        // 2. 隐藏对话 UI
        if (DialogueUIController.Instance != null)
        {
            DialogueUIController.Instance.HideDialogue();
        }

        // 3. 触发消失检查
        PresenceController pc = GetComponent<PresenceController>();
        if (pc != null)
        {
            pc.CheckPresence();
        }
    }
}