using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComputerGameController : MonoBehaviour
{
    public TMP_Text screenText;
    public GameObject choicePanel;
    public TMP_Text emailButton;
    public TMP_Text filesButton;

    public GameObject emailUI;
    public TMP_InputField emailInput;
    public GameObject sendEmailButton;
    public GameObject exitButton;
    public GameObject backButton;

    private int depthCount = 0;

    void Start()
    {
        ShowMainMenu();
    }

    void ShowMainMenu()
    {
        emailUI.SetActive(false);
        choicePanel.SetActive(true);
        backButton.SetActive(false);
        exitButton.SetActive(true);

        emailButton.text = "Email";
        filesButton.text = "Files";

        screenText.text = $"**Computer OS v1.{depthCount}**\nHello, World?";
    }

    public void EmailSelected()
    {
        choicePanel.SetActive(false);
        emailUI.SetActive(true);
        backButton.SetActive(true);

        screenText.text = "Your message:";
        emailInput.text = "";
    }

    public void SendEmail()
    {
        screenText.text = "Sent!";
        emailUI.SetActive(false);
        backButton.SetActive(false);
    }

    public void FilesSelected()
    {
        choicePanel.SetActive(false);
        backButton.SetActive(true);

        screenText.text = "Filesystem: \n\n[1] ???";
        filesButton.text = "Go deeper";
        emailButton.text = "Cancel";
        
        choicePanel.SetActive(true);
    }

    public void FilesConfirm()
    {
        depthCount++;

        // reload scene to simulate recursion
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CancelFiles()
    {
        ShowMainMenu();
    }

    public void Back()
    {
        backButton.SetActive(false);
        ShowMainMenu();
    }

    public void ExitToCity()
    {
        SceneManager.LoadScene("City");
    }
}
