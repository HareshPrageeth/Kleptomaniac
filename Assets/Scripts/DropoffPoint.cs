using UnityEngine;

public class DropoffPoint : MonoBehaviour
{
    private player_controller player;

    private AudioSource audioSource;
    public AudioClip goldSound;

    public GameObject stashPanel;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player entered dropoff point.");

        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<player_controller>();
        }

        stashPanel.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Player exited dropoff point.");
        if (other.CompareTag("Player"))
        {
            player = null;
        }

        stashPanel.SetActive(false);
    }

    private void Update()
    {
        if (player == null) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DepositItem(player);
        }
    }

    private void DepositItem(player_controller player)
    {
        TileObject objToDeposit = null;

        // try depositing the held item
        if (player.heldItem != null)
        {
            objToDeposit = player.heldItem;

            // clear held item
            player.heldItemRenderer.enabled = false;
            player.heldItemRenderer.gameObject.SetActive(false);
            player.heldItem = null;
        }
        else
        {
            // otherwise deposit the last item in the inventory
            int lastIndex = FindLastInventoryItem(player);

            if (lastIndex == -1)
            {
                Debug.Log("Player has nothing to deposit.");
                return;
            }

            objToDeposit = player.inventory[lastIndex];

            // remove it
            player.inventory[lastIndex] = null;
        }

        // add gold to player
        player.AddGold(objToDeposit.goldValue);
        audioSource.PlayOneShot(goldSound);


        Debug.Log($"Deposited {objToDeposit.objectName} for {objToDeposit.goldValue} gold.");
    }


    // finds the last non-null inventory slot
    private int FindLastInventoryItem(player_controller player)
    {
        for (int i = player.inventory.Length - 1; i >= 0; i--)
        {
            if (player.inventory[i] != null)
                return i;
        }
        return -1;
    }

}
