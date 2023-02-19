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
            obj.layer = Layer.Parse(MapGenerator.MAP_LAYER);
            SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = GetSprite();
            renderer.sortingOrder = sortingOrder;
            obj.transform.position = position;
            return obj;
        }

        public void Config(MapRoomLayer incoming)
        {
            if (!incoming.valid) return;
            this.valid = incoming.valid;
            this.name = incoming.name;
            this.type = incoming.type;
            this.position = incoming.position;
            this.sortingOrder = incoming.sortingOrder;
            this.sprite = incoming.sprite;
        }

        Sprite GetSprite()
        {
            if (!valid) return null;
            if (spriteOverride != null) return spriteOverride;
            return sprite;
        }

        string GetSpriteName()
        {
            return GetSprite() != null ? GetSprite().name : "";
        }

        public override string ToString()
        {
            if (!valid) return "Invalid MapRoomData";
            return $"{name} - {sortingOrder} - {position} - {GetSpriteName()}";
        }
    }
}
