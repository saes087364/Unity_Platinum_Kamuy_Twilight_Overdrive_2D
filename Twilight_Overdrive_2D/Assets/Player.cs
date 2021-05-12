using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //我是全域變數
    //物件欄位
    Transform playerTra;
    SpriteRenderer playerSpr;
    Rigidbody2D playerRig;
    Animator playerAni;

    //Force
    public float forceX = 25;
    public float forceY = 750;
    public float forceDash = 5000;

    //Camera
    public GameObject[] objectCam;
    public Camera playerCam;
    public Vector3 playerOffset;

    //Ground
    public GameObject objectGrounded;
    public bool isGrounded = false;

    //Jump
    public int jumpCount = 0;

    //Dash
    public bool canDash = false;
    public bool isDashing = false;

    //Rise
    public bool isRising = false;

    //Fall
    public bool isFalling = false;

    //public float horizontalDirection;
    //public float speedX;
    //public float speedY;

    private void Start()
    {
        //設定初始參數
        playerTra = GetComponent<Transform>();
        playerSpr = GetComponent<SpriteRenderer>();
        playerRig = GetComponent<Rigidbody2D>();
        playerAni = GetComponent<Animator>();
        objectCam = GameObject.FindGameObjectsWithTag("MainCamera");    //GameObject.Find("Main Camera");
        playerCam = objectCam[0].GetComponent<Camera>();                //objectCam.GetComponent<Camera>();
        playerOffset = playerTra.position;
        canDash = true;
    }

    private void Update()
    {
        //持續監聽並執行以下方法
        //指令監聽
        MovementX();
        MovementY();
        if (canDash)
            Dash();

        //動畫監聽
        if (!isRising)
            IsRising();
        if (!isFalling)
            IsFalling();
    }

    private void FixedUpdate()
    {

    }

    private void LateUpdate()
    {
        MovementCamera();
    }

    //水平移動
    void MovementX()
    {
        //horizontalDirection = Input.GetAxis("Horizontal");
        //playerRig.AddForce(new Vector3(forceX * horizontalDirection, 0));

        //左右同時按我就不動
        if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)))
        {
            playerAni.SetBool("isMoving", false);
        }
        //右
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            if (isGrounded)
                playerAni.SetBool("isMoving", true);
            else
                playerAni.SetBool("isMoving", false);

            //playerTra.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            playerSpr.flipX = false;
            playerRig.transform.Translate(Vector2.right * forceX * Time.deltaTime);
        }
        //左
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            if (isGrounded)
                playerAni.SetBool("isMoving", true);
            else
                playerAni.SetBool("isMoving", false);

            //playerTra.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            playerSpr.flipX = true;
            playerRig.transform.Translate(Vector2.left * forceX * Time.deltaTime);
        }
        //其它
        else
        {
            playerAni.SetBool("isMoving", false);
        }
    }

    //跳躍
    void MovementY()
    {
        //記得隱藏不要的按鍵
        //空白鍵跳躍
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpCount = 1;
            playerAni.SetBool("isMoving", false);
            playerRig.AddForce(Vector2.up * forceY);
            playerAni.SetBool("isJumping", true);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && !isGrounded && jumpCount != 2)
        {
            jumpCount = 2;
            playerRig.velocity = new Vector2(playerRig.velocity.x, 0);
            playerRig.AddForce(Vector2.up * forceY);
            playerAni.SetBool("isJumping", true);
        }

        //上或W跳躍
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            jumpCount = 1;
            playerAni.SetBool("isMoving", false);
            playerRig.AddForce(Vector2.up * forceY);
            playerAni.SetBool("isJumping", true);
        }
        else if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && !isGrounded && jumpCount != 2)
        {
            jumpCount = 2;
            playerRig.velocity = new Vector2(playerRig.velocity.x, 0);
            playerRig.AddForce(Vector2.up * forceY);
            playerAni.SetBool("isJumping", true);
        }
    }

    //相機追人
    void MovementCamera()
    {
        //只動X軸
        //playerCam.transform.position += new Vector3(playerTra.position.x, 0, 0) - new Vector3(offset.x, 0, 0);
        //offset = playerTra.position;

        //都動
        playerCam.transform.position += playerTra.position - playerOffset;
        playerOffset = playerTra.position;
    }

    //衝刺
    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.E))
            StartCoroutine(CoolDownDash());
    }

    IEnumerator CoolDownDash()
    {
        playerRig.velocity = new Vector2(0, 0);

        if (!playerSpr.flipX)
        {
            playerRig.AddForce(Vector2.right * forceDash);
        }
        else
        {
            playerRig.AddForce(Vector2.left * forceDash);
        }

        canDash = false;
        isDashing = true;
        playerAni.SetBool("isDashing", true);

        yield return new WaitForSeconds(0.2f);

        playerRig.velocity = new Vector2(0, 0);
        playerAni.SetBool("isDashing", false);
        isDashing = false;

        yield return new WaitUntil(() => isGrounded == true);

        canDash = true;
    }

    //是否正在上升
    void IsRising()
    {
        if (playerTra.position.y - playerOffset.y > 0 && !isGrounded)
        {
            isRising = true;
            playerAni.SetBool("isJumping", true);
            playerAni.SetBool("isFalling", false);
        }
        else
        {
            isRising = false;
            playerAni.SetBool("isJumping", false);
        }
    }

    //是否正在下降
    void IsFalling()
    {
        if (playerTra.position.y - playerOffset.y < 0 && !isGrounded)
        {
            isFalling = true;
            playerAni.SetBool("isFalling", true);
            playerAni.SetBool("isJumping", false);
        }
        else
        {
            isFalling = false;
            playerAni.SetBool("isFalling", false);
        }
    }

    //碰撞中
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D element in collision.contacts)
            {
                if (element.normal.y > 0.2f)
                {
                    isGrounded = true;
                    objectGrounded = collision.gameObject;
                    isRising = false;
                    isFalling = false;
                    playerAni.SetBool("isJumping", false);
                    playerAni.SetBool("isFalling", false);
                    break;
                }
            }
        }
    }

    //離開碰撞
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == objectGrounded)
        {
            isGrounded = false;
            objectGrounded = null;
        }
    }
}
