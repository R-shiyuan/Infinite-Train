using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    void Update()
    {
        // 鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    void HandleClick()
    {
        // 1. 获取鼠标点击的世界位置
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 2. 发射一根点射线检测碰撞
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null)
        {
            // 尝试获取交互接口
            Interactable target = hit.collider.GetComponent<Interactable>();
            if (target != null)
            {
                // 直接调用，具体的“距离够不够”由 NPC 脚本内部判断
                target.OnInteract();
            }
        }
    }
}