using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float maxSpeed = 6;
    public bool facingRight = true;

    bool grounded = false;
    public Transform groundCheck;
    float groundRadius = 0.2f;
    public LayerMask whatIsGround;
	public float jumpForce = 700f;
    public float oldPos;

    public bool playerMove = false;

    public Animator playerAnimator;

    public Animator playerHair;
    public Animator playerFace;
    public Animator playerBody;

    public GameObject face;
 
    public bool cameraWalk=false;

    int k = 0;


	// Use this for initialization
	void Start () 
    {
        Rigidbody2D player = GetComponent<Rigidbody2D>();
        oldPos = transform.localPosition.x;

        Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, player.transform.localPosition.y + 4f, Camera.main.transform.localPosition.z);

        Debug.Log(PlayerModel.instance.playerSpot.x + " " + PlayerModel.instance.playerSpot.y);

        if (PlayerModel.instance.GetDay() != 0)
            player.transform.localPosition = PlayerModel.instance.playerSpot;
        else
        {
            player.transform.localPosition = new Vector3(-16.3f, 30.33f, 0);
        }
	}
    
    void LateUpdate()
    {
        if (Input.GetKey(KeyCode.L))
        {
            foreach (var renderer in face.GetComponents<SpriteRenderer>())
            {
                Debug.Log("SpriteName "+k+": " + renderer.sprite.name);
               // string spriteName = renderer.sprite.name;
                if(renderer.sprite.name == "Face_A_00")
                    renderer.sprite = Resources.Load<Sprite>("Sprites/Agent/Face/Face_B_00");
                else if(renderer.sprite.name == "Face_A_01")
                     renderer.sprite = Resources.Load<Sprite>("Sprites/Agent/Face/Face_B_01");
                else if(renderer.sprite.name == "Face_A_02")
                    renderer.sprite = Resources.Load<Sprite>("Sprites/Agent/Face/Face_B_02");
               
            }
        }
    }

	// Update is called once per frame
	void Update () 
    {
       Rigidbody2D player = GetComponent<Rigidbody2D>();

	    if(grounded && Input.GetKeyDown(KeyCode.Space))
        {
           player.AddForce(new Vector2(0,jumpForce));
        }
        
        if (playerMove)
        {
            //playerAnimator.SetBool("PlayerMove", true);
            playerHair.SetBool("Move",true);
            playerFace.SetBool("Move", true);
            playerBody.SetBool("Move", true);
        }

        else
        {
            //playerAnimator.SetBool("PlayerMove", false);
            playerHair.SetBool("Move", false);
            playerFace.SetBool("Move", false);
            playerBody.SetBool("Move", false);
        }

      if (player.velocity.x > 1 || player.velocity.x < -1)
      {
            if (oldPos != transform.localPosition.x)
            {
                playerMove = true;
            }
        }
        else
        {
            playerMove = false;
        }

        oldPos = transform.localPosition.x;
        // !PlatformerCamera.introWalk
        if ( false)
        {
            if(!cameraWalk)
            Camera.main.transform.localPosition = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y + 4f, Camera.main.transform.localPosition.z);
        }
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
        if (col.gameObject.tag == "Floor" || col.gameObject.tag == "SecurityHall")
        {
            Rigidbody2D player = GetComponent<Rigidbody2D>();
            player.gravityScale = 1;
        }
    }

}
