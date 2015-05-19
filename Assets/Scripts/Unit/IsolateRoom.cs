using UnityEngine;
using System.Collections;

public class IsolateRoom : MonoBehaviour, IObserver {

	public CreatureUnit _targetUnit;
	public TextMesh feelingText;
    public SpriteRenderer frameSpriteRenderer;
	public SpriteRenderer roomSpriteRenderer;
    public RectTransform touchButtonTransform;

    public GameObject workLog;
    public GameObject collection;

	public SpriteRenderer roomFogRenderer;

    public void Awake()
    {
        workLog = GameObject.FindGameObjectWithTag("AnimationController");
        //collection = GameObject.FindGameObjectWithTag("AnimCollectionController");
    }

    public void onClickWorkLog()
    {
        //workLog.GetComponent<Animator>().GetBool("isTrue") && 
        if (workLog.GetComponent<Animator>().GetBool("isTrue") && NarrationLoggerUI.instantNarrationLog.newInputCreature == _targetUnit)
        {
            workLog.GetComponent<Animator>().SetBool("isTrue", false);
            Debug.Log(workLog.GetComponent<Animator>().GetBool("isTrue"));
        }
        else if (workLog.GetComponent<Animator>().GetBool("isTrue") == false)
        {
            workLog.GetComponent<Animator>().SetBool("isTrue", true);
            Debug.Log("sibal");
        }

        NarrationLoggerUI.instantNarrationLog.targetCreature = _targetUnit;
        NarrationLoggerUI.instantNarrationLog.setLogList(_targetUnit);
    }
	
	public CreatureUnit targetUnit
	{   
		get{ return _targetUnit; }
		set
		{
			if(_targetUnit != null)
			{
				Notice.instance.Remove("UpdateCreatureState_"+_targetUnit.gameObject.GetInstanceID(), this);
				Notice.instance.Remove("ShowOutsideTypo_"+_targetUnit.gameObject.GetInstanceID(), this);
			}
			_targetUnit = value;
			if(_targetUnit != null)
			{
				Notice.instance.Observe("UpdateCreatureState_"+_targetUnit.gameObject.GetInstanceID(), this);
				Notice.instance.Observe("ShowOutsideTypo_"+_targetUnit.gameObject.GetInstanceID(), this);
			}
		}
	}

    public void Init()
    {
        float sizex = Mathf.Max(
            roomSpriteRenderer.sprite.bounds.size.x * roomSpriteRenderer.gameObject.transform.localScale.x,
            frameSpriteRenderer.sprite.bounds.size.x * frameSpriteRenderer.gameObject.transform.localScale.x
            );
        float sizey = Mathf.Max(
            roomSpriteRenderer.sprite.bounds.size.y * roomSpriteRenderer.gameObject.transform.localScale.y,
            frameSpriteRenderer.sprite.bounds.size.y * frameSpriteRenderer.gameObject.transform.localScale.y
            );

        touchButtonTransform.sizeDelta = new Vector2(sizex, sizey);
    }

	public void UpdateStatus()
	{
		if(targetUnit != null)
		{
            // 잠시 안 띄움
			feelingText.text = targetUnit.feeling.ToString ();
            //feelingText.text = "";
		}
	}

	void OnDisable()
	{
		if(_targetUnit != null)
		{
			Notice.instance.Remove("UpdateCreatureState_"+_targetUnit.gameObject.GetInstanceID(), this);
			Notice.instance.Remove("ShowOutsideTypo_"+_targetUnit.gameObject.GetInstanceID(), this);
		}
	}

    public void OnClick()
    {
        CreatureUnit oldCreature = (CollectionWindow.currentWindow != null )? CollectionWindow.currentWindow.GetCreature() : null;
        CollectionWindow.Create(_targetUnit);


        // TODO : 최적화 필요
        collection = GameObject.FindGameObjectWithTag("AnimCollectionController");

        
        if (collection.GetComponent<Animator>().GetBool("isTrue"))
        {
            Debug.Log(collection.GetComponent<Animator>().GetBool("isTrue"));
            collection.GetComponent<Animator>().SetBool("isTrue", false);
        }
        else if (oldCreature == _targetUnit)
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

        if (_targetUnit.state == CreatureState.WORKING)
        {
            color.a = 0f;
            roomFogRenderer.color = color;
        }
        else
        {
            color.a = (1 - 0.2f * _targetUnit.observeProgress)*0.75f;
            roomFogRenderer.color = color;
        }
	}
}