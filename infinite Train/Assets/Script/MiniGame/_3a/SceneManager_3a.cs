using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager_3a : MonoBehaviour
{
    public Draggable_3a[] drag;

    public Transform vicBoard;

    public string nextSceneName; 

    private void Start()
    {
        vicBoard.gameObject.SetActive(false);
    }
    private void Update()
    {
        for (int i = 0; i < drag.Length; i++)
        {
            if (drag[i].finishPlace == false)
                return;
        }
        vicBoard.gameObject.SetActive (true);
    }
    public void Next()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
