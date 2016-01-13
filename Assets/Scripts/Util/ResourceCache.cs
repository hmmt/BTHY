using UnityEngine;
using System.Collections;
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
    private Dictionary<string, GameObject> prefabCache;
    private Dictionary<string, Sprite[]> multipleSpriteCache;

    private int loadingCount = 0;

    public bool isLoadingDone
    {
        get
        {
            return loadingCount <= 0;
        }
    }


    public ResourceCache()
    {
    }

    private void Init()
    {
        textureCache = new Dictionary<string, Texture2D>();
        spriteCache = new Dictionary<string, Sprite>();
        prefabCache = new Dictionary<string, GameObject>();
        multipleSpriteCache = new Dictionary<string, Sprite[]>();
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
        output = Resources.Load<Sprite>(name);
        if (output == null)
        {
            Debug.Log("LOAD FAIL ERROR : " + name);
            return null;
        }

        InsertSpriteCache(name, output);

        return output;
    }

    public Sprite[] GetMultipleSprite(string name) {
        Sprite[] output;
        if (multipleSpriteCache.TryGetValue(name, out output)) {
            return output;
        }
        output = Resources.LoadAll<Sprite>(name);
        if (output == null) {
            Debug.Log("LOAD FAIL ERROR : " + name);
            return null;
        }

        InsertSpriteCache(name, output[0]);

        return output;

    }

    /*
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
    */
    public void InsertSpriteCache(string name, Sprite sprite)
    {
        if(!spriteCache.ContainsKey(name))
            spriteCache.Add(name, sprite);
    }

    public void InsertSpriteCache(string name, Sprite[] sprite) {
        if (!multipleSpriteCache.ContainsKey(name))
            multipleSpriteCache.Add(name, sprite);
    }

    public IEnumerator LoadSprites(string[] spriteNameList, Callback finishCallback)
    {
        yield return new WaitForFixedUpdate();
        loadingCount++;
        foreach (string name in spriteNameList)
        {
            if (spriteCache.ContainsKey(name))
                continue;
            //Debug.Log("start >> " + name);
            ResourceRequest resourceRequest = Resources.LoadAsync(name);
            while (!resourceRequest.isDone)
            {
                yield return new WaitForFixedUpdate();
            }
            //Debug.Log("load >> " + name);
            Sprite sprite = resourceRequest.asset as Sprite;
            if(sprite != null)
                InsertSpriteCache(name, sprite);
            
            /*
            Debug.Log("load >> " + name);
            Sprite sprite = Resources.Load<Sprite>(name);
            if (sprite != null)
                InsertSpriteCache(name, sprite);
            yield return new WaitForFixedUpdate();
            */
        }
        loadingCount--;
        if (finishCallback != null)
        {
            finishCallback();
        }
    }

    public GameObject LoadPrefab(string name)
    {
        GameObject output;
        string path = "Prefabs/" + name;

        if (prefabCache.TryGetValue(name, out output))
        {
            return GameObject.Instantiate(output) as GameObject;
        }
        
        output = Resources.Load<GameObject>(path);

        prefabCache.Add(name, output);

        return GameObject.Instantiate(output) as GameObject;
    }
}
