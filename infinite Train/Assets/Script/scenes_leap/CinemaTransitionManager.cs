using NodeCanvas.DialogueTrees;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    public float targetAspect = 16f / 9f;

    // 事件：动画完成
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
    private DialogueTreeController cachedDialogue;
    private bool isPlaying = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ⭐必须加
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

   



    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
            cameraFollow = mainCamera.GetComponent<CameraFollow>();
    }
    private void EnsureCamera()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (cameraFollow == null && mainCamera != null)
        {
            cameraFollow = mainCamera.GetComponent<CameraFollow>();
        }
    }
    public void Play(Transform window, Sprite memory, DialogueTreeController dc)
    {
        if (isPlaying) return;

        targetWindow = window;
        memorySprite = memory;
        cachedDialogue = dc;

        StartCoroutine(PlayCoroutine());
    }

    public void End()
    {
        if (!isPlaying) return;
        StartCoroutine(ExitCoroutine());
    }

    private IEnumerator PlayCoroutine()
    {
        EnsureCamera();

        if (mainCamera == null)
        {
            Debug.LogError("CinemaTransitionManager: 找不到主摄像机");
            yield break;
        }
        isPlaying = true;
        Debug.Log("播放车窗电影效果...");

        originalCamPos = mainCamera.transform.position;
        originalCamSize = mainCamera.orthographicSize;
        if (cameraFollow != null) cameraFollow.enabled = false;
        if (PlayerController.Instance != null) PlayerController.Instance.SetCanMove(false);

        CreateMaskCanvas();
        CreateScreenCanvas();

        windowScreen.sprite = memorySprite;
        windowScreen.color = new Color(1, 1, 1, 0);

        Rect startRect = GetWindowScreenRect(targetWindow);
        float targetWidth = Screen.width * targetScreenWidthRatio;
        float targetHeight = targetWidth / targetAspect;
        Rect targetRect = new Rect(
            (Screen.width - targetWidth) / 2f,
            (Screen.height - targetHeight) / 2f,
            targetWidth,
            targetHeight
        );

        Vector3 targetCamPos = targetWindow.position + cameraOffset;
        float targetCamSize = finalOrthoSize;

        float elapsed = 0f;
        while (elapsed < pushDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / pushDuration;
            float smoothT = eleganceCurve.Evaluate(t);

            mainCamera.transform.position = Vector3.Lerp(originalCamPos, targetCamPos, smoothT);
            mainCamera.orthographicSize = Mathf.Lerp(originalCamSize, targetCamSize, smoothT);

            if (fullscreenMask != null)
            {
                Color c = fullscreenMask.color;
                c.a = Mathf.Lerp(0, maskAlpha, smoothT);
                fullscreenMask.color = c;
            }

            float curX = Mathf.Lerp(startRect.x, targetRect.x, smoothT);
            float curY = Mathf.Lerp(startRect.y, targetRect.y, smoothT);
            float curW = Mathf.Lerp(startRect.width, targetRect.width, smoothT);
            float curH = Mathf.Lerp(startRect.height, targetRect.height, smoothT);
            SetWindowScreenRect(new Rect(curX, curY, curW, curH));

            if (windowScreen != null)
            {
                Color c = windowScreen.color;
                c.a = Mathf.Lerp(0, 1, smoothT);
                windowScreen.color = c;
            }
            yield return null;
        }

        mainCamera.transform.position = targetCamPos;
        mainCamera.orthographicSize = targetCamSize;
        SetWindowScreenRect(targetRect);

        if (fullscreenMask != null) fullscreenMask.color = new Color(0, 0, 0, maskAlpha);
        if (windowScreen != null) windowScreen.color = Color.white;

        // --- 修复点 2：动画结束，手动唤醒对话 ---
        Debug.Log("车窗动画完成，打开对话UI");

        OnCinemaReady?.Invoke();
    }


    private IEnumerator ExitCoroutine()
    {
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

        if (cameraFollow != null) cameraFollow.enabled = true;
        if (PlayerController.Instance != null) PlayerController.Instance.SetCanMove(true);
        CleanupUI();
        isPlaying = false;
    }

    private void CreateMaskCanvas()
    {
        if (maskCanvas != null) return;
        GameObject go = new GameObject("MaskCanvas");
        maskCanvas = go.AddComponent<Canvas>();
        maskCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        maskCanvas.sortingOrder = 100;

        GameObject maskImg = new GameObject("FullscreenMask");
        maskImg.transform.SetParent(go.transform, false);
        fullscreenMask = maskImg.AddComponent<Image>();
        fullscreenMask.color = Color.clear;
        fullscreenMask.raycastTarget = false;

        RectTransform rt = fullscreenMask.rectTransform;
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
        screenCanvas.sortingOrder = 200; // 这里的层级是 200

        GameObject screenImg = new GameObject("WindowScreen");
        screenImg.transform.SetParent(go.transform, false);
        windowScreen = screenImg.AddComponent<Image>();
        windowScreen.raycastTarget = false;
        windowScreen.color = Color.clear;
        windowScreen.preserveAspect = false;
    }

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