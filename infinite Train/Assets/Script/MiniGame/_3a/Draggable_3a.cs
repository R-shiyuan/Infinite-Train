using UnityEngine;

public class Draggable_3a : MonoBehaviour
{
    Vector2 mousePos;
    Vector2 distance;

    // 目标位置
    public Transform target;

    // 吸附距离
    public float snapDistance = 0.5f;

    // 是否已经放置完成
    private bool isPlaced = false;
    public bool finishPlace = false;
    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDown()
    {
        // 已经放置后不能再拖
        if (isPlaced)
            return;

        distance =
            (Vector2)transform.position - mousePos;
    }

    private void OnMouseDrag()
    {
        // 已经放置后不能再拖
        if (isPlaced)
            return;

        // 正常拖动
        transform.position = mousePos + distance;

        // 检测是否接近目标
        float dis =
            Vector2.Distance(transform.position,
                             target.position);

        if (dis <= snapDistance)
        {
            // 自动吸附
            transform.position = target.position;

            // 锁定
            isPlaced = true;
            finishPlace = true;
        }
    }
}