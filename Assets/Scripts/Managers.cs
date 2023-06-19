using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StageManager))]
[RequireComponent(typeof(LevelManager))]
public class Managers : MonoBehaviour
{
    public static StageManager Scene {get; private set;}
    public static LevelManager Level {get; private set;}
    public static AudioManager Audio {get; private set;}

    private List<IGameManager> startSequence;

    void Awake() {
        DontDestroyOnLoad(gameObject);

        Scene = GetComponent<StageManager>();
        Level = GetComponent<LevelManager>();
        Audio = GetComponent<AudioManager>();

        startSequence = new List<IGameManager>();
        startSequence.Add(Scene);
        startSequence.Add(Level);
        startSequence.Add(Audio);

        StartCoroutine(StartupManagers());
    }

    private IEnumerator StartupManagers() {
        foreach (IGameManager manager in startSequence) {
            manager.Startup();
        }

        yield return null;

        int numModules = startSequence.Count;
        int numReady = 0;

        while (numReady < numModules) {
            int lastReady = numReady;
            numReady = 0;

            foreach (IGameManager manager in startSequence) {
                if (manager.status == ManagerStatus.Started) {
                    numReady++;
                }
            }

            if (numReady > lastReady) {
                Debug.Log($"Progress: {numReady}/{numModules}");
                Messenger<int, int>.Broadcast(StartupEvent.MANAGERS_PROGRESS, numReady, numModules);
            }
            yield return null;
        }

        Debug.Log("All managers setup!");
        Messenger.Broadcast(StartupEvent.MANAGERS_STARTED);
        Audio.play("MainTheme");
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
