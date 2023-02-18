using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

using DTDEV.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace MapGen
{

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
        const string IMPORTER_SETTINGS_PATH = "Assets/Settings/MapGenTextureImporter.preset";

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
                try
                {
                    Debug.Log($"Processing Scene \"{scene.name}\"...");
                    ProcessScene(scene);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
                finally
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
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
            Sprite backgroundSprite = SaveMapLayerImages(data.backgroundLayer);
            Sprite borderSprite = SaveMapLayerImages(data.borderLayer);
            Sprite tileSprite = SaveMapLayerImages(data.tileLayer);

            // TODO: find or create SO data asset file for room data

            // TODO: save data, referencing above textures
        }

        Sprite SaveMapLayerImages(MinimapRoomLayer layer)
        {
            return SaveImageAsset(layer.sprite.texture, layer.name);
        }

        Sprite SaveImageAsset(Texture2D texture, string name)
        {
            string relativePath = $"Assets/{IMAGE_PATH}{name}.png";
            string absolutePath = $"{Application.dataPath}/{IMAGE_PATH}{name}.png";
            string directryPath = $"{Application.dataPath}/{IMAGE_PATH}";
            byte[] byteArray = texture.EncodeToPNG();
            System.IO.Directory.CreateDirectory(directryPath);
            System.IO.File.WriteAllBytes(absolutePath, byteArray);

            // all this to just override default import settings...
            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(relativePath);
            importer.textureType = TextureImporterType.Sprite;
            TextureImporterSettings importerSettings = new TextureImporterSettings();
            importer.ReadTextureSettings(importerSettings);
            importerSettings.filterMode = FilterMode.Point;
            importerSettings.spritePixelsPerUnit = 1;
            importerSettings.spriteExtrude = 0;
            importerSettings.spriteGenerateFallbackPhysicsShape = false;
            importerSettings.spriteMeshType = SpriteMeshType.FullRect;
            importerSettings.spriteMode = (int)SpriteImportMode.Single;
            importer.SetTextureSettings(importerSettings);
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.maxTextureSize = 1024; // or whatever
            importer.alphaIsTransparency = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();

            Sprite newSprite = (Sprite)AssetDatabase.LoadAssetAtPath<Sprite>(relativePath);
            if (newSprite == null) Debug.LogError($"Unable to import sprite for img \"{relativePath}\"");
            return newSprite;
        }

        // this did not work for sprites, but I think it will work for ScriptableObjects
        void ApplySpriteDefaults(Sprite sprite)
        {
            if (sprite == null) return;

            var so = new SerializedObject(sprite);
            // you can Shift+Right Click on property names in the Inspector to see their paths (ONLY SOMETIMES)
            so.FindProperty("m_PixelsToUnits").floatValue = 1;
            so.ApplyModifiedProperties();
            so = new SerializedObject(sprite.texture);
            so.FindProperty("m_TextureSettings.m_FilterMode").intValue = 0;
            so.ApplyModifiedProperties();

            EditorUtility.SetDirty(sprite);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        void CreateDirIfNotExists(string dir)
        {
            System.IO.Directory.CreateDirectory($"{dir}");
        }

        // best way to use this is to search in the console for the property
        // sometimes copying the name from the Inspector does not work, maddeningly!
        void DebugSerializedObject(SerializedObject so)
        {
            SerializedProperty prop;
            prop = so.GetIterator();
            Debug.Log($"{prop.name} {prop.propertyPath} {prop.type}");
            int i = 0;
            while (prop.Next(true) && i < 1000)
            {
                Debug.Log($"{i} {prop.name} {prop.propertyPath} {prop.type}");
                i++;
            }
        }

#endif
    }
}

