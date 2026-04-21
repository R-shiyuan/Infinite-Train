using UnityEngine;
using NodeCanvas.DialogueTrees;

public class NPC : MonoBehaviour, Interactable
{
    [Header("����")]
    public OutlineHandler outline;
    public DialogueTreeController dialogueController;

    [Header("��������")]
    public float activeDistance = 2.5f;

    [Header("�������")]
    public bool canInteract = true;

    private bool isPlayerNearby = false;
    private bool isMouseOver = false;
    private bool isTalking = false;
    private Collider2D npcCollider;

    void Awake()
    {
        npcCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (PlayerController.Instance == null || npcCollider == null) return;

        // 1. ʵʱ������������
        Collider2D playerCollider = PlayerController.Instance.GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            float currentDist = npcCollider.Distance(playerCollider).distance;
            isPlayerNearby = (currentDist <= activeDistance);
        }

        // 2. ʵʱͬ������״̬
        RefreshHighlightState();
    }

    private void OnMouseEnter() { isMouseOver = true; }
    private void OnMouseExit() { isMouseOver = false; }

    private void RefreshHighlightState()
    {
        if (outline != null)
        {
            bool shouldShow = isTalking || (isPlayerNearby && isMouseOver && canInteract);
            if (outline.gameObject.activeSelf != shouldShow)
            {
                outline.ShowOutline(shouldShow);
            }
        }
    }

    public void OnInteract()
    {
        // �����ж�
        if (!isPlayerNearby || !canInteract || isTalking)
        {
            return;
        }

        Debug.Log("����Ի�ģʽ");

        // 1. ״̬����
        isTalking = true;

        // 2. ���� UI ����������ȫ�ֶԻ� UI (��������壬�����ֶ����)
        if (DialogueUIController.Instance != null)
        {
            DialogueUIController.Instance.ShowDialogue("NPC����", "�����ǶԻ�����");
        }

        // 3. ��������ƶ�
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.SetCanMove(false);
        }

        // 4. ���� NodeCanvas �Ի���
        // ֮���� DialogueUGUI (NodeCanvas�Դ�) �����Զ���Ľӿ�ȥ�Խ� CSV ����
        if (dialogueController != null)
        {
            dialogueController.StartDialogue();
        }
    }

    public void EndConversation()
    {
        isTalking = false;

        // 1. �ָ�����
        if (outline != null) outline.ShowOutline(false);

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.SetCanMove(true);
        }

        // 2. ���ضԻ� UI
        if (DialogueUIController.Instance != null)
        {
            DialogueUIController.Instance.HideDialogue();
        }

        // 3. ������ʧ���
        PresenceController pc = GetComponent<PresenceController>();
        if (pc != null)
        {
            pc.CheckPresence();
        }
    }
}