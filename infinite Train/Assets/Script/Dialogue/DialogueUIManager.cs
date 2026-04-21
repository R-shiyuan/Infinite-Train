using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueUIManager : MonoBehaviour
{
    public static DialogueUIManager Instance;

    [Header("UI 引用")]
    public Image dimmerImage;
    public float fadeDuration = 0.8f; // 稍微调大一点，比如 0.8秒，会显得更优雅

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartDialogueUI()
    {
        StopAllCoroutines();
        StartCoroutine(FadeDimmer(dimmerImage.color.a, 0.6f)); // 无论当前多亮，都平滑过渡到 0.6
    }

    public void EndDialogueUI()
    {
        StopAllCoroutines();
        StartCoroutine(FadeDimmer(dimmerImage.color.a, 0f)); // 平滑消失
    }

    private IEnumerator FadeDimmer(float startAlpha, float endAlpha)
    {
        if (dimmerImage == null) yield break;

        dimmerImage.gameObject.SetActive(true);
        float elapsed = 0f;
        Color c = dimmerImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            // 使用 SmoothStep 替代 Lerp，会让变暗的开始和结束更圆润，没有生硬感
            float t = Mathf.SmoothStep(0f, 1f, elapsed / fadeDuration);
            c.a = Mathf.Lerp(startAlpha, endAlpha, t);
            dimmerImage.color = c;
            yield return null;
        }

        c.a = endAlpha;
        dimmerImage.color = c;
        if (endAlpha <= 0) dimmerImage.gameObject.SetActive(false);
    }
}