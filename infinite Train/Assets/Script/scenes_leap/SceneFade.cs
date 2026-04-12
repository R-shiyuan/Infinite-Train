using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour
{
    public static SceneFade Instance;
    public static string currentSceneName;

    [Header("淡入淡出时间")]
    public float fadeDuration = 0.6f;

    private CanvasGroup fadeCanvasGroup;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            // 自动创建一个不受任何限制的全屏黑幕
            CreateFadeUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        StartCoroutine(PlayFadeIn());
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    IEnumerator LoadSceneRoutine(string sceneName)
    {
        fadeCanvasGroup.blocksRaycasts = true;
        yield return PlayFadeOut();
        SceneManager.LoadScene(sceneName);
    }

    public IEnumerator PlayFadeOut()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1;
    }

    public IEnumerator PlayFadeIn()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0;
        fadeCanvasGroup.blocksRaycasts = false;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;
        StartCoroutine(PlayFadeIn());
    }

    // ====================== 自动生成全屏 UI 核心逻辑 ======================
    void CreateFadeUI()
    {
        // 1. 创建 Canvas
        GameObject canvasObj = new GameObject("Auto_FadeCanvas");
        canvasObj.transform.SetParent(transform);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // 保证在最上层

        // 2. 设置适配参数
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        // 3. 创建 Image (黑幕)
        GameObject imageObj = new GameObject("Auto_FadeImage");
        imageObj.transform.SetParent(canvasObj.transform);

        RectTransform rect = imageObj.AddComponent<RectTransform>();
        // 强制全屏 (无视任何父物体限制)
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // 4. 设置纯黑颜色
        Image image = imageObj.AddComponent<Image>();
        image.color = Color.black;

        // 5. 创建一个 1x1 的纯白 Sprite 作为纹理
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        image.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);

        // 6. 添加控制组
        fadeCanvasGroup = imageObj.AddComponent<CanvasGroup>();
        fadeCanvasGroup.alpha = 0;
        fadeCanvasGroup.blocksRaycasts = false;
    }
}