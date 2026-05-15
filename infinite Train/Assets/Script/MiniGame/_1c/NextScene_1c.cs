using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene_1c : MonoBehaviour
{
    public string nextSceneName;

    public Transform board;
    private void Start()
    {
        board.gameObject.SetActive(false);
    }
    public void NextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
