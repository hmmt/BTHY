using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TraitIconScript : MonoBehaviour {

	
}

public class TraitIcon {
    private static TraitIcon _instance;
    public static TraitIcon instnace {
        get {
            if (_instance == null)
                _instance = new TraitIcon();
            return _instance;
        }
    }

    private Sprite[] iconSheet;
    private int std = 10000;

    private TraitIcon() {
        //iconSheet = Resources.LoadAll<Sprite>("Resource",
        iconSheet = ResourceCache.instance.GetMultipleSprite("UIResource/Icons/SVG-icons");
        
    }

    public Sprite GetSprite(int id) {
        int val = id % _instance.std;
        return _instance.iconSheet[val];
    }

    public Sprite GetSprite(TraitTypeInfo type) {
        if (type.id > int.MaxValue) {
            Debug.Log("int range overflow");
            return null;
        }
        int val = (int)type.id % _instance.std;
        if (val >= _instance.iconSheet.Length) return _instance.iconSheet[0];
        return _instance.iconSheet[val];
    }

    public Sprite[] GetSpriteByTrait(TraitTypeInfo type, out string[] list){
        List<Sprite> output = new List<Sprite>();
        List<string> outputText = new List<string>();

        if (type.hp > 0) {
            output.Add(iconSheet[0]);
            outputText.Add("HP+");
        }
        else if (type.hp < 0) {
            output.Add(iconSheet[1]);
            outputText.Add("HP-");
        }

        if (type.mental > 0) {
            output.Add(iconSheet[2]);
            outputText.Add("Mental+");
        }
        else if (type.mental < 0) {
            output.Add(iconSheet[3]);
            outputText.Add("Mental-");
        }

        if (type.moveSpeed > 0)
        {
            output.Add(iconSheet[4]);
            outputText.Add("Movement speed+");
        }
        else if (type.moveSpeed < 0)
        {
            output.Add(iconSheet[5]);
            outputText.Add("Movement speed-");
        }

        if (type.workSpeed > 0)
        {
            output.Add(iconSheet[6]);
            outputText.Add("Work speed+");
        }
        else if (type.workSpeed < 0)
        {
            output.Add(iconSheet[7]);
            outputText.Add("Work speed-");
        }

        if (output.Count <= 0) {
            output.Add(iconSheet[20]);
        }

        list = outputText.ToArray();
        return output.ToArray();
    }
}
