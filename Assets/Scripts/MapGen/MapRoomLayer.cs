using UnityEngine;

namespace MapGen
{

    public enum MapLayerType
    {
        Tile,
        Background,
        Border
    }

    [System.Serializable]
    public struct MapRoomLayer
    {
        public bool valid;
        public string name;
        public MapLayerType type;
        public Vector2 position;
        public int sortingOrder;
        public Sprite sprite;
        public Sprite spriteOverride;

        public GameObject CreateGameObject()
        {
            if (!valid) return null;
            GameObject obj = new GameObject($"MapRoomLayer_{name}");
            SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = GetSprite();
            renderer.sortingOrder = sortingOrder;
            obj.transform.position = position;
            return obj;
        }

        Sprite GetSprite()
        {
            if (!valid) return null;
            if (spriteOverride != null) return spriteOverride;
            return sprite;
        }

        public override string ToString()
        {
            if (!valid) return "Invalid MinimapRoomData";
            return $"{name} - {sortingOrder} - {position}";
        }
    }
}
