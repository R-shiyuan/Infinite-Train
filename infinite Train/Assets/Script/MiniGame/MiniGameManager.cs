using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance;

    private HashSet<string> completedGames = new HashSet<string>();
    private System.Action onCompleteCallback;
    private string currentGameID;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Play(string gameID, System.Action onComplete)
    {
        currentGameID = gameID;
        onCompleteCallback = onComplete;

        Debug.Log("开始小游戏: " + gameID);
    }

    public void FinishGame(string gameID)
    {
        completedGames.Add(gameID);

        Debug.Log("小游戏完成: " + gameID);

        if (gameID == currentGameID)
        {
            onCompleteCallback?.Invoke();
            onCompleteCallback = null;
            currentGameID = null;
        }
    }

    public bool IsCompleted(string gameID)
    {
        return completedGames.Contains(gameID);
    }
}