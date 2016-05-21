using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteInfo
{
	public string path;
	public Sprite sprite;

	public SpriteInfo(string path, Sprite sprite)
	{
		this.path = path;
		this.sprite = sprite;
	}
}

public class WorkerSpriteManager : MonoBehaviour {
    private static WorkerSpriteManager _instance = null;
    public static WorkerSpriteManager instance {
        get { return _instance; }
    }

	public List<SpriteInfo> hairList;
	public List<SpriteInfo> faceList;
	public List<SpriteInfo> bothHair;
	public List<SpriteInfo> femaleHair;
	public List<SpriteInfo> maleHair;

    public void Awake() {
		/*
		// DontDestroyOnLoad() is called in GameScreen.cs
		*/
		if (_instance != null) {
			Destroy (gameObject);
			return;
		}
        _instance = this;
        LoadSprites();
		DontDestroyOnLoad (gameObject);
    }

    public void LoadSprites() { 
        
		string facePath = "Sprites/Agent/Face";
		string bothHairPath = "Sprites/Agent/Hair/Both";
		string femaleHairPath = "Sprites/Agent/Hair/Female";
		string maleHairPath = "Sprites/Agent/Hair/Male";

		Sprite[] _faceList = Resources.LoadAll<Sprite>(facePath);
		Sprite[] _bothHair = Resources.LoadAll<Sprite>(bothHairPath);
		Sprite[] _femaleHair = Resources.LoadAll<Sprite>(femaleHairPath);
		Sprite[] _maleHair = Resources.LoadAll<Sprite>(maleHairPath);

		hairList = new List<SpriteInfo> ();
		faceList = new List<SpriteInfo> ();
		bothHair = new List<SpriteInfo> ();
		femaleHair = new List<SpriteInfo> ();
		maleHair = new List<SpriteInfo> ();

		foreach (Sprite s in _faceList)
		{
			SpriteInfo info = new SpriteInfo (facePath + "/" + s.name, s);
			faceList.Add (info);
		}
		foreach (Sprite s in _bothHair)
        {
			SpriteInfo info = new SpriteInfo (bothHairPath + "/" + s.name, s);
			bothHair.Add (info);
			hairList.Add (info);
        }
		foreach (Sprite s in _femaleHair)
		{
			SpriteInfo info = new SpriteInfo (femaleHairPath + "/" + s.name, s);
			femaleHair.Add (info);
			hairList.Add (info);
        }
		foreach (Sprite s in _maleHair)
        {
			SpriteInfo info = new SpriteInfo (maleHairPath + "/" + s.name, s);
			maleHair.Add (info);
			hairList.Add (info);
        }
    }

	public SpriteInfo GetRandomHairSprite() {
        int randVal = UnityEngine.Random.Range(0, this.hairList.Count);
        return this.hairList[randVal];    
    }

	public SpriteInfo GetRandomHairSprite(string gender) {
        if (gender == "female" || gender == "Female") {
            int maxRange = this.bothHair.Count + this.femaleHair.Count;
            int randResult = Random.Range(0, maxRange);
            if (randResult >= this.bothHair.Count)
            {
                return this.femaleHair[randResult - this.bothHair.Count];
            }
            else {
                return this.bothHair[randResult];
            }
        }
        else if (gender == "male" || gender == "Male")
        {
            int maxRange = this.bothHair.Count + this.maleHair.Count;
            int randResult = Random.Range(0, maxRange);
            if (randResult >= this.bothHair.Count)
            {
                return this.maleHair[randResult - this.bothHair.Count];
            }
            else
            {
                return this.bothHair[randResult];
            }
        }
        else {
            return this.bothHair[Random.Range(0, this.bothHair.Count)];
        }
    }

	public SpriteInfo GetRandomFaceSprite() {
        int randVal = UnityEngine.Random.Range(0, this.faceList.Count);
        return this.faceList[randVal];
    }


}
