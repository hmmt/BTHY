using UnityEngine;
using System.Collections.Generic;

public class WindowQueue : MonoBehaviour {

	private WindowQueue _instance;

	public WindowQueue instance
	{
		get
		{
			if(_instance == null)
				_instance = new WindowQueue();
			return _instance;
		}
	}

	public interface IWindow
	{
		void OnOpen();
		void OnClose();
	}

	private Queue<IWindow> queue;
	private Dictionary<System.Type, CurrentUIState> UIStateTable;

	public WindowQueue()
	{
		queue = new Queue<IWindow> ();
		UIStateTable = new Dictionary<System.Type, CurrentUIState> ();
		UIStateTable.Add (typeof(IsolateRoomStatus), CurrentUIState.UNIT_WINDOW);
		UIStateTable.Add (typeof(SelectActionWindow), CurrentUIState.UNIT_WINDOW);
	}

	public CurrentUIState GetCurrentUIState()
	{
		if (queue.Count == 0)
			return CurrentUIState.DEFAULT;
		CurrentUIState state = CurrentUIState.DEFAULT;
		UIStateTable.TryGetValue(queue.Peek().GetType(), out state);
		return CurrentUIState.DEFAULT;
	}

	public void OpenWindow(IWindow wnd)
	{
		queue.Enqueue (wnd);

		if(queue.Count > 1)
		{
			wnd.OnOpen();
		}
	}

	public void NextWindow()
	{
		queue.Peek ().OnClose ();
		queue.Dequeue();

		if(queue.Count > 0)
		{
			queue.Peek().OnOpen();
		}
	}
}
