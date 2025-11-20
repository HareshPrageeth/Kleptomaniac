using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public player_controller player;     
    public GameObject slotPrefab;        
    public GridLayoutGroup grid;

    private InventorySlotUI[] slots;

    private void Start()
    {
        BuildSlots();
        Refresh();
    }

    // Creates UI slot objects dynamically based on inventory array length
    private void BuildSlots()
    {
        int count = player.inventory.Length;
        slots = new InventorySlotUI[count];

        for (int i = 0; i < count; i++)
        {
            GameObject slot = Instantiate(slotPrefab, grid.transform);
            slots[i] = slot.GetComponent<InventorySlotUI>();
            slots[i].slotIndex = i;
        }
    }

    private void Update()
    {
        Refresh();
    }

    // Updates icons whenever inventory changes
    public void Refresh()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            TileObject obj = player.inventory[i];
            slots[i].SetItem(obj);
        }
    }
}
