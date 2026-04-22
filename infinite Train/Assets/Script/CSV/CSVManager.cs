using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

[System.Serializable]
public class DialogueRow
{
    public string plotID;
    public int lineIndex;
    public string actorID;
    public string actorName;
    public string express;
    public string pos;
    public string state;
    public string content;
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
        // 确保文件在 Assets/Resources/Data/ 文件夹下
        TextAsset[] files = Resources.LoadAll<TextAsset>("Data");
        foreach (TextAsset file in files)
        {
            if (file.name.Contains("Plot_"))
            {
                ParseCSV(file.text);
            }
        }
    }

    private void ParseCSV(string csvText)
    {
        string[] lines = csvText.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 从 i = 1 开始跳过标题行
        for (int i = 1; i < lines.Length; i++)
        {
            // 正则处理：匹配 CSV 中的逗号，但忽略引号内的逗号
            string[] cols = Regex.Split(lines[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            if (cols.Length < 8) continue;

            try
            {
                DialogueRow row = new DialogueRow
                {
                    plotID = cols[0].Trim(),
                    lineIndex = int.TryParse(cols[1], out int index) ? index : 0,
                    actorID = cols[2].Trim(),
                    actorName = cols[3].Trim(),
                    express = cols[4].Trim(),
                    pos = cols[5].Trim(),
                    state = cols[6].Trim(),
                    content = cols[7].Trim().Replace("\"", "") // 去掉内容里可能残留的引号
                };

                if (!dialogueLibrary.ContainsKey(row.plotID))
                    dialogueLibrary[row.plotID] = new List<DialogueRow>();

                dialogueLibrary[row.plotID].Add(row);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"CSV 解析错误在第 {i} 行: {e.Message}");
            }
        }
        Debug.Log($"CSV 解析完成，当前剧情段落数：{dialogueLibrary.Count}");
    }

    public List<DialogueRow> GetPlot(string plotID)
    {
        if (dialogueLibrary.ContainsKey(plotID))
        {
            // 使用 OrderBy 确保 LineIndex 顺序正确
            return dialogueLibrary[plotID].OrderBy(x => x.lineIndex).ToList();
        }
        Debug.LogWarning($"未找到 plotID: {plotID}");
        return null;
    }
}