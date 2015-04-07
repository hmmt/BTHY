using UnityEngine;
using System.Collections.Generic;

public class Notice {

	private static Notice _instance;

	public static Notice instance
	{
		get
		{
			if(_instance == null)
				_instance = new Notice();
			return _instance;
		}
	}

	private Dictionary<string, List<IObserver>> noticeList;

	private Notice()
	{
		noticeList = new Dictionary<string, List<IObserver>> ();
	}

	public void Observe(string notice, IObserver observer)
	{
		List<IObserver> obList;

		if(noticeList.TryGetValue(notice, out obList))
		{
			obList.Add(observer);
		}
		else
		{
			obList = new List<IObserver> ();
			obList.Add(observer);
			noticeList.Add(notice, obList);
		}
	}

	public void Remove(string notice, IObserver observer)
	{
		List<IObserver> obList;
		
		if(noticeList.TryGetValue(notice, out obList))
		{
			obList.Remove(observer);
		}
	}

	public void Send(string notice, params object[] param)
	{
		List<IObserver> obList;
		
		if(noticeList.TryGetValue(notice, out obList))
		{
			foreach(IObserver ob in obList)
			{
				ob.OnNotice(notice, param);
			}
		}
	}
}
