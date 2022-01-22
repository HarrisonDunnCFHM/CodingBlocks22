using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : MonoBehaviour
{
    public enum Direction { Up, Down, Left, Right, None};

    //config params
    [SerializeField] bool immovable;
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float moveSnapThreshold = 0.001f;
    [SerializeField] float pushDistance = 1f;
    [SerializeField] float upperYBound = 2.5f;
    [SerializeField] float lowerYBound = -5.5f;
    [SerializeField] ParticleSystem myDust;

    //cached refs
    Direction directionFromPlayerToObj;
    Vector3 targetPosition;
    Player player;
    bool pushed;
    public bool pushable;
    List<PushableObject> pushableObjects;
    List<Pickup> pickups;

    // Start is called before the first frame update
    void Start()
    {
        targetPosition = transform.position;
        player = FindObjectOfType<Player>();
        pushed = false;
        pushableObjects = new List<PushableObject>(FindObjectsOfType<PushableObject>());
        pickups = new List<Pickup>(FindObjectsOfType<Pickup>());
    }

    // Update is called once per frame
    void Update()
    {
        if (immovable) { pushable = false; return; }
        CheckPlayerDirection(); //check for direction of this object relative to player
        CheckIfPushable(); //check if there are boxes directly behind this object relative to player
        if (!pushable) { return; }
        PushedByPlayer(); //check if box is pushed
        MoveWhenPushed(); //actually move box
    }

    private void CheckPlayerDirection()
    {
        Vector2 dirToPlayer = (player.transform.position - transform.position);
        Vector2 dirToPlayerNormalized = dirToPlayer.normalized;
        if (dirToPlayerNormalized == Vector2.up)
        {
            directionFromPlayerToObj = Direction.Up;
        }
        else if (dirToPlayerNormalized == Vector2.down)
        {
            directionFromPlayerToObj = Direction.Down;
        }
        else if (dirToPlayerNormalized == Vector2.left)
        {
            directionFromPlayerToObj = Direction.Left;
        }
        else if (dirToPlayerNormalized == Vector2.right)
        {
            directionFromPlayerToObj = Direction.Right;
        }
    }

    private void CheckIfPushable()
    {
        if (targetPosition == transform.position)
        {
            Vector3 checkVector;
            switch (directionFromPlayerToObj)
            {
                case Direction.Up:
                    checkVector = new Vector3(0f, -1f, 0f);
                    pushable = CheckForObstructions(checkVector);
                    break;
                case Direction.Down:
                    checkVector = new Vector3(0f, 1f, 0f);
                    pushable = CheckForObstructions(checkVector);
                    break;
                case Direction.Right:
                    checkVector = new Vector3(-1f, 0f, 0f);
                    pushable = CheckForObstructions(checkVector);
                    break;
                case Direction.Left:
                    checkVector = new Vector3(1f, 0f, 0f);
                    pushable = CheckForObstructions(checkVector);
                    break;
                case Direction.None:
                    checkVector = new Vector3(0f, 0f, 0f);
                    pushable = CheckForObstructions(checkVector);
                    break;
                default:
                    break;
            }
        }
    }

    private bool CheckForObstructions(Vector3 pushDirection)
    {
        if (transform.position.y + pushDirection.y > upperYBound || transform.position.y + pushDirection.y < lowerYBound)
        {
            return false;
        }
        foreach (PushableObject pushable in pushableObjects)
        {
            if (transform.position + pushDirection == pushable.transform.position)
            {
                return false;
            }
        }
        foreach (Pickup pickup in pickups)
        {
            if (pickup != null)
            {
                if (transform.position + pushDirection == pickup.transform.position)
                {
                    return false;
                }
            }
        }
        return true;
    }
    private void PushedByPlayer()
    {
        Vector2 dirToPlayer = (player.transform.position - transform.position);
        float distToPlayer = Vector2.SqrMagnitude(dirToPlayer);
        if (distToPlayer < pushDistance)
        {
            pushed = true;
        }
    }
    private void MoveWhenPushed()
    {
        Vector2 direction;
        switch (directionFromPlayerToObj)
        {
            case Direction.Up:
                direction = Vector2.down;
                break;
            case Direction.Down:
                direction = Vector2.up;
                break;
            case Direction.Right:
                direction = Vector2.left;
                break;
            case Direction.Left:
                direction = Vector2.right;
                break;
            case Direction.None:
                direction = new Vector2(0f, 0f);
                break;
            default:
                direction = new Vector2 (0f, 0f);
                break;
        }
        if (pushed)
        {
            targetPosition = transform.position + (Vector3)direction;
            if (targetPosition.y > upperYBound || targetPosition.y < lowerYBound || !CheckForObstructions(direction))
            {
                targetPosition = transform.position;
                if(!myDust.isPlaying)
            {
                myDust.Play();
            }
            }
            pushed = false;
        }
        if (targetPosition != transform.position)
        {
            if(!myDust.isPlaying)
            {
                myDust.Play();
            }
            transform.Translate(direction.x * Time.deltaTime * moveSpeed, direction.y * Time.deltaTime * moveSpeed, 0);
            var distanceMag = Vector2.SqrMagnitude(targetPosition - transform.position);
            if (distanceMag < moveSnapThreshold)
            {
                SnapToGrid();
            }
        }
    }
    private void SnapToGrid()
    {
        int newX = Mathf.RoundToInt(gameObject.transform.position.x);
        int newY = Mathf.RoundToInt(gameObject.transform.position.y);
        transform.position = new Vector2(newX, newY);
        directionFromPlayerToObj = Direction.None;
        //pushed = false;
    }


}
