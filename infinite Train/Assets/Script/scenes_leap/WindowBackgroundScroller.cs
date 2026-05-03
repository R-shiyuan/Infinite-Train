using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WindowBackgroundScroller : MonoBehaviour
{
    [Header("滚动速度")]
    public float scrollSpeed = 0.5f;
    [Header("水平平铺次数（覆盖5个车窗）")]
    public float tilingX = 5f;

    private Material mat;

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        // 复制材质避免影响其他物体
        mat = new Material(sr.material);
        sr.material = mat;
        // 设置纹理平铺（水平重复 tilingX 次）
        mat.SetTextureScale("_MainTex", new Vector2(tilingX, 1f));
    }

    void Update()
    {
        if (mat != null)
            mat.SetFloat("_ScrollSpeed", scrollSpeed);
    }

    public void ChangeBackground(Texture2D newTex)
    {
        if (mat != null && newTex != null)
            mat.SetTexture("_MainTex", newTex);
    }

    void OnDestroy()
    {
        if (mat != null) Destroy(mat);
    }
}