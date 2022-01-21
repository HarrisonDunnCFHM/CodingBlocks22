using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //config params
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float moveSnapThreshold = 1f;

    //cached refs
    [SerializeField] Vector3 targetPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
        MovePlayer();
    }

    private void MovePlayer()
    {
        if ( transform.position != targetPosition)
        {
            var direction = targetPosition - transform.position;
            transform.Translate(direction.normalized * moveSpeed * Time.deltaTime);
        }
        var distanceMag = Vector2.SqrMagnitude(targetPosition - transform.position );
        Debug.Log(distanceMag);
        if (distanceMag < moveSnapThreshold)
        {
            SnapToGrid();
            Debug.Log("Snapping!");
        }
    }

    private void SnapToGrid()
    {
        int newX = Mathf.RoundToInt(gameObject.transform.position.x);
        int newY = Mathf.RoundToInt(gameObject.transform.position.y);
        transform.position = new Vector2(newX, newY);
        //targetPosition = transform.position;
    }

    private void ProcessInputs()
    {
        if (Input.GetAxis("Horizontal") != 0)
        {
            if (transform.position == targetPosition)
            { 
                int direction = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
                targetPosition = new Vector2(transform.position.x + direction, transform.position.y);
                Debug.Log("Horizontal " + targetPosition);
            }
        }
        if(Input.GetAxis("Vertical") != 0)
        {
            if (transform.position == targetPosition)
            {
                int direction = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
                targetPosition = new Vector2(transform.position.x, transform.position.y + direction);
                Debug.Log("Vertical " + targetPosition);

            }
        }
    }
}
