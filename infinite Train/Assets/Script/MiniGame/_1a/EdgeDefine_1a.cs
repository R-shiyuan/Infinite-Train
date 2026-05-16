using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDefine_1a : MonoBehaviour
{
    [SerializeField] private Transform victoryBoard;
    [SerializeField] private Transform failBoard;

    public float vicTime = 10f;
    public float currentTime = 0f;
    private void Start()
    {
        currentTime = 0f;
        victoryBoard.gameObject.SetActive(false);
        failBoard.gameObject.SetActive(false);
    }
    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= vicTime)
            Victory();
    }
    private void Victory()
    {
        Time.timeScale = 0;
        victoryBoard.gameObject.SetActive (true);
    }
    private void Fail()
    {
        Time.timeScale = 0;
        failBoard.gameObject.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Soup"))
        {
            Fail();
        }
    }
}
