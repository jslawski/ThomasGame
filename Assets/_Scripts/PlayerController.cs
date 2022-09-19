using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerSegment { Top, Right, Bottom, Left }

public class PlayerController : MonoBehaviour
{    
    private KeyCode currentMoveKey = KeyCode.None;

    private Rigidbody playerRb;

    [SerializeField, Range(0, 50)]
    private float moveSpeed;

    [SerializeField, Range(0, 10)]
    private float initialJumpForce = 0f;

    [SerializeField, Range(0, 5)]
    private float nudgeForce = 0f;

    private Dictionary<PlayerSegment, ObjectColor> segmentColors;

    private bool jumpNextFrame = false;

    private Animator playerAnimator;

    private void Awake()
    {
        CollisionObserver.SetupCollisionObserver();
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
    }

    private KeyCode GetInput()
    {                
        //First, prioritize the latest new key press
        if (Input.GetKeyDown(KeyCode.A))
        {
            return KeyCode.A;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            return KeyCode.D;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            return KeyCode.W;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            return KeyCode.S;
        }

        //Next, prioritize the current key being pressed if it is still being held
        if (Input.GetKey(this.currentMoveKey))
        {
            return this.currentMoveKey;
        }

        //Finally, check for any held keys
        if (Input.GetKey(KeyCode.A))
        {
            return KeyCode.A;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            return KeyCode.D;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            return KeyCode.W;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            return KeyCode.S;
        }
        
        //Return None if no valid key is being pressed
        return KeyCode.None;        
    }

    // Update is called once per frame
    void Update()
    {
        this.currentMoveKey = this.GetInput();

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
        this.UpdateGravity();
        
        if (this.currentMoveKey != KeyCode.None)
        {
            if (this.currentMoveKey != KeyCode.Space)
            {
                this.AttemptMovePlayer();
            }            
        }

        if (this.jumpNextFrame == true)
        {
            this.AttemptJumpPlayer();
            this.jumpNextFrame = false;
        }
    }

    private bool IsValidMove()
    {
        //Up and down inputs are only valid if the left or right segment
        //is colliding with a matching color platform
        if (this.currentMoveKey == KeyCode.W || this.currentMoveKey == KeyCode.S)
        {
            return (CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Left]) ||
                    CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Right]));
        }

        //Left and right inputs are only valid if the top or bottom segment
        //is colliding with a matching color platform
        //OR the player is currently in the air/on the ground
        if (this.currentMoveKey == KeyCode.A || this.currentMoveKey == KeyCode.D)
        {
            return (CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Top]) ||
                    CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Bottom]) ||
                    CollisionObserver.IsMatchingCollisionFree());
        }

        return false;
    }

    private void AttemptMovePlayer()
    {
        Vector3 finalDirection = Vector3.zero;

        if (this.IsValidMove() == true)
        {
            switch (this.currentMoveKey)
            {
                case KeyCode.A:
                    this.MovePlayer(Vector3.left);
                    break;
                case KeyCode.D:
                    this.MovePlayer(Vector3.right);
                    break;
                case KeyCode.W:
                    this.MovePlayer(Vector3.up);
                    break;
                case KeyCode.S:
                    this.MovePlayer(Vector3.down);
                    break;
                default:
                    break;
            }
        }
    }

    private void MovePlayer(Vector3 direction)
    {
        if (CollisionObserver.IsCollisionFree() == true &&
            (direction == Vector3.left || direction == Vector3.right))
        {
            playerRb.velocity = new Vector3(0.0f, playerRb.velocity.y, 0.0f);
        }

        Vector3 targetDestination = this.playerRb.position + (direction * this.moveSpeed * Time.fixedDeltaTime);
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
            if (this.currentMoveKey == KeyCode.W)
            {
                this.JumpPlayer(new Vector3(-1.0f, 1.0f, 0.0f).normalized);
            }
            else if (this.currentMoveKey == KeyCode.S)
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
            if (this.currentMoveKey == KeyCode.W)
            {
                this.JumpPlayer(new Vector3(1.0f, 1.0f, 0.0f).normalized);
            }
            else if (this.currentMoveKey == KeyCode.S)
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
    }

    public void AttemptNudgePlayer()
    {
        if (CollisionObserver.ObjectIsCollidingWithAnyPlatform(this.segmentColors[PlayerSegment.Top]) ||
            CollisionObserver.ObjectIsCollidingWithAnyPlatform(this.segmentColors[PlayerSegment.Bottom]))
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
        Debug.LogError("Nudging");

        if (nudgedSegment == PlayerSegment.Right)
        {
            if (this.currentMoveKey == KeyCode.W)
            {
                this.playerRb.AddForce(new Vector3(-1.0f, 1.0f, 0.0f).normalized * this.nudgeForce, ForceMode.Impulse);
            }
            else if (this.currentMoveKey == KeyCode.S)
            {
                this.playerRb.AddForce(new Vector3(-1.0f, -1.0f, 0.0f).normalized * this.nudgeForce, ForceMode.Impulse);
            }            
        }
        else if (nudgedSegment == PlayerSegment.Left)
        {
            if (this.currentMoveKey == KeyCode.W)
            {
                this.playerRb.AddForce(new Vector3(1.0f, 1.0f, 0.0f).normalized * this.nudgeForce, ForceMode.Impulse);
            }
            else if (this.currentMoveKey == KeyCode.S)
            {
                this.playerRb.AddForce(new Vector3(1.0f, -1.0f, 0.0f).normalized * this.nudgeForce, ForceMode.Impulse);
            }
        }
    }
}
