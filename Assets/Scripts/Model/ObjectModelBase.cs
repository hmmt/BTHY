using UnityEngine;
using System.Collections.Generic;

public class ObjectModelBase
{
    public Vector3 position;

    public virtual bool CanOpenDoor()
    {
        return true;
    }

    public virtual void InteractWithDoor(DoorObjectModel door)
    {
    }
}

public interface MovableObjectModel
{
    MovableObjectNode GetMovableObjectNode();
    void InteractWithDoor(DoorObjectModel door);
}