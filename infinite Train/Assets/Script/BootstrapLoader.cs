using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BootstrapLoader : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return null;

        SceneManager.LoadScene("car-start");
    }
}