using UnityEngine;
using System.Collections;

public interface IObserver
{
	void OnNotice(string notice, params object[] param);
}