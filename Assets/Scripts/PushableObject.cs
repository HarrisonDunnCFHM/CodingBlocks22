using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PushableObject : MonoBehaviour
{
    enum ObjectDirection { Up, Down, Left, Right, None};

    //config params
    [SerializeField] bool immovable;
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float moveSnapThreshold = 0.001f;
    [SerializeField] float pushDistance = 1f;
    [SerializeField] float upperYBound = 2.5f;
    [SerializeField] float lowerYBound = -5.5f;
    [SerializeField] ParticleSystem myDust;
    public bool pushable;

    //cached refs
    ObjectDirection directionFromPlayerToObj;
    Vector3 targetPosition;
    Player player;
    bool pushed;
    List<PushableObject> pushableObjects;
    List<Pickup> pickups;
    Tilemap hazards;
    AudioManager audioManager;
    GameData gameData;
    Shadows shadow;

    // Start is called before the first frame update
    void Start()
    {
        targetPosition = transform.position;
        player = FindObjectOfType<Player>();
        pushed = false;
        pushableObjects = new List<PushableObject>(FindObjectsOfType<PushableObject>());
        pickups = new List<Pickup>(FindObjectsOfType<Pickup>());
        hazards = FindObjectOfType<Tilemap>();
        audioManager = FindObjectOfType<AudioManager>();
        gameData = FindObjectOfType<GameData>();
        shadow = FindObjectOfType<Shadows>();
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
        SinkIntoHazard();
    }

    private void CheckPlayerDirection()
    {
        Vector2 dirToPlayer = (player.transform.position - transform.position);
        Vector2 dirToPlayerNormalized = dirToPlayer.normalized;
        if (dirToPlayerNormalized == Vector2.up)
        {
            directionFromPlayerToObj = ObjectDirection.Up;
        }
        else if (dirToPlayerNormalized == Vector2.down)
        {
            directionFromPlayerToObj = ObjectDirection.Down;
        }
        else if (dirToPlayerNormalized == Vector2.left)
        {
            directionFromPlayerToObj = ObjectDirection.Left;
        }
        else if (dirToPlayerNormalized == Vector2.right)
        {
            directionFromPlayerToObj = ObjectDirection.Right;
        }
    }

    private void CheckIfPushable()
    {
        if (targetPosition == transform.position)
        {
            Vector3 checkVector;
            switch (directionFromPlayerToObj)
            {
                case ObjectDirection.Up:
                    checkVector = new Vector3(0f, -1f, 0f);
                    pushable = CheckForObstructions(checkVector);
                    break;
                case ObjectDirection.Down:
                    checkVector = new Vector3(0f, 1f, 0f);
                    pushable = CheckForObstructions(checkVector);
                    break;
                case ObjectDirection.Right:
                    checkVector = new Vector3(-1f, 0f, 0f);
                    pushable = CheckForObstructions(checkVector);
                    break;
                case ObjectDirection.Left:
                    checkVector = new Vector3(1f, 0f, 0f);
                    pushable = CheckForObstructions(checkVector);
                    break;
                case ObjectDirection.None:
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
        if (transform.position.y + pushDirection.y > upperYBound || transform.position.y + pushDirection.y < lowerYBound )
        {
            return false;
        }
        foreach (PushableObject pushable in pushableObjects)
        {
            if (pushable != null)
            {
                if (!gameData.unlockedFloat && hazards.HasTile(Vector3Int.RoundToInt(transform.position + pushDirection))) { return false; }
                if (transform.position + pushDirection == pushable.transform.position)
                {
                    return false;
                }
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
        if(transform.position + pushDirection == shadow.transform.position)
        {
            return false;
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
            case ObjectDirection.Up:
                direction = Vector2.down;
                break;
            case ObjectDirection.Down:
                direction = Vector2.up;
                break;
            case ObjectDirection.Right:
                direction = Vector2.left;
                break;
            case ObjectDirection.Left:
                direction = Vector2.right;
                break;
            case ObjectDirection.None:
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
        directionFromPlayerToObj = ObjectDirection.None;
        //pushed = false;
    }

    private void SinkIntoHazard()
    {
        if(hazards.HasTile(Vector3Int.RoundToInt(transform.position)))
        {
            hazards.SetTile(Vector3Int.RoundToInt(transform.position),null);
            Destroy(gameObject);
        }
    }

}
