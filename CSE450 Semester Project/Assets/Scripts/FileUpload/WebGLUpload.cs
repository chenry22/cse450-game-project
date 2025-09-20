using System.Runtime.InteropServices;
using UnityEngine;

// Workaround to allow WebGL uploads to interact with files
// Basically FileUploader button will call some JS that then sends a signal back once a user selects a file
// This signal gets processed as a virtual path the rest of the game can use

public class WebGLUpload : MonoBehaviour
{
    // in Assets/Plugins
    [DllImport("__Internal")]
    private static extern void OpenFileUploadWeb(string gameObjectName, string methodName);

    public void UploadFile() {
        OpenFileUploadWeb(gameObject.name, "OnFileSelected");
    }

    // Called from JS one file is uploaded
    public void OnFileSelected(string virtualPath) {
        Debug.Log("Virtual file available at: " + virtualPath);
        gameObject.GetComponent<FileUploader>().HandleFileUpload(virtualPath);
    }
}