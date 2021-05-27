using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //�ڬO�����ܼ�
    //�������
    Transform playerTra;
    SpriteRenderer playerSpr;
    Rigidbody2D playerRig;
    Animator playerAni;

    //Camera
    public GameObject[] objectCam;
    public Camera playerCam;
    public Vector3 playerOffset;

    //Force
    public float forceX = 25;
    public float forceJump = 750;
    public float forceDash = 3000;

    //HP
    public float healthPoint = 3.00f;

    //Move
    //J�����FK���D�BL�Ĩ�
    public bool canMove = false;

    //Jump
    public bool canJump = false;
    public int multiJump = 1;
    public int countJump = 0;

    //Dash
    public bool canDash = false;
    public bool isDashing = false;
    public int multiDash = 1;
    public int countDash = 0;

    //Attack
    public bool canAttack = false;
    public bool isAttacking = false;

    //Hurt
    public bool canHurt = false;

    //Ground
    public GameObject objectGrounded;
    public bool isGrounded = false;

    //Rise
    public bool isRising = false;

    //Fall
    public bool isFalling = false;

    //Stun
    public bool isStunning = false;

    //Dead
    public bool isDead = false;

    //public float horizontalDirection;
    //public float speedX;
    //public float speedY;

    private void Start()
    {
        //�]�w��l�Ѽ�
        playerTra = GetComponent<Transform>();
        playerSpr = GetComponent<SpriteRenderer>();
        playerRig = GetComponent<Rigidbody2D>();
        playerAni = GetComponent<Animator>();
        objectCam = GameObject.FindGameObjectsWithTag("MainCamera");    //GameObject.Find("Main Camera");
        playerCam = objectCam[0].GetComponent<Camera>();                //objectCam.GetComponent<Camera>();
        playerOffset = playerTra.position;
        canMove = true;
        canJump = true;
        multiJump = 2;
        canDash = true;
        multiDash = 2;
        canAttack = true;
        canHurt = true;
    }

    private void Update()
    {
        //�����ť�ð���H�U��k
        //���O��ť
        if (canMove)
        {
            MovementX();
            Jump();
            Dash();
            Attack();

            //�ʵe��ť
            if (!isRising)
                IsRising();
            if (!isFalling)
                IsFalling();
        }
    }

    private void FixedUpdate()
    {
    }

    private void LateUpdate()
    {
        MovementCamera();
    }

    //public Vector3 off;
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(playerTra.position + new Vector3(0, -0.4f, 0.2f), 0.2f);
    }

    //�۾��l�H
    void MovementCamera()
    {
        //�u��X�b
        //playerCam.transform.position += new Vector3(playerTra.position.x, 0, 0) - new Vector3(offset.x, 0, 0);
        //offset = playerTra.position;

        //����
        playerCam.transform.position += playerTra.position - playerOffset;
        playerOffset = playerTra.position;
    }

    //���]�ʧ@
    void ResetAni()
    {
        isRising = false;
        isFalling = false;
        playerAni.SetBool("isJumping", false);
        playerAni.SetBool("isFalling", false);
        playerAni.SetBool("isDashing", false);
        playerAni.SetBool("isAttacking", false);
    }

    //��������
    void MovementX()
    {
        //horizontalDirection = Input.GetAxis("Horizontal");
        //playerRig.AddForce(new Vector3(forceX * horizontalDirection, 0));

        //���k�P�ɫ��ڴN����
        if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)))
        {
            playerAni.SetBool("isMoving", false);
        }
        //�k
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
        //��
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
        //�䥦
        else
        {
            playerAni.SetBool("isMoving", false);
        }
    }

    //���D
    void Jump()
    {
        //K���D
        if (Input.GetKeyDown(KeyCode.K))
            if (canJump)
            {
                playerRig.velocity = new Vector2(0, 0);
                ResetAni();
                StartCoroutine(CoolDownJump());
            }
    }

    IEnumerator CoolDownJump()
    {
        canJump = false;
        countJump++;
        playerRig.AddForce(Vector2.up * forceJump);
        isRising = true;
        playerAni.SetBool("isJumping", true);

        yield return new WaitForSeconds(0.10f);

        if (countJump < multiJump)
            canJump = true;
        else
            canJump = false;
    }

    //�Ĩ�
    void Dash()
    {
        //L�Ĩ�
        if (Input.GetKeyDown(KeyCode.L))
            if (canDash)
            {
                playerRig.velocity = new Vector2(0, 0);
                ResetAni();
                StartCoroutine(CoolDownDash());
            }
    }

    IEnumerator CoolDownDash()
    {
        canDash = false;
        countDash++;

        if (!playerSpr.flipX)
            playerRig.AddForce(Vector2.right * forceDash);
        else
            playerRig.AddForce(Vector2.left * forceDash);

        isDashing = true;
        playerAni.SetBool("isDashing", true);

        yield return new WaitForSeconds(0.25f);

        playerRig.velocity = new Vector2(0, 0);
        playerAni.SetBool("isDashing", false);
        isDashing = false;

        if (countDash < multiDash)
            canDash = true;
        else
            canDash = false;
    }

    //����
    void Attack()
    {
        //J����
        if (Input.GetKeyDown(KeyCode.J))
            if (canAttack)
            {
                ResetAni();
                StartCoroutine(CoolDownAttack());
            }
    }

    IEnumerator CoolDownAttack()
    {
        canAttack = false;
        isAttacking = true;
        playerAni.SetBool("isAttacking", true);

        yield return new WaitForSeconds(0.75f);

        playerAni.SetBool("isAttacking", false);
        isAttacking = false;
        canAttack = true;
    }

    //����
    void Hurt(float damage = 1.00f)
    {
        if (canHurt)
        {
            healthPoint -= damage;

            if (healthPoint > 0)
            {
                StartCoroutine(CoolDownHurt());
            }
            else
            {
                playerRig.velocity = new Vector2(0, 0);
                ResetAni();
                StartCoroutine(IsDead());
            }
        }
    }

    IEnumerator CoolDownHurt(bool withStun = true, float timeStunning = 0.50f, float timeInvincible = 1.50f)
    {
        canHurt = false;

        if (withStun)
        {
            playerRig.velocity = new Vector2(0, 0);
            ResetAni();
            StartCoroutine(CoolDownStun(timeStunning));
        }

        yield return new WaitForSeconds(timeInvincible);

        canHurt = true;
    }

    IEnumerator CoolDownStun(float stunningTime)
    {
        canMove = false;
        isStunning = true;
        playerAni.SetBool("isStunning", true);

        yield return new WaitForSeconds(stunningTime);

        playerAni.SetBool("isStunning", false);
        isStunning = false;
        canMove = true;
    }

    IEnumerator IsDead()
    {
        canMove = false;
        playerAni.SetBool("isDead", true);

        yield return 0;
        /*
        playerAni.SetBool("isDead", false);
        canMove = true;
        */
    }

    //�O�_���b�W��
    void IsRising()
    {
        if (playerTra.position.y - playerOffset.y > 0 && !isGrounded)
        {
            isRising = true;
            isFalling = false;
            playerAni.SetBool("isJumping", true);
            playerAni.SetBool("isFalling", false);
        }
        else
        {
            isRising = false;
            playerAni.SetBool("isJumping", false);
        }
    }

    //�O�_���b�U��
    void IsFalling()
    {
        if (playerTra.position.y - playerOffset.y < 0 && !isGrounded)
        {
            isFalling = true;
            isRising = false;
            playerAni.SetBool("isFalling", true);
            playerAni.SetBool("isJumping", false);
        }
        else
        {
            isFalling = false;
            playerAni.SetBool("isFalling", false);
        }
    }


    //�I����
    private void OnCollisionStay2D(Collision2D collision)
    {
        //if (collision.gameObject.CompareTag("Ground"))
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Water"))
        {
            foreach (ContactPoint2D element in collision.contacts)
            {
                if (element.normal.y > 0.20f)
                {
                    objectGrounded = collision.gameObject;
                    isGrounded = true;
                    canJump = true;
                    canDash = true;
                    countJump = 0;
                    countDash = 0;
                    ResetAni();

                    break;
                }
            }

            if (collision.gameObject.CompareTag("Water"))
                Hurt();
        }
    }

    //���}�I��
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == objectGrounded)
        {
            objectGrounded = null;
            isGrounded = false;
        }
    }

}
