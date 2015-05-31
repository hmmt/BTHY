using UnityEngine;
using System.Collections;

/*
 * 뷰어 역할이어야 한다. 
 * 
 * 
 * 
*/
public class IsolateRoom : MonoBehaviour, IObserver {

	public CreatureUnit _targetUnit;
	public TextMesh feelingText;
    public SpriteRenderer frameSpriteRenderer;
	public SpriteRenderer roomSpriteRenderer;
    public RectTransform touchButtonTransform;

    public GameObject workLog;
    public GameObject collection;

	public SpriteRenderer roomFogRenderer;

    public SpriteRenderer frameRedRenderer;
    public SpriteRenderer frameYellowRenderer;
    public SpriteRenderer frameGreenRenderer;

    public SpriteRenderer workingOffRenderer;
    public SpriteRenderer workingOnRenderer;

    public void Awake()
    {
        workLog = GameObject.FindGameObjectWithTag("AnimationController");
        //collection = GameObject.FindGameObjectWithTag("AnimCollectionController");
    }

    public void onClickWorkLog()
    {
        //workLog.GetComponent<Animator>().GetBool("isTrue") && 
        if (workLog.GetComponent<Animator>().GetBool("isTrue") && NarrationLoggerUI.instantNarrationLog.newInputCreature == _targetUnit.model)
        {
            workLog.GetComponent<Animator>().SetBool("isTrue", false);
        }
        else if (workLog.GetComponent<Animator>().GetBool("isTrue") == false)
        {
            workLog.GetComponent<Animator>().SetBool("isTrue", true);
        }

        NarrationLoggerUI.instantNarrationLog.targetCreature = _targetUnit.model;
        NarrationLoggerUI.instantNarrationLog.setLogList(_targetUnit.model);
    }
	
	public CreatureUnit targetUnit
	{   
		get{ return _targetUnit; }
		set
		{
			if(_targetUnit != null)
			{
				Notice.instance.Remove("UpdateCreatureState_"+_targetUnit.model.instanceId, this);
				Notice.instance.Remove("ShowOutsideTypo_"+_targetUnit.model.instanceId, this);
			}
			_targetUnit = value;
			if(_targetUnit != null)
			{
                Notice.instance.Observe("UpdateCreatureState_" + _targetUnit.model.instanceId, this);
                Notice.instance.Observe("ShowOutsideTypo_" + _targetUnit.model.instanceId, this);
			}
		}
	}

    public void Init()
    {
        float sizex = Mathf.Max(
            roomSpriteRenderer.sprite.bounds.size.x * roomSpriteRenderer.gameObject.transform.localScale.x,
            roomSpriteRenderer.sprite.bounds.size.x * roomSpriteRenderer.gameObject.transform.localScale.x
            );
        float sizey = Mathf.Max(
            roomSpriteRenderer.sprite.bounds.size.y * roomSpriteRenderer.gameObject.transform.localScale.y,
            roomSpriteRenderer.sprite.bounds.size.y * roomSpriteRenderer.gameObject.transform.localScale.y
            );

        touchButtonTransform.sizeDelta = new Vector2(sizex, sizey);
    }

	public void UpdateStatus()
	{
		if(targetUnit != null)
		{
            // 잠시 안 띄움
			//feelingText.text = targetUnit.model.feeling.ToString ();
            feelingText.text = "";

            int feeling = targetUnit.model.feeling;

            int length = targetUnit.model.metaInfo.genEnergy.Length;

            if (length >= 3)
            {
                if (feeling > targetUnit.model.metaInfo.feelingMax * 2 / 3)
                {
                    frameRedRenderer.gameObject.SetActive(false);
                    frameYellowRenderer.gameObject.SetActive(false);
                    frameGreenRenderer.gameObject.SetActive(true);
                }
                else if (feeling > targetUnit.model.metaInfo.feelingMax / 3)
                {
                    frameRedRenderer.gameObject.SetActive(false);
                    frameYellowRenderer.gameObject.SetActive(true);
                    frameGreenRenderer.gameObject.SetActive(false);
                }
                else
                {
                    frameRedRenderer.gameObject.SetActive(true);
                    frameYellowRenderer.gameObject.SetActive(false);
                    frameGreenRenderer.gameObject.SetActive(false);
                }
            }
            else if (length <= 2)
            {
                if (feeling > targetUnit.model.metaInfo.feelingMax / 2)
                {
                    frameRedRenderer.gameObject.SetActive(false);
                    frameYellowRenderer.gameObject.SetActive(false);
                    frameGreenRenderer.gameObject.SetActive(true);
                }
                else
                {
                    frameRedRenderer.gameObject.SetActive(true);
                    frameYellowRenderer.gameObject.SetActive(false);
                    frameGreenRenderer.gameObject.SetActive(false);
                }
            }

            if (targetUnit.model.state == CreatureState.WORKING)
            {
                workingOnRenderer.gameObject.SetActive(true);
                workingOffRenderer.gameObject.SetActive(false);
            }
            else
            {
                workingOnRenderer.gameObject.SetActive(false);
                workingOffRenderer.gameObject.SetActive(true);
            }
		}
	}

	void OnDisable()
	{
		if(_targetUnit != null)
		{
			Notice.instance.Remove("UpdateCreatureState_"+_targetUnit.model.instanceId, this);
			Notice.instance.Remove("ShowOutsideTypo_"+_targetUnit.model.instanceId, this);
		}
	}

    public void OnClick()
    {
        CreatureModel oldCreature = (CollectionWindow.currentWindow != null )? CollectionWindow.currentWindow.GetCreature() : null;
        if (SelectWorkAgentWindow.currentWindow != null)
        SelectWorkAgentWindow.currentWindow.CloseWindow();
        CollectionWindow.Create(_targetUnit.model);


        // TODO : 최적화 필요
        collection = GameObject.FindGameObjectWithTag("AnimCollectionController");

        
        if (collection.GetComponent<Animator>().GetBool("isTrue"))
        {
            Debug.Log(collection.GetComponent<Animator>().GetBool("isTrue"));
            collection.GetComponent<Animator>().SetBool("isTrue", false);
        }
        else if (oldCreature == _targetUnit.model)
        {
            Debug.Log(collection.GetComponent<Animator>().GetBool("isTrue"));
            collection.GetComponent<Animator>().SetBool("isTrue", true);
        }
    }

	public void OnNotice(string notice, params object[] param)
	{
		if (notice.Split ('_') [0] == "UpdateCreatureState")
			UpdateStatus ();
		else
		{
		}
	}

	public void OpenStatusWindow()
	{

		//IsolateRoomStatus.CreateWindow (targetUnit);
	}

	public void Update()
	{
        Color color = roomFogRenderer.color;

        if (_targetUnit.model.state == CreatureState.WORKING)
        {
            color.a = 0f;
            roomFogRenderer.color = color;
        }
        else
        {
            color.a = (1 - 0.2f * _targetUnit.model.observeProgress)*0.75f;
            roomFogRenderer.color = color;
        }
	}
}