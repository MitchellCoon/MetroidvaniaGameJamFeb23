using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

using DTDEV.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// DESCRIPTION
/// The MapGenerator script does the following:
/// - Iterates through all scenes that exist in Build Settings
/// - Tries to grab a MappableTileset component from each scene
/// - If present, gets tile data from MappableTileset
/// - Generates a separate sprite image for each scene
/// </summary>
public class MapGenerator : MonoBehaviour
{
    [SerializeField] Color colorTile = Color.yellow;
    [SerializeField] Color colorVoid = Color.red;
    [SerializeField] Color colorBorder = Color.white;

    // NOTE - full path will be /Assets/<DIR_NAME>
    const string IMAGE_PATH = "Generated/MapImages/";
    const string DATA_PATH = "Generated/MapData/";

#if UNITY_EDITOR

    Scene initialScene;

    const string GENERATED_SPRITE_TAG = "EditorOnly";

    public enum FillType
    {
        Tile,
        Background,
        Border
    }

    [System.Serializable]
    public struct MinimapRoomData
    {
        public string roomGuid;
        public MinimapRoomLayer tileLayer;
        public MinimapRoomLayer backgroundLayer;
        public MinimapRoomLayer borderLayer;

        public override string ToString()
        {
            return $"{roomGuid}\n{borderLayer}\n{tileLayer}\n{backgroundLayer}";
        }
    }

    [System.Serializable]
    public struct MinimapRoomLayer
    {
        public bool valid;
        public string name;
        public FillType fillType;
        public Vector2 position;
        public int sortingOrder;
        public Sprite sprite;

        public override string ToString()
        {
            if (!valid) return "Invalid MinimapRoomData";
            return $"{name} - {sortingOrder} - {position}";
        }
    }

    void Start()
    {
        Debug.LogError("This script is not meant to be run in Play mode");
    }

    void Validate()
    {
        Assert.IsFalse(EditorSceneManager.GetActiveScene().buildIndex >= 0, "Map Generator Scene needs to be removed from Build Settings");
    }

    public void DiscardTestSprites()
    {
        foreach (var item in GameObject.FindGameObjectsWithTag(GENERATED_SPRITE_TAG))
        {
            if (item.GetComponent<SpriteRenderer>() != null) DestroyImmediate(item);
        }
    }

    [ContextMenu("Generate Map")]
    public void Generate()
    {
        initialScene = EditorSceneManager.GetActiveScene();
        DiscardTestSprites();
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
        Room room = FindObjectOfType<Room>();
        if (room == null)
        {
            Debug.Log("- no room found");
            return;
        }
        MappableTileset[] mappableTilesets = FindObjectsOfType<MappableTileset>();
        if (mappableTilesets.Length == 0)
        {
            Debug.Log("- no mappableTilesets found");
            return;
        }

        room.Validate();
        MinimapRoomData roomData = new MinimapRoomData { roomGuid = room.guid };

        for (int i = 0; i < mappableTilesets.Length; i++)
        {
            if (mappableTilesets[i].IsMapBorder)
            {
                roomData.borderLayer = ProcessTiles(mappableTilesets[i], scene, FillType.Border);
            }
            else
            {
                roomData.backgroundLayer = ProcessTiles(mappableTilesets[i], scene, FillType.Background);
                roomData.tileLayer = ProcessTiles(mappableTilesets[i], scene, FillType.Tile);
            }
        }

        Debug.Log(roomData);

        SaveMapData(roomData);
    }

    MinimapRoomLayer ProcessTiles(MappableTileset mappableTileset, Scene scene, FillType fillType)
    {
        MinimapRoomLayer data = new MinimapRoomLayer { valid = false };
        if (mappableTileset == null) return data;
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

        Texture2D texture = TilesToTexture2D(tiles, size.x, size.y, fillType);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, size.x, size.y), new Vector2(0.5f, 0.5f), 1f, 0, SpriteMeshType.Tight, Vector4.zero, false);

        data.valid = true;
        data.name = $"{scene.name}_Layer{GetFillTypeName(fillType)}";
        data.fillType = fillType;
        data.position = center * 0.5f;
        data.sortingOrder = GetSortingOrderFromFillType(fillType);
        data.sprite = sprite;

        // add a new sprite to the original scene
        GameObject generated = new GameObject(data.name);
        SpriteRenderer spriteRenderer = generated.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = GetSortingOrderFromFillType(fillType);
        generated.transform.position = data.position;
        generated.tag = GENERATED_SPRITE_TAG;
        EditorSceneManager.MoveGameObjectToScene(generated, initialScene);
        return data;
    }

    Texture2D TilesToTexture2D(TileBase[] tiles, int width, int height, FillType fillType)
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
                    if (fillType == FillType.Background) colors[i] = colorVoid;
                }
                else
                {
                    if (fillType == FillType.Tile) colors[i] = colorTile;
                    if (fillType == FillType.Border) colors[i] = colorBorder;
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

    int GetSortingOrderFromFillType(FillType fillType)
    {
        if (fillType == FillType.Border) return 2;
        if (fillType == FillType.Tile) return 1;
        return 0;
    }

    string GetFillTypeName(FillType fillType)
    {
        return System.Enum.GetName(typeof(FillType), fillType);
    }

    void SaveMapData(MinimapRoomData data)
    {
        SaveMapLayerImages(data.backgroundLayer);
        SaveMapLayerImages(data.borderLayer);
        SaveMapLayerImages(data.tileLayer);
    }

    void SaveMapLayerImages(MinimapRoomLayer layer)
    {
        SaveImageAsset(layer.sprite.texture, layer.name);
    }

    void SaveImageAsset(Texture2D texture, string name)
    {
        CreateDirIfNotExists($"{Application.dataPath}/{IMAGE_PATH}");
        AssetDatabase.CreateAsset(texture, $"{Application.dataPath}/{IMAGE_PATH}{name}.png");

        // byte[] byteArray = texture.EncodeToPNG();
        // System.IO.Directory.CreateDirectory($"{Application.dataPath}/{IMAGE_PATH}");
        // System.IO.File.WriteAllBytes($"{Application.dataPath}/{IMAGE_PATH}{name}.png", byteArray);
    }

    void CreateDirIfNotExists(string dir)
    {
        System.IO.Directory.CreateDirectory($"{dir}");
    }

#endif
}
