using UnityEngine;
using System.Collections.Generic;

public class ResourceCache {

    private ResourceCache _instance;

    public ResourceCache instance
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
}
