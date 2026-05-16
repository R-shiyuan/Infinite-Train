using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugarManager : MonoBehaviour
{
    public Transform[] sugars;
    public Transform[] sugarPaper;
    public Transform VicBoard;
    private int sugarAmount;
    private int showIndex = 0;
    private void Awake()
    {
        VicBoard.gameObject.SetActive(false);
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
        VicBoard.gameObject.SetActive(true);
    }
}
