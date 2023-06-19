using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExitButton : MonoBehaviour
{
    [SerializeField] GuardMovement[] guardMovements;
    [SerializeField] ChickenMovement chickenMovement;
    [SerializeField] FenceState fenceState;
    public float timeRemaining;
    public bool timerIsRunning = false;
    public TMP_Text timerText;
    void OnMouseUpAsButton() {
        InitiateExit();
        timeRemaining = 0;
        timerIsRunning = false;
        DisplayTime(timeRemaining);
    }
    void OnMouseEnter() {
        Debug.Log("Mouse entered");
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        if (renderer != null) {
            Debug.Log("Found renderer");
            renderer.color = new Color(1f, 200/255f, 200/255f, 1f);
        }
    }
    void OnMouseExit() {
        Debug.Log("Mouse exited");
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        if (renderer != null) {
            renderer.color = Color.white;
        }
    }
    private void InitiateExit() {
        foreach (GuardMovement movement in guardMovements) {
            movement.changeStatus("EXIT");
        }
        if (chickenMovement != null) {
            chickenMovement.changeStatus("EXIT");
        }
        if (fenceState != null) {
            fenceState.changeStatus("EXIT");
        }
        Managers.Audio.stop("BackingTheme");
        Managers.Audio.play("EscapeTheme");
    }
    // Start is called before the first frame update
    void Start()
    {
        timerIsRunning = true;
    }
    void Update()
    {
        if (timerIsRunning) {
            if (timeRemaining > 0) {
                timeRemaining -= Time.deltaTime;
            } else {
                InitiateExit();
                timeRemaining = 0;
                timerIsRunning = false;
            }
            DisplayTime(timeRemaining);
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
