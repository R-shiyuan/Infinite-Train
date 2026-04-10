using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    [Header("目标场景名称")]
    public string targetSceneName;

    [Header("这是左边的门吗？")]
    public bool isLeftDoor = false;

    private bool isPlayerInRange;

    // 关键：必须在这里定义静态变量，供PlayerSceneSpawn访问
    public static bool spawnFromLeftNextScene;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // 记录从哪边进 → 下一个场景的出生点相反
            spawnFromLeftNextScene = !isLeftDoor;

            if (SceneFade.Instance != null)
            {
                SceneFade.currentSceneName = targetSceneName;
                SceneFade.Instance.LoadScene(targetSceneName);
            }
                
            else
                SceneManager.LoadScene(targetSceneName);

            if (DoorTipManager.Instance != null)
                DoorTipManager.Instance.HideAllTips();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            if (DoorTipManager.Instance != null)
            {
                if (isLeftDoor)
                    DoorTipManager.Instance.ShowLeftTip();
                else
                    DoorTipManager.Instance.ShowRightTip();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            if (DoorTipManager.Instance != null)
                DoorTipManager.Instance.HideAllTips();
        }
    }
}