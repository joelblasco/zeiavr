using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YumisBehaviour : MonoBehaviour {
    #region VARIABLES
    bool FacingClockWise=true, ObstacleDetected, FloorDetected;
	public GameObject Root_GO, YumiModel_GO, Player_GO;
	public float Speed;
    private float RayRange_FW,RayRange_FWD;
    private float ChangeDirectionTime;
    private bool InvokeStarted;
    AudioSource myAudio;
    [SerializeField] AudioClip IdleSound; 
    #endregion
    private void Awake()
    {
        if (!Player_GO)
        {
            Player_GO = GameObject.Find("Zeia");
        }
    }
    void Start ()
    {
        RayRange_FW = 2.5f;
        RayRange_FWD = 3.2f;
        ChangeDirectionTime = 0.2f;
        myAudio = this.gameObject.GetComponent<AudioSource>();
        Invoke("PlayIdleSound", Random.Range(2, 6));
    }
	
	void Update () {
        #region DEBUG
		Debug.DrawRay(YumiModel_GO.transform.position, (YumiModel_GO.transform.forward-YumiModel_GO.transform.up)*RayRange_FWD, Color.green); //forward - DOWN
		Debug.DrawRay(YumiModel_GO.transform.position, YumiModel_GO.transform.forward *RayRange_FW, Color.yellow); //forward
        Root_GO.transform.LookAt(new Vector3(0, Root_GO.transform.position.y, 0));
        #endregion

        #region RAYCASTS

        //FORWARD + DOWN RAY DETECTION should detect if there is a floor to walk

		Ray Forward_Down_Ray = new Ray(YumiModel_GO.transform.position, (YumiModel_GO.transform.forward - YumiModel_GO.transform.up)*1.3f);
        RaycastHit Forward_Down_RayHit = new RaycastHit();
		if(Physics.Raycast(Forward_Down_Ray,out Forward_Down_RayHit,RayRange_FWD))
        {
            if(Forward_Down_RayHit.transform.tag == "Floor" || Forward_Down_RayHit.transform.tag == "Platform")
            {
                FloorDetected = true;
            }
        }
        else
        {
            FloorDetected = false;
        }

        //FORWARD RAY DETECTION should detect obstacles

        Ray Forward_Ray = new Ray(YumiModel_GO.transform.position, YumiModel_GO.transform.forward*1);
        RaycastHit Forward_RayHit = new RaycastHit();
		if (Physics.Raycast(Forward_Ray,out Forward_RayHit, RayRange_FW))
        {
            if (Forward_RayHit.transform.tag == "Platform"
                || Forward_RayHit.transform.tag == "Enemy"
                || Forward_RayHit.transform.tag == "Wall"
                || Forward_RayHit.transform.tag == "Breakable")
            {
                ObstacleDetected = true;
                //print("TEST DETECTION: " + name + " detected something in " + Time.time);
            }
            if (Forward_RayHit.transform.tag == "Player")
            {
                Invoke("Attack",1); //provisional death
            }
        }
        else
        {
            ObstacleDetected = false;
            CancelInvoke("Attack");
        }
        #endregion

        #region MOVEMENT 
        if (!ObstacleDetected) //NO SE ESTÁ DETECTANDO OBSTACULO
        {
            if (FloorDetected) // HAY SUELO DELANTE
            {
                if (FacingClockWise)
                {
                    Walk(Speed);
                }
                else
                {
                    Walk(-Speed);
                }
            }
            else  //NO HAY SUELO DELANTE
            {
                ChangeDirection();
            }
        }
        else if (!InvokeStarted) // SE ESTÁ DETECTANDO UN OBSTACULO DELANTE
        {
            InvokeStarted = true;
            Invoke("ChangeDirection", ChangeDirectionTime);
        }
        #endregion

        #region CHARACTER POSITION FIX

                transform.forward = YumiModel_GO.transform.forward; // CHARACTER POSITION FIX
        // <To fix and set the position and orientation of the Game Object, there are 3
        //  game objects: Parent, Root and Model. Parent will be this object, root is 
        //  the first child and will look at Vector3.zero, finally, this transform.fw
        //  is the same as the child of the root(the model) to help in the orientation
        //  of it through the root ..................................................>

        float radius = 31.8f;
        Vector3 centerPos = Vector3.zero;
        float distance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), centerPos);
        if (distance > radius)
        { // IF POINT IS OUT OF RANGE
            Vector3 fromOriginToObject = new Vector3(transform.position.x, 0, transform.position.z) - centerPos;
            fromOriginToObject *= radius / distance;
            transform.position = new Vector3(centerPos.x + fromOriginToObject.x,transform.position.y, centerPos.z + fromOriginToObject.z);
        }
        if (distance < radius)
        { // IF POINT IS OUT OF RANGE
            Vector3 fromOriginToObject = new Vector3(transform.position.x, 0, transform.position.z) - centerPos;
            fromOriginToObject *= radius / distance;
            transform.position = new Vector3(centerPos.x + fromOriginToObject.x, transform.position.y, centerPos.z + fromOriginToObject.z);
        }

#endregion
    }

    public void Walk(float Speed)
    {
        transform.RotateAround(Vector3.zero, Vector3.up, Speed*Time.deltaTime);
    }

    void ChangeDirection()
    {
        InvokeStarted = false;
        //print("Change direction: " + name + " changed its direction in " + Time.time);
        if (FacingClockWise)
        {
            YumiModel_GO.transform.localEulerAngles = new Vector3(0, 90, 0);
            FacingClockWise = false;
        }
        else
        {
            YumiModel_GO.transform.localEulerAngles = new Vector3(0, -90, 0);
            FacingClockWise = true;
        }
    }
    private void Attack()
    {
        Player_GO.GetComponent<Keyboard_Controller>().SetDeadTrue();
    }
    void PlayIdleSound()
    {
        myAudio.PlayOneShot(IdleSound);
        Invoke("PlayIdleSound", Random.Range(2, 6));
    }
}


















