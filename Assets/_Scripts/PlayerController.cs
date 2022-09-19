using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerSegment { Top, Right, Bottom, Left }

public class PlayerController : MonoBehaviour
{    
    private KeyCode currentKey = KeyCode.None;

    private Rigidbody playerRb;

    [SerializeField, Range(0, 50)]
    private float moveSpeed;

    private Dictionary<PlayerSegment, ObjectColor> segmentColors;

    // Start is called before the first frame update
    void Start()
    {
        this.segmentColors = new Dictionary<PlayerSegment, ObjectColor>();
        this.segmentColors.Add(PlayerSegment.Top, ObjectColor.Blue);
        this.segmentColors.Add(PlayerSegment.Right, ObjectColor.Grey);
        this.segmentColors.Add(PlayerSegment.Bottom, ObjectColor.Red);
        this.segmentColors.Add(PlayerSegment.Left, ObjectColor.Purple);

        this.playerRb = GetComponent<Rigidbody>();
    }

    private void RotatePlayerClockwise()
    {
        ObjectColor newTop = (ObjectColor)((int)(this.segmentColors[PlayerSegment.Left] + 1) % 4);
        ObjectColor newRight = (ObjectColor)((int)(this.segmentColors[PlayerSegment.Top] + 1) % 4);
        ObjectColor newBottom = (ObjectColor)((int)(this.segmentColors[PlayerSegment.Right] + 1) % 4);
        ObjectColor newLeft = (ObjectColor)((int)(this.segmentColors[PlayerSegment.Bottom] + 1) % 4);

        this.segmentColors[PlayerSegment.Top] = newTop;
        this.segmentColors[PlayerSegment.Right] = newRight;
        this.segmentColors[PlayerSegment.Bottom] = newBottom;
        this.segmentColors[PlayerSegment.Left] = newLeft;

        //Play animation here
    }

    private void RotatePlayerCounterClockwise()
    {
        ObjectColor newTop = (ObjectColor)((int)(this.segmentColors[PlayerSegment.Right] + 3) % 4);
        ObjectColor newRight = (ObjectColor)((int)(this.segmentColors[PlayerSegment.Bottom] + 3) % 4);
        ObjectColor newBottom = (ObjectColor)((int)(this.segmentColors[PlayerSegment.Left] + 3) % 4);
        ObjectColor newLeft = (ObjectColor)((int)(this.segmentColors[PlayerSegment.Top] + 3) % 4);

        this.segmentColors[PlayerSegment.Top] = newTop;
        this.segmentColors[PlayerSegment.Right] = newRight;
        this.segmentColors[PlayerSegment.Bottom] = newBottom;
        this.segmentColors[PlayerSegment.Left] = newLeft;

        //Play animation here
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
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            return KeyCode.Space;
        }

        //Next, prioritize the current key being pressed if it is still being held
        if (Input.GetKey(this.currentKey))
        {
            return this.currentKey;
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
        this.currentKey = this.GetInput();
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
        
        if (this.currentKey != KeyCode.None)
        {
            if (this.currentKey != KeyCode.Space)
            {
                this.AttemptMovePlayer();
            }
            else
            {
                this.AttemptJumpPlayer();
            }
        }
    }

    private bool IsValidMove()
    {
        //Up and down inputs are only valid if the left or right segment
        //is colliding with a matching color platform
        if (this.currentKey == KeyCode.W || this.currentKey == KeyCode.S)
        {
            return (CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Left]) ||
                    CollisionObserver.ObjectIsCollidingWithMatchingPlatform(this.segmentColors[PlayerSegment.Right]));
        }

        //Left and right inputs are always valid
        return true;
    }

    private void AttemptMovePlayer()
    {
        Vector3 finalDirection = Vector3.zero;

        if (this.IsValidMove() == true)
        {
            switch (this.currentKey)
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
        Vector3 targetDestination = this.playerRb.position + (direction * this.moveSpeed * Time.fixedDeltaTime);
        this.playerRb.MovePosition(targetDestination);
    }

    private void AttemptJumpPlayer()
    { 
        
    }
}
