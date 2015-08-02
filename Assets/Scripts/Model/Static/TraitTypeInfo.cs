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

    public int moveSpeed;
    public int workSpeed;

    public string description;

    public float directWork;
    public float inDirectWork;
    public float blockWork;

    public int traitFlag;
}
