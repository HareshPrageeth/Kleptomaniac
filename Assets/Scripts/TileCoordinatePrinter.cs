using UnityEngine;
using UnityEngine.Tilemaps;

public class TileCoordinatePrinter : MonoBehaviour
{
    public Tilemap tilemap;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = tilemap.WorldToCell(mouseWorld);

            Debug.Log($"Tile clicked at: {cell}");
        }
    }
}
