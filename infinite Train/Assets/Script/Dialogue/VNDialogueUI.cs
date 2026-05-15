using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VNDialogueUI : MonoBehaviour
{
    public static VNDialogueUI Instance;

    [System.Serializable]
    public class SubtitleDelays
    {
        public float characterDelay = 0.03f;
        public float sentenceDelay = 0.3f;
        public float commaDelay = 0.08f;
    }

    [Header("输入")]
    public bool skipOnInput = true;
    public bool waitForInput = true;

    [Header("主面板")]
    public GameObject dialoguePanel;

    [Header("角色立绘")]
    public Image leftCharacter;
    public Image rightCharacter;

    [Header("对话 UI")]
    public Text speechText;
    public Text nameText;

    [Header("等待输入提示")]
    public GameObject waitInput;

    [Header("打字机")]
    public SubtitleDelays subtitleDelays = new SubtitleDelays();

    [Header("立绘亮暗")]
    [Range(0f, 1f)]
    public float dimAlpha = 0.45f;

    private bool clicked;
    private Coroutine typingCoroutine;

    private Sprite lastLeftSprite;
    private Sprite lastRightSprite;

    // =====================================================
    // ✅ 核心修复：单例 + 防场景重复初始化
    // =====================================================
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        
        DontDestroyOnLoad(gameObject);

        
        HideAll();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clicked = true;
        }
    }

    void HideAll()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (waitInput != null)
            waitInput.SetActive(false);

        if (leftCharacter != null)
            leftCharacter.gameObject.SetActive(false);

        if (rightCharacter != null)
            rightCharacter.gameObject.SetActive(false);
    }

    public void ShowDialogue(DialogueRow row)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(ShowDialogueCoroutine(row));
    }

    IEnumerator ShowDialogueCoroutine(DialogueRow row)
    {
        clicked = false;

        dialoguePanel.SetActive(true);
        nameText.text = row.actorName;

        bool isMonologue =
            row.state != null &&
            row.state.ToLower() == "monologue";

        bool isLeft = row.pos.ToLower() == "left";

        Sprite portrait = null;

        if (!isMonologue)
        {
            portrait = Resources.Load<Sprite>(
                "Portraits/" + row.actorID + "_" + row.express
            );

            if (portrait != null)
            {
                if (isLeft)
                {
                    leftCharacter.gameObject.SetActive(true);
                    leftCharacter.sprite = portrait;
                    lastLeftSprite = portrait;
                }
                else
                {
                    rightCharacter.gameObject.SetActive(true);
                    rightCharacter.sprite = portrait;
                    lastRightSprite = portrait;
                }
            }
        }
        else
        {
            if (lastLeftSprite != null)
            {
                leftCharacter.gameObject.SetActive(true);
                leftCharacter.sprite = lastLeftSprite;
            }

            if (lastRightSprite != null)
            {
                rightCharacter.gameObject.SetActive(true);
                rightCharacter.sprite = lastRightSprite;
            }
        }

        Color bright = Color.white;
        Color dim = new Color(1f, 1f, 1f, dimAlpha);

        if (isMonologue)
        {
            leftCharacter.color = dim;
            rightCharacter.color = dim;
        }
        else
        {
            if (isLeft)
            {
                leftCharacter.color = bright;
                rightCharacter.color = dim;
            }
            else
            {
                rightCharacter.color = bright;
                leftCharacter.color = dim;
            }
        }

        speechText.text = "";
        string fullText = row.text;
        string current = "";

        for (int i = 0; i < fullText.Length; i++)
        {
            if (skipOnInput && clicked)
            {
                speechText.text = fullText;
                break;
            }

            char c = fullText[i];
            current += c;
            speechText.text = current;

            float delay = subtitleDelays.characterDelay;

            if (c == ',' || c == '，')
                delay = subtitleDelays.commaDelay;

            if (c == '.' || c == '。' ||
                c == '!' || c == '?' ||
                c == '！' || c == '？')
                delay = subtitleDelays.sentenceDelay;

            yield return new WaitForSeconds(delay);
        }

        clicked = false;

        if (waitForInput)
        {
            waitInput.SetActive(true);

            while (!clicked)
                yield return null;

            waitInput.SetActive(false);
        }

        DialogueBridge.Instance.Next();
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
        waitInput.SetActive(false);

        speechText.text = "";
        nameText.text = "";

        leftCharacter.gameObject.SetActive(false);
        rightCharacter.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}