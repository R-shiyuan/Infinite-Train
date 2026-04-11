using UnityEngine;

// 程序A 所有对外接口一键测试脚本
// 挂载后运行，按对应按键测试，测完直接删除
public class Test_AllInterfaces : MonoBehaviour
{
    private PlayerController player;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();

        if (player != null)
            Debug.Log("=== ✅ 接口测试环境正常 ===");
        else
            Debug.LogError("❌ 找不到 PlayerController");
    }

    void Update()
    {
        // --------------- 1. 角色移动开关 ---------------
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (player != null)
            {
                player.SetCanMove(false);
                Debug.Log("✅ 角色已锁定（不能移动）");
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (player != null)
            {
                player.SetCanMove(true);
                Debug.Log("✅ 角色已解锁（可以移动）");
            }
        }

        // --------------- 2. 当前车厢名称 ---------------
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("✅ 当前车厢：" + SceneFade.currentSceneName);
        }

        // --------------- 3. 手动测试场景跳转 ---------------
        if (Input.GetKeyDown(KeyCode.H))
        {
            // 这里改成你真实存在的场景名
            string targetScene = "car1";

            if (SceneFade.Instance != null)
            {
                Debug.Log("✅ 开始跳转到场景：" + targetScene);
                SceneFade.Instance.LoadScene(targetScene);
            }
            else
            {
                Debug.LogError("❌ SceneFade 单例不存在");
            }
        }

        // --------------- 4. 测试NPC点击交互（按U模拟点击） ---------------
        if (Input.GetKeyDown(KeyCode.U))
        {
            TestNPCInteract();
        }
    }

    // 模拟点击NPC，测试 Interactable 接口是否可用
    void TestNPCInteract()
    {
        // 在场景里找任意一个NPC
        NPC[] npcs = FindObjectsOfType<NPC>();

        if (npcs.Length > 0)
        {
            npcs[0].OnInteract();
            Debug.Log("✅ 已模拟点击NPC，接口正常");
        }
        else
        {
            Debug.LogWarning("⚠ 场景中暂无NPC，可后续再测");
        }
    }
}