using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugarManager : MonoBehaviour
{
    public Transform[] sugars;
    public Transform[] sugarPaper;

    private void Awake()
    {
        for (int i = 0;i<sugarPaper.Length;i++) 
            sugarPaper[i].gameObject.SetActive(false);
    }


}
