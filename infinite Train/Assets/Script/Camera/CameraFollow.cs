using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标（拖入角色）")]
    public Transform target;

    [Header("火车地板白模（拖入你的地板对象）")]
    public Transform trainFloor; // 拖入你的火车地板白模

    [Header("跟随平滑度（0-10，越大越跟得紧）")]
    public float smoothSpeed = 5f;

    // 自动计算的相机边界
    private float cameraHalfWidth;
    private float minCameraX;
    private float maxCameraX;

    void Start()
    {
        // 🔴 自动计算相机水平半宽（核心！保证视野<地图长度）
        float screenAspect = (float)Screen.width / Screen.height;
        cameraHalfWidth = GetComponent<Camera>().orthographicSize * screenAspect;

        // 🔴 自动计算相机的左右边界（永远不超出地板）
        SpriteRenderer floorRenderer = trainFloor.GetComponent<SpriteRenderer>();
        float floorHalfWidth = floorRenderer.bounds.extents.x;
        float floorMinX = trainFloor.position.x - floorHalfWidth;
        float floorMaxX = trainFloor.position.x + floorHalfWidth;

        // 相机边界 = 地板边界 ± 相机半宽（保证相机永远在地板内）
        minCameraX = floorMinX + cameraHalfWidth;
        maxCameraX = floorMaxX - cameraHalfWidth;

        // 初始相机位置对齐
        transform.position = new Vector3(Mathf.Clamp(target.position.x, minCameraX, maxCameraX), transform.position.y, transform.position.z);
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 计算目标相机位置（只跟随X轴，Y/Z固定）
        float targetX = Mathf.Clamp(target.position.x, minCameraX, maxCameraX);
        Vector3 targetPos = new Vector3(targetX, transform.position.y, transform.position.z);

        // 平滑跟随（Lerp实现丝滑过渡）
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}