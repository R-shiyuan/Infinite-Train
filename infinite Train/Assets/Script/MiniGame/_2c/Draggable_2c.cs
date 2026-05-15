using UnityEngine;
using UnityEngine.SceneManagement;

public class Draggable_2c : MonoBehaviour
{
    Vector2 mousePos;
    Vector2 distance;
    public Vector2 upMax;
    public Vector2 downMax;

    public Transform whiteFlower;
    public Transform endButton;
    [SerializeField]private Transform arrow;
    [SerializeField]private Sprite arrowDown;
    public bool isShown = false;
    public string nextSceneName;

    // 存储初始位置
    private Vector2 initialPosition;

    private void Start()
    {
        // 记录物体初始位置
        initialPosition = transform.position;
        endButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (transform.position.y >= (initialPosition.y + upMax.y - 0.3) && isShown == false)
            ShowYellow();
        if (isShown && (transform.position.y <= (initialPosition.y + 0.45)))
            GameEnd();
    }

    private void OnMouseDown()
    {
        distance = new Vector2(transform.position.x, transform.position.y) - mousePos;
    }

    private void OnMouseDrag()
    {
        // 计算目标位置
        Vector2 targetPosition = mousePos + distance;

        // 只修改Y轴位置，X轴保持原位
        targetPosition.x = initialPosition.x;

        // 限制Y轴移动范围
        float minY = initialPosition.y + downMax.y;
        float maxY = initialPosition.y + upMax.y;

        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        // 应用位置
        transform.position = targetPosition;
    }
    private void ShowYellow()
    {
        whiteFlower.gameObject.SetActive(false);
        isShown = true;
        SpriteRenderer sr = arrow.GetComponent<SpriteRenderer>();
        sr.sprite = arrowDown;
    }
    public void GameEnd()
    {
        endButton.gameObject.SetActive(true);
    }
    public void Next()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}