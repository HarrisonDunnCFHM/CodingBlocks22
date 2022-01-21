using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    enum Direction { Up, Down, Left, Right };
    //config params
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float moveSnapThreshold = 1f;

    //cached refs
    Vector3 targetPosition;
    Animator myAnimator;
    Direction myDirection;
    SpriteRenderer mySprite;
    bool blocked;
    bool pushing;


    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myDirection = Direction.Down;
        mySprite = GetComponent<SpriteRenderer>();
        blocked = false;
        pushing = false;
    }

    // Update is called once per frame
    void Update()
    {
        blocked = false;

        ProcessInputs();
        AnimatePlayer();
        MovePlayer();
    }

    private void AnimatePlayer()
    {
        switch (myDirection)
        {
            case Direction.Down:
                if (transform.position != targetPosition)
                {
                    mySprite.flipX = false;
                    if (pushing)
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
                    mySprite.flipX = false;
                    myAnimator.Play("Player Idle Down");
                }
                break;
            case Direction.Up:
                if (transform.position != targetPosition)
                {
                    mySprite.flipX = false;
                    if (pushing)
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
                    mySprite.flipX = false;
                    myAnimator.Play("Player Idle Up");
                }
                break;
            case Direction.Left:
                if (transform.position != targetPosition)
                {
                    mySprite.flipX = true;
                    if (pushing)
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
                    mySprite.flipX = true;
                    myAnimator.Play("Player Idle Right");
                }
                break;
            case Direction.Right:
                if (transform.position != targetPosition)
                {
                    mySprite.flipX = false;
                    if (pushing)
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
                    mySprite.flipX = false;
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
            if (blocked) { targetPosition = transform.position;  return; }
            var direction = targetPosition - transform.position;
            transform.Translate(direction.normalized * moveSpeed * Time.deltaTime);
        }
        var distanceMag = Vector2.SqrMagnitude(targetPosition - transform.position );
        if (distanceMag < moveSnapThreshold)
        {
            SnapToGrid();
        }
    }

    private void CheckForObstacle()
    {
        List<PushableObject> pushables = new List<PushableObject>(FindObjectsOfType<PushableObject>());
        foreach (PushableObject pushable in pushables)
        {
            var distToPlayer = Vector2.SqrMagnitude(pushable.transform.position - transform.position);
            if (!pushable.pushable && targetPosition == pushable.transform.position)
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
    }

    private void ProcessInputs()
    {
        if (Input.GetAxis("Horizontal") != 0)
        {
            if (transform.position == targetPosition)
            { 
                int direction = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
                if(direction == 1) { myDirection = Direction.Right; }
                if(direction == -1) { myDirection = Direction.Left; }
                targetPosition = new Vector2(transform.position.x + direction, transform.position.y);
            }
        }
        if(Input.GetAxis("Vertical") != 0)
        {
            if (transform.position == targetPosition)
            {
                int direction = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
                if (direction == 1) { myDirection = Direction.Up; }
                if (direction == -1) { myDirection = Direction.Down; }
                targetPosition = new Vector2(transform.position.x, transform.position.y + direction);

            }
        }
      
    }
}
