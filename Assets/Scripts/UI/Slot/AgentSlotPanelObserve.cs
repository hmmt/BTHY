using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AgentSlotPanelObserve : MonoBehaviour {
    public AgentModel target;
    public Image Face;
    public Image Hair;
    public Text name;
    public Text grade;
    public Text curretAction;
    public Text expectSuccess;
    public Image successImage;
    public Image BackGround;

    public Sprite notselected;
    public Sprite isSelected;

    private bool selected = false;
    private Color white, gray;

    void Awake() {
        white = Color.white;
        gray = Color.gray;
    }

    public void Init(AgentModel model) {
        this.target = model;
        this.BackGround.sprite = notselected;
        this.Face.sprite = target.tempFaceSprite;
        this.Hair.sprite = target.tempHairSprite;
        this.name.text = target.name;
        this.grade.text = AgentModel.GetLevelGradeText(target);
        this.curretAction.text = "";
        this.expectSuccess.text = target.successPercent.ToString();
        this.successImage.color = gray;
    }

    public void OnClick() {
        SelectObserveAgentWindow window = SelectObserveAgentWindow.currentWindow;
        if (this.selected)
        {
            if (window.RemoveAgentFromObserveList(this.target))
            {
                this.selected = false;
                //this.successImage.color = gray;
                this.BackGround.sprite = notselected;
            }
            else {
                Debug.Log("Error in " + this.gameObject.name + " from Remove Agent from List");
            }
        }
        else {
            if (window.AddAgentToObserveList(this.target))
            {
                this.selected = true;
                //this.successImage.color = white;
                this.BackGround.sprite = isSelected;
            }
            else {
                Debug.Log("Error in " + this.gameObject.name + " from Add Agent to List");
            }
        }
    }
}
