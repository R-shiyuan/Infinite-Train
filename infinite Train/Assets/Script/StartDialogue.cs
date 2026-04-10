using NodeCanvas.DialogueTrees;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogue : MonoBehaviour
{
    DialogueTreeController dialogueTreeController;
    private void Start()
    {
        dialogueTreeController = GetComponent<DialogueTreeController>();
    }
    public void talk()
    {
        dialogueTreeController.StartDialogue();
    }
}
