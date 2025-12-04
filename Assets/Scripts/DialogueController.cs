using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; }
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;


    private NPC_Dialogue_Controller currentNPC;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void showDialogueUI(bool show)
    {
        dialoguePanel.SetActive(show);
    }

    public void setNPCInfo(string npcName, Sprite portrait, NPC_Dialogue_Controller npc)
    {
        nameText.text = npcName;
        portraitImage.sprite = portrait;
        currentNPC = npc;
    }
    public void setDialogueText(string text)
    {
        dialogueText.text = text;
    }
    public void EndDialogueFromUI()
    {
        if (currentNPC != null)
        {
            currentNPC.EndDialogue();
            currentNPC = null;
        }
    }
    public void ClearChoices()
    {
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
    }
    public void CreateChoiceButton(string choiceText, UnityEngine.Events.UnityAction onClick)
    {
        GameObject choiceButton = Instantiate(choiceButtonPrefab, choiceContainer);
        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText;
        choiceButton.GetComponent<Button>().onClick.AddListener(onClick);
    }
}
