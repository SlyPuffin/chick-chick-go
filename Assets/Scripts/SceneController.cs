using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SceneController : MonoBehaviour
{
    private bool isFinished = false;
    [SerializeField] ChickenMovement chicken;
    [SerializeField] TMP_Text winText;
    [SerializeField] TMP_Text lossText;
    void OnEnable() {
        Messenger.AddListener(GameEvent.GUARD_WINS, OnGuardWins);
        Messenger.AddListener(GameEvent.CHICKEN_WINS, OnChickenWins);
        isFinished = false;
        winText.gameObject.SetActive(false);
        lossText.gameObject.SetActive(false);
    }
    void OnDisable() {
        Messenger.RemoveListener(GameEvent.GUARD_WINS, OnGuardWins);
        Messenger.RemoveListener(GameEvent.CHICKEN_WINS, OnChickenWins);
    }
    private void OnGuardWins() {
        if (!isFinished) {
            lossText.gameObject.SetActive(true);
            isFinished = true;
            StartCoroutine(RetryLevel());
        }
    }
    private void OnChickenWins() {
        if (!isFinished) {
            winText.gameObject.SetActive(true);
            isFinished = true;
            StartCoroutine(CompleteLevel());
        }
    }
    private IEnumerator RetryLevel() {
        yield return new WaitForSeconds(5);
        Managers.Level.RestartLevel();
    }
    private IEnumerator CompleteLevel() {
        yield return new WaitForSeconds(5);
        Managers.Level.GoToNext();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            if (chicken != null && chicken.getSelectedStatus()) {
                if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y < GuardMovement.TOP) {
                    chicken.setDestination(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
            }
        }
    }
}
