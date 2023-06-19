using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartupController : MonoBehaviour
{
    [SerializeField] Slider progressBar;
    [SerializeField] TMP_Text buttonText;
    private bool isButtonActive = false;
    
    public void PressButton() {
        if (isButtonActive) {
            Managers.Level.GoToNext();
        }
    }
    void OnEnable() {
        Messenger<int, int>.AddListener(StartupEvent.MANAGERS_PROGRESS, OnManagersProgress);
        Messenger.AddListener(StartupEvent.MANAGERS_STARTED, OnManagersStarted);
        buttonText.text = "Loading...";
        isButtonActive = false;
    }
    void OnDisable() {
        Messenger<int, int>.RemoveListener(StartupEvent.MANAGERS_PROGRESS, OnManagersProgress);
        Messenger.RemoveListener(StartupEvent.MANAGERS_STARTED, OnManagersStarted);
    }
    private void OnManagersProgress(int numReady, int numModules) {
        float progress = (float) numReady / numModules;
        progressBar.value = progress;
    }

    private void OnManagersStarted() {
        buttonText.text = "Start";
        isButtonActive = true;
        progressBar.gameObject.SetActive(false);
    }
}
