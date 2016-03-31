using UnityEngine;
using System.Collections;

public class OverlayObject : MonoBehaviour, IOverlay
{
    public string text;
    public Vector3 pos;
    private bool _start = false;

    public void Overlay()
    {
        return;
        OverlayScript.instance.Overlay(this.gameObject, text);
    }


    public void Hide()
    {
        return;
        OverlayScript.instance.Hide();
    }
}