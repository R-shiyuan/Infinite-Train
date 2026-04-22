using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogueUIController : MonoBehaviour, IPointerClickHandler
{
    public static DialogueUIController Instance;

    [Header("UI 引用")]
    public GameObject dialoguePanel;
    public Image leftImage;
    public Image rightImage;
    public Text nameText;
    public Text contentText;

    void Awake()
    {
        Instance = this;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (dialoguePanel != null && dialoguePanel.activeSelf)
        {
            DialogueBridge bridge = FindObjectOfType<DialogueBridge>();
            if (bridge != null)
            {
                bridge.Proceed();
            }
        }
    }

    // 核心修改：增加了参数默认值 (portrait = null, isLeft = true)
    // 这样以后调用时直接写 ShowDialogue("名字", "内容"); 即可，不再报错
    public void ShowDialogue(string characterName, string content, Sprite portrait = null, bool isLeft = true)
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        if (DialogueUIManager.Instance != null)
        {
            DialogueUIManager.Instance.StartDialogueUI();
        }

        if (contentText != null) contentText.text = content;

        bool isMonologue = string.IsNullOrEmpty(characterName) || characterName == "none";

        if (nameText != null)
        {
            nameText.gameObject.SetActive(!isMonologue);
            nameText.text = characterName;
        }

        if (leftImage != null) leftImage.gameObject.SetActive(!isMonologue);
        if (rightImage != null) rightImage.gameObject.SetActive(!isMonologue);

        if (!isMonologue)
        {
            UpdatePortraits(portrait, isLeft);
        }
    }

    public void HideDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        if (DialogueUIManager.Instance != null)
        {
            DialogueUIManager.Instance.EndDialogueUI();
        }
    }

    private void UpdatePortraits(Sprite portrait, bool isLeft)
    {
        // 增加对 portrait 是否为空的防御性检查
        if (portrait == null) return;

        if (isLeft)
        {
            if (leftImage != null) { leftImage.sprite = portrait; leftImage.color = Color.white; }
            if (rightImage != null) rightImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
        else
        {
            if (rightImage != null) { rightImage.sprite = portrait; rightImage.color = Color.white; }
            if (leftImage != null) leftImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }
}