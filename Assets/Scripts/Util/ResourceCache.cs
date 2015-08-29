using UnityEngine;
using System.Collections.Generic;

public class ResourceCache {

    private static ResourceCache _instance;

    public static ResourceCache instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ResourceCache();
                _instance.Init();
            }
            return _instance;
        }
    }

    private Dictionary<string, Texture2D> textureCache;


    public ResourceCache()
    {
    }

    private void Init()
    {
        textureCache = new Dictionary<string, Texture2D>();
    }

    public Texture2D GetTexture(string name)
    {
        Texture2D tex;
        if (textureCache.TryGetValue(name, out tex))
        {
            return tex;
        }

        return null;
    }

    public Sprite GetSprite(string name)
    {
        Sprite output = Resources.Load<Sprite>(name);
        if (output == null)
        {
            Debug.Log("ERROR : " + name);
        }
        return output;
    }
}
