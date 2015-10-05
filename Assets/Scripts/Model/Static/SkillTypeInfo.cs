using UnityEngine;
using System.Collections;

public class SkillTypeInfo {

	public long id;
	public string name;
	public string type;

	public int amount;

    public string imgsrc;

    public string bonusType;

    public int amountBonusD;
    public int feelingBonusD;
    public int mentalReduceD;
    public int mentalTickD;

    public int amountBonusI;
    public int feelingBonusI;
    public int mentalReduceI;
    public int mentalTickI;

    public int amountBonusS;
    public int feelingBonusS;
    public int mentalReduceS;
    public int mentalTickS;

    public int amountBonusC;
    public int feelingBonusC;
    public int mentalReduceC;
    public int mentalTickC;

    public long[] nextSkillIdList;
}
