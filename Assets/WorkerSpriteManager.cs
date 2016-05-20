using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorkerSpriteManager : MonoBehaviour {
    private static WorkerSpriteManager _instance = null;
    public static WorkerSpriteManager instance {
        get { return _instance; }
    }

    public List<Sprite> hairList;
    public List<Sprite> faceList;
    public List<Sprite> bothHair;
    public List<Sprite> femaleHair;
    public List<Sprite> maleHair;

    public void Awake() {
        _instance = this;
        LoadSprites();
    }

    public void LoadSprites() { 
        
        faceList = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/Agent/Face"));
        bothHair = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/Agent/Hair/Both"));
        femaleHair = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/Agent/Hair/Female"));
        maleHair = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/Agent/Hair/Male"));
        hairList = new List<Sprite>();
        foreach (Sprite s in bothHair)
        {
            hairList.Add(s);
        }
        foreach (Sprite s in femaleHair) {
            hairList.Add(s);
        }
        foreach (Sprite s in maleHair)
        {
            hairList.Add(s);
        }
    }

    public Sprite GetRandomHairSprite() {
        int randVal = UnityEngine.Random.Range(0, this.hairList.Count);
        return this.hairList[randVal];    
    }

    public Sprite GetRandomHairSprite(string gender) {
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

    public Sprite GetRandomFaceSprite() {
        int randVal = UnityEngine.Random.Range(0, this.faceList.Count);
        return this.faceList[randVal];
    }


}
