using UnityEngine;

public class NPC : MonoBehaviour, Interactable
{
    public string npcID = "NPC_01";

    public void OnInteract()
    {
        Debug.Log("和NPC对话：" + npcID);
        // 以后这里接对话系统,写在这里
    }
}