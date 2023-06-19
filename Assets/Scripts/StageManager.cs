using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour, IGameManager {
    public ManagerStatus status {get; private set;}

    public void Startup() {
        Debug.Log("SceneManager starting...");
        status = ManagerStatus.Started;
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
