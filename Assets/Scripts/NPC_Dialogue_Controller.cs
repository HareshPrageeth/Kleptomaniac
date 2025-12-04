using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class NPC_Dialogue_Controller : MonoBehaviour, Interactable
{
    public NPC_Dialogue dialogueData;
    private DialogueController dialogueUI;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    void Start()
    {
        dialogueUI = DialogueController.Instance;
    }

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if (dialogueData == null || (PauseController.gameIsPaused && !isDialogueActive))
        {
            return;
        }
        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;
        dialogueUI.setNPCInfo(dialogueData.npcName, dialogueData.npcPortrait,this);
        dialogueUI.showDialogueUI(true);
        PauseController.PausedForDialogue = true;
        Time.timeScale = 0f;
        DisplayCurrentLine();
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueUI.setDialogueText(dialogueData.dilaogueLines[dialogueIndex]);
            isTyping = false;
        }

        dialogueUI.ClearChoices();
        if (dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }
        foreach(DialogueChoice dialogueChoice in dialogueData.choices)
        {
            if (dialogueChoice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(dialogueChoice);
                return;
            }
        }
        if (++dialogueIndex < dialogueData.dilaogueLines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueUI.setDialogueText("");

        foreach (char letter in dialogueData.dilaogueLines[dialogueIndex])
        {
            dialogueUI.setDialogueText(dialogueUI.dialogueText.text += letter);
            yield return new WaitForSecondsRealtime(dialogueData.typingSpeed);
        }

        isTyping = false;

        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSecondsRealtime(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    void DisplayChoices(DialogueChoice choice)
    {
        int[] assignIDs = choice.QueststoAssign ?? new int[0];
        int[] completeIDs = choice.QueststoMarkComplete ?? new int[0];

        for (int i = 0; i < choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndexes[i];
            dialogueUI.CreateChoiceButton(choice.choices[i], 
                () => ChooseOption(nextIndex, assignIDs, completeIDs));
        }
    }
    void ChooseOption(int nextIndex, int[] assign, int[] markcomplete)
    {
        ExecuteQuestAssignments(assign);
        ExecuteQuestCompletions(markcomplete);

        dialogueIndex = nextIndex;
        dialogueUI.ClearChoices();
        DisplayCurrentLine();
    }
    private void ExecuteQuestAssignments(int[] assignIDs)
    {
        if (assignIDs != null)
        {
            foreach (int questID in assignIDs)
            {
                if (questID >= 0)
                {
                    QuestController.AssignQuest(questID); 
                }
            }
        }
    }

    private void ExecuteQuestCompletions(int[] completeIDs)
    {
        if (completeIDs != null)
        {
            foreach (int questID in completeIDs)
            {
                if (questID >= 0)
                {
                    QuestController.CompleteQuest(questID);
                }
            }
        }
    }
    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }
    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.setDialogueText("");
        dialogueUI.showDialogueUI(false);
        PauseController.PausedForDialogue = false;
        Time.timeScale = 1f;
    }
}

