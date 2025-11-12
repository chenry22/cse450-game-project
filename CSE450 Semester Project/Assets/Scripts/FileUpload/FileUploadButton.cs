using UnityEngine;
using UnityEngine.EventSystems;

public class FileUploadButton : MonoBehaviour, IPointerDownHandler
{
    public WebGLUpload webUploader;
    
    public void OnPointerDown(PointerEventData eventData)
    {
#if UNITY_EDITOR
        string path = UnityEditor.EditorUtility.OpenFilePanel("Upload a file", "", "*");
        HandleFileUpload(path);
#elif UNITY_WEBGL
        webUploader.UploadFile();
#else
        Debug.Log("Not yet implemented...");
#endif
    }

    private void HandleFileUpload(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("No file selected.");
            return;
        }

        Debug.Log("File uploaded: " + path);
        // process your file
    }
}
