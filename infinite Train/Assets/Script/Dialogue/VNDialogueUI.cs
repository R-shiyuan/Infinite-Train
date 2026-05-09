using NodeCanvas.DialogueTrees;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VNDialogueUI : MonoBehaviour, IPointerClickHandler
{
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

    [Header("对话UI")]
    public Text speechText;
    public Text nameText;
    public Image portraitImage;
    public GameObject nameBG;
    public GameObject waitInput;

    [Header("选项")]
    public RectTransform optionsGroup;
    public Button optionButtonPrefab;

    [Header("打字机")]
    public SubtitleDelays subtitleDelays = new SubtitleDelays();

    private Dictionary<Button, int> cachedButtons;

    private bool anyKeyDown;
    private bool isTyping;

    public void OnPointerClick(PointerEventData eventData)
    {
        anyKeyDown = true;
    }

    void LateUpdate()
    {
        anyKeyDown = false;
    }

    void Awake()
    {
        Subscribe();
        HideAll();
    }

    void OnEnable()
    {
        UnSubscribe();
        Subscribe();
    }

    void OnDisable()
    {
        UnSubscribe();
    }

    void Subscribe()
    {
        DialogueTree.OnDialogueStarted += OnDialogueStarted;
        DialogueTree.OnDialogueFinished += OnDialogueFinished;
        DialogueTree.OnDialoguePaused += OnDialoguePaused;
        DialogueTree.OnSubtitlesRequest += OnSubtitlesRequest;
        DialogueTree.OnMultipleChoiceRequest += OnMultipleChoiceRequest;
    }

    void UnSubscribe()
    {
        DialogueTree.OnDialogueStarted -= OnDialogueStarted;
        DialogueTree.OnDialogueFinished -= OnDialogueFinished;
        DialogueTree.OnDialoguePaused -= OnDialoguePaused;
        DialogueTree.OnSubtitlesRequest -= OnSubtitlesRequest;
        DialogueTree.OnMultipleChoiceRequest -= OnMultipleChoiceRequest;
    }

    void HideAll()
    {
        dialoguePanel.SetActive(false);

        waitInput.SetActive(false);

        optionsGroup.gameObject.SetActive(false);

        optionButtonPrefab.gameObject.SetActive(false);
    }

    void OnDialogueStarted(DialogueTree dlg)
    {
        dialoguePanel.SetActive(true);
    }

    void OnDialoguePaused(DialogueTree dlg)
    {
        dialoguePanel.SetActive(false);

        StopAllCoroutines();
    }

    void OnDialogueFinished(DialogueTree dlg)
    {
        dialoguePanel.SetActive(false);

        optionsGroup.gameObject.SetActive(false);

        if (cachedButtons != null)
        {
            foreach (var btn in cachedButtons.Keys)
            {
                if (btn != null)
                    Destroy(btn.gameObject);
            }
        }

        cachedButtons = null;

        StopAllCoroutines();

        // 对话结束后关闭电影效果
        if (CinemaTransitionManager.Instance != null)
        {
            CinemaTransitionManager.Instance.End();
        }
    }

    //========================================================
    // 字幕
    //========================================================

    void OnSubtitlesRequest(SubtitlesRequestInfo info)
    {
        StartCoroutine(Internal_Subtitles(info));
    }

    IEnumerator Internal_Subtitles(SubtitlesRequestInfo info)
    {
        isTyping = true;

        dialoguePanel.SetActive(true);

        string fullText = info.statement.text;

        var actor = info.actor;

        // 名字
        nameText.text = actor.name;

        // 颜色
        speechText.color = actor.dialogueColor;

        // 肖像
        if (actor.portraitSprite != null)
        {
            portraitImage.gameObject.SetActive(true);
            portraitImage.sprite = actor.portraitSprite;
        }
        else
        {
            portraitImage.gameObject.SetActive(false);
        }

        // 清空
        speechText.text = "";

        // 打字机
        string current = "";

        for (int i = 0; i < fullText.Length; i++)
        {
            if (skipOnInput && anyKeyDown)
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

            if (c == '.' || c == '。' || c == '!' || c == '?' || c == '！' || c == '？')
                delay = subtitleDelays.sentenceDelay;

            yield return new WaitForSeconds(delay);
        }

        isTyping = false;

        // 等待点击继续
        if (waitForInput)
        {
            waitInput.SetActive(true);

            while (!anyKeyDown)
            {
                yield return null;
            }

            waitInput.SetActive(false);
        }

        info.Continue();
    }

    //========================================================
    // 选项
    //========================================================

    void OnMultipleChoiceRequest(MultipleChoiceRequestInfo info)
    {
        optionsGroup.gameObject.SetActive(true);

        cachedButtons = new Dictionary<Button, int>();

        foreach (Transform child in optionsGroup)
        {
            if (child != optionButtonPrefab.transform)
            {
                Destroy(child.gameObject);
            }
        }

        int i = 0;

        foreach (var pair in info.options)
        {
            Button btn = Instantiate(optionButtonPrefab, optionsGroup);

            btn.gameObject.SetActive(true);

            btn.GetComponentInChildren<Text>().text = pair.Key.text;

            cachedButtons.Add(btn, pair.Value);

            btn.onClick.AddListener(() =>
            {
                FinalizeChoice(info, cachedButtons[btn]);
            });

            i++;
        }
    }

    void FinalizeChoice(MultipleChoiceRequestInfo info, int index)
    {
        optionsGroup.gameObject.SetActive(false);

        foreach (var btn in cachedButtons.Keys)
        {
            Destroy(btn.gameObject);
        }

        cachedButtons.Clear();

        info.SelectOption(index);
    }
}