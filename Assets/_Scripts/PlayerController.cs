using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerSegment { Top, Right, Bottom, Left }

public class PlayerController : MonoBehaviour
{    
    public Rigidbody playerRb;

    [SerializeField, Range(0, 50)]
    public float moveSpeed;

    [SerializeField, Range(0, 30)]
    private float initialJumpForce = 0f;
    [SerializeField, Range(0, 100)]
    private float residualJumpForce = 0f;
    [SerializeField]
    private float residualJumpTime = 0f;

    private Coroutine residualJumpCoroutine;

    [SerializeField, Range(0, 30)]
    private float nudgeForce = 0f;

    private Dictionary<PlayerSegment, ObjectColor> segmentColors;

    private bool jumpNextFrame = false;

    private bool isRotating = false;

    private Animator playerAnimator;

    private Vector3 moveVector;

    private AudioObserver audioObserver;

    public void PrintStatus()
    {
        Debug.LogError("Top: " + this.segmentColors[PlayerSegment.Top] +
                       "\nRight: " + this.segmentColors[PlayerSegment.Right] +
                       "\nBottom: " + this.segmentColors[PlayerSegment.Bottom] +
                       "\nLeft: " + this.segmentColors[PlayerSegment.Left]);
    }

    private void Awake()
    {
        CollisionObserver.SetupCollisionObserver();

        this.audioObserver = GameObject.Find("AudioObserver").GetComponent<AudioObserver>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.segmentColors = new Dictionary<PlayerSegment, ObjectColor>();
        this.segmentColors.Add(PlayerSegment.Top, ObjectColor.Blue);
        this.segmentColors.Add(PlayerSegment.Right, ObjectColor.Grey);
        this.segmentColors.Add(PlayerSegment.Bottom, ObjectColor.Red);
        this.segmentColors.Add(PlayerSegment.Left, ObjectColor.Purple);

        this.playerRb = GetComponent<Rigidbody>();
        this.playerAnimator = GetComponent<Animator>();
    }

    private void RotatePlayer(bool clockwise)
    {
        this.isRotating = true;

        //Only rotate the player if they are currently in an "idle" state
        if (this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("IdleRedBottom") ||
            this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("IdleGreyBottom") ||
            this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("IdleBlueBottom") ||
            this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("IdlePurpleBottom"))
        {
            int incrementer = 0;

            if (clockwise == true)
            {
                this.playerAnimator.SetTrigger("RotateClockwise");
                incrementer = 3;
            }
            else
            {
                this.playerAnimator.SetTrigger("RotateCounterClockwise");
                incrementer = 1;
            }

            ObjectColor newTop = (ObjectColor)((int)(this.segmentColors[PlayerSegment.Top] + incrementer) % 4);
            ObjectColor newRight = (ObjectColor)((int)(this.segmentColors[PlayerSegment.Right] + incrementer) % 4);
            ObjectColor newBottom = (ObjectColor)((int)(this.segmentColors[PlayerSegment.Bottom] + incrementer) % 4);
            ObjectColor newLeft = (ObjectColor)((int)(this.segmentColors[PlayerSegment.Left] + incrementer) % 4);

            //Debug.LogError("NewTop: " + newTop + "\nNewRight: " + newRight + "\nNewBottom: " + newBottom + "\nNewLeft: " + newLeft);

            this.segmentColors[PlayerSegment.Top] = newTop;
            this.segmentColors[PlayerSegment.Right] = newRight;
            this.segmentColors[PlayerSegment.Bottom] = newBottom;
            this.segmentColors[PlayerSegment.Left] = newLeft;

            this.audioObserver.NotifyAudioTrigger(AudioTrigger.Rotate);
        }
    }

    private void UpdateMoveVector()
    {
        this.moveVector = Vector3.zero;
        
        //First, prioritize the latest new key press
        if (Input.GetKey(KeyCode.W) && this.IsValidMove(KeyCode.W))
        {
            this.moveVector = new Vector3(this.moveVector.x, 1.0f, 0.0f);
        }
        if (Input.GetKey(KeyCode.D) && this.IsValidMove(KeyCode.D))
        {
            this.moveVector = new Vector3(1.0f, this.moveVector.y, 0.0f);
        }
        if (Input.GetKey(KeyCode.S) && this.IsValidMove(KeyCode.S))
        {
            this.moveVector = new Vector3(this.moveVector.x, -1.0f, 0.0f);
        }
        if (Input.GetKey(KeyCode.A) && this.IsValidMove(KeyCode.A))
        {
            this.moveVector = new Vector3(-1.0f, this.moveVector.y, 0.0f);
        }

        this.moveVector.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
        
        this.UpdateMoveVector();

        this.UpdateGravity();

        if (this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("IdleRedBottom") ||
            this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("IdleGreyBottom") ||
            this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("IdleBlueBottom") ||
            this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("IdlePurpleBottom"))
        {
            this.isRotating = false;
        }

            if (Input.GetKeyDown(KeyCode.Space))
        {
            this.jumpNextFrame = true;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.RotatePlayer(true);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.RotatePlayer(false);
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            CollisionObserver.PrintDicts();
            this.PrintStatus();
        }
    }

