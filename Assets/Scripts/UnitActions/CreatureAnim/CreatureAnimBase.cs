using UnityEngine;
using System.Collections;

public class CreatureAnimBase {

    protected CreatureUnit unit;

    public void SetCreatureUnit(CreatureUnit unit)
    {
        this.unit = unit;
    }

    public virtual void Update()
    {

    }

}
