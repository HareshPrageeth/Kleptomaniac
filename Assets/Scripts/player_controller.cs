using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class player_controller : MonoBehaviour
{
    public float moveSpeed = 5f;
    public LayerMask solidObjectsLayer;
    public TileObject[] inventory = new TileObject[8]; // 8 slots

    // Assign these in Inspector
    public Tilemap collisionTilemap;
    public Tilemap walkBehindTilemap;
    public Tilemap groundTilemap;
    public Tilemap decorationTilemap;
    public Tilemap walkInFrontTilemap;

    public TileObjectRegistry objectRegistry;

    public TileObject heldItem = null;
    public SpriteRenderer heldItemRenderer;

    public int gold = 0;
    public  TextMeshProUGUI goldText;

    public GameObject commitPanel;
    public GameObject dropPanel;

    private Vector3Int heldItemOrigin;
    private TilemapType heldItemMainMap;

    public bool canMove = true;
    private bool isMoving;
    private Vector2 input;

    private Animator animator;
    private AudioSource audioSource;
    public AudioClip pickupSound;

    private void Start()
    {
        heldItemRenderer.enabled = false;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!canMove) 
        {
            animator.SetBool("isMoving", false);
            return;
        }

        if (!isMoving)
        {
            // get input
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // prevents diagonal movement
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                if (input.x < 0)
                    transform.localScale = new Vector3(-1, 1, 1);
                else if (input.x > 0)
                    transform.localScale = new Vector3(1, 1, 1);

                Vector3 targetPos = transform.position + new Vector3(input.x, input.y);

                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            BreakTile();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            CommitHeldItem();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropHeldItem();
        }

        animator.SetBool("isMoving", isMoving);
    }

    private IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
        input = Vector2.zero;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        return Physics2D.OverlapCircle(targetPos, 0.1f, solidObjectsLayer) == null;
    }

    // Converts TilemapType enum
    private Tilemap GetTilemap(TilemapType type)
    {
        return type switch
        {
            TilemapType.Collision => collisionTilemap,
            TilemapType.WalkBehind => walkBehindTilemap,
            TilemapType.Ground => groundTilemap,
            TilemapType.Decoration => decorationTilemap,
            _ => null
        };
    }

    private void BreakTile()
    {
        // Cannot pick up new item while holding one
        if (heldItem != null)
        {
            Debug.Log("Already holding item. Press E to store it.");
            return;
        }

        Vector3 facing = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        Vector3Int cellPos = collisionTilemap.WorldToCell(transform.position + facing);

        TileBase tile = collisionTilemap.GetTile(cellPos);
        if (tile == null)
            return;

        TileObject obj = objectRegistry.GetObjectForTile(tile);
        if (obj == null)
        {
            Debug.Log("Tile not part of a registered object.");
            return;
        }

        heldItemOrigin = cellPos;
        heldItemMainMap = obj.parts[0].mapType;

        // Break all connected parts
        foreach (var part in obj.parts)
        {
            Tilemap tm = GetTilemap(part.mapType);
            Vector3Int partPos = cellPos + part.offset;

            if (tm != null)
                tm.SetTile(partPos, null);
        }

        audioSource.PlayOneShot(pickupSound);

        // Hold the item above the head
        heldItem = obj;
        heldItemRenderer.sprite = obj.icon;
        heldItemRenderer.enabled = true;
        heldItemRenderer.gameObject.SetActive(true);

        if (heldItem.inventorySize == 1)
        {
            commitPanel.SetActive(true);
        }

        if (heldItem.name == "Crown")
        {
            Debug.Log("You WIN");
            SceneLoader.Instance.SwitchScene("YouWin");
        }

        dropPanel.SetActive(true);

        Debug.Log($"Picked up {obj.objectName} but not stored yet.");
    }



    private int FindInventorySpace(TileObject obj)
    {
        int size = obj.inventorySize;

        for (int i = 0; i <= inventory.Length - size; i++)
        {
            bool blockFree = true;

            for (int j = 0; j < size; j++)
            {
                if (inventory[i + j] != null)
                {
                    blockFree = false;
                    break;
                }
            }

            if (blockFree)
                return i; // return starting index of the block
        }

        return -1; // no space
    }

    private void CommitInventoryAdd(TileObject obj, int startIndex)
    {
        int size = obj.inventorySize;

        for (int j = 0; j < size; j++)
        {
            inventory[startIndex + j] = obj;
        }

        Debug.Log($"Added {obj.objectName}");
        commitPanel.SetActive(false);
        dropPanel.SetActive(false);
    }

    private void CommitHeldItem()
    {
        if (heldItem == null)
            return;

        // Large items cannot be added
        if (heldItem.inventorySize > 1)
        {
            Debug.Log($"{heldItem.objectName} is too big to fit in the inventory");
            return;
        }

        int slotIndex = FindInventorySpace(heldItem);
        if (slotIndex == -1)
        {
            Debug.Log("Inventory full! Cannot commit item.");
            return;
        }

        // Place item
        CommitInventoryAdd(heldItem, slotIndex);

        // Clear held item
        heldItemRenderer.enabled = false;
        heldItem = null;
        heldItemRenderer.gameObject.SetActive(false);

        Debug.Log("Committed held item to inventory.");
    }
    
    private void DropHeldItem()
    {
        if (heldItem == null)
            return;

        Debug.Log("Dropping held item and restoring tiles: " + heldItem.objectName);

        // Restore all tiles
        foreach (var part in heldItem.parts)
        {
            Tilemap tm = GetTilemap(part.mapType);
            Vector3Int pos = heldItemOrigin + part.offset;

            if (tm != null)
                tm.SetTile(pos, part.tile);   // restore original tile
        }

        heldItemRenderer.enabled = false;
        heldItemRenderer.gameObject.SetActive(false);
        heldItem = null;

        commitPanel.SetActive(false);
        dropPanel.SetActive(false);
    }

    public void ResetMovement()
    {
        isMoving = false;
        input = Vector2.zero;
    }

    public void FaceDirection(Vector2 dir)
    {
        animator.SetFloat("moveX", dir.x);
        animator.SetFloat("moveY", dir.y);

        if (dir.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (dir.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
        goldText.text = $"Gold: {gold}";
    }

}
