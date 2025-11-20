using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public int slotIndex;
    public Image backgroundImage;
    public Image itemImage;

    // Sets item in inventory
    public void SetItem(TileObject item)
    {
        if (item == null)
        {
            itemImage.enabled = false;
            itemImage.sprite = null;
        }
        else
        {
            itemImage.enabled = true;
            itemImage.sprite = item.icon;
        }
    }
}
