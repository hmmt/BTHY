using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SefiaOpenedCheck : MonoBehaviour {
    public RectTransform[] sefira;
    private static SefiaOpenedCheck _instance;
    public static SefiaOpenedCheck instance {
        get { return _instance; }
    }

    void Awake() {
        _instance = this;
        
    }

    public void SetSprites() {
        sefira[0].GetComponent<Image>().sprite = SefiraManager.instance.getSefira(SefiraName.Malkut).sefiraSprite;
        sefira[1].GetComponent<Image>().sprite = SefiraManager.instance.getSefira(SefiraName.Netach).sefiraSprite;
        sefira[2].GetComponent<Image>().sprite = SefiraManager.instance.getSefira(SefiraName.Hod).sefiraSprite;
        sefira[3].GetComponent<Image>().sprite = SefiraManager.instance.getSefira(SefiraName.Yesod).sefiraSprite;

    
    }

    public static void SetSefira() {
        string[] list = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
        bool[] isopend = new bool[10];
        for (int i = 0; i < 10; i++)
        {
            isopend[i] = PlayerModel.instance.IsOpenedArea(list[i]);
            instance.sefira[i].gameObject.SetActive(isopend[i]);
        }
        instance.SetSprites();
        instance.gameObject.SetActive(false);
    }
}
