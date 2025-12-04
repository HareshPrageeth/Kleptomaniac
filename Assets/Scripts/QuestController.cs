using System.Collections.Generic;
using UnityEngine;
public class QuestController : MonoBehaviour
{
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
        }
    };

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