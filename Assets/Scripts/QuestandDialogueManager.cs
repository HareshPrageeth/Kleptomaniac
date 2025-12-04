using UnityEngine;

public class QuestandDialogueManager : MonoBehaviour
{
    public GameObject NoQuestGuard, QuestGuard, castlecollider;
    public NPC_Dialogue waitforgold, thanksforgold;
    public GameObject player;
    int flag = 0;
    void Update()
    {
        if (flag == 0 && QuestController.AllGameQuests[6].isassigned)
        {
            flag = 1;
            QuestGuard.GetComponent<NPC_Dialogue_Controller>().dialogueData = waitforgold;
        }
        if (flag == 1)
        {
            int gold = player.GetComponent<player_controller>().gold;
            if (gold >= QuestController.AllGameQuests[6].RequiredObjectiveQuantity[0])
            {
                QuestGuard.GetComponent<NPC_Dialogue_Controller>().dialogueData = thanksforgold;
            }
            flag = 2;
        }
        if(flag==2 && QuestController.AllGameQuests[6].iscompleted)
        {
            castlecollider.SetActive(true);
        }
    

    }

}
