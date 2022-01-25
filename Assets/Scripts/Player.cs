using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public enum PlayerDirection { Up, Down, Left, Right, None };
    //config params
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float moveSnapThreshold = 1f;
    [SerializeField] float upperYBound = 2.5f;
    [SerializeField] float lowerYBound = -5.5f;
    [SerializeField] float upperXBound;
    [SerializeField] float lowerXBound;
    public bool movementDisabled;
    [SerializeField] Animator myAnimator;
    public PlayerDirection myDirection;
    [SerializeField] SpriteRenderer mySprite;
    [SerializeField] int inputFrames = 10;
    [SerializeField] int currentFrame;
    [SerializeField] bool horizontalMoveOnly;
    [SerializeField] GameObject myTorch;
    [SerializeField] int maxJumpDist = 2;
    [SerializeField] float footstepSFXLevel = 0.25f;
    [SerializeField] List<AudioClip> myFootsteps;

    //unlocks 
    bool unlockedStrength;
    bool unlockedTorch;
    [SerializeField] bool unlockedJump;
    bool unlockedReset;

    //cached refs
    public Vector3 targetPosition;
    bool blocked;
    bool pushing;
    [SerializeField] bool jumping;
    [SerializeField] bool canJump;
    List<PushableObject> pushables;
    Tilemap hazards;
    LevelManager levelManager;
    AudioManager audioManager;
    public bool levelCompleted;
    GameData gameData;


    // Start is called before the first frame update
    void Start()
    {
        blocked = false;
        pushing = false;
        targetPosition = transform.position;
        pushables = new List<PushableObject>(FindObjectsOfType<PushableObject>());
        hazards = FindObjectOfType<Tilemap>();
        levelManager = FindObjectOfType<LevelManager>();
        currentFrame = 0;
        audioManager = FindObjectOfType<AudioManager>();
        levelCompleted = false;
        gameData = FindObjectOfType<GameData>();
        UpdateUnlocks();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBounds();
        blocked = false;
        ProcessInputs();
        AnimatePlayer();
        MovePlayer();
        CheckForHazard();
    }

    public void UpdateUnlocks()
    {
        unlockedStrength = gameData.unlockedStrength;
        unlockedTorch = gameData.unlockedTorch;
        unlockedJump = gameData.unlockedJump;
        unlockedReset = gameData.unlockedFloat;
        if (unlockedTorch) { myTorch.SetActive(true); } else { myTorch.SetActive(false); }
    }

    private void UpdateBounds()
    {
        float cameraWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;
        upperXBound = Camera.main.transform.position.x + (cameraWidth / 2) + Convert.ToInt32(levelCompleted);
        lowerXBound = Camera.main.transform.position.x - (cameraWidth / 2) - 1;
    }

    private void AnimatePlayer()
    {
        //if (movementDisabled) { return; }
        switch (myDirection)
        {
            case PlayerDirection.Down:
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
                    if (pushing)
                    {
                        myAnimator.Play("Player Interact Down");
                        pushing = false;
                    }
                    else
                    {
                        myAnimator.Play("Player Idle Down");
                    }
                }
                break;
            case PlayerDirection.Up:
                mySprite.flipX = false;
                if (transform.position != targetPosition)
                {
                    if (jumping)
                    {
                        myAnimator.Play("Player Jump Up");
                    }
                    else if (pushing)
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
                    if (pushing)
                    {
                        myAnimator.Play("Player Interact Up");
                        pushing = false;
                    }
                    else
                    {
                        myAnimator.Play("Player Idle Up");
                    }
                }
                break;
            case PlayerDirection.Left:
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
                    if (pushing)
                    {
                        myAnimator.Play("Player Interact Right");
                        pushing = false;
                    }
                    else
                    {
                        myAnimator.Play("Player Idle Right");
                    }
                }
                break;
            case PlayerDirection.Right:
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
                    if (pushing)
                    {
                        myAnimator.Play("Player Interact Right");
                        pushing = false;
                    }
                    else
                    {
                        myAnimator.Play("Player Idle Right");
                    }
                }
                break;
            default:
                break;
        }
    }

    private void MovePlayer()
    {
        if (transform.position != targetPosition)
        {
            CheckForObstacle();
            if (targetPosition.y > upperYBound || targetPosition.y < lowerYBound || targetPosition.x > upperXBound || targetPosition.x < lowerYBound || blocked)
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

        var distanceMag = Vector2.SqrMagnitude(targetPosition - transform.position);
        if (distanceMag < moveSnapThreshold)
        {
            if(transform.position != targetPosition)
            {
                PlayFootstep();
            }
            SnapToGrid();

        }
    }

    private void CheckForObstacle()
    {

        foreach (PushableObject pushable in pushables)
        {
            if (pushable != null)
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
                    if (!unlockedStrength)
                    {
                        blocked = true;
                        pushing = true;
                        jumping = false;
                    }
                    else { pushing = true; }
                }
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

    private void PlayFootstep()
    {
        int clipIndex = UnityEngine.Random.Range(0, myFootsteps.Count - 1);
        AudioClip clipToPlay = myFootsteps[clipIndex];
        AudioSource.PlayClipAtPoint(clipToPlay, Camera.main.transform.position, audioManager.effectVolume * footstepSFXLevel);
    }


    private void ProcessInputs()
    {
        if (movementDisabled) { return; }
        if (currentFrame > 0)
        {
            currentFrame--;
            return;
        }
        if (Input.GetAxis("Horizontal") != 0)
        {
            canJump = false;
            if (transform.position == targetPosition)
            { 
                int direction = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
                if(direction == 1) 
                {
                    if (myDirection == PlayerDirection.Right)
                    {
                        CheckForClearPath(Vector2.right);
                    }
                    else
                    {
                        myDirection = PlayerDirection.Right;
                        currentFrame = inputFrames;
                    }
                }
                if(direction == -1) 
                {
                    if (myDirection == PlayerDirection.Left)
                    {
                        CheckForClearPath(Vector2.left);
                    }
                    else
                    {
                        myDirection = PlayerDirection.Left;
                        currentFrame = inputFrames;
                    }
                }
            }
        }
        if(Input.GetAxis("Vertical") != 0)
        {
            if (horizontalMoveOnly) { return; }
            canJump = false;
            if (transform.position == targetPosition)
            {
                int direction = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
                if (direction == 1)
                {
                    if (myDirection == PlayerDirection.Up)
                    {
                        CheckForClearPath(Vector2.up);
                    }
                    else
                    {
                        myDirection = PlayerDirection.Up;
                        currentFrame = inputFrames;
                    }
                }
                if (direction == -1)
                {
                    if (myDirection == PlayerDirection.Down)
                    {
                        CheckForClearPath(Vector2.down);
                    }
                    else
                    {
                        myDirection = PlayerDirection.Down;
                        currentFrame = inputFrames;
                    }
                }
            }
        }
        if (Input.GetButtonDown("Jump"))
        {
            if(!unlockedJump) { return; }
            if (!canJump) { return; }
            Debug.Log("jumpy jump");
            canJump = false;
            jumping = true;
            switch (myDirection)
            {
                case PlayerDirection.Up:
                    CheckForClearPath(Vector2.up);
                    break;
                case PlayerDirection.Down:
                    CheckForClearPath(Vector2.down);
                    break;
                case PlayerDirection.Left:
                    CheckForClearPath(Vector2.left);
                    break;
                case PlayerDirection.Right:
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
        if (targetPosition.y > upperYBound || targetPosition.y < lowerYBound) 
        { 
            targetPosition = transform.position;
            blocked = true;
            pushing = true;
            jumping = false;
            return; 
        }
        foreach (PushableObject pushable in pushables)
        {
            if (pushable == null) { break; }
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
            var jumpDist = 0;
            while (hazards.HasTile(Vector3Int.RoundToInt(targetPosition)) && jumpDist < maxJumpDist)
            {
                targetPosition = (Vector2)targetPosition + directionToCheck;
                jumpDist++;
            }
            if (hazards.HasTile(Vector3Int.RoundToInt(targetPosition))) { return; }
            if (targetPosition.y > upperYBound || targetPosition.y < lowerYBound) { return; }
            foreach (PushableObject pushable in pushables)
            {
                if (pushable == null) { break; }
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
        if (movementDisabled) { return; }
        Vector3Int positionAsInt = Vector3Int.RoundToInt(transform.position);
        if (hazards.HasTile(positionAsInt) && !jumping)
        {
            myDirection = PlayerDirection.None;
            myAnimator.Play("Player Fall");
            levelManager.TriggerWinningEnd(false);
        }
    }
}
