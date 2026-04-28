using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class WindowBackgroundScroller : MonoBehaviour
{
    [Header("滚动速度")]
    public float scrollSpeed = 1.0f;

    private RawImage rawImage;
    private Material mat;

    void Awake()
    {
        rawImage = GetComponent<RawImage>();
        // 创建材质实例，避免影响其他物体
        mat = new Material(rawImage.material);
        rawImage.material = mat;
    }

    void Update()
    {
        // 直接通过脚本控制速度，和Shader的逻辑对应
        mat.SetFloat("_ScrollSpeed", scrollSpeed);
    }

    /// <summary>
    /// 切换背景图，保持无缝滚动
    /// </summary>
    public void ChangeBackground(Texture2D newTex)
    {
        if (newTex != null)
        {
            mat.SetTexture("_MainTex", newTex);
        }
    }

    /// <summary>
    /// 暂停滚动
    /// </summary>
    public void Pause()
    {
        scrollSpeed = 0;
    }

    /// <summary>
    /// 恢复滚动
    /// </summary>
    public void Resume()
    {
        scrollSpeed = 1.0f;
    }

    void OnDestroy()
    {
        Destroy(mat);
    }
}