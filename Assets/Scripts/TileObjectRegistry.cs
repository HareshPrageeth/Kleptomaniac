using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TileObjectRegistry", menuName = "Game/Tile Object Registry")]
public class TileObjectRegistry : ScriptableObject
{
    public TileObject[] objects;

    private Dictionary<TileBase, TileObject> lookup;

    private void OnEnable()
    {
        lookup = new Dictionary<TileBase, TileObject>();

        foreach (var obj in objects)
        {
            if (obj.mainTile != null)
            {
                lookup[obj.mainTile] = obj;
            }
        }
    }

    public TileObject GetObjectForTile(TileBase tile)
    {
        lookup.TryGetValue(tile, out TileObject obj);
        return obj;
    }
}
