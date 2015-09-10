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
    private Dictionary<string, Sprite> spriteCache;


    public ResourceCache()
    {
    }

    private void Init()
    {
        textureCache = new Dictionary<string, Texture2D>();
        spriteCache = new Dictionary<string, Sprite>();
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

        Sprite output;

        if (spriteCache.TryGetValue(name, out output))
        {
            return output;
        }
        if (output == null)
        {
            Debug.Log("ERROR : " + name);
        }
        return output;
    }

    public void PreLoadSprite(string name)
    {
        Sprite loaded = Resources.Load<Sprite>(name);
        if (loaded == null)
        {
            Debug.Log("LOAD FAIL ERROR : " + name);
            return;
        }

        spriteCache.Add(name, loaded);
    }
}
