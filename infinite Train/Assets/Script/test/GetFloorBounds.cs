using UnityEngine;

public class GetFloorBounds : MonoBehaviour
{
    [Header("地板设置（如果拖入地板，会自动获取边界）")]
    public Transform trainFloor; // 可选：拖入地板物体（有SpriteRenderer或Collider）

    [Header("手动参数（如果地板为null，则使用以下值）")]
    public float manualFloorWidth = 32f;   // 地板总宽度
    public float sideMargin = 2f;          // 左右留空距离
    public float windowWidth = 3f;         // 每个车窗的宽度（BoxCollider2D的Size.x）
    public int windowCount = 5;            // 车窗数量

    void Start()
    {
        float leftBound, rightBound, floorWidth;

        if (trainFloor != null)
        {
            // 尝试获取边界（优先使用Collider，其次SpriteRenderer）
            BoxCollider2D col = trainFloor.GetComponent<BoxCollider2D>();
            SpriteRenderer sr = trainFloor.GetComponent<SpriteRenderer>();
            if (col != null)
            {
                leftBound = col.bounds.min.x;
                rightBound = col.bounds.max.x;
            }
            else if (sr != null)
            {
                leftBound = trainFloor.position.x - sr.bounds.extents.x;
                rightBound = trainFloor.position.x + sr.bounds.extents.x;
            }
            else
            {
                Debug.LogWarning("地板物体没有Collider或SpriteRenderer，使用手动宽度");
                floorWidth = manualFloorWidth;
                leftBound = -floorWidth / 2f;
                rightBound = floorWidth / 2f;
            }
        }
        else
        {
            // 没有拖入地板，使用手动宽度
            floorWidth = manualFloorWidth;
            leftBound = -floorWidth / 2f;
            rightBound = floorWidth / 2f;
        }

        // 计算可用区间
        float availableLeft = leftBound + sideMargin;
        float availableRight = rightBound - sideMargin;
        float availableWidth = availableRight - availableLeft;

        float totalWindowWidth = windowWidth * windowCount;
        if (totalWindowWidth > availableWidth)
        {
            Debug.LogError($"车窗总宽 {totalWindowWidth} 超出可用宽度 {availableWidth}，请减小车窗宽度或数量，或减小边距！");
            return;
        }

        float totalGap = availableWidth - totalWindowWidth;
        float gapBetween = totalGap / (windowCount - 1);

        Debug.Log($"地板边界: 左 {leftBound}, 右 {rightBound}, 宽度 {rightBound - leftBound}");
        Debug.Log($"可用区间: [{availableLeft}, {availableRight}], 可用宽度 {availableWidth}");
        Debug.Log($"车窗宽度 {windowWidth}, 数量 {windowCount}, 间隙 {gapBetween:F3}");

        float currentLeft = availableLeft;
        for (int i = 0; i < windowCount; i++)
        {
            float centerX = currentLeft + windowWidth / 2f;
            Debug.Log($"车窗 {i + 1}: 中心 X = {centerX:F3}   (左边缘 {currentLeft:F3}, 右边缘 {currentLeft + windowWidth:F3})");
            currentLeft += windowWidth + gapBetween;
        }
    }
}