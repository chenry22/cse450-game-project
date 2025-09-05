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

    private string[] imageFiles = { ".png", ".jpg", ".jpeg", ".gif", ".webp" };
    private string[] audioFiles = { ".wav", ".mp3" };
    private string[] videoFiles = { ".mp4", ".mov" };
    private string[] documentFiles = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt" };
    private string[] archiveFiles = { ".zip", ".rar", ".tar", ".gz" };
    private string[] codeFiles = { ".cs", ".js", ".jsx", ".ts", ".tsx", ".html", ".css", ".json", ".xml", ".yml", ".yaml", ".cpp", ".h", ".java", ".py", ".rb", ".php" };

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
