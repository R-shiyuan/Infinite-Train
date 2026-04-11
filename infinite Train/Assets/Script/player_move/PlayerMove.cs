using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("移动速度")]
    public float moveSpeed = 5f; // 可在 Inspector 面板调节速度

    private Rigidbody2D rb;
    private float moveInput;

    void Start()
    {
        // 获取角色身上的 2D 刚体
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 获取左右输入：A/左箭头=-1，D/右箭头=1，不按=0
        moveInput = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        // 物理移动（2D 移动必须写在 FixedUpdate 里，更稳定）
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }
}