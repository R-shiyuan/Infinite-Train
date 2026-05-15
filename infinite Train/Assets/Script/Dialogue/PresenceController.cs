using UnityEngine;
using System.Collections;

public class PresenceController : MonoBehaviour
{
    [Header("设置条件")]
    public string objectID;
    public string finishedKey;
    public string unlockedKey;

    private bool hasProcessedDisappearance = false;
    private bool isFading = false;
    private Collider2D myCollider;
    private SpriteRenderer sr;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private IEnumerator Start()
    {
        yield return null;

        if (GlobalManager.Instance != null)
        {
            GlobalManager.Instance.OnWorldStateChanged += HandleStateChanged;

            // --- 核心优化点：初始化时的瞬时检查 ---
            bool isFinished = GlobalManager.Instance.GetWorldState(finishedKey);
            if (isFinished)
            {
                // 如果已完成，直接隐藏，标记为已处理，不播放淡出
                hasProcessedDisappearance = true;
                if (sr != null) sr.enabled = false;
                if (myCollider != null) myCollider.enabled = false;
            }
            else
            {
                CheckPresence();
            }
        }
    }

    private void OnDisable()
    {
        if (GlobalManager.Instance != null)
            GlobalManager.Instance.OnWorldStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(string key, bool value)
    {
        if (key == finishedKey || key == unlockedKey)
        {
            CheckPresence();
        }
    }

    public void CheckPresence()
    {
        bool isFinished = GlobalManager.Instance.GetWorldState(finishedKey);
        bool isUnlocked = GlobalManager.Instance.GetWorldState(unlockedKey, true);

        if (isFinished)
        {
            // 如果还没淡出过，则触发淡出；否则直接保持隐藏
            if (!hasProcessedDisappearance && !isFading)
            {
                hasProcessedDisappearance = true;
                StartCoroutine(FadeOutAndDisable());
            }
        }
        else
        {
            // 重置状态
            hasProcessedDisappearance = false;
            if (myCollider != null) myCollider.enabled = isUnlocked;
            if (sr != null) sr.enabled = isUnlocked;
            gameObject.SetActive(true);
        }
    }

    private IEnumerator FadeOutAndDisable()
    {
        isFading = true;
        float duration = 1.0f;
        float elapsed = 0;
        Color startColor = sr.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsed / duration);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        if (sr != null) sr.enabled = false;
        if (myCollider != null) myCollider.enabled = false;
        isFading = false;
    }
}