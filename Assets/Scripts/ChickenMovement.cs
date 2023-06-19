using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChickenMovement : MonoBehaviour
{
    [SerializeField] AbilityButton abilityButton;
    [SerializeField] TMP_Text abilityText;
    private const float rightBound = 6.1f;
    private const float leftBound = -6.1f;
    private const float topBound = 1.3f;
    public const float bottomBound = -2.3f;
    private Animator anim;
    public float horSpeed = 4.0f;
    public float vertSpeed = 2.5f;
    public string audioName;
    public Vector3 exit = new Vector3(7.1f, -3.1f, 0);
    public Vector3 bottomRight = new Vector3(6.1f, -2.3f, 0);
    public bool isFarmer = false;
    private bool isSelected = false;
    private Vector3 destination = Vector3.zero;
    private Vector3 startingPosition = Vector3.zero;
    private string currentStatus = "STRATEGY";
    public bool getSelectedStatus() {
        return isSelected;
    }
    public string getCurrentStatus() {
        return currentStatus;
    }
    void OnDisable() {
        Managers.Audio.stop(audioName);
        Managers.Audio.stop("ChickenLove");
    }
    public void changeStatus(string status) {
        currentStatus = status;
        if (currentStatus == "EXIT") {
            if (anim != null) {
                anim.SetInteger("action", 1);
            }
            Managers.Audio.stop(audioName);
            Managers.Audio.stop("ChickenLove");
        }
        if (currentStatus == "CUTE") {
            if (anim != null) {
                anim.SetInteger("action", 2);
            }
            Managers.Audio.stop(audioName);
            Managers.Audio.play("ChickenLove");
            transform.Find("CuteCircle").gameObject.SetActive(true);
        } else {
            transform.Find("CuteCircle").gameObject.SetActive(false);
        }
    }
    public void toggleCute() {
        if (currentStatus == "CUTE") {
            currentStatus = "STRATEGY";
            if (anim != null) {
                anim.SetInteger("action", 0);
            }
            Managers.Audio.stop("ChickenLove");
            transform.Find("CuteCircle").gameObject.SetActive(false);
            abilityText.text = "Act Cute";
        } else {
            currentStatus = "CUTE";
            if (anim != null) {
                anim.SetInteger("action", 2);
            }
            Managers.Audio.stop(audioName);
            Managers.Audio.play("ChickenLove");
            transform.Find("CuteCircle").gameObject.SetActive(true);
            startingPosition = transform.position;
            destination = transform.position;
            abilityText.text = "Stop";
        }
    }
    public void setDestination(Vector3 newDestination) {
        if (currentStatus == "CUTE") {
            // TODO: Fix button text
            toggleCute();
        }
        float destX = newDestination.x;
        float destY = newDestination.y;
        float destZ = 0;
        if (destX > rightBound) {
            destX = rightBound;
        } else if (destX < leftBound) {
            destX = leftBound;
        }
        if (destY > topBound) {
            destY = topBound;
        } else if (destY < bottomBound) {
            destY = bottomBound;
        }
        startingPosition = transform.position;
        destination = new Vector3(destX, destY, destZ);
        if (anim != null) {
            anim.SetInteger("action", 1);
        }
        Managers.Audio.stop("ChickenLove");
        Managers.Audio.play(audioName);
    }
    void OnMouseUpAsButton() {
        isSelected = !isSelected;
        if (isSelected) {
            transform.Find("SelectedCircle").gameObject.SetActive(true);
            abilityButton.gameObject.SetActive(true);
        } else {
            transform.Find("SelectedCircle").gameObject.SetActive(false);
            abilityButton.gameObject.SetActive(false);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim != null) {
            anim.SetInteger("action", 0);
        }
        transform.Find("SelectedCircle").gameObject.SetActive(false);
        abilityButton.gameObject.SetActive(false);
        transform.Find("CuteCircle").gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentStatus == "STRATEGY") {
            walkToDestination();
        } else if (currentStatus == "EXIT") {
            runToExit();
        } else if (currentStatus == "CUTE") {
            actCute();
        } else {
            escape();
        }
    }
    private void actCute() {
    }
    private void runToExit() {
        if (transform.position != bottomRight) {
            float deltaX = horSpeed * Mathf.Sign((bottomRight.x + Mathf.Abs(leftBound)) - (transform.position.x + Mathf.Abs(leftBound)));
            float deltaY = vertSpeed * Mathf.Sign((bottomRight.y + Mathf.Abs(bottomBound)) - (transform.position.y + Mathf.Abs(bottomBound)));
            Vector3 movement = new Vector3(deltaX, deltaY, 0);
            movement = Vector3.ClampMagnitude(movement, (horSpeed + vertSpeed) / 2);
            movement *= Time.deltaTime;
            transform.position = new Vector3(movement.x + transform.position.x, movement.y + transform.position.y, transform.position.z);
            float distanceX = Mathf.Abs(bottomRight.x - transform.position.x);
            float distanceY = Mathf.Abs(bottomRight.y - transform.position.y);
            if (distanceX > 0.05) {
                float sign = (isFarmer) ? Mathf.Sign(deltaX) : -Mathf.Sign(deltaX);
                transform.localScale = new Vector3(sign, 1, 1);
            }
            if (distanceX < 0.05 && distanceY < 0.05) {
                transform.position = bottomRight;
                startingPosition = transform.position;
                changeStatus("ESCAPING");
            }
        }
    }
    private void escape() {
        if (transform.position != exit) {
            float deltaX = horSpeed * Mathf.Sign((exit.x + Mathf.Abs(leftBound)) - (transform.position.x + Mathf.Abs(leftBound)));
            float deltaY = vertSpeed * Mathf.Sign((exit.y + Mathf.Abs(bottomBound)) - (transform.position.y + Mathf.Abs(bottomBound)));
            Vector3 movement = new Vector3(deltaX, deltaY, 0);
            movement = Vector3.ClampMagnitude(movement, (horSpeed + vertSpeed) / 2);
            movement *= Time.deltaTime;
            transform.position = new Vector3(movement.x + transform.position.x, movement.y + transform.position.y, transform.position.z);
            float distanceX = Mathf.Abs(exit.x - transform.position.x);
            float distanceY = Mathf.Abs(exit.y - transform.position.y);
            if (distanceX > 0.05) {
                float sign = (isFarmer) ? Mathf.Sign(deltaX) : -Mathf.Sign(deltaX);
                transform.localScale = new Vector3(sign, 1, 1);
            }
            if (distanceX < 0.05 && distanceY < 0.05) {
                transform.position = exit;
                startingPosition = exit;
                if (anim != null) {
                    anim.SetInteger("action", 0);
                }
                Debug.Log("Chicken to exit!");
                Messenger.Broadcast(GameEvent.CHICKEN_WINS);
            }
        }
    }
    private void walkToDestination() {
        if (destination != startingPosition) {
            //float deltaX = Mathf.Sign(destination.x - )
            float deltaX = horSpeed * Mathf.Sign((destination.x + Mathf.Abs(leftBound)) - (transform.position.x + Mathf.Abs(leftBound)));
            float deltaY = vertSpeed * Mathf.Sign((destination.y + Mathf.Abs(bottomBound)) - (transform.position.y + Mathf.Abs(bottomBound)));
            Vector3 movement = new Vector3(deltaX, deltaY, 0);
            movement = Vector3.ClampMagnitude(movement, (horSpeed + vertSpeed) / 2);
            movement *= Time.deltaTime;
            transform.position = new Vector3(movement.x + transform.position.x, movement.y + transform.position.y, transform.position.z);
            float distanceX = Mathf.Abs(destination.x - transform.position.x);
            float distanceY = Mathf.Abs(destination.y - transform.position.y);
            if (distanceX > 0.05) {
                float sign = (isFarmer) ? Mathf.Sign(deltaX) : -Mathf.Sign(deltaX);
                transform.localScale = new Vector3(sign, 1, 1);
            }
            if (distanceX < 0.05 && distanceY < 0.05) {
                transform.position = destination;
                startingPosition = destination;
                if (anim != null) {
                    anim.SetInteger("action", 0);
                }
                Managers.Audio.stop(audioName);
            }
        }
    }
}
