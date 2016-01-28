using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreaturePriority : MonoBehaviour {
    public Button Decrease;
    public Button Increase;
    public Text Count;
    private int cnt;
    private Sefira.CreaturePriority priority;
    private int max;

    public void Init(CreatureModel model) {
        Debug.Log(model);
        cnt = SefiraManager.instance.getSefira(model.sefiraNum).priority.GetPriority(model);
        max = SefiraManager.instance.getSefira(model.sefiraNum).priority.Max;
        ItemSetting();
    }

    public void ItemSetting() {
        Count.text = cnt + "";
        Decrease.gameObject.SetActive(true);
        Increase.gameObject.SetActive(true);
        if (cnt == 0) {
            Decrease.gameObject.SetActive(false);
        }
        if (cnt == max) {
            Increase.gameObject.SetActive(false);
        }
    }

    public void OnAdd() {
        if (cnt == max) return;
        cnt++;
        ItemSetting();
    }

    public void OnSub() {
        if (cnt == 0) {
            return;
        }
        cnt--;
        ItemSetting();
    }

    public int GetCnt() {
        return cnt;
    }
    
}
