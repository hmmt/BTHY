using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CollectionPanelScript : MonoBehaviour {

    public CreatureModel model;
    public int index;
    public Image Portrait;
    public Text name;
    public Text grade;
    public Text observation;
    public Image Stress;
    public Image PanelImage;

    public void SetPanel(CreatureModel model) {
        this.model = model;
        //Debug.Log(model.metaInfo.name + "" + model.metaInfo.desc);
        Portrait.sprite = Resources.Load<Sprite>("Sprites/" + model.metaInfo.imgsrc);
        
        name.text = model.metaInfo.name;
        
		grade.text = model.metaInfo.attackType.ToString() + " / " + model.metaInfo.level;
        SetObservation(model);
        
    }

    public void SetObservation(CreatureModel model) {
        observation.text = (float)model.observeProgress / model.metaInfo.observeLevel * 100 + "%";
    }

    /*
    public void SetIndex() {
        CollectionListScript script = GameObject.FindWithTag("CollectionListPanel").GetComponent<CollectionListScript>();
        script.selected = index;
    }
     */

    public void OpenWindow() {
        CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(model.instanceId);
        unit.room.OnClick();
    }


}
