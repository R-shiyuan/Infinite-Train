using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour
{
    [Header("引用")]
    public OutlineHandler outline;

    [Header("交互设置")]
    public float activeDistance = 2.5f;

    [Header("剧情控制")]
    public bool canInteract = true;

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
        CheckPlayerDistance();

        RefreshHighlightState();
    }

    void CheckPlayerDistance()
    {
        if (PlayerController.Instance == null)
            return;

        if (npcCollider == null)
            return;

        Collider2D playerCollider =
            PlayerController.Instance.GetComponent<Collider2D>();

        if (playerCollider == null)
            return;

        float currentDist =
            npcCollider.Distance(playerCollider).distance;

        isPlayerNearby = currentDist <= activeDistance;
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
        if (outline == null)
            return;

        bool shouldShow =
            isTalking ||
            (isPlayerNearby && isMouseOver && canInteract);

        outline.ShowOutline(shouldShow);
    }

    //========================================================
    // 对外接口
    //========================================================

    public bool CanInteract()
    {
        return
            canInteract &&
            isPlayerNearby &&
            !isTalking;
    }

    public void BeginConversation()
    {
        isTalking = true;

        RefreshHighlightState();

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.SetCanMove(false);
        }
    }

    public void EndConversation()
    {
        isTalking = false;

        RefreshHighlightState();

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.SetCanMove(true);
        }

        // 关闭电影效果
        if (CinemaTransitionManager.Instance != null)
        {
            CinemaTransitionManager.Instance.End();
        }

        // Presence 刷新
        PresenceController pc =
            GetComponent<PresenceController>();

        if (pc != null)
        {
            StartCoroutine(
                DelayedPresenceCheck(pc)
            );
        }
    }

    IEnumerator DelayedPresenceCheck(
        PresenceController pc
    )
    {
        yield return new WaitForSeconds(0.2f);

        if (pc != null)
        {
            pc.CheckPresence();
        }
    }


    public Vector3 GetVisualCenterPosition()
    {
        if (visualCenter != null)
            return visualCenter.position;

        SpriteRenderer sr =
            GetComponent<SpriteRenderer>();

        if (sr != null)
            return sr.bounds.center;

        Collider2D col =
            GetComponent<Collider2D>();

        if (col != null)
            return col.bounds.center;

        return transform.position;
    }
}