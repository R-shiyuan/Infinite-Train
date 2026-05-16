using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Transform GuideBoard;
    public Transform NextBoard;

    private void Start()
    {
        GuideBoard.gameObject.SetActive(true);
        NextBoard.gameObject.SetActive(false);
    }
    public void ShowNext()
    {
        NextBoard.gameObject.SetActive(true);
    }
} 
