using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceState : MonoBehaviour
{
    private string currentStatus = "CLOSED";
    private Animator anim;
    public void changeStatus(string status) {
        currentStatus = status;
        Debug.Log($"Change status to {currentStatus}");
        if (anim != null) {
            if (currentStatus == "EXIT") {
                Debug.Log("Setting animator to true");
                anim.SetBool("isExit", true);
            } else {
                anim.SetBool("isExit", false);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
