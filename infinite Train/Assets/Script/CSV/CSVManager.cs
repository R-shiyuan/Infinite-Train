using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
public class CSVManager : MonoBehaviour
{
    public static CSVManager Instance { get; private set; }

    private Dictionary<string, List<DialogueRow>> dialogueLibrary = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllPlotFiles();
    }

    private void LoadAllPlotFiles()
    {
        TextAsset[] files = Resources.LoadAll<TextAsset>("Data");

        foreach (TextAsset file in files)
        {
            if (file.name.Contains("Plot_"))
                ParseCSV(file.text);
        }
    }

    private void ParseCSV(string csvText)
    {
        string[] lines = csvText.Split(new[] { "\r\n", "\r", "\n" },
            System.StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length <= 1) return;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] cols = Regex.Split(lines[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            if (cols.Length < 8) continue;

            try
            {
                DialogueRow row = new DialogueRow
                {
                    plotID = cols[0].Trim(),
                    lineIndex = int.TryParse(cols[1], out int idx) ? idx : 0,
                    actorID = cols[2].Trim(),
                    actorName = cols[3].Trim(),
                    express = cols[4].Trim(),
                    pos = cols[5].Trim(),
                    state = cols[6].Trim(),
                    text = cols[7].Trim().Replace("\"", "")
                };

                if (!dialogueLibrary.ContainsKey(row.plotID))
                    dialogueLibrary[row.plotID] = new List<DialogueRow>();

                dialogueLibrary[row.plotID].Add(row);
            }
            catch { }
        }
    }

    public List<DialogueRow> GetPlot(string plotID)
    {
        if (dialogueLibrary.ContainsKey(plotID))
            return dialogueLibrary[plotID].OrderBy(x => x.lineIndex).ToList();

        return null;
    }
}