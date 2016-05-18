using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SetAgentSefira : MonoBehaviour {
    private AgentModel model;

    public AgentModel Model {
        get { return model; }
        set { model = value; }
    }
    private static int _cnt;
    public int cnt;

    public bool slotOn;
    public UnityEngine.UI.Text agentLevel;
    public UnityEngine.UI.Text agentName;
    public UnityEngine.UI.Image agentBody;
    public UnityEngine.UI.Image agentFace;
    public UnityEngine.UI.Image agentHair;
    public UnityEngine.UI.Button cancelButton;
    public UnityEngine.UI.Image Bg;
    public UnityEngine.UI.Image Cancel;
    public Sprite headImg;
    public GameObject bodyObject;

    public void Start()
    {
        int index = this.transform.parent.GetSiblingIndex();
        _cnt = index;
        this.cnt = _cnt;
    }

    public void OnClick(BaseEventData eventData)
    {
        if (this.model == null) return;
        PointerEventData pointer = eventData as PointerEventData;
        if (pointer.button.Equals(PointerEventData.InputButton.Right))
        {
            Debug.Log("cancel " + _cnt);
            SefiraAgentSlot.instance.CancelSefiraAgent(model, _cnt);
        }
    }

    public void OnEnter() {
        Debug.Log(_cnt);
    }
    
}
