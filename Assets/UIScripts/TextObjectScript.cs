using UnityEngine;
using System.Collections;

public class TextObjectScript : MonoBehaviour {
    private int index;

    public void SetIndex(int i){
        this.index = i;
    }

    public int GetIndex() {
        return this.index;
    }
}
