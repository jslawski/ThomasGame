using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{    
    private KeyCode currentKey = KeyCode.None;

    private Rigidbody playerRb;

    [SerializeField, Range(0, 50)]
    private float moveSpeed;



    // Start is called before the first frame update
    void Start()
    {
        this.playerRb = GetComponent<Rigidbody>();
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

    private void FixedUpdate()
    {
        if (this.currentKey != KeyCode.None)
        {
            if (this.currentKey != KeyCode.Space)
            {
                this.AttemptMovePlayer();
            }
        }
    }

    private void AttemptMovePlayer()
    {
        Vector3 finalDirection = Vector3.zero;
        


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

    private void MovePlayer(Vector3 direction)
    {
        Vector3 targetDestination = this.playerRb.position + (direction * this.moveSpeed * Time.fixedDeltaTime);
        this.playerRb.MovePosition(targetDestination);
    }
}
