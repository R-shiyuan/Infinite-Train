using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowl_1a : MonoBehaviour
{
    public Collider2D cld;
    public Rigidbody2D rb;

    public float rotateForce = 5f;
    public float maxRotateVelocity = 30f;

    public float input;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cld = GetComponent<Collider2D>();
        Time.timeScale = 0f;
    }
    private void Update()
    {
        GetInput();
    }
    private void GetInput()
    {
        input = -Input.GetAxis("Horizontal");
        RotateBowl();
    }
    private void RotateBowl()
    {
        rb.AddTorque(input * rotateForce);
        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -maxRotateVelocity, maxRotateVelocity);
    }
}
