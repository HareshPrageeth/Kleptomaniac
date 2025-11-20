using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public enum TilemapType
{
    Collision,
    WalkBehind,
    Ground,
    Decoration,
    WalkInFront
}

[CreateAssetMenu(fileName = "TileObject", menuName = "Game/Tile Object")]
public class TileObject : ScriptableObject
{
    [System.Serializable]
    public struct TilePart
    {
        public TilemapType mapType;   // which tilemap layer this part is on
        public Vector3Int offset;     // relative position
        public TileBase tile;
    }

    public string objectName;
    public TileBase mainTile;       // tile the player picks up
    public List<TilePart> parts;    // all connected tiles
    public int inventorySize = 1;
    public Sprite icon;
}
