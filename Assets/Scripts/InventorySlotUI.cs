using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public int slotIndex;           // ← add this back
    public Image backgroundImage;   // always visible
    public Image itemImage;         // shows the TileObject icon

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
