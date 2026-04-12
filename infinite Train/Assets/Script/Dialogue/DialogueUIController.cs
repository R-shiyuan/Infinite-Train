using UnityEngine;
using UnityEngine.UI;

public class DialogueUIController : MonoBehaviour
{
    public static DialogueUIController Instance;

    [Header("状态管理")]
    // 拖入 @DialogueUGUI，确保它关闭时子物体（Dimmer, BG等）全部隐藏
    public GameObject dialoguePanel;

    [Header("UI 槽位引用")]
    public Image leftImage;
    public Image rightImage;
    public Text nameText;
    public Text contentText;

    void Awake()
    {
        Instance = this;

        // 初始隐藏“全家桶”
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    /// <summary>
    /// 进入对话状态：显示包含 Dimmer、背景、立绘在内的所有 UI
    /// </summary>
    public void ShowDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
            Debug.Log("对话 UI 全套已激活");
        }
    }

    /// <summary>
    /// 退出对话状态：关闭全套 UI
    /// </summary>
    public void HideDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
            Debug.Log("对话 UI 已全部隐藏");
        }
    }

    public void SetDialogue(string characterName, string content, Sprite portrait, bool isLeft)
    {
        // 念词前确保面板已打开
        if (dialoguePanel != null && !dialoguePanel.activeSelf)
        {
            ShowDialogue();
        }

        if (nameText != null) nameText.text = characterName;
        if (contentText != null) contentText.text = content;

        UpdatePortraits(portrait, isLeft);
    }

    private void UpdatePortraits(Sprite portrait, bool isLeft)
    {
        if (isLeft)
        {
            if (leftImage != null)
            {
                leftImage.sprite = portrait;
                leftImage.gameObject.SetActive(true);
                leftImage.color = Color.white;
            }
            if (rightImage != null) rightImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
        else
        {
            if (rightImage != null)
            {
                rightImage.sprite = portrait;
                rightImage.gameObject.SetActive(true);
                rightImage.color = Color.white;
            }
            if (leftImage != null) leftImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }
}