using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 单例模式，让NPC脚本能通过 PlayerController.Instance 找到它
    public static PlayerController Instance;

    [Header("移动设置")]
    public float moveSpeed = 6f;

    [Header("火车地板（拖入地板对象）")]
    public Transform trainFloor;

    private float minX;
    private float maxX;
    private Rigidbody2D rb;
    private float horizontal;
    private SpriteRenderer spriteRenderer;
    private bool canMove = true;

    // 新增：动画控制器引用
    private Animator animator;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();   // 获取动画组件

        if (trainFloor != null)
        {
            SpriteRenderer floorRenderer = trainFloor.GetComponent<SpriteRenderer>();
            float floorHalfWidth = floorRenderer.bounds.extents.x;
            minX = trainFloor.position.x - floorHalfWidth;
            maxX = trainFloor.position.x + floorHalfWidth;
        }
    }

    void Update()
    {
        if (canMove)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            if (horizontal > 0) spriteRenderer.flipX = true;
            else if (horizontal < 0) spriteRenderer.flipX = false;

            // 根据水平输入绝对值设置动画参数 Speed
            float speed = Mathf.Abs(horizontal);
            if (animator != null)
                animator.SetFloat("Speed", speed);
        }
        else
        {
            horizontal = 0;
            // 禁止移动时强制动画为待机
            if (animator != null)
                animator.SetFloat("Speed", 0f);
        }
    }

    void FixedUpdate()
    {
        Vector2 targetPos = rb.position + new Vector2(horizontal * moveSpeed, 0) * Time.fixedDeltaTime;
        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        rb.MovePosition(targetPos);
    }

    // 对话开始/结束时的调用接口
    public void SetCanMove(bool value)
    {
        canMove = value;
    }
}