using System;
using FileAnalysis;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FileUploader : MonoBehaviour {
    private const int maxFileNameLength = 12;

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

    public WebGLUpload webUploader;

    private void Awake()
    {
        webUploader = GetComponent<WebGLUpload>();
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

        fileAnalyzerMap = new Dictionary<string, Func<string, object>>(StringComparer.OrdinalIgnoreCase) {
            { ".html", path => HTMLAnalyzer.Analyze(path) },
            { ".json", path => JSONAnalyzer.Analyze(path) },
            { ".txt", path => TXTAnalyzer.Analyze(path) },
            // TODO: Add all the file analyzers here
            // { ".extension", path => [EXTENSION]Analyzer.Analyze(path) },
        };
    }
    

    public void UploadNewFile() {
#if UNITY_EDITOR
        string path = EditorUtility.OpenFilePanel("Upload a file", "", "*");
        HandleFileUpload(path);
#elif UNITY_WEBGL
        webUploader.UploadFile();
#else
        // TODO: there is probably some way to do this
        Debug.Log("not yet implemented...");
#endif
    }

    // because WebGL upload will not wait, needs to be a separate call
    public void HandleFileUpload(string path) {
        if (path.Length != 0) {
            // want to avoid super long names as to not clutter UI
            var name = Path.GetFileName(path).Split(".")[0];
            name = name[..Mathf.Min(maxFileNameLength, name.Length)];

            FileInfo fi = new FileInfo(path);
            var extension = fi.Extension.ToLowerInvariant();
            name += extension;
            long size = fi.Length;
            Debug.Log(extension + ", " + size);
            Debug.Log(fi.ToString());

            bool isSupported = prefabMap.ContainsKey(extension);
            GameObject prefabToUse = isSupported ? prefabMap[extension] : defaultCreaturePrefab;
            var mon = Instantiate(prefabToUse, prefabToUse.transform.position,
                prefabToUse.transform.rotation, GameObject.Find("Spawner").transform);

            Stats stats = null;
            if (isSupported && fileAnalyzerMap.ContainsKey(extension)) {
                var result = fileAnalyzerMap[extension](path);
                if (result is Stats s) { stats = s; }
            } else {
                // stats = new Stats(); // default stats (50 for everything)
                stats = new Stats(path); // make things a little interesting by using more variable constructor
            }

            if (stats != null) {
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
            GameObject.Find(GameManager.kitchenGameManager).GetComponent<GameManager>().RegisterCreature(mon);

            // Put each new creature into save state buffer.
            SaveData.Instance.creatures.Add(new SavedCreature(name, size, stats,
                extension, mon.transform.position));
        }
    }


    /* Retrieve Prefab from extension
     * Used for saving game data.
    */
    public GameObject Ext2Prefab(string extension)
    {
        if (prefabMap.ContainsKey(extension))
        {
            return prefabMap[extension];
        }

        return defaultCreaturePrefab;
    }
}
