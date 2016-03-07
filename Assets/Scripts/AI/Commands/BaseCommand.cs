using UnityEngine;
using System.Collections;

public class BaseCommand  {
    public bool isFinished = false;

    public virtual void OnInit(MovableObjectModel model)
    {
    }

    public virtual void OnStart(MovableObjectModel model)
    {
    }

    public virtual void Execute(MovableObjectModel model)
    {
    }

    public virtual void OnStop(MovableObjectModel model)
    {
    }

    public virtual void OnDestroy(MovableObjectModel model)
    {
    }

    public void Finish()
    {
        isFinished = true;
    }
}
