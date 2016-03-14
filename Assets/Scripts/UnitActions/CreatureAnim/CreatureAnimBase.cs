using UnityEngine;
using System.Collections;

// unused

public class CreatureAnimBase {

    protected CreatureUnit unit;

    public void SetCreatureUnit(CreatureUnit unit)
    {
        this.unit = unit;
    }

    public virtual void Init()
    {
    }
    public virtual void Update()
    {

    }

    public virtual void LateUpdate()
    {

    }

}
