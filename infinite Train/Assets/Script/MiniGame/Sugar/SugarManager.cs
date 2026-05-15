using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugarManager : MonoBehaviour
{
    public Transform[] sugars;
    public Transform[] sugarPaper;
    private int sugarAmount;
    private int showIndex = 0;
    private void Awake()
    {
        for (int i = 0;i<sugarPaper.Length;i++) 
            sugarPaper[i].gameObject.SetActive(false);

        sugarAmount = sugars.Length;
    }

    public void PaperShow()
    {
        sugarPaper[showIndex].gameObject.SetActive(true);
        showIndex++;
        if (showIndex == sugarAmount)
            Vic();
    }
    public void Vic()
    {
        Debug.Log("111");
    }
}
