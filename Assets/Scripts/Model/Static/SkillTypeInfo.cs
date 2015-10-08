using UnityEngine;
using System.Collections;

public class SkillTypeInfo {

	public long id;
	public string name;
	public string type;

	public int amount;

    public string imgsrc;

    public string bonusType;

    public float amountBonusD;
    public float feelingBonusD;
    public int mentalReduceD;
    public int mentalTickD;

    public float amountBonusI;
    public float feelingBonusI;
    public int mentalReduceI;
    public int mentalTickI;

    public float amountBonusS;
    public float feelingBonusS;
    public int mentalReduceS;
    public int mentalTickS;

    public float amountBonusC;
    public float feelingBonusC;
    public int mentalReduceC;
    public int mentalTickC;

    public long[] nextSkillIdList;
}
