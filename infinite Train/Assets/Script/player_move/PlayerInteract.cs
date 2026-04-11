using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("交互检测范围")]
    public float interactRange = 5f; // 2D项目建议调大，避免点不到

    void Update()
    {
        // 鼠标左键点击触发交互
        if (Input.GetMouseButtonDown(0))
        {
            // 1. 把屏幕坐标转成2D世界射线
            Vector3 mousePos = Input.mousePosition;
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            Ray2D ray = new Ray2D(worldPos, Vector2.zero);

            // 2. 2D射线检测，正确使用RaycastHit2D接收结果
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, interactRange);

            // 3. 检测到碰撞体，尝试获取交互接口
            if (hit.collider != null)
            {
                Interactable target = hit.collider.GetComponent<Interactable>();
                if (target != null)
                {
                    target.OnInteract();
                }
            }
        }
    }
}