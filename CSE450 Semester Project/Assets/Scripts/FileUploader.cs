using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FileUploader : MonoBehaviour
{
    public GameObject creaturePrefab;
    public GameObject audioCreaturePrefab;
    public GameObject imgCreaturePrefab;

    private string[] imageFiles = { ".png", ".jpg", ".jpeg" };
    private string[] audioFiles = { ".wav", ".mp3" };

    public void UploadNewFile()
    {
        string path = EditorUtility.OpenFilePanel("Upload a file", "", "*");
        if (path.Length != 0)
        {
            var name = Path.GetFileName(path);
            GameObject.Find("SelectedFile").GetComponent<Text>().text = name;
            FileInfo fi = new FileInfo(path);
            var extension = fi.Extension;
            long size = fi.Length;
            Debug.Log(extension + ", " + size);
            Debug.Log(fi.ToString());

            if (imageFiles.Contains(extension))
            {
                var mon = GameObject.Instantiate(imgCreaturePrefab);
                mon.GetComponent<CreatureSelect>().InitCreature(name, size);
            }
            else if (audioFiles.Contains(extension))
            {
                var mon = GameObject.Instantiate(audioCreaturePrefab);
                mon.GetComponent<CreatureSelect>().InitCreature(name, size);
            }
            else
            {
                var mon = Instantiate(creaturePrefab);
                mon.GetComponent<CreatureSelect>().InitCreature(name, size);
            }
        }
    }
}
