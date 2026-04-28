using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CinemaTransitionTest : MonoBehaviour
{
    [Header("目标车窗（世界空间中的 SpriteRenderer 或 BoxCollider2D）")]
    public Transform targetWindow;          // 拖入代表车窗的物体
    public Sprite memorySprite;              // 要显示的回忆图

    [Header("摄像机动画参数")]
    public float pushDuration = 1.2f;        // 动画总时长
    public float finalOrthoSize = 2.5f;      // 拉近后的正交大小
    public Vector3 cameraOffset = new Vector3(0, 0, -2f); // 镜头相对车窗的偏移

    [Header("UI 动画参数")]
    [Range(0, 1)] public float maskAlpha = 0.8f;   // 四周暗化的最终透明度
    public AnimationCurve eleganceCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 自定义缓动曲线

    // 内部变量
    private Camera mainCamera;
    private Vector3 originalCamPos;
    private float originalCamSize;
    private Canvas maskCanvas;
    private Image fullscreenMask;
    private Canvas screenCanvas;
    private Image windowScreen;
    private bool isPlaying = false;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null) Debug.LogError("未找到 Main Camera！");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !isPlaying && targetWindow != null && memorySprite != null)
        {
            StartCoroutine(PlayCinemaEffect());
        }
        if (Input.GetKeyDown(KeyCode.Y) && isPlaying)
        {
            StopAllCoroutines();
            StartCoroutine(ExitCinemaEffect());
        }
    }

    IEnumerator PlayCinemaEffect()
    {
        isPlaying = true;
        Debug.Log("=== 开始播放车窗电影效果（优雅同步版） ===");

        // 1. 保存相机原始状态
        originalCamPos = mainCamera.transform.position;
        originalCamSize = mainCamera.orthographicSize;

        // 2. 创建 UI 层
        CreateMaskCanvas();
        CreateScreenCanvas();

        // 3. 设置荧幕图片，初始完全透明
        windowScreen.sprite = memorySprite;
        windowScreen.color = new Color(1, 1, 1, 0);

        // 4. 目标参数
        Vector3 targetCamPos = targetWindow.position + cameraOffset;
        float targetCamSize = finalOrthoSize;

        float elapsed = 0f;

        // 同步动画：镜头移动 + 遮罩淡入 + 荧幕淡入 + 荧幕位置/大小实时更新
        while (elapsed < pushDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / pushDuration;                // 线性进度 0→1
            float smoothT = eleganceCurve.Evaluate(t);       // 应用缓动曲线

            // 镜头移动和变焦
            mainCamera.transform.position = Vector3.Lerp(originalCamPos, targetCamPos, smoothT);
            mainCamera.orthographicSize = Mathf.Lerp(originalCamSize, targetCamSize, smoothT);

            // 实时更新荧幕矩形（因为相机在动，车窗的屏幕投影会变化）
            UpdateWindowScreenRect();

            // 四周变暗：从 0 到 maskAlpha
            if (fullscreenMask != null)
            {
                Color c = fullscreenMask.color;
                c.a = Mathf.Lerp(0, maskAlpha, smoothT);
                fullscreenMask.color = c;
            }

            // 荧幕淡入：为了让荧幕更“突出”，可以比遮罩稍微快一点点（这里直接使用 smoothT 已经自然）
            // 若想更快显现，可将 smoothT 改为 Mathf.Pow(smoothT, 0.8f)
            if (windowScreen != null)
            {
                Color c = windowScreen.color;
                c.a = Mathf.Lerp(0, 1, smoothT);
                windowScreen.color = c;
            }

            yield return null;
        }

        // 确保最终精确到达目标
        mainCamera.transform.position = targetCamPos;
        mainCamera.orthographicSize = targetCamSize;
        UpdateWindowScreenRect();
        if (fullscreenMask != null) fullscreenMask.color = new Color(0, 0, 0, maskAlpha);
        if (windowScreen != null) windowScreen.color = new Color(1, 1, 1, 1);

        Debug.Log("动画完成！等待 3 秒后自动退出（测试用）...");
        yield return new WaitForSeconds(3f);
        StartCoroutine(ExitCinemaEffect());
    }

    IEnumerator ExitCinemaEffect()
    {
        Debug.Log("=== 退场动画（同步淡出） ===");
        float elapsed = 0f;

        Vector3 startCamPos = mainCamera.transform.position;
        float startCamSize = mainCamera.orthographicSize;
        float startMaskAlpha = fullscreenMask != null ? fullscreenMask.color.a : 0;
        float startScreenAlpha = windowScreen != null ? windowScreen.color.a : 0;

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

        CleanupUI();
        isPlaying = false;
        Debug.Log("退场完成");
    }

    // 创建全屏遮罩 Canvas（sortingOrder = 100）
    void CreateMaskCanvas()
    {
        if (maskCanvas != null) return;

        GameObject go = new GameObject("MaskCanvas");
        maskCanvas = go.AddComponent<Canvas>();
        maskCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        maskCanvas.sortingOrder = 100;

        // 全屏拉伸
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;

        // 黑色 Image
        GameObject maskImg = new GameObject("FullscreenMask");
        maskImg.transform.SetParent(go.transform, false);
        fullscreenMask = maskImg.AddComponent<Image>();
        fullscreenMask.color = new Color(0, 0, 0, 0);
        fullscreenMask.raycastTarget = false;

        // 确保 Image 全屏
        rt = fullscreenMask.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    // 创建荧幕 Canvas（sortingOrder = 200，高于遮罩）
    void CreateScreenCanvas()
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
        windowScreen.color = new Color(1, 1, 1, 0);
        // 关闭保持比例，强制拉伸填满车窗矩形
        windowScreen.preserveAspect = false;
    }

    // 更新荧幕的屏幕矩形（根据当前相机和目标车窗的边界）
    void UpdateWindowScreenRect()
    {
        if (targetWindow == null || windowScreen == null) return;

        Bounds bounds;
        // 优先使用 BoxCollider2D（可手动调整精确区域），否则用 SpriteRenderer
        BoxCollider2D boxCol = targetWindow.GetComponent<BoxCollider2D>();
        SpriteRenderer sr = targetWindow.GetComponent<SpriteRenderer>();

        if (boxCol != null)
        {
            bounds = boxCol.bounds;
        }
        else if (sr != null)
        {
            bounds = sr.bounds;
        }
        else
        {
            Debug.LogError("车窗物体需要 SpriteRenderer 或 BoxCollider2D 来获取边界！");
            return;
        }

        Vector3[] worldCorners = new Vector3[4];
        worldCorners[0] = new Vector3(bounds.min.x, bounds.min.y, 0);
        worldCorners[1] = new Vector3(bounds.max.x, bounds.min.y, 0);
        worldCorners[2] = new Vector3(bounds.max.x, bounds.max.y, 0);
        worldCorners[3] = new Vector3(bounds.min.x, bounds.max.y, 0);

        Vector2 minScreen = Vector2.positiveInfinity;
        Vector2 maxScreen = Vector2.negativeInfinity;

        foreach (var corner in worldCorners)
        {
            Vector2 screenPoint = mainCamera.WorldToScreenPoint(corner);
            minScreen = Vector2.Min(minScreen, screenPoint);
            maxScreen = Vector2.Max(maxScreen, screenPoint);
        }

        Rect screenRect = new Rect(minScreen.x, minScreen.y, maxScreen.x - minScreen.x, maxScreen.y - minScreen.y);

        // 应用给 windowScreen 的 RectTransform
        RectTransform rt = windowScreen.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.pivot = Vector2.zero;
        rt.anchoredPosition = new Vector2(screenRect.x, screenRect.y);
        rt.sizeDelta = new Vector2(screenRect.width, screenRect.height);
    }

    void CleanupUI()
    {
        if (maskCanvas != null) Destroy(maskCanvas.gameObject);
        if (screenCanvas != null) Destroy(screenCanvas.gameObject);
        maskCanvas = null;
        screenCanvas = null;
        fullscreenMask = null;
        windowScreen = null;
    }
}