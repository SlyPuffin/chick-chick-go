using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardMovement : MonoBehaviour
{
    [SerializeField] ChickenMovement chickenMovement;
    public float speed = 4.0f;
    public float patrolSpeed = 1.0f;
    public int inc;
    public bool canBeDistracted = true;
    public string audioName;
    private Animator anim;
    private int nextInc = 1;
    private float trackPercent = 0;
    private int direction = 1;
    private float max1D = (6.2f * 2f) + (15f * 2f);//41.6f;
    private float dazeTime = 2f;
    private float position1D = 0.0f;
    public string defaultStatus;
    private string currentStatus;
    private string nextStatus = "PATROL";
    public Vector3 startingPosition;
    private float[] cutoffs = {
        Mathf.Abs(BOTTOM) + TOP,//6.0f,
        Mathf.Abs(BOTTOM) + TOP + Mathf.Abs(LEFT) + RIGHT,//20.8f,
        ((Mathf.Abs(BOTTOM) + TOP) * 2) + Mathf.Abs(LEFT) + RIGHT//26.8f
    };
    private const float HALF = 6.2f + 15f;
    public const float TOP = 3f;//2.ff;
    public const float BOTTOM = -3.2f;//-3.5f;
    private const float RIGHT = 7.5f;//7.4f;
    private const float LEFT = -7.5f;//-7.4f;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        transform.position = startingPosition;//new Vector3(RIGHT, BOTTOM, 0);//positions[inc]; 
        position1D = ConvertVecTo1D(startingPosition);
        currentStatus = defaultStatus;
        changeStatus(currentStatus);
    }

    void OnDisable() {
        Managers.Audio.stop(audioName);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentStatus == "PATROL") {
            patrol();
            if (canBeDistracted) isChickenAlert();
        } else if (currentStatus == "FOLLOW") {
            followChicken();
        } else if (currentStatus == "ALERT") {
            followChicken();
            if (canBeDistracted) isChickenAlert();
        } else if (currentStatus == "EXIT") {
            raceToExit();
        } else if (currentStatus == "ENAMORED") {
            followChicken();
        } else if (currentStatus == "FINISHED") {
            transform.position = Convert1DToVec(0);
        }
        lockToBounds();
    }

    public void changeStatus(string status, bool sober = false) {
        if (currentStatus == "DAZED") {
            if (sober || status == "ENAMORED") {
                currentStatus = status;
            } else {
                nextStatus = status;
            }
        } else if (currentStatus == "ENAMORED" && status != "ENAMORED") {
            currentStatus = "DAZED";
            if (anim != null) anim.SetInteger("animStatus", 4);
            nextStatus = defaultStatus;
            StartCoroutine(snapOutOfIt(dazeTime));
        } else {
            currentStatus = status;
        }
        if (currentStatus == "ENAMORED") {
            if (anim != null) anim.SetInteger("animStatus", 4);
        } else if (currentStatus == "DAZED" || currentStatus == "FINISHED") {
            Managers.Audio.stop(audioName);
            if (anim != null) anim.SetInteger("animStatus", currentStatus == "DAZED" ? 4 : 0);
        } else if (currentStatus == "EXIT") {
            Managers.Audio.stop(audioName);
            if (anim != null) anim.SetInteger("animStatus", 1);
        } else {
            Managers.Audio.play(audioName);
            if (anim != null) anim.SetInteger("animStatus", 1);
        }
    }
    private IEnumerator snapOutOfIt(float time) {
        yield return new WaitForSeconds(time);
        changeStatus(nextStatus, true);
    }

    private float ConvertVecTo1D(Vector3 vec) {
        float result = 0;
        if (vec.x == RIGHT) {
            result += vec.y - BOTTOM;
        } else if (vec.y == TOP) {
            result += cutoffs[0];
            result += RIGHT - vec.x;
        } else if (vec.x == LEFT) {
            result += cutoffs[1];
            result += TOP - vec.y;
        } else {
            result += cutoffs[2];
            result += vec.x - LEFT;
        }
        return result;
    }
    private Vector3 Convert1DToVec(float pos) {
        Vector3 result = Vector3.zero;
        if (pos < cutoffs[0]) {
            // Right side
            result.x = RIGHT;
            result.y = BOTTOM + pos;
        } else if (pos < cutoffs[1]) {
            // Top side
            result.x = RIGHT - (pos - cutoffs[0]);
            result.y = TOP;
        } else if (pos < cutoffs[2]) {
            // Left side
            result.x = LEFT;
            result.y = TOP - (pos - cutoffs[1]);
        } else {
            // Bottom side
            result.x = LEFT + (pos - cutoffs[2]);
            result.y = BOTTOM;
            result.z = -2;
        }
        return result;
    }
    private string GetSideFrom1D(float pos) {
        if (pos < cutoffs[0]) {
            // Right side
            return "RIGHT";
        } else if (pos < cutoffs[1]) {
            // Top side
            return "TOP";
        } else if (pos < cutoffs[2]) {
            // Left side
            return "LEFT";
        } else {
            // Bottom side
            return "BOTTOM";
        }
    }
    private string GetLRFromChicken(Vector3 vec) {
        float x = vec.x + Mathf.Abs(LEFT);
        if (x > RIGHT) {
            return "RIGHT";
        } else {
            return "LEFT";
        }
    }
    private string GetTBFromChicken(Vector3 vec) {
        float y = vec.y + Mathf.Abs(BOTTOM);
        if (y > 3.0) {
            return "TOP";
        } else {
            return "BOTTOM";
        }
    }
    private string GetClosestFromChicken(Vector3 vec) {
        float distY = 3.0f - Mathf.Abs((vec.y + Mathf.Abs(BOTTOM)) - 3.5f);
        float distX = 7.4f - Mathf.Abs((vec.x + Mathf.Abs(LEFT)) - 7.4f);
        if (distY < distX) {
            return GetTBFromChicken(vec);
        } else {
            return GetLRFromChicken(vec);
        }
    }

    private void lockToBounds() {
        Vector3 vecPosition = Convert1DToVec(position1D);
        if (vecPosition.x < LEFT) vecPosition.x = LEFT;
        if (vecPosition.x > RIGHT) vecPosition.x = RIGHT;
        if (vecPosition.y > TOP) vecPosition.y = TOP;
        if (vecPosition.y < BOTTOM) vecPosition.y = BOTTOM;
    }
    private void raceToExit() {
        if (position1D > HALF) {
            direction = 1;
        } else {
            direction = -1;
        }
        position1D += direction * speed * Time.deltaTime;
        if (position1D < 0) {
            position1D = max1D + position1D;
        } else {
            position1D %= max1D;
        }
        transform.position = Convert1DToVec(position1D);
        if (Mathf.Abs(position1D) < 0.03) {
            changeStatus("FINISHED");
            Debug.Log("Guard reached exit!");
            Messenger.Broadcast(GameEvent.GUARD_WINS);
            transform.position = Convert1DToVec(0);
        }
    }

    private void patrol() {
        if (chickenMovement != null) {
            position1D += direction * patrolSpeed * Time.deltaTime;
            if (position1D < 0) {
                position1D = max1D + position1D;
            } else {
                position1D %= max1D;
            }
            transform.position = Convert1DToVec(position1D);
        }
    }
    private void isChickenAlert() {
        if (chickenMovement != null) {
            Vector3 chickenPos = chickenMovement.gameObject.transform.position;
            string tb = GetTBFromChicken(chickenPos);
            string lr = GetLRFromChicken(chickenPos);
            if (tb == "BOTTOM" && lr == "RIGHT") {
                changeStatus("ALERT");
            } else {
                changeStatus(defaultStatus);
            }
        }
    }

    private void followChicken() {
       if (chickenMovement != null) {
            bool willUpdate = true;
            Vector3 chickenPos = chickenMovement.gameObject.transform.position;
            float deltaX = (Mathf.Abs(LEFT) + chickenPos.x) - (Mathf.Abs(LEFT) + Convert1DToVec(position1D).x);
            float deltaY = (Mathf.Abs(BOTTOM) + chickenPos.y) - (Mathf.Abs(BOTTOM) + Convert1DToVec(position1D).y);
            if (GetSideFrom1D(position1D) == "RIGHT") {
                if (GetClosestFromChicken(chickenPos) == "RIGHT") {
                    direction = (deltaY >= 0) ? 1 : -1;
                    if (Mathf.Abs(deltaY) < 0.05) {
                        willUpdate = false;
                    }
                } else {
                    direction = GetTBFromChicken(chickenPos) == "TOP" ? 1 : -1;
                }
            } else if (GetSideFrom1D(position1D) == "TOP") {
                if (GetClosestFromChicken(chickenPos) == "TOP") {
                    direction = (deltaX >= 0) ? -1 : 1;
                    if (Mathf.Abs(deltaX) < 0.05) {
                        willUpdate = false;
                    }
                } else {
                    direction = GetLRFromChicken(chickenPos) == "RIGHT" ? -1 : 1;
                }
            } else if (GetSideFrom1D(position1D) == "BOTTOM") {
                if (GetClosestFromChicken(chickenPos) == "BOTTOM") {
                    direction = (deltaX >= 0) ? 1 : -1;
                    if (Mathf.Abs(deltaX) < 0.05) {
                        willUpdate = false;
                    }
                } else {
                    direction = GetLRFromChicken(chickenPos) == "RIGHT" ? 1 : -1;
                }
            } else {
                if (GetClosestFromChicken(chickenPos) == "LEFT") {
                    direction = (deltaY >= 0) ? -1 : 1;
                    if (Mathf.Abs(deltaY) < 0.05) {
                        willUpdate = false;
                    }
                } else {
                    direction = GetTBFromChicken(chickenPos) == "TOP" ? -1 : 1;
                }
            }
            if (willUpdate) {
                position1D += direction * speed * Time.deltaTime;
                if (position1D < 0) {
                    position1D = max1D + position1D;
                } else {
                    position1D %= max1D;
                }
                Managers.Audio.play(audioName);
            } else {
                Managers.Audio.stop(audioName);
            }
            if (anim != null) {
                if (currentStatus == "ENAMORED") {
                    anim.SetInteger("animStatus", 4);
                } else {
                    anim.SetInteger("animStatus", willUpdate ? 1 : (anim.GetInteger("animStatus") - 1));
                }
            }
            transform.position = Convert1DToVec(position1D);
       } 
    }

}
