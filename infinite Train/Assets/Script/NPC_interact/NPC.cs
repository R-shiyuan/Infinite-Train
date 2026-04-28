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

    [Header("车窗电影")]
    public Sprite memorySprite;

    [Header("视觉中心（用于距离计算，不指定则自动使用 SpriteRenderer/Collider 中心）")]
    public Transform visualCenter;

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

        Collider2D playerCollider = PlayerController.Instance.GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            float currentDist = npcCollider.Distance(playerCollider).distance;
            isPlayerNearby = (currentDist <= activeDistance);
        }

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
                outline.ShowOutline(shouldShow);
        }
    }

    private Vector3 GetVisualCenterPosition()
    {
        if (visualCenter != null)
            return visualCenter.position;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            return sr.bounds.center;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            return col.bounds.center;

        return transform.position;
    }

    private Transform FindNearestWindow()
    {
        GameObject[] windows = GameObject.FindGameObjectsWithTag("Window");
        Transform nearest = null;
        float minDistSqr = float.MaxValue;
        Vector3 visualPos = GetVisualCenterPosition();

        foreach (var win in windows)
        {
            BoxCollider2D winCol = win.GetComponent<BoxCollider2D>();
            if (winCol == null) continue;

            Vector3 closestPoint = winCol.bounds.ClosestPoint(visualPos);
            float d2 = (closestPoint - visualPos).sqrMagnitude;
            if (d2 < minDistSqr)
            {
                minDistSqr = d2;
                nearest = win.transform;
            }
        }
        return nearest;
    }

    private void OnCinemaFinished()
    {
        if (CinemaTransitionManager.Instance != null)
            CinemaTransitionManager.Instance.OnCinemaReady -= OnCinemaFinished;

        Debug.Log("车窗动画完成，打开对话UI");
        if (DialogueUIController.Instance != null)
            DialogueUIController.Instance.ShowDialogue("NPC名字", "这里是对话内容");
        if (dialogueController != null)
            dialogueController.StartDialogue();
    }

    private void StartDialogue()
    {
        if (DialogueUIController.Instance != null)
            DialogueUIController.Instance.ShowDialogue("NPC名字", "这里是对话内容");
        if (dialogueController != null)
            dialogueController.StartDialogue();
    }

    public void OnInteract()
    {
        if (!isPlayerNearby || !canInteract || isTalking) return;

        Transform nearestWindow = FindNearestWindow();

        if (CinemaTransitionManager.Instance != null && nearestWindow != null && memorySprite != null)
        {
            Debug.Log("播放车窗电影效果");
            isTalking = true;
            if (PlayerController.Instance != null)
                PlayerController.Instance.SetCanMove(false);

            CinemaTransitionManager.Instance.OnCinemaReady += OnCinemaFinished;
            CinemaTransitionManager.Instance.Play(nearestWindow, memorySprite);
        }
        else
        {
            Debug.Log("直接打开对话（无车窗或回忆图）");
            StartDialogue();
        }
    }

    public void EndConversation()
    {
        isTalking = false;

        if (outline != null) outline.ShowOutline(false);
        if (PlayerController.Instance != null)
            PlayerController.Instance.SetCanMove(true);
        if (DialogueUIController.Instance != null)
            DialogueUIController.Instance.HideDialogue();
        if (CinemaTransitionManager.Instance != null)
            CinemaTransitionManager.Instance.End();

        PresenceController pc = GetComponent<PresenceController>();
        if (pc != null)
            pc.CheckPresence();
    }
}