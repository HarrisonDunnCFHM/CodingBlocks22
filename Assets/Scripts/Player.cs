using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    enum Direction { Up, Down, Left, Right };
    //config params
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float moveSnapThreshold = 1f;
    [SerializeField] float upperYBound = 2.5f;
    [SerializeField] float lowerYBound = -5.5f;
    [SerializeField] float upperXBound;
    [SerializeField] float lowerXBound;

    //cached refs
    public Vector3 targetPosition;
    [SerializeField] Animator myAnimator;
    Direction myDirection;
    [SerializeField] SpriteRenderer mySprite;
    bool blocked;
    bool pushing;
    public bool movementDisabled;
    bool jumping;
    bool canJump;
    List<PushableObject> pushables;
    Tilemap hazards;
    LevelManager levelManager;


    // Start is called before the first frame update
    void Start()
    {
        //myAnimator = GetComponent<Animator>();
        myDirection = Direction.Down;
        //mySprite = GetComponent<SpriteRenderer>();
        blocked = false;
        pushing = false;
        movementDisabled = false;
        targetPosition = transform.position;
        pushables = new List<PushableObject>(FindObjectsOfType<PushableObject>());
        hazards = FindObjectOfType<Tilemap>();
        levelManager = FindObjectOfType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        blocked = false;
        ProcessInputs();
        AnimatePlayer();
        MovePlayer();
        CheckForHazard();
    }

    private void AnimatePlayer()
    {
        switch (myDirection)
        {
            case Direction.Down:
                mySprite.flipX = false;
                if (transform.position != targetPosition)
                {
                    if (jumping)
                    {
                        myAnimator.Play("Player Jump Down");
                    }
                    else if (pushing)
                    { 
                        myAnimator.Play("Player Interact Down");
                        pushing = false;
                    }
                    else
                    {
                        myAnimator.Play("Player Walk Down");
                    }
               
                }
                else
                {
                    myAnimator.Play("Player Idle Down");
                }
                break;
            case Direction.Up:
                mySprite.flipX = false;
                if (transform.position != targetPosition)
                {
                    if (jumping)
                    {
                        myAnimator.Play("Player Jump Up");
                    }
                    else if(pushing)
                    {
                        myAnimator.Play("Player Interact Up");
                        pushing = false;
                    }
                    else
                    {
                        myAnimator.Play("Player Walk Up");
                    }
                }
                else
                {
                    myAnimator.Play("Player Idle Up");
                }
                break;
            case Direction.Left:
                mySprite.flipX = true;
                if (transform.position != targetPosition)
                {
                    if (jumping)
                    {
                        myAnimator.Play("Player Jump Right");
                    }
                    else if (pushing)
                    {
                        myAnimator.Play("Player Interact Right");
                        pushing = false;
                    }
                    else
                    {
                        myAnimator.Play("Player Walk Right");
                    }
                }
                else
                {
                    myAnimator.Play("Player Idle Right");
                }
                break;
            case Direction.Right:
                mySprite.flipX = false;
                if (transform.position != targetPosition)
                {
                    if (jumping)
                    {
                        myAnimator.Play("Player Jump Right");
                    }
                    else if (pushing)
                    {
                        myAnimator.Play("Player Interact Right");
                        pushing = false;
                    }
                    else
                    {
                        myAnimator.Play("Player Walk Right");
                    }
                }
                else
                {
                    myAnimator.Play("Player Idle Right");
                }
                break;
            default:
                break;
        }
    }

    private void MovePlayer()
    {
        if ( transform.position != targetPosition)
        {
            CheckForObstacle();
            if (targetPosition.y > upperYBound || targetPosition.y < lowerYBound || blocked) 
            { 
                targetPosition = transform.position;
                pushing = true;
                return; 
            }
            var direction = targetPosition - transform.position;
            if (jumping)
            {
                transform.Translate(direction.normalized * moveSpeed * 2 * Time.deltaTime);
            }
            else
            {
                transform.Translate(direction.normalized * moveSpeed * Time.deltaTime);
            }
        }
        var distanceMag = Vector2.SqrMagnitude(targetPosition - transform.position );
        if (distanceMag < moveSnapThreshold)
        {
            SnapToGrid();
        }
    }

    private void CheckForObstacle()
    {
        foreach (PushableObject pushable in pushables)
        {
            
            if (jumping && targetPosition == pushable.transform.position)
            {
                blocked = true;
                pushing = true;
                jumping = false;
            }
            else if (!pushable.pushable && targetPosition == pushable.transform.position)
            {
                blocked = true;
                pushing = true;
            }
            else if (pushable.pushable && targetPosition == pushable.transform.position)
            {
                pushing = true;
            }
        }
    }

    private void SnapToGrid()
    {
        int newX = Mathf.RoundToInt(gameObject.transform.position.x);
        int newY = Mathf.RoundToInt(gameObject.transform.position.y);
        transform.position = new Vector2(newX, newY);
        targetPosition = transform.position;
        jumping = false;
        canJump = true;
    }

    private void ProcessInputs()
    {
        if (movementDisabled) { return; }

        if (Input.GetAxis("Horizontal") != 0)
        {
            canJump = false;
            if (transform.position == targetPosition)
            { 
                int direction = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
                if(direction == 1) 
                { 
                    myDirection = Direction.Right;
                    CheckForClearPath(Vector2.right);
                }
                if(direction == -1) 
                { 
                    myDirection = Direction.Left;
                    CheckForClearPath(Vector2.left);
                }
            }
        }
        if(Input.GetAxis("Vertical") != 0)
        {
            canJump = false;
            if (transform.position == targetPosition)
            {
                int direction = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
                if (direction == 1)
                {
                    myDirection = Direction.Up;
                    CheckForClearPath(Vector2.up);
                }
                if (direction == -1)
                {
                    myDirection = Direction.Down;
                    CheckForClearPath(Vector2.down);
                }
            }
        }
        if (Input.GetButtonDown("Jump"))
        {
            if (!canJump) { return; }
            Debug.Log("jumpy jump");
            canJump = false;
            jumping = true;
            switch (myDirection)
            {
                case Direction.Up:
                    CheckForClearPath(Vector2.up);
                    break;
                case Direction.Down:
                    CheckForClearPath(Vector2.down);
                    break;
                case Direction.Left:
                    CheckForClearPath(Vector2.left);
                    break;
                case Direction.Right:
                    CheckForClearPath(Vector2.right);
                    break;
                default:
                    break;
            }
        }
    }

    private void CheckForClearPath(Vector2 directionToCheck)
    {
        targetPosition = (Vector2)transform.position + directionToCheck;
        if (targetPosition.y > upperYBound || targetPosition.y < lowerYBound) { Debug.Log("out of bounds!"); targetPosition = transform.position; return; }
        foreach (PushableObject pushable in pushables)
        {
            if ((Vector2)targetPosition == (Vector2)pushable.transform.position)
            {
                if (!pushable.pushable || jumping)
                {
                    targetPosition = transform.position;
                    blocked = true;
                    pushing = true;
                    jumping = false;
                    return;
                }
            }
        }
        if (jumping)
        {
            while (hazards.HasTile(Vector3Int.RoundToInt(targetPosition)))
            {
                targetPosition = (Vector2)targetPosition + directionToCheck;
            }
            if (targetPosition.y > upperXBound || targetPosition.y < lowerYBound) { return; }
            foreach (PushableObject pushable in pushables)
            {
                if ((Vector2)targetPosition == (Vector2)pushable.transform.position)
                {
                    targetPosition = transform.position;
                    blocked = true;
                    pushing = true;
                    jumping = false;
                    return;
                }
            }
        }
    }

    private void CheckForHazard()
    {
        Vector3Int positionAsInt = Vector3Int.RoundToInt(transform.position);
        if (hazards.HasTile(positionAsInt) && !jumping)
        {
            levelManager.TriggerEnding();
        }
    }
}
