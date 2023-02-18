using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class MappableTileset : MonoBehaviour
{
    [SerializeField] bool isMapBorder;

    Tilemap tilemap;
    Bounds bounds;
    BoundsInt cellBounds;

    public bool IsMapBorder => isMapBorder;

    void Awake()
    {
        if (isMapBorder) gameObject.SetActive(false);
    }

    public void PrepareMapGen()
    {
        if (Application.isPlaying) throw new UnityException("This class is editor-only");
        tilemap = GetComponent<Tilemap>();
        tilemap.RefreshAllTiles();
        tilemap.CompressBounds();
        bounds = tilemap.localBounds;
        cellBounds = tilemap.cellBounds;
    }

    public (Vector2 min, Vector2 max) GetBounds()
    {
        if (Application.isPlaying) throw new UnityException("This class is editor-only");
        return (
            transform.position + bounds.center - bounds.extents,
            transform.position + bounds.center + bounds.extents
        );
    }

    public Vector2 GetSize()
    {
        if (Application.isPlaying) throw new UnityException("This class is editor-only");
        return bounds.size;
    }

    public Vector2Int GetCellBasedSize()
    {
        if (Application.isPlaying) throw new UnityException("This class is editor-only");
        return (Vector2Int)cellBounds.size;
    }

    public Vector2 GetCenter()
    {
        if (Application.isPlaying) throw new UnityException("This class is editor-only");
        float scale = Mathf.Max(transform.localScale.x, transform.localScale.y);
        return transform.position + bounds.center * scale;
    }

    public (TileBase[], int) GetTileData()
    {
        if (Application.isPlaying) throw new UnityException("This class is editor-only");



        int width = cellBounds.size.x;
        int height = cellBounds.size.y;
        TileBase[] tiles = new TileBase[width * height];
        int numTilesFound = 0;
        int i = 0;
        for (int y = cellBounds.yMin; y < cellBounds.yMax; y++)
        {
            for (int x = cellBounds.xMin; x < cellBounds.xMax; x++)
            {
                tiles[i] = tilemap.GetTile(new Vector3Int(x, y));
                if (tiles[i] != null) numTilesFound++;
                i++;
            }
        }
        return (tiles, numTilesFound);
    }

    // void OnDrawGizmosSelected()
    // {
    //     if (!Application.isPlaying) return;
    //     if (tilemap == null) return;

    //     Gizmos.color = Color.green;
    //     Vector2 offset = transform.position + bounds.center;
    //     Vector2 TL = offset + new Vector2(-bounds.extents.x, bounds.extents.y);
    //     Vector2 TR = offset + new Vector2(bounds.extents.x, bounds.extents.y);
    //     Vector2 BR = offset + new Vector2(bounds.extents.x, -bounds.extents.y);
    //     Vector2 BL = offset + new Vector2(-bounds.extents.x, -bounds.extents.y);
    //     Gizmos.DrawLine(TL, TR);
    //     Gizmos.DrawLine(TR, BR);
    //     Gizmos.DrawLine(BL, BR);
    //     Gizmos.DrawLine(TL, BL);
    // }
}
