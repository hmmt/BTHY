using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float maxSpeed = 10;
    public bool facingRight = true;

    bool grounded = false;
    public Transform groundCheck;
    float groundRadius = 0.2f;
    public LayerMask whatIsGround;
	public float jumpForce = 700f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
       Rigidbody2D player = GetComponent<Rigidbody2D>();

	    if(grounded && Input.GetKeyDown(KeyCode.Space))
        {
           player.AddForce(new Vector2(0,jumpForce));
        }

        Camera.main.transform.localPosition = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y+2, Camera.main.transform.localPosition.z);
	}

    void FixedUpdate()
    {
       grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);

       Rigidbody2D player = GetComponent<Rigidbody2D>();
        
        float move = Input.GetAxis("Horizontal");
        player.velocity = new Vector2(move * maxSpeed, player.velocity.y);

       

        if (move < 0 && facingRight)
            Filp();
        else if (move > 0 && !facingRight)
            Filp();
    }

    void Filp()
    {
        facingRight = !facingRight;

        Vector3 theScale = transform.localScale;
        GameObject flash = GameObject.Find("Flash");
        Vector3 flashScale = flash.transform.localScale;
        Quaternion theRotate;
        float z = flash.transform.localRotation.z;

        theScale.x *= -1;
        transform.localScale = theScale;
        flashScale.x *= -1;
       flash.transform.localScale = flashScale;

        if (!facingRight)
        {
            theRotate = Quaternion.Euler(0, 0, z+180);
        }
        else
        {
            theRotate = Quaternion.Euler(0, 0, z);
        }
      
      flash.transform.localRotation = theRotate;

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Floor")
        {
            Rigidbody2D player = GetComponent<Rigidbody2D>();
            player.gravityScale = 1;
            Debug.Log("ㅇ호애에ㅔㅇ" + player.gravityScale);
        }

    }
}
