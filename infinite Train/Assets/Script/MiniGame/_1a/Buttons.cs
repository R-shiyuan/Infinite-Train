using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public string nextSceneName;

    public void Again()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Next()
    {
        SceneManager.LoadScene(nextSceneName); 
    }
    public void OkIGetIt()
    {
        Time.timeScale = 1.0f;
        this.gameObject.SetActive(false);
    }
}
