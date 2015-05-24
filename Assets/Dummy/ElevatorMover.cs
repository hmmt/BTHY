using UnityEngine;
using System.Collections;

public class ElevatorMover : MonoBehaviour {

	private enum STATE {MOVE, RMOVE, STOP, RSTOP};

	public Transform startPoint;
	public Transform endPoint;

	public Vector2 startPosition;
	public Vector2 endPosition;

	private STATE currentState = STATE.STOP;
	public float totalTime = 1.0f;
	public float moveDelay = 1.0f;
	private float timer = 0.0f;

    public bool playerOn=false;

    public Rigidbody2D playerBody;

	void Awake()
	{
		gameObject.AddComponent<Rigidbody2D> ();
		GetComponent<Rigidbody2D>().isKinematic = true;
	}

	// Use this for initialization
	void Start () {
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
		timer += Time.deltaTime;

		switch(currentState)
		{
		case STATE.MOVE:
			if(timer > totalTime)
			{
				timer = 0.0f;
				//transform.position = new Vector3(endPosition.x, endPosition.y);
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
			currentState = STATE.STOP;
			break;
		case STATE.STOP:
			currentState = STATE.MOVE;
			break;
		case STATE.RSTOP:
			currentState = STATE.RMOVE;
			break;
		}
	}

	private float Interpolation(float start, float end, float t)
	{
		/*
		 * f'(a) = 0
		 * f'(b) = 0
		 * f'(x)/F = (x-a)(x-b) = x^2 -(a+b)x + ab
		 * f(x)/F =  0.33 x^3 - (a+b)/2 x^2 + abx + C
		 * f(a)/F = 0.33a^3 - 0.5 a^2 - 0.5ab + a^2 * b + C = 0
		 *  C = - (0.33a^3 - 0.5 a^2 - 0.5ab + a^2 * b)
		 * f(b)/F = 0.33b^3 - 0.5 b^2 - 0.5ab + b^2 * a + C = H
		 * 
		 * f'(0) = 0
		 * f'(1) = 0
		 * f'(x)/F = -x(x-1) = x - x^2
		 * f(x)/F = - (1/3) x^3 + 1/2 x^2
		 * f(1)/F = -(1/3) + 0.5 = 1/6
		 * 
		 * f(x) = -6h/3 * x^3 + 6h /2 * x^2
		 *      = -2hx^3 + 3h*x^2
		 */

		t = Mathf.Clamp (t, 0.0f, 1.0f);
		return start + -2 * (end-start) * t*t*t + 3 *(end-start) * t*t;
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		//Debug.Log (col.gameObject);
		
		//if(col.gameObject.transform.position.y < gameObject.transform.position.y)
        if (col.gameObject.tag == "Player")
        {
            //Physics2D.IgnoreCollision(collider2D, col.collider, true);
           // Debug.Log("으으으으흠");
            currentState = STATE.STOP;
            playerOn = true;
           // playerBody.gravityScale = 100;
        }

        else
        {
           // playerBody.gravityScale = 1;
           playerOn = false;
        }
		
	}

	void OnCollisionExit2D(Collision2D col)
	{
		//Debug.Log ("exit");
		//Physics2D.IgnoreCollision (collider2D, col.collider, false);
		//Physics2D.igno
	}
}
