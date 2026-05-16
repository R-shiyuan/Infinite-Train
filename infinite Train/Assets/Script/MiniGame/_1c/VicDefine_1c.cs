using UnityEngine;
using UnityEngine.EventSystems;

public class VicDefine_1c : MonoBehaviour
{
    public Dirt_1b[] dirts;

    public Transform thisPage;
    public Transform nextPage;

    public float moveSpeed = 8f;

    public Transform board;

    public bool shouldShow = false;

    private void Start()
    {
        dirts = GetComponentsInChildren<Dirt_1b>();
        board.gameObject.SetActive(false);
        thisPage.gameObject.SetActive(shouldShow);
    }

    private void Update()
    {
        for (int i = 0; i < dirts.Length; i++)
        {
            if (dirts[i].sr.color.a >= 0.65f)
                return;
        }
        ShowNext();
    }
    private void ShowNext()
    {
        board.gameObject.SetActive(true);
    }
    public void MovePage()
    {
        board.gameObject.SetActive(false);
        thisPage.gameObject.SetActive(false);
        nextPage.gameObject.SetActive(true);
    }
}
