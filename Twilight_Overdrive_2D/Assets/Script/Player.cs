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
    AnimatorStateInfo playerAniStateInfo;

    /*
    //Camera
    public GameObject[] objectCam;
    public Camera playerCam;
    */

    //���a�e�@�V��m
    public Vector3 playerOffset;

    //Force
    public float forceX = 25;
    public float forceJump = 1500;
    public float forceDash = 3000;

    //HP
    public float healthPoint = 3.00f;

    //Move
    //J�����FK���D�BL�Ĩ�
    //public bool canMove = false;

    //Attack
    public bool canAttack = false;
    public bool isAttacking = false;
    public int multiAttack = 1;
    public int countAttack = 0;

    //Jump
    public bool canJump = false;
    public int multiJump = 1;
    public int countJump = 0;

    //Dash
    public bool canDash = false;
    public bool isDashing = false;
    public int multiDash = 1;
    public int countDash = 0;

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
        //objectCam = GameObject.FindGameObjectsWithTag("MainCamera");    //GameObject.Find("Main Camera");
        //playerCam = objectCam[0].GetComponent<Camera>();                //objectCam.GetComponent<Camera>();
        playerOffset = playerTra.position;
        multiAttack = 3;
        multiJump = 2;
        multiDash = 2;
        canAttack = true;
        canJump = true;
        canDash = true;
        canHurt = true;
    }

    private void Update()
    {
        //���o�ʵe���A
        playerAniStateInfo = playerAni.GetCurrentAnimatorStateInfo(0);

        //�����ť�ð���H�U��k
        if (!(isStunning || isDead))
        {
            //�ʵe��ť
            if (!isRising)
                IsRising();
            if (!isFalling)
                IsFalling();

            //���O��ť
            if (!((isAttacking && isGrounded) || isDashing))
                MovementX();
            if (!isDashing)
                Attack();
            if (!((isAttacking && isGrounded) || isDashing))
                Jump();
            if (!isAttacking)
                Dash();

        }
    }

    private void FixedUpdate()
    {
    }

    private void LateUpdate()
    {
    }

    //���]�ʧ@
    void ResetAni()
    {
        //isAttacking = false;
        //isRising = false;
        //isFalling = false;
        //isDashing = false;
        countAttack = 0;
        playerAni.SetInteger("attack", countAttack);
        playerAni.SetBool("isJumping", false);
        playerAni.SetBool("isFalling", false);
        playerAni.SetBool("isDashing", false);
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
            //playerTra.eulerAngles = Vector3.zero;
            playerSpr.flipX = false;
            playerRig.transform.Translate(Vector2.right * forceX * Time.deltaTime);

            if (isGrounded)
                playerAni.SetBool("isMoving", true);
            else
                playerAni.SetBool("isMoving", false);
        }
        //��
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            //playerTra.eulerAngles = new Vector3(0, 180, 0);
            playerSpr.flipX = true;
            playerRig.transform.Translate(Vector2.left * forceX * Time.deltaTime);

            if (isGrounded)
                playerAni.SetBool("isMoving", true);
            else
                playerAni.SetBool("isMoving", false);
        }
        //�䥦
        else
        {
            playerAni.SetBool("isMoving", false);
        }
    }

    //����
    void Attack()
    {
        if (!(playerAniStateInfo.IsTag("ATK01") || playerAniStateInfo.IsTag("ATK02")
            || playerAniStateInfo.IsTag("ATK03")) && isAttacking)
            StartCoroutine(CoolDownAttack());

        if ((playerAniStateInfo.IsTag("ATK01") || playerAniStateInfo.IsTag("ATK02")
            || playerAniStateInfo.IsTag("ATK03")) && playerAniStateInfo.normalizedTime > 1.00f)
            StartCoroutine(CoolDownAttack());

        if (countAttack >= multiAttack)
            canAttack = false;

        //J����
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (canAttack)
                if (countAttack == 0)
                {
                    ResetAni();
                    countAttack = 1;
                    isAttacking = true;
                    playerAni.SetInteger("attack", countAttack);
                }
                else if (playerAniStateInfo.IsTag("ATK01") && countAttack == 1 && playerAniStateInfo.normalizedTime < 0.8f)
                {
                    countAttack = 2;
                }
                else if (playerAniStateInfo.IsTag("ATK02") && countAttack == 2 && playerAniStateInfo.normalizedTime < 0.8f)
                {
                    countAttack = 3;
                }
        }
    }

    IEnumerator CoolDownAttack()
    {
        countAttack = 0;
        playerAni.SetInteger("attack", countAttack);
        isAttacking = false;
        canAttack = false;

        yield return new WaitForSeconds(0.10f);

        canAttack = true;
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
        countJump++;
        playerRig.AddForce(Vector2.up * forceJump);
        playerAni.SetBool("isJumping", true);
        canJump = false;

        yield return new WaitForSeconds(0.10f);

        if (countJump < multiJump)
            canJump = true;
        else
            canJump = false;
    }

    //�Ĩ�
    void Dash()
    {
        if (!playerAniStateInfo.IsTag("Dash") && isDashing)
            StartCoroutine(CoolDownDash());

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
        countDash++;
        playerRig.gravityScale = 0.0f;

        if (!playerSpr.flipX)
            playerRig.AddForce(Vector2.right * forceDash);
        else
            playerRig.AddForce(Vector2.left * forceDash);

        playerAni.SetBool("isDashing", true);
        isDashing = true;
        canDash = false;

        yield return new WaitForSeconds(0.25f);

        playerRig.velocity = new Vector2(0, playerRig.velocity.y);
        playerRig.gravityScale = 5.0f;
        playerAni.SetBool("isDashing", false);
        isDashing = false;

        if (countDash < multiDash)
            canDash = true;
        else
            canDash = false;
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
        isStunning = true;
        playerAni.SetBool("isStunning", true);

        yield return new WaitForSeconds(stunningTime);

        playerAni.SetBool("isStunning", false);
        isStunning = false;
    }

    IEnumerator IsDead()
    {
        isDead = true;
        playerAni.SetBool("isDead", true);

        yield return 0;

        /*
        playerAni.SetBool("isDead", false);
        */
    }

    //�O�_���b�W��
    //�]�ثe���ϧ@�ΤO�A�G�ƭȤ���0
    void IsRising()
    {
        if (playerTra.position.y - playerOffset.y > 0.25 && !isGrounded)
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

    //�ʵe�ƥ�
    void GoToNextAttackAction()
    {
        playerAni.SetInteger("attack", countAttack);
    }

    //�i�J�I��
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.CompareTag("Ground"))
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Water"))
        {
            foreach (ContactPoint2D element in collision.contacts)
            {
                if (element.normal.y > 0.25f)
                {
                    countJump = 0;
                    countDash = 0;
                    ResetAni();

                    break;
                }
            }
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
                if (element.normal.y > 0.25f)
                {
                    objectGrounded = collision.gameObject;
                    isGrounded = true;
                    canJump = true;
                    canDash = true;

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
