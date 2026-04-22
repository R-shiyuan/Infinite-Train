// 新建一个脚本叫 TestTool.cs
using UnityEngine;

public class TestTool : MonoBehaviour
{
    void Update()
    {
        // 按下 T 键触发测试
        if (Input.GetKeyDown(KeyCode.T))
        {
            GlobalManager.Instance.TriggerTestScenario();
        }
    }
}