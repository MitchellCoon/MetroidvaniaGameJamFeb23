using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// DESCRIPTION
/// The MapGenerator script does the following:
/// - Iterates through all scenes that exist in Build Settings
/// - Tries to grab a MappableTileset component from each scene
/// - If present, gets tile data from MappableTileset
/// - Generates a separate sprite image for each scene
/// TODO:
/// - [ ] add map outline
/// - [ ] add different room sprites for hiding unexplored areas on map
/// - [ ] add different colors depending on biome / region
/// - [ ] show the minimap in-game
/// - [ ] add pan / zoom controls to minimap
/// - [ ] show the player's position on the minimap
/// - [ ] highlight the current room on the minimap
/// - [ ] show areas of interest on the minimap (only if in an already-explored region)
/// </summary>
public class MapGenerator : MonoBehaviour
{
#if UNITY_EDITOR

    Scene initialScene;

    void Start()
    {
        Debug.LogError("This script is not meant to be run in Play mode");
    }

    void Validate()
    {
        Assert.IsFalse(EditorSceneManager.GetActiveScene().buildIndex >= 0, "Map Generator Scene needs to be removed from Build Settings");
    }

    [ContextMenu("Generate Map")]
    void Generate()
    {
        initialScene = EditorSceneManager.GetActiveScene();
        StopAllCoroutines();
        Validate();
        Debug.ClearDeveloperConsole();
        Debug.Log("** RUNNING MAP GENERATION **");
        for (var i = 0; i < EditorSceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            Debug.Log($"Processing Scene \"{scene.name}\"...");
            ProcessScene(scene);
            EditorSceneManager.CloseScene(scene, true);
        }
        Debug.Log("✓ all done!");
    }

    void ProcessScene(Scene scene)
    {
        MappableTileset mappableTileset = FindObjectOfType<MappableTileset>();
        if (mappableTileset == null)
        {
            Debug.Log("- no mappableTileset found for ");
            return;
        }
        Debug.Log("- processing mappableTileset...");
        mappableTileset.PrepareMapGen();
        (TileBase[] tiles, int numTilesFound) = mappableTileset.GetTileData();
        Vector2Int size = mappableTileset.GetCellBasedSize();
        Vector2 center = mappableTileset.GetCenter();
        Debug.Log($"- {numTilesFound} tiles found");
        Debug.Log($"- center: {mappableTileset.GetCenter()}");
        Debug.Log($"- bounds: {mappableTileset.GetBounds()}");
        Debug.Log($"- cellbounds: {mappableTileset.GetCellBasedSize()}");
        Debug.Log($"- position: {mappableTileset.transform.position}");

        Texture2D texture = TilesToTexture2D(tiles, size.x, size.y);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, size.x, size.y), new Vector2(0.5f, 0.5f), 1f, 0, SpriteMeshType.Tight, Vector4.zero, false);

        // add a new sprite to the original scene
        GameObject generated = new GameObject($"MapPortion:{scene.name}");
        SpriteRenderer spriteRenderer = generated.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        generated.transform.position = center;
        generated.transform.localScale = new Vector3(2, 2, 1);
        EditorSceneManager.MoveGameObjectToScene(generated, initialScene);
    }

    Texture2D TilesToTexture2D(TileBase[] tiles, int width, int height)
    {
        Color[] colors = new Color[width * height];
        int i = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                i = x + y * width;
                if (tiles[i] == null)
                {
                    colors[i] = Color.red;
                }
                else
                {
                    colors[i] = Color.yellow;
                }
            }
        }
        return ColorToTexture2D(colors, width, height);
    }

    Texture2D ColorToTexture2D(Color[] colors, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        texture.SetPixels(colors);
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        return texture;
    }

#endif
}
