using UnityEngine;
using UnityEngine.UI;

public class DialogueUIController : MonoBehaviour
{
    public static DialogueUIController Instance;

    [Header("状态管理")]
    public GameObject dialoguePanel; // 挂在 @DialogueUGUI 上

    [Header("UI 槽位引用")]
    public Image leftImage;
    public Image rightImage;
    public Text nameText;
    public Text contentText;

    void Awake()
    {
        Instance = this;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    public void ShowDialogue(string characterName, string content, Sprite portrait, bool isLeft)
    {
        // 1. 激活面板
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        // 2. 联动背景变暗 (调用你原本的 UIManager)
        if (DialogueUIManager.Instance != null)
        {
            DialogueUIManager.Instance.StartDialogueUI();
        }

        // 3. 更新内容
        if (nameText != null) nameText.text = characterName;
        if (contentText != null) contentText.text = content;

        UpdatePortraits(portrait, isLeft);
    }

    public void HideDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        // 联动背景恢复
        if (DialogueUIManager.Instance != null)
        {
            DialogueUIManager.Instance.EndDialogueUI();
        }
    }

    private void UpdatePortraits(Sprite portrait, bool isLeft)
    {
        if (isLeft)
        {
            if (leftImage != null) { leftImage.sprite = portrait; leftImage.gameObject.SetActive(true); leftImage.color = Color.white; }
            if (rightImage != null) rightImage.color = new Color(0.5f, 0.5f, 0.5f, 1f); // 非焦点变暗
        }
        else
        {
            if (rightImage != null) { rightImage.sprite = portrait; rightImage.gameObject.SetActive(true); rightImage.color = Color.white; }
            if (leftImage != null) leftImage.color = new Color(0.5f, 0.5f, 0.5f, 1f); // 非焦点变暗
        }
    }
}