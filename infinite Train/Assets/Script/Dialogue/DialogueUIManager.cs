using UnityEngine;

using UnityEngine.UI;

using System.Collections;



public class DialogueUIManager : MonoBehaviour

{

    public static DialogueUIManager Instance;



    [Header("UI 引用")]

    public Image dimmerImage; // 拖入刚才做的 DialogueDimmer

    public float fadeDuration = 0.5f; // 变暗的过渡时间



    void Awake()

    {

        if (Instance == null) Instance = this;

        else Destroy(gameObject);

    }



    // 开启对话模式：背景变暗

    public void StartDialogueUI()

    {

        StopAllCoroutines();

        StartCoroutine(FadeDimmer(0, 0.6f)); // 从透明到 0.6 的透明度

    }



    // 结束对话模式：背景恢复

    public void EndDialogueUI()

    {

        StopAllCoroutines();

        StartCoroutine(FadeDimmer(dimmerImage.color.a, 0f));

    }



    private IEnumerator FadeDimmer(float startAlpha, float endAlpha)

    {

        dimmerImage.gameObject.SetActive(true);

        float elapsed = 0f;

        Color c = dimmerImage.color;



        while (elapsed < fadeDuration)

        {

            elapsed += Time.deltaTime;

            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);

            dimmerImage.color = c;

            yield return null;

        }



        if (endAlpha <= 0) dimmerImage.gameObject.SetActive(false);

    }

}