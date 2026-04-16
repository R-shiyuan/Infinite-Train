using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleaner_1b : MonoBehaviour
{
    public Texture2D cleanerTxt;

    private void Start()
    {
        Cursor.SetCursor(cleanerTxt, Vector2.zero, CursorMode.Auto);
    }
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Dirt_1b dirt = hit.collider.GetComponent<Dirt_1b>();
                if (dirt != null)
                    dirt.Wash();
            }
        }
    }
}
