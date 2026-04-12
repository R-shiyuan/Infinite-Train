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
    private bool isTalking = false; // 新增：标记是否正在对话
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

    private void OnMouseEnter()
    {
        isMouseOver = true;
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
    }

    private void RefreshHighlightState()
    {
        if (outline != null)
        {
            // 逻辑修改：
            // 如果正在对话 -> 强制高亮（常亮）
            // 如果没在对话 -> 只有 范围内 + 悬停 + 允许交互 才会亮
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

        Debug.Log("进入对话模式，锁定高亮并显示 UI");

        // 1. 状态锁定
        isTalking = true;

        // 2. 调用 UI 控制器显示所有 UI（包含 Dimmer 等）
        if (DialogueUIController.Instance != null)
        {
            DialogueUIController.Instance.ShowDialogue();
        }

        // 3. 锁定玩家移动
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.SetCanMove(false);
        }

        // 4. 触发对话
        if (dialogueController != null)
        {
            dialogueController.StartDialogue();
        }
    }

    // 新增：由 CloseDialogueAction 脚本在对话结束时调用
    public void EndConversation()
    {
        isTalking = false;
        if (outline != null) outline.ShowOutline(false);

        // 恢复玩家移动
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.SetCanMove(true);
        }
    }
}