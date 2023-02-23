
using UnityEngine;

public static class Layer
{
    public static bool LayerMaskContainsLayer(int mask, int layer)
    {
        bool contains = ((mask & (1 << layer)) != 0);
        return contains;
    }

    // get the layer num from a layermask
    // see: https://forum.unity.com/threads/get-the-layernumber-from-a-layermask.114553/#post-3021162
    public static int Parse(int layerMask)
    {
        int result = layerMask > 0 ? 0 : 31;
        while (layerMask > 1)
        {
            layerMask = layerMask >> 1;
            result++;
        }
        return result;
    }

    public static int Parse(string layerName)
    {
        return LayerMask.NameToLayer(layerName);
    }
}
