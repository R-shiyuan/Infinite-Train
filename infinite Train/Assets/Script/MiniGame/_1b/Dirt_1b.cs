using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dirt_1b : MonoBehaviour
{
    public float washSpeed = 0.5f;

    public SpriteRenderer sr {  get; private set; }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Wash()
    {
        Color c = sr.color;
        c.a -= washSpeed*Time.deltaTime;
        c.a = Mathf.Clamp01(c.a);

        sr.color = c;
    }
}