    private void UpdateGravity()
    {
        //Disable gravity if the player is colliding with any matching colors
        if (CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Top]) == true ||
            CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Right]) == true ||
            CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Bottom]) == true ||
            CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Left]) == true)
        {
            this.playerRb.useGravity = false;
            return;
        }

        this.playerRb.useGravity = true;
    }

    private void FixedUpdate()
    {
        this.MovePlayer();

        if (this.jumpNextFrame == true)
        {
            this.AttemptJumpPlayer();
            this.jumpNextFrame = false;
        }
    }

    private bool IsValidMove(KeyCode currentKey)
    {
        //Up and down inputs are only valid if the left or right segment
        //is colliding with a matching color platform
        if (currentKey == KeyCode.W || currentKey == KeyCode.S)
        {
            return (CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Left]) ||
                    CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Right]));
        }

        //Left and right inputs are only valid if the top or bottom segment
        //is colliding with a matching color platform
        //OR the player is currently in the air/on the ground
        if (currentKey == KeyCode.A || currentKey == KeyCode.D)
        {
            return (CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Top]) ||
                    CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Bottom]) ||
                    CollisionObserver.IsMatchingCollisionFree());
        }

        return false;
    }

    private void MovePlayer()
    {
        //Immediately halt any current velocity if player changes direction in mid-air
        if (CollisionObserver.IsCollisionFree() == true && (this.moveVector.x != 0.0f))
        {
            if ((this.moveVector.x > 0 && this.playerRb.velocity.x < 0) ||
                (this.moveVector.x < 0 && this.playerRb.velocity.x > 0))
            {
                playerRb.velocity = new Vector3(0.0f, playerRb.velocity.y, 0.0f);
            }
        }

        Vector3 targetDestination = this.playerRb.position + (this.moveVector * this.moveSpeed * Time.fixedDeltaTime);
        this.playerRb.MovePosition(targetDestination);
    }

    private void AttemptJumpPlayer()
    {
        if (CollisionObserver.IsCollisionFree() == true)
        {
            return;
        }

        if (CollisionObserver.ObjectIsCollidingWithAnyPlatform(this.segmentColors[PlayerSegment.Bottom]))
        {
            this.JumpPlayer(Vector3.up);
        }
        else if (CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Top]))
        {
            this.JumpPlayer(Vector3.down);
        }
        else if (CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Right]))
        {
            if (this.moveVector.y > 0)
            {
                this.JumpPlayer(new Vector3(-1.0f, 1.0f, 0.0f).normalized);
            }
            else if (this.moveVector.y < 0)
            {
                this.JumpPlayer(new Vector3(-1.0f, -1.0f, 0.0f).normalized);
            }
            else
            {
                this.JumpPlayer(Vector3.left);
            }
        }
        else if (CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Left]))
        {
            if (this.moveVector.y > 0)
            {
                this.JumpPlayer(new Vector3(1.0f, 1.0f, 0.0f).normalized);
            }
            else if (this.moveVector.y < 0)
            {
                this.JumpPlayer(new Vector3(1.0f, -1.0f, 0.0f).normalized);
            }
            else
            {
                this.JumpPlayer(Vector3.right);
            }
        }
    }

    private void JumpPlayer(Vector3 direction)
    {
        this.playerRb.AddForce(direction * this.initialJumpForce, ForceMode.Impulse);        

        if (direction == Vector3.up)
        {
            if (this.residualJumpCoroutine != null)
            {
                this.StopCoroutine(this.residualJumpCoroutine);
            }

            this.residualJumpCoroutine = this.StartCoroutine(this.ResidualJump());
        }

        this.audioObserver.NotifyAudioTrigger(AudioTrigger.Jump);
    }

    private IEnumerator ResidualJump()
    {
        float jumpTime = 0.0f;        

        while (jumpTime < this.residualJumpTime && Input.GetKey(KeyCode.Space) &&
                CollisionObserver.ObjectIsCollidingWithAnyPlatform(this.segmentColors[PlayerSegment.Top]) == false &&
                CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Left]) == false &&
                CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Right]) == false)
        {
            this.ApplyResidualJump();
            jumpTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        this.playerRb.velocity = Vector3.zero;
        this.residualJumpCoroutine = null;
    }

    private void ApplyResidualJump()
    {
        this.playerRb.AddForce(Vector3.up * this.residualJumpForce, ForceMode.Force);        
    }

    public void AttemptNudgePlayer()
    {
        if (CollisionObserver.ObjectIsCollidingWithAnyPlatform(this.segmentColors[PlayerSegment.Top]) ||
            CollisionObserver.ObjectIsCollidingWithAnyPlatform(this.segmentColors[PlayerSegment.Bottom]) ||
            this.isRotating == true)
        {
            return;
        }

            if (CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Left]))
        {
            this.NudgePlayer(PlayerSegment.Left);
        }
        else if (CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Right]))
        {
            this.NudgePlayer(PlayerSegment.Right);
        }
    }

    private void NudgePlayer(PlayerSegment nudgedSegment)
    {
        if (nudgedSegment == PlayerSegment.Right)
        {
            if (this.moveVector.y > 0)
            {
                this.playerRb.AddForce(new Vector3(-1.0f, 2.0f, 0.0f).normalized * this.nudgeForce, ForceMode.Impulse);
            }
            else if (this.moveVector.y < 0)
            {
                this.playerRb.AddForce(new Vector3(-1.0f, -2.0f, 0.0f).normalized * this.nudgeForce, ForceMode.Impulse);
            }            
        }
        else if (nudgedSegment == PlayerSegment.Left)
        {
            if (this.moveVector.y > 0)
            {
                this.playerRb.AddForce(new Vector3(1.0f, 2.0f, 0.0f).normalized * this.nudgeForce, ForceMode.Impulse);
            }
            else if (this.moveVector.y < 0)
            {
                this.playerRb.AddForce(new Vector3(1.0f, -2.0f, 0.0f).normalized * this.nudgeForce, ForceMode.Impulse);
            }
        }
    }
}
