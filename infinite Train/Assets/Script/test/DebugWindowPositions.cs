using UnityEngine;

public class DebugWindowPositions : MonoBehaviour
{
    public Transform targetNPC; // 将 NPC 拖入此字段

    void Start()
    {
        GameObject[] windows = GameObject.FindGameObjectsWithTag("Window");
        foreach (GameObject w in windows)
        {
            float dist = Vector3.Distance(targetNPC.position, w.transform.position);
            Debug.Log($"车窗: {w.name} | 位置: {w.transform.position} | 距离 NPC: {dist:F2}");
        }
    }
}