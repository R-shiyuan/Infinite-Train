using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lines_2b : MonoBehaviour
{
    [Header("Line")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth = 0.15f;
    [SerializeField] private float pointSpacing = 0.1f;
    [SerializeField] private int targetConnectionCount = 3;
    [SerializeField] private Transform vicBoard;

    private LineRenderer currentLine;
    private Blocks_2b startBlock;
    private readonly List<Vector3> points = new();
    private readonly Dictionary<int, LineRenderer> finishedLines = new();

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        vicBoard.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            StartDraw();

        if (Input.GetMouseButton(0) && currentLine != null)
            Draw();

        if (Input.GetMouseButtonUp(0) && currentLine != null)
            EndDraw();
    }

    private void StartDraw()
    {
        Blocks_2b hitBlock = GetBlockUnderMouse();
        if (hitBlock == null)
            return;

        if (finishedLines.ContainsKey(hitBlock.colorID))
            return;

        startBlock = hitBlock;
        points.Clear();

        GameObject lineObj = new GameObject("Line_" + startBlock.colorID);
        currentLine = lineObj.AddComponent<LineRenderer>();

        currentLine.material = lineMaterial != null
            ? lineMaterial
            : new Material(Shader.Find("Sprites/Default"));

        currentLine.useWorldSpace = true;
        currentLine.positionCount = 0;
        currentLine.startWidth = lineWidth;
        currentLine.endWidth = lineWidth;
        currentLine.numCapVertices = 8;
        currentLine.sortingOrder = 10;

        SpriteRenderer sr = startBlock.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            currentLine.startColor = sr.color;
            currentLine.endColor = sr.color;
        }

        AddPoint(startBlock.transform.position, true);
    }

    private void Draw()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        mouseWorldPos.z = startBlock.transform.position.z;
        AddPoint(mouseWorldPos);
    }

    private void EndDraw()
    {
        Blocks_2b endBlock = GetBlockUnderMouse();

        bool isValid =
            endBlock != null &&
            endBlock != startBlock &&
            endBlock.colorID == startBlock.colorID;

        if (isValid)
        {
            AddPoint(endBlock.transform.position, true);
            finishedLines[startBlock.colorID] = currentLine;
            CheckWin();
        }
        else
        {
            Destroy(currentLine.gameObject);
        }

        currentLine = null;
        startBlock = null;
        points.Clear();
    }

    private void AddPoint(Vector3 point, bool force = false)
    {
        if (!force && points.Count > 0 &&
            Vector3.Distance(points[points.Count - 1], point) < pointSpacing)
            return;

        points.Add(point);
        currentLine.positionCount = points.Count;
        currentLine.SetPosition(points.Count - 1, point);
    }

    private Blocks_2b GetBlockUnderMouse()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (!hit.collider)
            return null;

        return hit.collider.GetComponent<Blocks_2b>();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        return worldPos;
    }

    private void CheckWin()
    {
        if (finishedLines.Count >= targetConnectionCount)
        {
            vicBoard.gameObject.SetActive(true);
        }
    }

    public void ClearAllLines()
    {
        foreach (LineRenderer line in finishedLines.Values)
        {
            if (line != null)
                Destroy(line.gameObject);
        }

        finishedLines.Clear();

        if (currentLine != null)
        {
            Destroy(currentLine.gameObject);
            currentLine = null;
        }

        startBlock = null;
        points.Clear();
    }
}
