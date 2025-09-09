using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    private HashSet<string> imageFiles = new HashSet<string> { ".png", ".jpg", ".jpeg", ".gif", ".webp" };
    private HashSet<string> audioFiles = new HashSet<string> { ".wav", ".mp3" };
    private HashSet<string> videoFiles = new HashSet<string> { ".mp4", ".mov" };
    private HashSet<string> documentFiles = new HashSet<string> { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt" };
    private HashSet<string> archiveFiles = new HashSet<string> { ".zip", ".rar", ".tar", ".gz" };
    private HashSet<string> codeFiles = new HashSet<string> { ".cs", ".js", ".jsx", ".ts", ".tsx", ".html", ".css", ".json", ".xml", ".yml", ".yaml", ".cpp", ".h", ".java", ".py", ".rb", ".php" };

    public void UploadNewFile()
    {
        string path = EditorUtility.OpenFilePanel("Upload a file", "", "*");
        if (path.Length != 0)
        {
            var name = Path.GetFileName(path);
            GameObject.Find("SelectedFile").GetComponent<Text>().text = name;
            FileInfo fi = new FileInfo(path);
            var extension = fi.Extension.ToLowerInvariant();
            long size = fi.Length;
            Debug.Log(extension + ", " + size);
            Debug.Log(fi.ToString());

            GameObject prefabToUse = defaultCreaturePrefab;

            if (imageFiles.Contains(extension))
                prefabToUse = imageCreaturePrefab;
            else if (audioFiles.Contains(extension))
                prefabToUse = audioCreaturePrefab;
            else if (videoFiles.Contains(extension))
                prefabToUse = videoCreaturePrefab;
            else if (documentFiles.Contains(extension))
                prefabToUse = documentCreaturePrefab;
            else if (archiveFiles.Contains(extension))
                prefabToUse = archiveCreaturePrefab;
            else if (codeFiles.Contains(extension))
                prefabToUse = codeCreaturePrefab;

            var mon = Instantiate(prefabToUse);
            mon.GetComponent<CreatureSelect>().InitCreature(name, size);
        }
    }
}
