using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugarClicked : MonoBehaviour
{
    public SugarManager manager;
    private void OnMouseDown()
    {
        transform.gameObject.SetActive(false);
        manager.PaperShow();
    }

}
