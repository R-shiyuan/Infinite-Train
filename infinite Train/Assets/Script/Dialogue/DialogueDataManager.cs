using UnityEngine;
using System.Collections.Generic;

public class DialogueDataManager : MonoBehaviour
{
    public static DialogueDataManager Instance;

    // 临时测试用的立绘（之后可以从 Resources 加载）
    public Sprite testPortrait;

    void Awake() { Instance = this; }

    // 模拟 Excel 查询：输入 ID，返回对应的 名字、台词、立绘、是否在左边
    public (string name, string content, Sprite portrait, bool isLeft) GetDialogueData(string id)
    {
        // 这里以后会替换成真正的读取 Excel/JSON 的逻辑
        if (id == "Ray_01")
        {
            return ("Ray", "欢迎来到这趟无限列车，我是你的导游。", testPortrait, true);
        }
        if (id == "Ray_02")
        {
            return ("Ray", "别盯着窗外看太久，会迷失的。", testPortrait, true);
        }

        return ("未知角色", "未找到该 ID 的台词", null, false);
    }
}