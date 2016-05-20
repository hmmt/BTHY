using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MouseCursorType { 
    NORMAL,
    SCROLL,
    CLICK,
    LEFT,
    RIGHT,
    NONE
}

public class CursorManager : MonoBehaviour {
    private static CursorManager _instance = null;
    public static CursorManager instance {
        get { return _instance; }
    }

    public List<Texture2D> cursorSprite;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    public MouseCursorType currentType = MouseCursorType.NORMAL;

    public void Awake() {
        _instance = this;   
    }

    public void Start() {
        Cursor.SetCursor(cursorSprite[(int)currentType], hotSpot, cursorMode);
    }

    public void CursorSet(MouseCursorType type) {
        if (currentType == type) return;
        if ((int)type >= cursorSprite.Count) { 
            HideCursor();
            return;
        }

        Cursor.SetCursor(cursorSprite[(int)type], hotSpot, cursorMode);
        currentType = type;
    }

    public void HideCursor() {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
	
}
