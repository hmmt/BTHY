using UnityEngine;
using System.Collections;

public class ElevatorMover : MonoBehaviour {

	public enum STATE {MOVE, RMOVE, STOP, RSTOP};

	public Transform startPoint;
	public Transform endPoint;

	public Vector2 startPosition;
	public Vector2 endPosition;

	
	public float totalTime = 1.5f;
	public float moveDelay = 1.0f;
	public float timer = 0.0f;

    public static STATE currentState = STATE.STOP;
    public bool playerOn=false;
    public bool isStop = false;

    public Rigidbody2D playerBody;

	void Awake()
	{
		gameObject.AddComponent<Rigidbody2D> ();
		GetComponent<Rigidbody2D>().isKinematic = true;
	}

	// Use this for initialization
	void Start () {

        if (PlayerModel.instance.GetDay() != 0)
        {
            transform.localPosition = endPoint.localPosition;
            //currentState = STATE.RSTOP;
        }

        else
        {
            //transform.localPosition = startPoint.localPosition;
        }

		if(startPoint != null)
		{
			startPosition = new Vector2(startPoint.position.x, startPoint.position.y);
		}
		else
		{
			startPosition = new Vector2(transform.position.x, transform.position.y);
		}

		if(endPoint != null)
		{
			endPosition = new Vector2(endPoint.position.x, endPoint.position.y);
		}
		else
		{
			endPosition = new Vector2(startPosition.x, startPosition.y);
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	void FixedUpdate()
	{

		Vector2 nextPosition = GetComponent<Rigidbody2D>().position;
        if(playerOn && !isStop)
        {
		    timer += Time.deltaTime;
        }


		switch(currentState)
		{
		case STATE.MOVE:
			if(timer > totalTime)
			{
				timer = 0.0f;
                //transform.position = new Vector3(endPosition.x, endPosition.y);-1.175.03
				nextPosition = endPosition;
				NextState();
				break;
			}
			nextPosition = new Vector2(
				Interpolation(startPosition.x, endPosition.x, timer/totalTime),
				Interpolation(startPosition.y, endPosition.y, timer/totalTime));
			break;
		case STATE.RMOVE:
			if(timer > totalTime)
			{
				timer = 0.0f;
				//transform.position = new Vector3(startPosition.x, startPosition.y);
				//nextPosition = startPosition;
                nextPosition = startPosition;
				NextState();
				break;
			}
			nextPosition = new Vector2(
				Interpolation(endPosition.x, startPosition.x, timer/totalTime),
				Interpolation(endPosition.y, startPosition.y, timer/totalTime));
			break;
		case STATE.STOP:
		case STATE.RSTOP:
			if(timer > moveDelay)
			{
				timer = 0.0f;
				NextState();
			}
			break;
		}

		GetComponent<Rigidbody2D>().MovePosition(nextPosition);
		GetComponent<Rigidbody2D>().velocity = (nextPosition - GetComponent<Rigidbody2D>().position)/Time.deltaTime;
       }

	private void NextState()
	{
		switch(currentState)
		{
		case STATE.MOVE:
			currentState = STATE.RSTOP;
			break;
		case STATE.RMOVE:
            playerOn = false;
			currentState = STATE.STOP;
			break;
		case STATE.STOP:
			currentState = STATE.MOVE;
			break;
		case STATE.RSTOP:
            playerOn = false;
			currentState = STATE.RMOVE;
			break;
		}
	}

	private float Interpolation(float start, float end, float t)
	{
		t = Mathf.Clamp (t, 0.0f, 1.0f);
		return start + -2 * (end-start) * t*t*t + 3 *(end-start) * t*t;
	}

	void OnCollisionEnter2D(Collision2D col)
	{
        if (col.gameObject.tag == "Player")
        {
            playerOn = true;
            playerBody.gravityScale = 40;
        }

        else
        {
           playerOn = false;
        }
		
	}

	void OnCollisionExit2D(Collision2D col)
	{

	}
}
