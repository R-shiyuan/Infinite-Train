using UnityEngine;

public class PlayerSceneSpawn : MonoBehaviour
{
    void Start()
    {
        // 延迟一帧执行，确保场景物体全部初始化
        StartCoroutine(SpawnNextFrame());
    }

    System.Collections.IEnumerator SpawnNextFrame()
    {
        yield return null;

        // 找到当前场景的CarSceneInfo
        CarSceneInfo spawnInfo = FindObjectOfType<CarSceneInfo>();
        if (spawnInfo == null)
        {
            Debug.LogError("当前场景未挂载CarSceneInfo！");
            yield break;
        }

        // 加空值判断，避免访问未初始化的静态变量
        bool fromLeft = DoorTrigger.spawnFromLeftNextScene;
        Debug.Log($"出生点逻辑：从左门进? {fromLeft}");

        if (fromLeft)
        {
            if (spawnInfo.spawnLeft != null)
                transform.position = spawnInfo.spawnLeft.position;
            else
                Debug.LogError("spawnLeft 未绑定！");
        }
        else
        {
            if (spawnInfo.spawnRight != null)
                transform.position = spawnInfo.spawnRight.position;
        }
    }
}