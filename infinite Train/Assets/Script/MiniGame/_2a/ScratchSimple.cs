using UnityEngine;
using UnityEngine.UI;

public class ScratchSimple : MonoBehaviour
{
    public RawImage maskImage;
    public int textureSize = 1024;
    public int brushSize = 20;

    int totalPixels;
    int clearedPixels = 0;
    bool[] clearedMap;

    private Texture2D maskTex;

    public Transform NextBoard;
    void Start()
    {
        maskTex = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);

        // 初始化灰色
        Color[] colors = new Color[textureSize * textureSize];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = new Color(0.7f, 0.7f, 0.7f, 1);

        maskTex.SetPixels(colors);
        maskTex.Apply();

        maskImage.texture = maskTex;
        totalPixels = textureSize * textureSize;
        clearedMap = new bool[totalPixels];
    }
    void ShowNext()
    {
        NextBoard.gameObject.SetActive(true);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                maskImage.rectTransform,
                Input.mousePosition,
                null,
                out localPos
            );

            Draw(localPos);
        }
        if (GetProgress() >= 0.9f)
        {
            ShowNext();
        }
    }

    void Draw(Vector2 pos)
    {
        int x = (int)((pos.x / maskImage.rectTransform.rect.width + 0.5f) * textureSize);
        int y = (int)((pos.y / maskImage.rectTransform.rect.height + 0.5f) * textureSize);

        for (int i = -brushSize; i <= brushSize; i++)
        {
            for (int j = -brushSize; j <= brushSize; j++)
            {
                int px = x + i;
                int py = y + j;

                if (px >= 0 && px < textureSize && py >= 0 && py < textureSize)
                {
                    float dist = Mathf.Sqrt(i * i + j * j);
                    if (dist < brushSize)
                    {
                        int index = py * textureSize + px;

                        if (!clearedMap[index]) // 只统计一次
                        {
                            clearedMap[index] = true;
                            clearedPixels++;
                        }

                        maskTex.SetPixel(px, py, new Color(0, 0, 0, 0));
                    }
                }
            }
        }

        maskTex.Apply();
    }
    float GetProgress()
    {
        return (float)clearedPixels / totalPixels;
    }
}