using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VicDefine_1b : MonoBehaviour
{
    private Dirt_1b[] dirts;
    [SerializeField] private Transform vicBoard;
    [SerializeField] private Transform guidingBoard;

    private void Start()
    {
        Time.timeScale = 1.0f;
        dirts= GetComponentsInChildren<Dirt_1b>();
        vicBoard.gameObject.SetActive(false);
    }
    private void Update()
    {
        for (int i = 0; i < dirts.Length; i++)
        {
            if (dirts[i].sr.color.a > 0.01)
                return;
        }
        vicBoard.gameObject.SetActive(true);
    }
}
