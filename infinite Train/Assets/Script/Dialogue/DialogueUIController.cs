using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogueUIController : MonoBehaviour, IPointerClickHandler
{
    public static DialogueUIController Instance;

    [Header("UI 引用")]
    public GameObject dialoguePanel; // 对话框父物体
    public Image leftImage;
    public Image rightImage;
    public Text nameText;
    public Text contentText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (dialoguePanel != null && dialoguePanel.activeSelf)
        {
            DialogueBridge bridge = FindObjectOfType<DialogueBridge>();
            if (bridge != null) bridge.Proceed();
        }
    }

    public void ShowDialogue(string characterName, string content, Sprite portrait = null, bool isLeft = true)
    {
        if (dialoguePanel == null) return;

        // 1. 激活面板
        dialoguePanel.SetActive(true);

        // 2. 放到最前
        dialoguePanel.transform.SetAsLastSibling();

        // 3. 更新内容
        if (contentText != null)
            contentText.text = content;

        bool isMonologue = string.IsNullOrEmpty(characterName) || characterName == "none";

        if (nameText != null)
        {
            nameText.gameObject.SetActive(!isMonologue);
            nameText.text = characterName;
        }

        if (leftImage != null)
            leftImage.gameObject.SetActive(!isMonologue);

        if (rightImage != null)
            rightImage.gameObject.SetActive(!isMonologue);

        if (!isMonologue)
            UpdatePortraits(portrait, isLeft);
    }

    public void HideDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }
    private void UpdatePortraits(Sprite portrait, bool isLeft)
    {
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