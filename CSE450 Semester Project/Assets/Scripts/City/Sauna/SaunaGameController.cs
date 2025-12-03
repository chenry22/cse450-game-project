using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using FileAnalysis;

public class SaunaGameController : MonoBehaviour
{
    public TMP_Text dialogueText;
    public GameObject choicePanel;
    public TMP_Text choiceA;
    public TMP_Text choiceB;
    public GameObject returnButton;

    private CreatureStats stats;
    private List<DialogueNode> selectedNodes;
    private int currentNodeIndex = -1; // -1 for start, 0-2 for selected nodes

    void Start()
    {
        var creature = GameManager.instance?.playerCreature;

        if (creature == null)
        {
            Debug.LogWarning("No creature found!");
            ReturnToCity();
            return;
        }
        
        GameObject c = Instantiate(creature);
        stats = c.GetComponent<CreatureStats>();

        // pick 3 random dialogue options
        selectedNodes = GetRandomNodes(3);
        
        ShowStartDialogue();
    }

    List<DialogueNode> GetRandomNodes(int count)
    {
        List<DialogueNode> available = new List<DialogueNode>(SaunaDialogue.allNodes);
        List<DialogueNode> selected = new List<DialogueNode>();
        
        count = Mathf.Min(count, available.Count);
        
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, available.Count);
            selected.Add(available[randomIndex]);
            available.RemoveAt(randomIndex);
        }
        
        return selected;
    }

    void ShowStartDialogue()
    {
        dialogueText.text = SaunaDialogue.startText;
        choiceA.text = SaunaDialogue.startChoiceA;
        choiceB.text = SaunaDialogue.startChoiceB;
        choicePanel.SetActive(true);
    }

    void ShowNode(int nodeIndex)
    {
        if (nodeIndex < 0 || nodeIndex >= selectedNodes.Count)
        {
            EndSauna();
            return;
        }

        DialogueNode node = selectedNodes[nodeIndex];
        
        dialogueText.text = node.text;
        choiceA.text = node.choiceAText;
        choiceB.text = node.choiceBText;
        choicePanel.SetActive(true);
    }

    public void ChooseA()
    {
        currentNodeIndex++;
        
        if (currentNodeIndex >= selectedNodes.Count)
        {
            EndSauna();
        }
        else
        {
            ShowNode(currentNodeIndex);
        }
    }

    public void ChooseB()
    {
        currentNodeIndex++;
        
        if (currentNodeIndex >= selectedNodes.Count)
        {
            EndSauna();
        }
        else
        {
            ShowNode(currentNodeIndex);
        }
    }

    void EndSauna()
    {
        dialogueText.text = SaunaDialogue.endText;
        choicePanel.SetActive(false);

        var name = GameManager.instance.playerCreature.GetComponent<CreatureSelect>().GetName();
        for(int i = 0; i < GameManager.instance.GetRegisteredCreatures().Count; i++) {
            var registered = GameManager.instance.GetRegisteredCreatures()[i];
            if (registered.GetComponent<CreatureSelect>().GetName() == name) {
                registered.GetComponent<CreatureStats>().GetStats().Cooking += 1;
                break;
            }
        }
        
        returnButton.SetActive(true);
    }

    public void ReturnToCity()
    {
        SceneManager.LoadScene("City");
    }
}