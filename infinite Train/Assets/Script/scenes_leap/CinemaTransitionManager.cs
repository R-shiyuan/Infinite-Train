using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CinemaTransitionManager : MonoBehaviour
{
    public static CinemaTransitionManager Instance;

    [Header("动画参数")]
    public float pushDuration = 1.2f;
    public float finalOrthoSize = 2.5f;
    public Vector3 cameraOffset = new Vector3(0, 0, -2f);
    [Range(0, 1)] public float maskAlpha = 0.8f;
    public AnimationCurve eleganceCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("荧幕最终尺寸（屏幕宽度的比例）")]
    [Range(0.3f, 1f)] public float targetScreenWidthRatio = 0.7f;
    public float targetAspect = 16f / 9f;   // 宽高比，可调整

    // 事件：动画完成，可显示对话UI
    public System.Action OnCinemaReady;

    // 内部变量
    private Camera mainCamera;
    private CameraFollow cameraFollow;
    private Vector3 originalCamPos;
    private float originalCamSize;
    private Canvas maskCanvas;
    private Image fullscreenMask;
    private Canvas screenCanvas;
    private Image windowScreen;
    private Transform targetWindow;
    private Sprite memorySprite;
    private bool isPlaying = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
            cameraFollow = mainCamera.GetComponent<CameraFollow>();
    }

    public void Play(Transform window, Sprite memory)
    {
        if (isPlaying)
        {
            Debug.LogWarning("动画已在进行，忽略新请求");
            return;
        }
        if (window == null || memory == null)
        {
            Debug.LogError("车窗或回忆图为空，直接触发对话");
            OnCinemaReady?.Invoke();
            return;
        }
        targetWindow = window;
        memorySprite = memory;
        StartCoroutine(PlayCoroutine());
    }

    public void End()
    {
        if (!isPlaying) return;
        StartCoroutine(ExitCoroutine());
    }

    private IEnumerator PlayCoroutine()
    {
        isPlaying = true;

        // 1. 记录原始状态
        originalCamPos = mainCamera.transform.position;
        originalCamSize = mainCamera.orthographicSize;
        if (cameraFollow != null) cameraFollow.enabled = false;
        if (PlayerController.Instance != null) PlayerController.Instance.SetCanMove(false);

        // 2. 创建 UI 层
        CreateMaskCanvas();
        CreateScreenCanvas();

        // 3. 设置回忆图
        windowScreen.sprite = memorySprite;
        windowScreen.color = new Color(1, 1, 1, 0);

        // 4. 计算起始窗口矩形（当前相机状态下的车窗投影）
        Rect startRect = GetWindowScreenRect(targetWindow);
        // 5. 计算目标矩形（屏幕中央，指定宽高比和宽度比例）
        float targetWidth = Screen.width * targetScreenWidthRatio;
        float targetHeight = targetWidth / targetAspect;
        Rect targetRect = new Rect(
            (Screen.width - targetWidth) / 2f,
            (Screen.height - targetHeight) / 2f,
            targetWidth,
            targetHeight
        );

        // 6. 目标相机参数
        Vector3 targetCamPos = targetWindow.position + cameraOffset;
        float targetCamSize = finalOrthoSize;

        float elapsed = 0f;
        while (elapsed < pushDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / pushDuration;
            float smoothT = eleganceCurve.Evaluate(t);

            // 镜头移动
            mainCamera.transform.position = Vector3.Lerp(originalCamPos, targetCamPos, smoothT);
            mainCamera.orthographicSize = Mathf.Lerp(originalCamSize, targetCamSize, smoothT);

            // 遮罩淡入
            if (fullscreenMask != null)
            {
                Color c = fullscreenMask.color;
                c.a = Mathf.Lerp(0, maskAlpha, smoothT);
                fullscreenMask.color = c;
            }

            // 荧幕变形：位置和大小从 startRect 插值到 targetRect
            float curX = Mathf.Lerp(startRect.x, targetRect.x, smoothT);
            float curY = Mathf.Lerp(startRect.y, targetRect.y, smoothT);
            float curW = Mathf.Lerp(startRect.width, targetRect.width, smoothT);
            float curH = Mathf.Lerp(startRect.height, targetRect.height, smoothT);
            SetWindowScreenRect(new Rect(curX, curY, curW, curH));

            // 荧幕淡入
            if (windowScreen != null)
            {
                Color c = windowScreen.color;
                c.a = Mathf.Lerp(0, 1, smoothT);
                windowScreen.color = c;
            }

            yield return null;
        }

        // 最终精确到位
        mainCamera.transform.position = targetCamPos;
        mainCamera.orthographicSize = targetCamSize;
        SetWindowScreenRect(targetRect);
        if (fullscreenMask != null) fullscreenMask.color = new Color(0, 0, 0, maskAlpha);
        if (windowScreen != null) windowScreen.color = Color.white;

        OnCinemaReady?.Invoke();
    }

    private IEnumerator ExitCoroutine()
    {
        float elapsed = 0f;
        Vector3 startCamPos = mainCamera.transform.position;
        float startCamSize = mainCamera.orthographicSize;
        float startMaskAlpha = fullscreenMask != null ? fullscreenMask.color.a : 0;
        float startScreenAlpha = windowScreen != null ? windowScreen.color.a : 0;
        Rect startScreenRect = windowScreen != null ? GetWindowScreenRectFromTransform() : new Rect(0, 0, 0, 0);
        // 退场时，荧幕需要反向变形回车窗投影（起始位置为当前荧幕矩形，目标为原始车窗投影矩形，相机恢复）
        // 但为了简化，我们可以直接淡出荧幕和遮罩，同时相机后退。
        // 由于车窗投影矩形在相机移动过程中是变化的，为了简单，我们不做荧幕反向变形，只淡出。
        // 若希望更平滑，可以将荧幕逐渐缩小并移回车窗位置，但会增加复杂度。
        // 此处仅淡出 + 相机恢复。

        while (elapsed < pushDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / pushDuration;
            float smoothT = eleganceCurve.Evaluate(t);

            mainCamera.transform.position = Vector3.Lerp(startCamPos, originalCamPos, smoothT);
            mainCamera.orthographicSize = Mathf.Lerp(startCamSize, originalCamSize, smoothT);

            if (fullscreenMask != null)
            {
                Color c = fullscreenMask.color;
                c.a = Mathf.Lerp(startMaskAlpha, 0, smoothT);
                fullscreenMask.color = c;
            }
            if (windowScreen != null)
            {
                Color c = windowScreen.color;
                c.a = Mathf.Lerp(startScreenAlpha, 0, smoothT);
                windowScreen.color = c;
            }
            yield return null;
        }

        if (cameraFollow != null) cameraFollow.enabled = true;
        CleanupUI();
        isPlaying = false;
    }

    // ---------- UI 创建 ----------
    private void CreateMaskCanvas()
    {
        if (maskCanvas != null) return;
        GameObject go = new GameObject("MaskCanvas");
        maskCanvas = go.AddComponent<Canvas>();
        maskCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        maskCanvas.sortingOrder = 100;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;

        GameObject maskImg = new GameObject("FullscreenMask");
        maskImg.transform.SetParent(go.transform, false);
        fullscreenMask = maskImg.AddComponent<Image>();
        fullscreenMask.color = Color.clear;
        fullscreenMask.raycastTarget = false;
        rt = fullscreenMask.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private void CreateScreenCanvas()
    {
        if (screenCanvas != null) return;
        GameObject go = new GameObject("ScreenCanvas");
        screenCanvas = go.AddComponent<Canvas>();
        screenCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        screenCanvas.sortingOrder = 200;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;

        GameObject screenImg = new GameObject("WindowScreen");
        screenImg.transform.SetParent(go.transform, false);
        windowScreen = screenImg.AddComponent<Image>();
        windowScreen.raycastTarget = false;
        windowScreen.color = Color.clear;
        windowScreen.preserveAspect = false;   // 拉伸填满
    }

    // ---------- 矩形操作 ----------
    private Rect GetWindowScreenRect(Transform window)
    {
        if (window == null) return new Rect(0, 0, 0, 0);
        Bounds bounds;
        BoxCollider2D box = window.GetComponent<BoxCollider2D>();
        SpriteRenderer sr = window.GetComponent<SpriteRenderer>();
        if (box != null) bounds = box.bounds;
        else if (sr != null) bounds = sr.bounds;
        else return new Rect(0, 0, 0, 0);

        Vector3[] corners = new Vector3[4];
        corners[0] = new Vector3(bounds.min.x, bounds.min.y, 0);
        corners[1] = new Vector3(bounds.max.x, bounds.min.y, 0);
        corners[2] = new Vector3(bounds.max.x, bounds.max.y, 0);
        corners[3] = new Vector3(bounds.min.x, bounds.max.y, 0);

        Vector2 min = Vector2.positiveInfinity;
        Vector2 max = Vector2.negativeInfinity;
        foreach (var c in corners)
        {
            Vector2 sp = mainCamera.WorldToScreenPoint(c);
            min = Vector2.Min(min, sp);
            max = Vector2.Max(max, sp);
        }
        return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }

    private Rect GetWindowScreenRectFromTransform()
    {
        if (windowScreen == null) return new Rect(0, 0, 0, 0);
        RectTransform rt = windowScreen.rectTransform;
        return new Rect(rt.anchoredPosition.x, rt.anchoredPosition.y, rt.sizeDelta.x, rt.sizeDelta.y);
    }

    private void SetWindowScreenRect(Rect rect)
    {
        if (windowScreen == null) return;
        RectTransform rt = windowScreen.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.pivot = Vector2.zero;
        rt.anchoredPosition = new Vector2(rect.x, rect.y);
        rt.sizeDelta = new Vector2(rect.width, rect.height);
    }

    private void CleanupUI()
    {
        if (maskCanvas != null) Destroy(maskCanvas.gameObject);
        if (screenCanvas != null) Destroy(screenCanvas.gameObject);
        maskCanvas = null;
        screenCanvas = null;
        fullscreenMask = null;
        windowScreen = null;
    }
}