using UnityEngine;

public class UIBootstrap : MonoBehaviour
{
    public static UIBootstrap Instance;

    public GameObject dialogueCanvasPrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (VNDialogueUI.Instance == null)
        {
            Instantiate(dialogueCanvasPrefab);
        }
    }
}