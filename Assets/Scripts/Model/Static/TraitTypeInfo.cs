using UnityEngine;
using System.Collections;

public class TraitTypeInfo {
    //<trait id="10001" name="열등감" hp="0" 
    //mental="-20" moveSpeed="0" workSpeed="0" />
    
    public long id;
    public string name;

    public int level;
    public int randomFlag;
    
    public int hp;
    public int mental;

    public float move;
    public float work;

    public float energy;
    public float attack;
    public float success;

    public string upwork;

    public string description;
    public string effect;
    public string condition;

    public int discType;

    public bool haveImg;
    public string imgsrc;
    public string imgPos;
    public Sprite image;
}
