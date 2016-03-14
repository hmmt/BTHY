using UnityEngine;
using System.Collections;


public class DoorObjectModel : ObjectModelBase
{
	private string id;

	private PassageObjectModel passage;
	public int hp;
	private bool closed;

	// TypeInfo로 변경필요
	public string type;
	public MapNode node;

	private DoorObjectModel connectedDoor = null;

	private float autoCloseCount;

	public DoorObjectModel(string id, string type, PassageObjectModel passage, MapNode node)
	{
		this.id = id;
		//openProgress = 0;
		this.type = type;
		this.passage = passage;
		this.node = node;
	}

	public string GetId()
	{
		return id;
	}

	public bool IsClosed()
	{
		return closed;
	}

	public void Connect(DoorObjectModel door)
	{
		connectedDoor = door;
		door.connectedDoor = this;
	}

	public void Open()
	{
		closed = false;
		node.closed = false;
		if(connectedDoor != null && connectedDoor.closed == true)
		{
			connectedDoor.Open ();
		}
		//Notice.instance.Send(NoticeName.OpenPassageDoor, passage, this);
	}

	public void Close()
	{
		autoCloseCount = 0;
		closed = true;
		node.closed = true;
		if(connectedDoor != null && connectedDoor.closed == false)
		{
			connectedDoor.Close ();
		}
		//Notice.instance.Send(NoticeName.ClosePassageDoor, passage, this);
	}

	public void OnObjectPassed()
	{
		autoCloseCount = 0;
		if(connectedDoor != null)
		{
			connectedDoor.autoCloseCount = 0;
		}
	}
	public void FixedUpdate()
	{
		if (!closed)
		{
			autoCloseCount += Time.deltaTime;
			if (autoCloseCount > 1)
			{
				Close();
			}
		}
	}
}