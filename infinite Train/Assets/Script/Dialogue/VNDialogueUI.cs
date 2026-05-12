using System.Collections;
using System.Collections.Generic;
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

    [Header("选项")]
    public RectTransform optionsGroup;
    public Button optionButtonPrefab;

    [Header("打字机")]
    public SubtitleDelays subtitleDelays = new SubtitleDelays();

    [Header("立绘亮暗")]
    [Range(0f, 1f)]
    public float dimAlpha = 0.45f;

    private bool clicked;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        Instance = this;
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
        dialoguePanel.SetActive(false);
        waitInput.SetActive(false);
        optionsGroup.gameObject.SetActive(false);
        optionButtonPrefab.gameObject.SetActive(false);

        leftCharacter.gameObject.SetActive(false);
        rightCharacter.gameObject.SetActive(false);
    }


    public void ShowDialogue(DialogueRow row)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine =
            StartCoroutine(ShowDialogueCoroutine(row));
    }

    IEnumerator ShowDialogueCoroutine(DialogueRow row)
    {
        clicked = false;

        dialoguePanel.SetActive(true);

        //====================================================
        // 名字
        //====================================================

        nameText.text = row.actorName;

        //====================================================
        // 加载立绘
        //====================================================

        Sprite portrait =
            Resources.Load<Sprite>(
                "Portraits/" +
                row.actorID +
                "_" +
                row.express
            );

        bool isLeft =
            row.pos.ToLower() == "left";

        //====================================================
        // 设置立绘
        //====================================================

        if (portrait != null)
        {
            if (isLeft)
            {
                leftCharacter.gameObject.SetActive(true);
                leftCharacter.sprite = portrait;
            }
            else
            {
                rightCharacter.gameObject.SetActive(true);
                rightCharacter.sprite = portrait;
            }
        }

        bool isMonologue =
            row.state != null &&
            row.state.ToLower() == "monologue";

        Color bright = Color.white;
        Color dim = new Color(1f, 1f, 1f, dimAlpha);

        if (isMonologue)
        {
            // 👉 两边全部变暗
            if (leftCharacter.gameObject.activeSelf)
                leftCharacter.color = dim;

            if (rightCharacter.gameObject.activeSelf)
                rightCharacter.color = dim;
        }
        else
        {
            //================================================
            // 正常对话逻辑
            //================================================

            if (isLeft)
            {
                leftCharacter.color = bright;

                if (rightCharacter.gameObject.activeSelf)
                    rightCharacter.color = dim;
            }
            else
            {
                rightCharacter.color = bright;

                if (leftCharacter.gameObject.activeSelf)
                    leftCharacter.color = dim;
            }
        }

        //====================================================
        // 打字机
        //====================================================

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

        //====================================================
        // 等待点击继续
        //====================================================

        clicked = false;

        if (waitForInput)
        {
            waitInput.SetActive(true);

            while (!clicked)
                yield return null;

            waitInput.SetActive(false);
        }

        clicked = false;

        DialogueBridge.Instance.Next();
    }

    //========================================================
    // 隐藏
    //========================================================

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
        waitInput.SetActive(false);

        speechText.text = "";
        nameText.text = "";

        leftCharacter.gameObject.SetActive(false);
        rightCharacter.gameObject.SetActive(false);
    }
}