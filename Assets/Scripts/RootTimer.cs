using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RootTimer : MonoBehaviour {

	public class Timer
	{
		public string name;
		public float elapsedTime;
		public float tick;

		public Timer(string name, float tick)
		{
			this.name = name;
			this.elapsedTime = 0;
			this.tick = tick;
		}
	}

	private float elapsedTime = 0;

	private List<Timer> timers = new List<Timer> ();

	public void AddTimer(string noticeName, float tick)
	{
		timers.Add (new Timer (noticeName, tick));
	}

	public void RemoveTimer(string noticeName)
	{
		foreach(Timer timer in timers)
		{
			if(timer.name == noticeName)
			{
				timers.Remove(timer);
				break;
			}
		}
	}

	void FixedUpdate()
	{
		float delta = Time.deltaTime;

		foreach(Timer timer in timers)
		{
			timer.elapsedTime += delta;
			if(timer.elapsedTime > timer.tick)
			{
				timer.elapsedTime -= timer.tick;
				Notice.instance.Send(timer.name);
			}
		}
	}


}
