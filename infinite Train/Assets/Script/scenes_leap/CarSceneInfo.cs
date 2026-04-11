using UnityEngine;

public class CarSceneInfo : MonoBehaviour
{
    public static CarSceneInfo Instance;

    [Header("从左边门进来的出生点")]
    public Transform spawnLeft;

    [Header("从右边门进来的出生点")]
    public Transform spawnRight;

    void Awake()
    {
        Instance = this;
    }
}