using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PortraitManager : MonoBehaviour
{
    // 单例模式，方便 Action 脚本快速找到它
    public static PortraitManager Instance;

    [Header("所有立绘 Image 物体")]
    public List<Image> portraitImages;

    public Color activeColor = Color.white;
    public Color dimColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    void Awake()
    {
        Instance = this;
    }

    public void UpdatePortraits(string speakerName)
    {
        foreach (var img in portraitImages)
        {
            if (img == null) continue;

            // 逻辑：如果 Image 的名字包含发言人名字，则亮起
            bool isSpeaker = img.gameObject.name.Contains(speakerName);

            // 使用 Unity 内置的渐变方法，看起来更平滑
            img.CrossFadeColor(isSpeaker ? activeColor : dimColor, 0.2f, false, false);

            if (isSpeaker)
            {
                // 让说话的人显示在最上层
                img.transform.SetAsLastSibling();
            }
        }
    }
}