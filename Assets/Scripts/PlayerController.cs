using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Tilemap tilemap;

    public bool smoothTransition = false;
    public float transitionSpeed = 10f;
    public float rotationSpeed = 500f;

    private string facing = "front";
    private bool hasKey = false;
    public int sceneBuildIndex;

    private Vector3 initialPosition;

    Vector3 prevTargetGridPos;
    Vector3 targetGridPos;

    Vector3 targetRotation;

    Vector3[] walls = new Vector3[500];

    bool isMoving
    {
        get
        {
            if ((Vector3.Distance(transform.position,targetGridPos) < 0.05f) &&
                    (Vector3.Distance(transform.eulerAngles, targetRotation) < 0.05f))
                    return true;
            else
                return false;
        }
    }

    public void RotateLeft() { 
        if (isMoving) 
        {
            ToggleFacingL();
            targetRotation -= Vector3.up * 90f; 
        } 
    }
    public void RotateRight()
    {
        if (isMoving)
        {
            ToggleFacingR();
            targetRotation += Vector3.up * 90f;
        }
    }
    public void MoveForward() {
        if (isMoving) {
            switch (facing)
            {
                case "front":
                    targetGridPos -= Vector3Int.forward;
                    break;
                case "right":
                    targetGridPos -= Vector3Int.right;
                    break;
                case "back":
                    targetGridPos += Vector3Int.forward;
                    break;
                case "left":
                    targetGridPos += Vector3Int.right;
                    break;
                default:
                    break;
            }
            
        } 
    }
    public void MoveBackward() {
        if (isMoving)
        {
            switch (facing)
            {
                case "front":
                    targetGridPos += Vector3Int.forward;
                    break;
                case "right":
                    targetGridPos += Vector3Int.right;
                    break;
                case "back":
                    targetGridPos -= Vector3Int.forward;
                    break;
                case "left":
                    targetGridPos -= Vector3Int.right;
                    break;
                default:
                    break;
            }
        }
    }
    public void MoveLeft() {
        if (isMoving)
        {
            switch (facing)
            {
                case "front":
                    targetGridPos += Vector3Int.right;
                    break;
                case "right":
                    targetGridPos -= Vector3Int.forward;
                    break;
                case "back":
                    targetGridPos -= Vector3Int.right;
                    break;
                case "left":
                    targetGridPos += Vector3Int.forward;
                    break;
                default:
                    break;
            }
        }
    }
    public void MoveRight() {
        if (isMoving)
        {
            switch (facing)
            {
                case "front":
                    targetGridPos -= Vector3Int.right;
                    break;
                case "right":
                    targetGridPos += Vector3Int.forward;
                    break;
                case "back":
                    targetGridPos += Vector3Int.right;
                    break;
                case "left":
                    targetGridPos -= Vector3Int.forward;
                    break;
                default:
                    break;
            }
        } 
    }

    void ToggleFacingR()
    {
        switch (facing)
        {
            case "front":
                facing = "right";
                break;
            case "right":
                facing = "back";
                break;
            case "back":
                facing = "left";
                break;
            case "left":
                facing = "front";
                break;
            default:
                break;
        }
    }

    void ToggleFacingL()
    {
        switch (facing)
        {
            case "front":
                facing = "left";
                break;
            case "right":
                facing = "front";
                break;
            case "back":
                facing = "right";
                break;
            case "left":
                facing = "back";
                break;
            default:
                break;
        }
    }


    private void Start()
    {
        initialPosition = transform.position;
        transform.position = initialPosition;
        targetGridPos = Vector3Int.RoundToInt(transform.position);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag =="Rat" )
        {
            // Reset the character's position to the initial position
            transform.position = initialPosition;
            targetGridPos = Vector3Int.RoundToInt(transform.position);
        }
        if (collision.gameObject.tag == "Key")
        {
            hasKey = true;
            collision.gameObject.SetActive(false);
            print("Got Key");
            print(hasKey);
        }
        if (collision.gameObject.tag == "Finish")
        {
            if (hasKey)
            {
                print("changing scene...");
                SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Single);
            }
            else
            {
                print("No key!");
            }
        }
    }



    bool IsTileWalkable()
    {
        Vector3 targetPosition = targetGridPos;

        Collider[] colliders = Physics.OverlapBox(targetPosition, new Vector3(0.1f, 0.1f, 0.1f));

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Wall"))
            {
                return false;
            }
        }

        return true;
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    void MoveCharacter()
    {
        if (IsTileWalkable())
        {
            prevTargetGridPos = targetGridPos;
            Vector3 targetPosition = targetGridPos;
            if (targetRotation.y > 270f && targetRotation.y < 361f) targetRotation.y = 0f;
            if (targetRotation.y < 0f) targetRotation.y = 270f;

            if (!smoothTransition)
            {
                transform.position = targetPosition;
                transform.rotation = Quaternion.Euler(targetRotation);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * transitionSpeed);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * rotationSpeed);
            }

        }
        else
        {
            targetGridPos = prevTargetGridPos;
        }
    }

   

}
