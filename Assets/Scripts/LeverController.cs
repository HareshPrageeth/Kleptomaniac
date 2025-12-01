using UnityEngine;
using UnityEngine.Tilemaps;

public class LeverController : MonoBehaviour
{
    public Transform player;
    public float activationDistance = 3f;

    private bool isOn = false;

    public Tilemap collisionTilemap;
    public Vector3Int[] tilesToRemove;

    public TileBase tileToAdd;
    public Vector3Int tileToAddPosition;

    private void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist <= activationDistance && Input.GetKeyDown(KeyCode.Space))
        {
            ActivateLever();
        }
    }

    private void ActivateLever()
    {
        if (isOn) return;

        isOn = true;

        // remove tiles
        foreach (Vector3Int pos in tilesToRemove)
            collisionTilemap.SetTile(pos, null);

        // add the new lever tile
        if (tileToAdd != null)
            collisionTilemap.SetTile(tileToAddPosition, tileToAdd);

        Debug.Log("Lever activated: removed tiles + added new tile.");
    }
}
