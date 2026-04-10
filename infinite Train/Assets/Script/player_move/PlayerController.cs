using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("移动设置")]
    public float moveSpeed = 6f;

    [Header("火车地板白模（拖入你的地板对象）")]
    public Transform trainFloor; // 拖入你的火车地板白模

    // 自动计算的地图边界
    private float minX;
    private float maxX;

    private Rigidbody2D rb;
    private float horizontal;
    private SpriteRenderer spriteRenderer;
    private bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 🔴 自动计算火车地板的左右边界（核心！不用手动填数）
        SpriteRenderer floorRenderer = trainFloor.GetComponent<SpriteRenderer>();
        float floorHalfWidth = floorRenderer.bounds.extents.x; // 地板半宽
        minX = trainFloor.position.x - floorHalfWidth; // 地板最左X
        maxX = trainFloor.position.x + floorHalfWidth; // 地板最右X

        // 角色初始位置对齐地板
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), transform.position.y, transform.position.z);
    }

    void Update()
    {
        if (canMove)
        {
            // 获取A/D/左右键输入
            horizontal = Input.GetAxisRaw("Horizontal");

            // 角色自动翻转（面朝移动方向）
            if (horizontal > 0)
                spriteRenderer.flipX = false;
            else if (horizontal < 0)
                spriteRenderer.flipX = true;
        }
        
    }

    void FixedUpdate()
    {
        // 计算目标位置
        Vector2 targetPos = rb.position + new Vector2(horizontal * moveSpeed, 0) * Time.fixedDeltaTime;

        // 🔴 限制角色在火车地板范围内（永远不跑出白模）
        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);

        // 物理移动（稳定不抖动）
        rb.MovePosition(targetPos);
    }
    public void SetCanMove(bool value)
    {
        canMove = value;
    }
}