using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //[SerializeField] private Vector3 velocity;              // 移動方向
    //[SerializeField] private float moveSpeed = 5.0f;        // 移動速度
    //[SerializeField] private Transform refCamera;  // カメラの水平回転を参照する用
    Animator animator;
    Rigidbody rb;

    public float currentSpeed;//現在速度
    public float jumpForce = 500;//ジャンプ力
    public int maxSpeed = 5;//最大速度
    public int walkingSpeed = 3;//歩き速度
    public int runningSpeed = 5;//走り速度
    [HideInInspector] public Vector3 latestPos;
    [HideInInspector] public Quaternion moveAngle;
    public float moveAngle_Y;
    public float moveAngle_Y_local;

    public float deaccelerationSpeed = 20.0f;//減速度
    public float accelerationSpeed = 5000.0f;//加速度
    public bool grounded;//接地判定
    
    


    [Header("Player SOUNDS")]
    [Tooltip("Jump sound when player jumps.")]
    public AudioSource _jumpSound;
    [Tooltip("Sound while player makes when successfully reloads weapon.")]
    public AudioSource _freakingZombiesSound;
    [Tooltip("Sound Bullet makes when hits target.")]
    public AudioSource _hitSound;
    [Tooltip("Walk sound player makes.")]
    public AudioSource _walkSound;
    [Tooltip("Run Sound player makes.")]
    public AudioSource _runSound;

    private Vector3 slowdownV;
    private Vector2 horizontalMovement;
    private LayerMask ignoreLayer;//to ignore player layer

    public MouseLook mouseLook;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        PlayerMovementLogic();
    }

    void Update()
    {
        Sprint();
        Jumping();
        MoveAnime();
        Crouching();
        WalkingSound();
    }

    void PlayerMovementLogic()
    {
        currentSpeed = rb.velocity.magnitude;

        horizontalMovement = new Vector2(rb.velocity.x, rb.velocity.z);

        if (horizontalMovement.magnitude > maxSpeed)
        {
            horizontalMovement = horizontalMovement.normalized;
            horizontalMovement *= maxSpeed;
        }

        rb.velocity = new Vector3(
            horizontalMovement.x,
            rb.velocity.y,
            horizontalMovement.y);

        if (grounded)
        {
            rb.velocity = Vector3.SmoothDamp(rb.velocity,
                new Vector3(0, rb.velocity.y, 0),
                ref slowdownV,
                deaccelerationSpeed);
        }

        if (grounded)
        {
            rb.AddRelativeForce(Input.GetAxis("Horizontal") * accelerationSpeed * Time.deltaTime, 0, Input.GetAxis("Vertical") * accelerationSpeed * Time.deltaTime);
        }
        else
        {
            rb.AddRelativeForce(Input.GetAxis("Horizontal") * accelerationSpeed / 2 * Time.deltaTime, 0, Input.GetAxis("Vertical") * accelerationSpeed / 2 * Time.deltaTime);
        }

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            deaccelerationSpeed = 0.5f;
        }
        else
        {
            deaccelerationSpeed = 0.1f;
        }
    }

    void Jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddRelativeForce(Vector3.up * jumpForce);
            if (_jumpSound)
                _jumpSound.Play();
            else
                print("Missig jump sound.");
            _walkSound.Stop();
            _runSound.Stop();
        }
    }

    void MoveAnime()
    {
        moveAngle = Quaternion.LookRotation(transform.position - latestPos);
        latestPos = transform.position;
        moveAngle_Y = moveAngle.eulerAngles.y;
        moveAngle_Y_local = moveAngle_Y - this.GetComponent<MouseLook>().playerAngle;

        //if (currentSpeed > 0f && moveAngle_Y_local > 0f && moveAngle_Y_local < 45f || moveAngle_Y_local > 315f) //入力が不安定なので不採用

            if (grounded && Input.GetAxis("Vertical") > 0f)
                animator.SetBool("moveFront", true);
            else
                animator.SetBool("moveFront", false);

            if (grounded && Input.GetAxis("Vertical") < 0f)
                animator.SetBool("moveBack", true);
            else
                animator.SetBool("moveBack", false);

            if (grounded && Input.GetAxis("Horizontal") > 0f)
                animator.SetBool("moveLeft", true);
            else
                animator.SetBool("moveLeft", false);

            if (grounded && Input.GetAxis("Horizontal") < 0f)
                animator.SetBool("moveRight", true);
            else
                animator.SetBool("moveRight", false);
    }

    void MoveMotion()
    {
        if (currentSpeed > 0f)
        animator.SetFloat("Angle", moveAngle_Y_local);
    }

    void WalkingSound()
    {
        if (_walkSound && _runSound)
        {
            if (RayCastGrounded())
            { //for walk sounsd using this because suraface is not straigh			
                if (currentSpeed > 1)
                {
                    //				print ("unutra sam");
                    if (maxSpeed == 3)
                    {
                        //	print ("tu sem");
                        if (!_walkSound.isPlaying)
                        {
                            //	print ("playam hod");
                            _walkSound.Play();
                            _runSound.Stop();
                        }
                    }
                    else if (maxSpeed == 5)
                    {
                        //	print ("NE tu sem");

                        if (!_runSound.isPlaying)
                        {
                            _walkSound.Stop();
                            _runSound.Play();
                        }
                    }
                }
                else
                {
                    _walkSound.Stop();
                    _runSound.Stop();
                }
            }
            else
            {
                _walkSound.Stop();
                _runSound.Stop();
            }
        }
        else
        {
            print("Missing walk and running sounds.");
        }

    }

    /*
* Raycasts down to check if we are grounded along the gorunded method() because if the
* floor is curvy it will go ON/OFF constatly this assures us if we are really grounded
*/
    private bool RayCastGrounded()
    {

        RaycastHit groundedInfo;
        if (Physics.Raycast(transform.position, transform.up * -1f, out groundedInfo, 1, ~ignoreLayer))
        {
            Debug.DrawRay(transform.position, transform.up * -1f, Color.red, 0.0f);
            if (groundedInfo.transform != null)
            {
                //print ("vracam true");
                return true;
            }
            else
            {
                //print ("vracam false");
                return false;
            }
        }
        //print ("nisam if dosao");

        return false;
    }

    void OnCollisionStay(Collision other)
    {
        foreach (ContactPoint contact in other.contacts)
        {
            if (Vector2.Angle(contact.normal, Vector3.up) < 60)
            {
                grounded = true;
            }
        }
    }

    void OnCollisionExit()
    {
        grounded = false;
    }

    void Sprint()
    {// Running();  so i can find it with CTRL + F
        if (Input.GetAxis("Vertical") > 0 && Input.GetAxisRaw("Fire2") == 0 && Input.GetAxisRaw("Fire1") == 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (maxSpeed == walkingSpeed)
                {
                    maxSpeed = runningSpeed;
                }
                else
                {
                    maxSpeed = walkingSpeed;
                }
            }
        }
        else
        {
            maxSpeed = walkingSpeed;
        }
    }


        void Crouching()
    {
        if (Input.GetKey(KeyCode.C))
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 0.6f, 1), Time.deltaTime * 15);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 1, 1), Time.deltaTime * 15);

        }
    }

}
