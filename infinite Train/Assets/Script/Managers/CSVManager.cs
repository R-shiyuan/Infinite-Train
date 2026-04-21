using System.Collections.Generic;
using UnityEngine;

// 建议将 DialogueRow 单独放在一个文件 DialogueRow.cs 中
// 或者放在 CSVManager.cs 顶部，作为同一个命名空间下的类
[System.Serializable]
public class DialogueRow
{
    public string plotID;
    public int lineIndex;
    public string actorName;
    public string actorID;
    public string content;
    public string pos;
    public string express;
    public string state; // 关键：增加 State 字段，用于存储剧情触发指令
}

public class CSVManager : MonoBehaviour
{
    public static CSVManager Instance { get; private set; }
    private Dictionary<string, List<DialogueRow>> dialogueLibrary = new Dictionary<string, List<DialogueRow>>();

    private void Awake()
    {
        Instance = this;
        LoadAllPlotFiles();
    }

    private void LoadAllPlotFiles()
    {
        TextAsset[] files = Resources.LoadAll<TextAsset>("Data");
        foreach (TextAsset file in files)
        {
            if (file.name.StartsWith("Plot_")) ParseCSV(file.text);
        }
    }

    private void ParseCSV(string csvText)
    {
        string[] lines = csvText.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] cols = lines[i].Split(',');
            // 这里确保数组长度足够，防止越界
            if (cols.Length < 8) continue;

            DialogueRow row = new DialogueRow
            {
                plotID = cols[0],
                lineIndex = int.Parse(cols[1]),
                actorName = cols[2],
                actorID = cols[3],
                content = cols[4],
                pos = cols[5],
                express = cols[6],
                state = cols[7] // 对应 CSV 的第8列
            };

            if (!dialogueLibrary.ContainsKey(row.plotID)) dialogueLibrary[row.plotID] = new List<DialogueRow>();
            dialogueLibrary[row.plotID].Add(row);
        }
    }

    public List<DialogueRow> GetPlot(string plotID)
    {
        return dialogueLibrary.ContainsKey(plotID) ? dialogueLibrary[plotID] : null;
    }
}