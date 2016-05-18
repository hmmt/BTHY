using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextObjectScript : MonoBehaviour {
    private int _index = -1;
    public int index {
        get { return _index; }
        set { _index = value; }
    }

    public GameObject ImageObject;
    public GameObject TextObject;

    private Text textTarget;
    private Image imageTarget;

    public void Start() {
        imageTarget = ImageObject.GetComponent<Image>();
        textTarget = TextObject.GetComponent<Text>();
    }

    public Image GetImage() {
        return ImageObject.GetComponent<Image>();
    }

    public Text GetText() {
        return TextObject.GetComponent<Text>();
    }

    public void SetContent(string context) {
        textTarget.text = context;

        
    }
}
