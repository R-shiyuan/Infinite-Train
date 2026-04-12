using UnityEngine;

public class OutlineHandler : MonoBehaviour
{
    // 以后如果你想做呼吸灯效果、渐隐渐现，都在这里改，不影响NPC脚本
    public void ShowOutline(bool show)
    {
        gameObject.SetActive(show);
    }
}