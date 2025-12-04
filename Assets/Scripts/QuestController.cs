using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
public class QuestController : MonoBehaviour
{
    public GameObject player;
    public static QuestController Instance;
    public GameObject questContent;
    public GameObject QuestEntryPrefab, QuestObjectivePrefab;
    public static List<QuestItems> CurrentAssignedQuests = new List<QuestItems>();
    public static QuestItems[] AllGameQuests = new QuestItems[]
    {
        new QuestItems
        {
            QuestName = "Help Grandma get home",
            ObjectiveItems = new string[] { "Drop Grandma at Chef's House" },
            quantized = false,
            RequiredObjectiveQuantity = new int[] { },
            CurrentObjectiveQuantity = new int[] { },
            isassigned = false,
            iscompleted = false
        },

        new QuestItems
        {
            QuestName = "Get produce for the chef",
            ObjectiveItems = new string[] { "Carrots", "Lettuce", "Pumpkin", "Turnips" },
            quantized = true,
            RequiredObjectiveQuantity = new int[] { 2, 1, 1, 2 },
            CurrentObjectiveQuantity = new int[] { 0, 0, 0, 0 },
            isassigned = false,
            iscompleted = false
        },

        new QuestItems
        {
            QuestName = "Distract the Farmer",
            ObjectiveItems = new string[] { "Find poison berries to feed the horse" },
            quantized = false,
            RequiredObjectiveQuantity = new int[] { },
            CurrentObjectiveQuantity = new int[] { },
            isassigned = false,
            iscompleted = false
        },

        new QuestItems
        {
            QuestName = "Get the cat to the Guard",
            ObjectiveItems = new string[] { "Find Fredrick the cat and get him back to the gaurd" },
            quantized = false,
            RequiredObjectiveQuantity = new int[] { },
            CurrentObjectiveQuantity = new int[] { },
            isassigned = false,
            iscompleted = false
        },

        new QuestItems
        {
            QuestName = "Help the cat come down",
            ObjectiveItems = new string[] { "Find a tall object and place it down near the cat to help it climb down" },
            quantized = false,
            RequiredObjectiveQuantity = new int[] { },
            CurrentObjectiveQuantity = new int[] { },
            isassigned = false,
            iscompleted = false
        },

        new QuestItems
        {
            QuestName = "Give the cat food",
            ObjectiveItems = new string[] { "Get some food for the cat so it will come with you" },
            quantized = false,
            RequiredObjectiveQuantity = new int[] { },
            CurrentObjectiveQuantity = new int[] { },
            isassigned = false,
            iscompleted = false
        },
        new QuestItems
        {
            QuestName = "Bribe the gaurd",
            ObjectiveItems = new string[] { "Gold" },
            quantized = true,
            RequiredObjectiveQuantity = new int[] {100 },
            CurrentObjectiveQuantity = new int[] { 0},
            isassigned = false,
            iscompleted = false
        }
        
    };

    private void Awake()
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

    void Update()
    {
        if (AllGameQuests[6].isassigned && !AllGameQuests[6].iscompleted && Mathf.Min(200,player.GetComponent<player_controller>().gold)!=AllGameQuests[6].CurrentObjectiveQuantity[0])
        {
            int gold = player.GetComponent<player_controller>().gold;
            gold = Mathf.Min(200,gold);
            UpdateQuestQuantity(6, 0, gold - AllGameQuests[6].CurrentObjectiveQuantity[0]);
        }
    }

    public static void AssignQuest(int questID)
    {
        if (questID >= 0 && questID < AllGameQuests.Length)
        {
            QuestItems questToAssign = AllGameQuests[questID];
            if (!questToAssign.isassigned)
            {
                questToAssign.isassigned = true;
                CurrentAssignedQuests.Add(questToAssign);

                Debug.Log($"Quest Assigned and added to order: {questToAssign.QuestName}");
            }
            else
            {
                Debug.Log($"Quest {questToAssign.QuestName} is already assigned or completed.");
            }
        }
        if (Instance != null)
        {
            Instance.UpdateQuestContent();
        }
    }


    public void UpdateQuestContent()
    {
        // Clear old content
        foreach (Transform child in questContent.transform)
        {
            Destroy(child.gameObject);
        }

        // Generate new content for each assigned quest
        foreach (QuestItems quest in CurrentAssignedQuests)
        {
            
            GameObject questEntryGO = Instantiate(QuestEntryPrefab, questContent.transform);
            
            TextMeshProUGUI questNameText = questEntryGO.transform.Find("QuestName").GetComponent<TextMeshProUGUI>();
            Transform objectiveListParent = questEntryGO.transform.Find("ObjectiveList");
            
            
            if (questNameText != null)
            {
                questNameText.text = quest.QuestName;
            }
            
            GameObject incompleteImage = questEntryGO.transform.Find("QuestName/Incomplete").gameObject;
            GameObject completeImage = questEntryGO.transform.Find("QuestName/Complete").gameObject;

            incompleteImage.SetActive(!quest.iscompleted);
            completeImage.SetActive(quest.iscompleted);

            for (int i = 0; i < quest.ObjectiveItems.Length; i++)
            {
                GameObject objectiveGO = Instantiate(QuestObjectivePrefab, objectiveListParent);
                TextMeshProUGUI objectiveText = objectiveGO.GetComponent<TextMeshProUGUI>();

                if (objectiveText != null)
                {
                    string objectiveDescription = quest.ObjectiveItems[i];
                    
                    if (quest.quantized)
                    {
                        int current = quest.CurrentObjectiveQuantity[i];
                        int required = quest.RequiredObjectiveQuantity[i];
                        objectiveText.text = $"{objectiveDescription} ({current}/{required})";
                    }
                    else 
                    {
                        objectiveText.text = objectiveDescription;
                    }
                }
                
            }
        }
    }

    public static void UpdateQuestQuantity(int questID, int objectiveIndex, int quantityIncrease)
    {
        if (questID >= 0 && questID < AllGameQuests.Length)
        {
            QuestItems quest = AllGameQuests[questID];
            if (quest.quantized && objectiveIndex >= 0 && objectiveIndex < quest.CurrentObjectiveQuantity.Length)
            {
                quest.CurrentObjectiveQuantity[objectiveIndex] += quantityIncrease;
                Debug.Log($"Updated '{quest.QuestName}' - '{quest.ObjectiveItems[objectiveIndex]}' to {quest.CurrentObjectiveQuantity[objectiveIndex]}");
            }
        }
    }

    public static void CompleteQuest(int questID)
    {
        if (questID >= 0 && questID < AllGameQuests.Length)
        {
            AllGameQuests[questID].iscompleted = true;
            Debug.Log($"Quest Completed: {AllGameQuests[questID].QuestName}");
        }
    }

}

[System.Serializable]
public class QuestItems
{
    public string QuestName;
    public string[] ObjectiveItems;
    public bool quantized;
    public int[] RequiredObjectiveQuantity;
    public int[] CurrentObjectiveQuantity;
    public bool isassigned, iscompleted;
}