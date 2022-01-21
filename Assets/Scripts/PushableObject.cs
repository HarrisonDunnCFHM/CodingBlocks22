using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : MonoBehaviour
{
    public enum Direction { Up, Down, Left, Right, None};

    //config params
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float moveSnapThreshold = 0.001f;
    [SerializeField] float pushDistance = 1f;

    //cached refs
    public Direction pushedFrom;
    public Vector3 targetPosition;
    PlayerMovement player;
    bool pushed;

    // Start is called before the first frame update
    void Start()
    {
        targetPosition = transform.position;
        player = FindObjectOfType<PlayerMovement>();
        pushed = false;
    }

    // Update is called once per frame
    void Update()
    {
        PushedByPlayer();
        MoveWhenPushed();
    }

    private void MoveWhenPushed()
    {
        Vector2 direction;
        switch (pushedFrom)
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
            pushed = false;
        }
        if (targetPosition != transform.position)
        {
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
        pushedFrom = Direction.None;
    }

    private void PushedByPlayer()
    {
        Vector2 dirToPlayer =  (player.transform.position - transform.position);
        float distToPlayer = Vector2.SqrMagnitude(dirToPlayer);
        Vector2 dirToPlayerNormalized = dirToPlayer.normalized;
        if (distToPlayer < pushDistance)
        {
            pushed = true;
            if (dirToPlayerNormalized == Vector2.up)
            {
                pushedFrom = Direction.Up;
            }
            else if (dirToPlayerNormalized == Vector2.down)
            {
                pushedFrom = Direction.Down;
            }
            else if (dirToPlayerNormalized == Vector2.left)
            {
                pushedFrom = Direction.Left;
            }
            else if (dirToPlayerNormalized == Vector2.right)
            {
                pushedFrom = Direction.Right;
            }
        }
    }
}
