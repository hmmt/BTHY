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

    public Light2D Warning1;
    public Light2D Warning2;
    public Light2D Warning3;
    public Light2D Warning4;

    public Light2D Warning_Yellow1;
    public Light2D Warning_Yellow2;
    public Light2D Warning_Yellow3;
    public Light2D Warning_Yellow4;


    public Light2D Warning_Green1;
    public Light2D Warning_Green2;
    public Light2D Warning_Green3;
    public Light2D Warning_Green4;

    public SpriteRenderer observeCatuionSprite;
    public Animator observeRoom;

    public TextMesh creatureLevel;
    public TextMesh creatureName;

    public UnityEngine.UI.Text FeelingTextForDebug;

    public void Awake()
    {
        workLog = GameObject.FindGameObjectWithTag("AnimationController");
        //collection = GameObject.FindGameObjectWithTag("AnimCollectionController");
        observeRoom.SetBool("ObserveStart", false);
        //Debug.Log("격리실 : " + observeRoom.GetBool("ObserveStart"));
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

        roomSpriteRenderer.transform.localScale = new Vector3(0.16f, 0.16f, 1);
        float sizex = Mathf.Max(
            roomSpriteRenderer.sprite.bounds.size.x * roomSpriteRenderer.gameObject.transform.localScale.x,
            roomSpriteRenderer.sprite.bounds.size.x * roomSpriteRenderer.gameObject.transform.localScale.x
            );
        float sizey = Mathf.Max(
            roomSpriteRenderer.sprite.bounds.size.y * roomSpriteRenderer.gameObject.transform.localScale.y,
            roomSpriteRenderer.sprite.bounds.size.y * roomSpriteRenderer.gameObject.transform.localScale.y
            );


        touchButtonTransform.sizeDelta = new Vector2(sizex, sizey);


        if (targetUnit.model.sefiraNum == "1")
        {
            frameRedRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/IsolateRoom/isolateRoom_Red");
            frameYellowRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/IsolateRoom/isolateRoom_Yellow");
            frameGreenRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/IsolateRoom/isolateRoom_Green");
            workingOffRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/IsolateRoom/isolateRoom_Frame");
            workingOnRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/IsolateRoom/isolateRoom_Frame");

        }

        else if (targetUnit.model.sefiraNum == "2")
        {
            frameRedRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/IsolateRoom/Nezzach_Feel_Red");
            frameYellowRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/IsolateRoom/Nezzach_Feel_Yellow");
            frameGreenRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/IsolateRoom/Nezzach_Feel_Green");
            workingOffRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/IsolateRoom/Nezzach_Work_Off");
            workingOnRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/IsolateRoom/Nezzach_Work_On");
        }

        else if (targetUnit.model.sefiraNum == "3")
        {

            frameRedRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/IsolateRoom/Hod_Feel_Red");
            frameYellowRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/IsolateRoom/Hod_Feel_Yellow");
            frameGreenRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/IsolateRoom/Hod_Feel_Green");
            workingOffRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/IsolateRoom/Hod_Work_Off");
            workingOnRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/IsolateRoom/Hod_Work_On");
        }

        else if (targetUnit.model.sefiraNum == "4")
        {
            frameRedRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/IsolateRoom/Yessod_Feel_Red");
            frameYellowRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/IsolateRoom/Yessod_Feel_Yellow");
            frameGreenRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/IsolateRoom/Yessod_Feel_Green");
            workingOffRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/IsolateRoom/Yessod_Work_Off");
            workingOnRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/IsolateRoom/Yessod_Work_On");

        }
    }

	public void UpdateStatus()
	{

		if(targetUnit != null)
		{
            // 잠시 안 띄움
			//feelingText.text = targetUnit.model.feeling.ToString ();
			//feelingText.text = targetUnit.model.energyPoint.ToString();
            creatureLevel.text = targetUnit.model.metaInfo.level;
            creatureName.text = targetUnit.model.metaInfo.name;


			feelingText.text = "";
			creatureLevel.text = "";
			creatureName.text = "";
            FeelingTextForDebug.text =this.targetUnit.model.feeling + "/" + this.targetUnit.model.metaInfo.feelingMax + " " + this.targetUnit.model.GetFeelingPercent() + "%";

            //feelingText.text = "";
            
            CreatureFeelingState feelingState = targetUnit.model.GetFeelingState();

            if (feelingState == CreatureFeelingState.GOOD)
            {
                Warning1.gameObject.SetActive(false);
                Warning2.gameObject.SetActive(false);
                Warning3.gameObject.SetActive(false);
                Warning4.gameObject.SetActive(false);

                Warning_Yellow1.gameObject.SetActive(false);
                Warning_Yellow2.gameObject.SetActive(false);
                Warning_Yellow3.gameObject.SetActive(false);
                Warning_Yellow4.gameObject.SetActive(false);

                Warning_Green1.gameObject.SetActive(true);
                Warning_Green2.gameObject.SetActive(true);
                Warning_Green3.gameObject.SetActive(true);
                Warning_Green4.gameObject.SetActive(true);

                frameRedRenderer.gameObject.SetActive(false);
                frameYellowRenderer.gameObject.SetActive(false);
                frameGreenRenderer.gameObject.SetActive(true);
            }
            else if (feelingState == CreatureFeelingState.NORM)
            {
                Warning1.gameObject.SetActive(false);
                Warning2.gameObject.SetActive(false);
                Warning3.gameObject.SetActive(false);
                Warning4.gameObject.SetActive(false);

                Warning_Yellow1.gameObject.SetActive(true);
                Warning_Yellow2.gameObject.SetActive(true);
                Warning_Yellow3.gameObject.SetActive(true);
                Warning_Yellow4.gameObject.SetActive(true);

                Warning_Green1.gameObject.SetActive(false);
                Warning_Green2.gameObject.SetActive(false);
                Warning_Green3.gameObject.SetActive(false);
                Warning_Green4.gameObject.SetActive(false);

                frameRedRenderer.gameObject.SetActive(false);
                frameYellowRenderer.gameObject.SetActive(true);
                frameGreenRenderer.gameObject.SetActive(false);
            }
            else
            {
                Warning1.gameObject.SetActive(true);
                Warning2.gameObject.SetActive(true);
                Warning3.gameObject.SetActive(true);
                Warning4.gameObject.SetActive(true);

                Warning_Yellow1.gameObject.SetActive(false);
                Warning_Yellow2.gameObject.SetActive(false);
                Warning_Yellow3.gameObject.SetActive(false);
                Warning_Yellow4.gameObject.SetActive(false);

                Warning_Green1.gameObject.SetActive(false);
                Warning_Green2.gameObject.SetActive(false);
                Warning_Green3.gameObject.SetActive(false);
                Warning_Green4.gameObject.SetActive(false);

                frameRedRenderer.gameObject.SetActive(true);
                frameYellowRenderer.gameObject.SetActive(false);
                frameGreenRenderer.gameObject.SetActive(false);
            }

            if (targetUnit.model.state == CreatureState.WORKING)
            {
                //workingOnRenderer.gameObject.SetActive(true);
                //workingOffRenderer.gameObject.SetActive(false);
                observeRoom.SetBool("ObserveStart", false);
                observeRoom.SetInteger("ObserveProcess",0);
                //observeCatuionSprite.gameObject.SetActive(false);
            }

            else if (targetUnit.model.state == CreatureState.OBSERVE)
            {
                //workingOnRenderer.gameObject.SetActive(true);
                //workingOffRenderer.gameObject.SetActive(false);
                observeRoom.SetBool("ObserveStart", true);
                observeRoom.SetInteger("ObserveProcess", 1);
                // observeCatuionSprite.gameObject.SetActive(true);
            }

            else
            {
                //workingOnRenderer.gameObject.SetActive(false);
                //workingOffRenderer.gameObject.SetActive(true);
                observeRoom.SetBool("ObserveStart", false);
                observeRoom.SetInteger("ObserveProcess", 0);
                //observeCatuionSprite.gameObject.SetActive(false);
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
        targetUnit.OnClick();
        
    }

    public void OnClickedCreatureRoom() {
        CreatureModel oldCreature = (CollectionWindow.currentWindow != null) ? CollectionWindow.currentWindow.GetCreature() : null;
        
        if (SelectWorkAgentWindow.currentWindow != null)
            SelectWorkAgentWindow.currentWindow.CloseWindow();

        if (WorkAllocateWindow.currentWindow != null)
        {
            WorkAllocateWindow.currentWindow.CloseWindow();
        }


        CollectionWindow.Create(_targetUnit.model);


        // TODO : 최적화 필요
        collection = GameObject.FindGameObjectWithTag("AnimCollectionController");


        if (collection.GetComponent<Animator>().GetBool("isTrue"))
        {
            //Debug.Log(collection.GetComponent<Animator>().GetBool("isTrue"));
            collection.GetComponent<Animator>().SetBool("isTrue", false);
        }
        else if (oldCreature == _targetUnit.model)
        {
            //Debug.Log(collection.GetComponent<Animator>().GetBool("isTrue"));
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

		if (_targetUnit.model.IsTargeted () == true)
		{
			color.a = 0f;
			roomFogRenderer.color = color;
		}
        else if (_targetUnit.model.state == CreatureState.WORKING)
        {
            color.a = 0f;
            roomFogRenderer.color = color;
        }

        else if (_targetUnit.model.state == CreatureState.OBSERVE)
        {
            color.a = 0f;
            roomFogRenderer.color = color;
        }

        else if(_targetUnit.model.sefiraEmpty)
        {
            color.a = 0.9f;
            roomFogRenderer.color = color;
        }

        else
        {
            color.a = (1 - 0.2f * _targetUnit.model.observeProgress)*0.75f;
            roomFogRenderer.color = color;
        }
	}
}