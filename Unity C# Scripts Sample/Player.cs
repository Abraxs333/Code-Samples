using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    private CharacterController _controller;

    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private float _gravity = 1.0f;
    [SerializeField]
    private float _jumpHeight = 15.0f;

    private float _yVelocity=0;
    private bool _2JumpAvailable=false;
    private UIManager _UIManager;

    private Animator _Animator;

    private float lastinput=1;

    GameObject Model;
    private bool NeedtoTurnLeft = false;
    private bool NeedtoTurnRight=false;

    private Soundeffects _Soundeffects;
    private bool wasgrounded = true;
    private bool nowgrounded = true;

    private float Idlestart=0;
    private float timeIdle=0;

    private bool _idleactivated;

    private bool _isGameOver = false;

    private GameManager _GameManager;



    // Start is called before the first frame update
    void Start()
    {
        Idlestart = Time.timeSinceLevelLoad;
        _yVelocity = 0;
        _controller = GetComponent<CharacterController>();
        _UIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        _Animator = GetComponent<Animator>();
        _Soundeffects = GameObject.Find("SoundManager").GetComponent<Soundeffects>();
        Model = GameObject.FindGameObjectWithTag("Model");
        _GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }



    // Update is called once per frame
    void Update()
    {
        if (_isGameOver)
        {
            return;
        }

        nowgrounded = _controller.isGrounded;
        if (_controller.velocity.x != 0)
        {
            Idlestart = Time.timeSinceLevelLoad;
            _idleactivated = false;
        }
        timeIdle = Time.timeSinceLevelLoad - Idlestart;
        //Debug.Log("Time Idle; "+timeIdle);
        if (timeIdle > 3&& _idleactivated==false)
        {
            _Soundeffects.Play("Idle");
            _Animator.SetTrigger("Idle");
            _idleactivated = true;
        }

        float HorizontalInput = CrossPlatformInputManager.GetAxis("Horizontal");
        Vector3 direction = new Vector3(HorizontalInput, 0, 0);
        Vector3 velocity = direction * _speed;
        _Animator.SetInteger("Speed", (int)(_speed*HorizontalInput));
        if (HorizontalInput != 0 )
        {
            if( HorizontalInput>0 && lastinput < 0)
            {
               // _Animator.SetTrigger("ChangeDir");
                Debug.Log("Turning Right");
                NeedtoTurnRight=true;
                NeedtoTurnLeft = false;
                //Model.transform.Rotate(new Vector3(0, -540, 0));
            }
            if (HorizontalInput < 0 && lastinput > 0)
            {
               // _Animator.SetTrigger("ChangeDir");
                Debug.Log("Turning Left");
                NeedtoTurnRight = false;
                NeedtoTurnLeft = true;
                //Model.transform.Rotate(0, 540, 0);
            }
        }
       // Debug.Log("WASGROUNDED=" + wasgrounded);

        if (nowgrounded==true)
        {
            if (!wasgrounded)
            {
               // Debug.Log("HITGROUND");
                _Soundeffects.Play("Land");
            }

            //Debug.Log("Grounded");
           
            _Animator.SetBool("IsGrounded", true);

           // Debug.Log(Model.transform.rotation.y);
            if (!_Animator.GetCurrentAnimatorStateInfo(0).IsName("DoubleJump"))
            {
                if (NeedtoTurnLeft )
                {
                    Model.transform.Rotate(0, 540, 0);
                    NeedtoTurnRight = false;
                    NeedtoTurnLeft = false;
                }
                if (NeedtoTurnRight  )
                {
                    Model.transform.Rotate(0, -540, 0);
                    NeedtoTurnLeft = false;
                    NeedtoTurnRight = false;
                }
            }
        }

            

        if (nowgrounded == true)
        {
            if (CrossPlatformInputManager.GetButtonDown("Jump")){
                _Animator.SetTrigger("Jump");
                _yVelocity = _jumpHeight;
                _2JumpAvailable = true;
                _Soundeffects.Play("Jump");
            }
        }
        else {
            if (_2JumpAvailable && CrossPlatformInputManager.GetButtonDown("Jump"))
            {
                _Animator.SetTrigger("DoubleJump");
                _yVelocity = _jumpHeight;
                _2JumpAvailable = false;
                _Soundeffects.Play("DoubleJump");
            }
            _yVelocity -= _gravity;
        }

        velocity.y = _yVelocity;
        _controller.Move(velocity * Time.deltaTime);
        if (HorizontalInput !=0 ) {
            lastinput = HorizontalInput;
        }
        wasgrounded = nowgrounded;
    }
    private void LateUpdate()
    {
        
    }

    public void ObtainedCollectable(string tag)
    {
        if (_isGameOver)
        {
            return;
        }
        switch (tag)
        {
            case "Coin":
                _Soundeffects.Play("Collectable");
                //_UIManager.UpdateScore(10);
                break;
            default:
                break;
        }
    }

    public void MisileHit()
    {
        if (_isGameOver)
        {
            return;
        }
        _Soundeffects.Play("Explosion");
        
        _Animator.SetTrigger("Death");
        _Soundeffects.Play("Damage");
        _isGameOver = true;
        _UIManager.GameOver();
        _GameManager.GameOVer();



        //_Animator.enabled = false;
        //_controller.enabled = false;
        //this.enabled = false;
    }


}
