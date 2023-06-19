using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour, IGameManager {
    public ManagerStatus status {get; private set;}

    public int curLevel {get; private set;}
    public int maxLevel {get; private set;}

    public void Startup() {
        Debug.Log("Level manager starting up");
        curLevel = 0;
        maxLevel = 5;
        status = ManagerStatus.Started;
    }

    public void GoToNext() {
        if (curLevel == 0) {
            Managers.Audio.stop("MainTheme");
        } else {
            Managers.Audio.stop("EscapeTheme");
        }
        if (curLevel < maxLevel) {
            curLevel++;
            string name = $"Level{curLevel}";
            Debug.Log($"Loading: {name}");
            SceneManager.LoadScene(name);
            Managers.Audio.play("BackingTheme");
        } else {
            Debug.Log("Last level");
            SceneManager.LoadScene(maxLevel + 1);
            Managers.Audio.play("MainTheme");
        }
    }

    public void RestartLevel() {
        Managers.Audio.stop("EscapeTheme");
        SceneManager.LoadScene($"Level{curLevel}");
        Managers.Audio.play("BackingTheme");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
