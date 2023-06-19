using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AbilityButton : MonoBehaviour
{
    [SerializeField] TMP_Text abilityText;
    private string first = "Act Cute";
    private string second = "Stop";
    private bool isActing = false;
    public void toggleText() {
        isActing = !isActing;
        if (isActing) {
            abilityText.text = "Stop";
        } else {
            abilityText.text = "Act Cute";
        }
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
