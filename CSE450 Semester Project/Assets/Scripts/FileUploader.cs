using System;
using FileAnalysis;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FileUploader : MonoBehaviour
{

    [Header("Default Creature")]
    public GameObject defaultCreaturePrefab;
    [Header("Audio Creature")]
    public GameObject audioCreaturePrefab;
    [Header("Image Creature")]
    public GameObject imageCreaturePrefab;
    [Header("Video Creature")]
    public GameObject videoCreaturePrefab;
    [Header("Document Creature")]
    public GameObject documentCreaturePrefab;
    [Header("Archive Creature")]
    public GameObject archiveCreaturePrefab;
    [Header("Code Creature")]
    public GameObject codeCreaturePrefab;

    private Dictionary<string, GameObject> prefabMap;
    private Dictionary<string, Func<string, object>> fileAnalyzerMap;

    private void Awake()
    {
    prefabMap = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase)
        {
            { ".png", imageCreaturePrefab }, { ".jpg", imageCreaturePrefab }, { ".jpeg", imageCreaturePrefab }, { ".gif", imageCreaturePrefab }, { ".webp", imageCreaturePrefab },
            { ".wav", audioCreaturePrefab }, { ".mp3", audioCreaturePrefab },
            { ".mp4", videoCreaturePrefab }, { ".mov", videoCreaturePrefab },
            { ".pdf", documentCreaturePrefab }, { ".doc", documentCreaturePrefab }, { ".docx", documentCreaturePrefab }, { ".xls", documentCreaturePrefab }, { ".xlsx", documentCreaturePrefab },
            { ".ppt", documentCreaturePrefab }, { ".pptx", documentCreaturePrefab }, { ".txt", documentCreaturePrefab },
            { ".zip", archiveCreaturePrefab }, { ".rar", archiveCreaturePrefab }, { ".tar", archiveCreaturePrefab }, { ".gz", archiveCreaturePrefab },
            { ".cs", codeCreaturePrefab }, { ".js", codeCreaturePrefab }, { ".jsx", codeCreaturePrefab }, { ".ts", codeCreaturePrefab }, { ".tsx", codeCreaturePrefab },
            { ".html", codeCreaturePrefab }, { ".css", codeCreaturePrefab }, { ".json", codeCreaturePrefab }, { ".xml", codeCreaturePrefab }, { ".yml", codeCreaturePrefab },
            { ".yaml", codeCreaturePrefab }, { ".cpp", codeCreaturePrefab }, { ".h", codeCreaturePrefab }, { ".java", codeCreaturePrefab }, { ".py", codeCreaturePrefab },
            { ".rb", codeCreaturePrefab }, { ".php", codeCreaturePrefab }
        };

        fileAnalyzerMap = new Dictionary<string, Func<string, object>>(StringComparer.OrdinalIgnoreCase)
        {
            { ".html", path => HTMLAnalyzer.Analyze(path) },
            { ".json", path => JSONAnalyzer.Analyze(path) },
            // TODO: Add all the file analyzers here
            // { ".extension", path => [EXTENSION]Analyzer.Analyze(path) },
        };
    }
    

    public void UploadNewFile()
    {
        string path = EditorUtility.OpenFilePanel("Upload a file", "", "*");
        if (path.Length != 0)
        {
            var name = Path.GetFileName(path);
            var selectedFileObj = GameObject.Find("SelectedFile");
            if (selectedFileObj != null)
            {
                var textComp = selectedFileObj.GetComponent<Text>();
                if (textComp != null)
                    textComp.text = name;
            }
            FileInfo fi = new FileInfo(path);
            var extension = fi.Extension.ToLowerInvariant();
            long size = fi.Length;
            Debug.Log(extension + ", " + size);
            Debug.Log(fi.ToString());

            bool isSupported = prefabMap.ContainsKey(extension);
            GameObject prefabToUse = isSupported ? prefabMap[extension] : defaultCreaturePrefab;
            var mon = Instantiate(prefabToUse);

            Stats stats = null;
            if (isSupported && fileAnalyzerMap.ContainsKey(extension))
            {
                var result = fileAnalyzerMap[extension](path);
                if (result is Stats s) { stats = s; }
            }
            else
            {
                stats = new Stats(); // default stats (50 for everything)
            }

            if (stats != null)
            {
                Debug.Log(
                    $"Stats:\n" +
                    $"Dough Handling: {stats.DoughHandling}\n" +
                    $"Toppings: {stats.Toppings}\n" +
                    $"Cooking: {stats.Cooking}\n" +
                    $"Cutting: {stats.Cutting}\n" +
                    $"Speed: {stats.Speed}\n" +
                    $"Stamina: {stats.Stamina}"
                );
            }
            mon.GetComponent<CreatureSelect>().InitCreature(name, size, stats);
        }
    }
}
