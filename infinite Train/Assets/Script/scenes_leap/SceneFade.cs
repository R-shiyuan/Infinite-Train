using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFade : MonoBehaviour
{
    public static SceneFade Instance;
    public static string currentSceneName;

    [Header("全屏遮罩")]
    public CanvasGroup fadeCanvasGroup;

    [Header("淡入淡出时间")]
    public float fadeDuration = 0.6f;

    void Awake()
    {
        transform.SetParent(null);

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
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
        if (fadeCanvasGroup != null)
        {
            currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            fadeCanvasGroup.alpha = 0;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }

    // 外部统一调用接口
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    // 场景加载流程：淡出 → 加载 → 淡入
    IEnumerator LoadSceneRoutine(string sceneName)
    {
        fadeCanvasGroup.blocksRaycasts = true;
        yield return PlayFadeOut();

        SceneManager.LoadScene(sceneName);
    }

    // 淡出动画（后期可替换成你们的车厢动画）
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

    // 淡入动画（后期可替换）
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

    // 新场景加载完成后自动淡入
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;
        StartCoroutine(PlayFadeIn());
    }
}