using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
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
    /// - Generates a MapRoomData ScriptableObject for each scene
    /// - Saves appropriate assets and links them up
    /// - Creates a final WorldMap component to be used within scenes
    /// </summary>
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] Color colorTile = Color.yellow;
        [SerializeField] Color colorVoid = Color.red;
        [SerializeField] Color colorBorder = Color.white;

        // NOTE - full path will be /Assets/<DIR_NAME>
        const string PREFAB_PATH = "Generated/MapPrefabs/";
        const string IMAGE_PATH = "Generated/MapImages/";
        const string DATA_PATH = "Generated/MapData/";
        const string IMPORTER_SETTINGS_PATH = "Assets/Settings/MapGenTextureImporter.preset";

        const string PREFAB_NAME_WORLD_MAP = "GeneratedWorldMap";
        const string GENERATED_SPRITE_TAG = "Map";

        const int MAP_SORTING_ORDER = 10;
        public const string MAP_LAYER = "Map";


        List<MapRoomData> mapRoomDataList = new List<MapRoomData>();

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

        public void DiscardTestSprites()
        {
            foreach (var item in GameObject.FindGameObjectsWithTag(GENERATED_SPRITE_TAG))
            {
                if (item.GetComponent<WorldMap>() != null) DestroyImmediate(item);
            }
        }

        [ContextMenu("Generate Map")]
        public void Generate()
        {
            mapRoomDataList.Clear();
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
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                EditorSceneManager.CloseScene(scene, true);
            }
            Debug.Log("Generating Map Prefab...");
            GenerateMapPrefab();
            Debug.Log("✓ all done!");
        }

        void GenerateMapPrefab()
        {
            GameObject worldMapObj = new GameObject(PREFAB_NAME_WORLD_MAP);
            worldMapObj.layer = Layer.Parse(MAP_LAYER);
            worldMapObj.transform.position = Vector2.zero;
            WorldMap worldMap = worldMapObj.AddComponent<WorldMap>();
            SortingGroup sortingGroup = worldMapObj.AddComponent<SortingGroup>();
            worldMap.SetMapRoomDataList(mapRoomDataList);
            sortingGroup.sortingOrder = MAP_SORTING_ORDER;
            foreach (var mapRoomData in mapRoomDataList)
            {
                GameObject mapRoomObj = mapRoomData.CreateGameObject();
                mapRoomObj.layer = Layer.Parse(MAP_LAYER);
                mapRoomObj.transform.SetParent(worldMapObj.transform);
            }
            worldMapObj.tag = GENERATED_SPRITE_TAG;
            SavePrefab(worldMapObj);
        }

        void SavePrefab(GameObject worldMapObj)
        {
            string relativePath = $"Assets/{PREFAB_PATH}{PREFAB_NAME_WORLD_MAP}.prefab";
            CreateDirIfNotExists($"Assets/{PREFAB_PATH}");
            bool prefabSuccess;
            PrefabUtility.SaveAsPrefabAssetAndConnect(worldMapObj, relativePath, InteractionMode.UserAction, out prefabSuccess);
            if (!prefabSuccess) Debug.LogError("Unable to save WorldMap prefab");
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
            MapRoomData roomData = LoadOrCreateMapRoomData(room.guid);

            for (int i = 0; i < mappableTilesets.Length; i++)
            {
                if (mappableTilesets[i].IsMapBorder)
                {
                    roomData.SetBorderLayer(ProcessTiles(mappableTilesets[i], scene, MapLayerType.Border));
                }
                else
                {
                    roomData.SetBackgroundLayer(ProcessTiles(mappableTilesets[i], scene, MapLayerType.Background));
                    roomData.SetTileLayer(ProcessTiles(mappableTilesets[i], scene, MapLayerType.Tile));
                }
            }

            SaveMapRoomData(roomData, room.guid, scene);
            // We need to save the map images as a separate step, because
            // calling AssetDatabase.ImportAsset() clobbers changes made
            // to the ScriptableObject. In other words, ImportAsset() sets
            // off a chain reaction that forcibly refreshes all assets.
            // Per docs:
            //   This imports an Asset at the specified path, and triggers a number
            //   of callbacks including AssetModificationProcessor.OnWillSaveAssets and
            //   AssetPostProcessor.OnPostProcessAllAssets
            //   see: https://docs.unity3d.com/ScriptReference/AssetDatabase.ImportAsset.html
            SaveMapRoomImages(roomData);
            room.SetMapRoomData(roomData);
            mapRoomDataList.Add(roomData);
        }

        MapRoomLayer ProcessTiles(MappableTileset mappableTileset, Scene scene, MapLayerType layerType)
        {
            MapRoomLayer layer = new MapRoomLayer { valid = false };
            if (mappableTileset == null) return layer;
            Debug.Log("- processing mappableTileset...");
            mappableTileset.PrepareMapGen();
            (TileBase[] tiles, int numTilesFound) = mappableTileset.GetTileData();
            Vector2Int size = mappableTileset.GetCellBasedSize();
            Vector2 center = mappableTileset.GetCenter();

            Texture2D texture = TilesToTexture2D(tiles, size.x, size.y, layerType);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, size.x, size.y), new Vector2(0.5f, 0.5f), 1f, 0, SpriteMeshType.Tight, Vector4.zero, false);

            layer.valid = true;
            layer.name = $"{scene.name}_Layer{GetMapLayerTypeName(layerType)}";
            layer.type = layerType;
            layer.position = center * 0.5f;
            layer.scale = mappableTileset.GetUnscaleValue();
            layer.sortingOrder = GetSortingOrderFromMapLayerType(layerType);
            layer.sprite = sprite;
            return layer;
        }

        Texture2D TilesToTexture2D(TileBase[] tiles, int width, int height, MapLayerType layerType)
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
                        if (layerType == MapLayerType.Background) colors[i] = colorVoid;
                    }
                    else
                    {
                        if (layerType == MapLayerType.Tile) colors[i] = colorTile;
                        if (layerType == MapLayerType.Border) colors[i] = colorBorder;
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

        int GetSortingOrderFromMapLayerType(MapLayerType layerType)
        {
            if (layerType == MapLayerType.Border) return 2;
            if (layerType == MapLayerType.Tile) return 1;
            return 0;
        }

        string GetMapLayerTypeName(MapLayerType layerType)
        {
            return System.Enum.GetName(typeof(MapLayerType), layerType);
        }

        MapRoomData LoadOrCreateMapRoomData(string roomGuid)
        {
            string assetName = $"map-data-{roomGuid}";
            string relativePath = $"Assets/{DATA_PATH}{assetName}.asset";
            CreateDirIfNotExists($"Assets/{DATA_PATH}");
            MapRoomData mapRoomData = AssetDatabase.LoadAssetAtPath<MapRoomData>(relativePath);

            if (mapRoomData == null)
            {
                Debug.Log("- unable to find MapRoomData... creating new one");
                mapRoomData = ScriptableObject.CreateInstance<MapRoomData>();
                AssetDatabase.CreateAsset(mapRoomData, relativePath);
            }
            else
            {
                Debug.Log("- found existing MapRoomData");
            }

            return mapRoomData;
        }

        void SaveMapRoomData(MapRoomData data, string roomGuid, Scene scene)
        {
            data.roomGuid = roomGuid;
            data.roomName = scene.name;
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        void SaveMapRoomImages(MapRoomData data)
        {
            Sprite backgroundSprite = SaveLayerImage(data.backgroundLayer);
            Sprite borderSprite = SaveLayerImage(data.borderLayer);
            Sprite tileSprite = SaveLayerImage(data.tileLayer);
            data.SetBackgroundSprite(backgroundSprite);
            data.SetBorderSprite(borderSprite);
            data.SetTileSprite(tileSprite);
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        Sprite SaveLayerImage(MapRoomLayer layer)
        {
            if (layer.sprite == null) return null;
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
            ApplySpriteDefaults(relativePath);
            AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.DontDownloadFromCacheServer);
            // ResetSpriteImporter(relativePath);
            Sprite newSprite = (Sprite)AssetDatabase.LoadAssetAtPath<Sprite>(relativePath);
            if (newSprite == null) Debug.LogError($"Unable to import sprite for img \"{relativePath}\"");
            return newSprite;
        }

        void ApplySpriteDefaults(string relativePath)
        {
            // all this to just override default import settings...
            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(relativePath);
            if (importer == null)
            {
                Debug.LogError($"Could not get TextureImporter for {relativePath}");
                return;
            }
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
            // importer.SaveAndReimport();
        }

        void ResetSpriteImporter(string relativePath)
        {
            // all this to just override default import settings...
            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(relativePath);
            importer.textureType = TextureImporterType.Sprite;
            TextureImporterSettings importerSettings = new TextureImporterSettings();
            importer.ReadTextureSettings(importerSettings);
            importerSettings.spritePixelsPerUnit = 100;
            importer.SetTextureSettings(importerSettings);
            EditorUtility.SetDirty(importer);
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


// // HANDY CODE SNIPPETS
//
// // Here is an example of using SerializedObject to update a room.
// // It turns out that simply setting room properties, marking the scene as dirty,
// // and saving the scene was enough to save changes to a room. But this approach
// // could be used for dynamically updating other object references.
// void UpdateRoom(Room room, MapRoomData roomData)
// {
//     room.SetMapRoomData(roomData);
//     // SerializedObject so = new SerializedObject(room);
//     // so.FindProperty("mapRoomData").objectReferenceValue = roomData;
//     // so.ApplyModifiedProperties();
//     // EditorUtility.SetDirty(room);
//     // AssetDatabase.SaveAssets();
//     // AssetDatabase.Refresh();
// }

// // this did not work for sprites, but I think it will work for ScriptableObjects
// void ApplySpriteDefaults(Sprite sprite)
// {
//     if (sprite == null) return;

//     var so = new SerializedObject(sprite);
//     // you can Shift+Right Click on property names in the Inspector to see their paths (ONLY SOMETIMES)
//     so.FindProperty("m_PixelsToUnits").floatValue = 1;
//     so.ApplyModifiedProperties();
//     so = new SerializedObject(sprite.texture);
//     so.FindProperty("m_TextureSettings.m_FilterMode").intValue = 0;
//     so.ApplyModifiedProperties();

//     EditorUtility.SetDirty(sprite);
//     AssetDatabase.SaveAssets();
//     AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
// }

// // Originally placed in ProcessTiles()
// // add a new sprite to the original scene
// GameObject generated = new GameObject(data.name);
// SpriteRenderer spriteRenderer = generated.AddComponent<SpriteRenderer>();
// spriteRenderer.sprite = sprite;
// spriteRenderer.sortingOrder = GetSortingOrderFromMapLayerType(layerType);
// generated.transform.position = data.position;
// generated.tag = GENERATED_SPRITE_TAG;
// EditorSceneManager.MoveGameObjectToScene(generated, initialScene);
