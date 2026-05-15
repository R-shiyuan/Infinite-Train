using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance;

    private Action onMiniGameComplete;

    private string returnSceneName =
        "CarriageScene";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //====================================================
    // 开始小游戏
    //====================================================

    public void Play(
        string miniGameID,
        Action callback
    )
    {
        onMiniGameComplete = callback;

        string targetScene =
            "Scene_Game_" + miniGameID;

        Debug.Log(
            "加载小游戏场景: " +
            targetScene
        );

        SceneManager.LoadScene(targetScene);
    }

    //====================================================
    // 小游戏完成
    //====================================================

    public void CompleteMiniGame()
    {
        Debug.Log("小游戏完成");

        SceneManager.LoadScene(returnSceneName);

        onMiniGameComplete?.Invoke();

        onMiniGameComplete = null;
    }
}

