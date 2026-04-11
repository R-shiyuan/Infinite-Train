using UnityEngine;

public class DoorTipManager : MonoBehaviour
{
    public static DoorTipManager Instance;

    [Header("左边门提示")]
    public GameObject tipLeft;

    [Header("右边门提示")]
    public GameObject tipRight;

    void Awake()
    {
        //  强制根化
        transform.SetParent(null);

        // 单例 + 全局不销毁
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

    // 显示左边提示
    public void ShowLeftTip()
    {
        HideAllTips();
        tipLeft.SetActive(true);
    }

    // 显示右边提示
    public void ShowRightTip()
    {
        HideAllTips();
        tipRight.SetActive(true);
    }

    // 全部隐藏
    public void HideAllTips()
    {
        tipLeft.SetActive(false);
        tipRight.SetActive(false);
    }
}