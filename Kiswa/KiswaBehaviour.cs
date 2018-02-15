using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KiswaBehaviour : MonoBehaviour
{
    #region VARIABLES 
    [SerializeField]    [Header(" AI ")]    private bool DirChangeTimer;
    public bool FacingClockWise, ObstacleDetected, FloorDetected, PlayerDetectedClockWise, PlayerDetectedCounterClockWise;
    [SerializeField]    private GameObject Root_GO, KiswaModel_GO,Player_GO,CollidersRoot_GO, ExclamationFX;
    [SerializeField]    [Range(1,10)] private float Speed;
    [SerializeField]    private Animator myAnimator;
    AudioSource myAudio;
    [SerializeField] AudioClip[] Sound;
    [Space(20f)]    [Header("Helps")]    [SerializeField]    private int ChangedDirectionCounter;
    // <This will keep a value of times that this character has changed its direction
    // in a certain time............................................................>

    [SerializeField][Tooltip("This is to see the range of this enemy, press R to enable/disable")] private MeshRenderer[] CollidersRenderers;
    #endregion
    private void Awake()
    {
        if (!Player_GO)
        {
            Player_GO = GameObject.Find("Zeia");
        }
    }
    void Start()
    {
        myAudio = this.gameObject.GetComponent<AudioSource>();
        InvokeRepeating("DirChangeCounterLess", 3, 1);
        CollidersRoot_GO.transform.localEulerAngles = new Vector3(0, -80, 0);
    }

    void Update()
    {
        #region DEBUG
        Debug.DrawRay(KiswaModel_GO.transform.position, (KiswaModel_GO.transform.forward - KiswaModel_GO.transform.up) * 1.3f, Color.green); //forward - DOWN
        Debug.DrawRay(KiswaModel_GO.transform.position, KiswaModel_GO.transform.forward * 1, Color.yellow); //forward

        Root_GO.transform.LookAt(new Vector3(0, Root_GO.transform.position.y, 0));
        //TESTS
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeDirection();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (CollidersRenderers[0].enabled)
            {
                for (int i = 0; i < CollidersRenderers.Length; i++)
                {
                    CollidersRenderers[i].enabled = false;
                }
            }
            else
            {
                for (int i = 0; i < CollidersRenderers.Length; i++)
                {
                    CollidersRenderers[i].enabled = true;
                }
            }
        }
#endif
        #endregion

        #region (RAYCASTS)
        //FORWARD + DOWN RAY DETECTION should detect if there is a floor to walk
        #region FLOOR DETECTION RAY
        Ray Forward_Down_Ray = new Ray(KiswaModel_GO.transform.position, (KiswaModel_GO.transform.forward - KiswaModel_GO.transform.up) * 1.3f);
        RaycastHit Forward_Down_RayHit = new RaycastHit();
        if (Physics.Raycast(Forward_Down_Ray, out Forward_Down_RayHit, 2F))
        {
            if (Forward_Down_RayHit.transform.tag == "Floor" || Forward_Down_RayHit.transform.tag == "Platform")
            {
                FloorDetected = true;
            }
        }
        else
        {
            FloorDetected = false;
        }
        #endregion
        //FORWARD RAY DETECTION should detect obstacles
        #region OBSTACLE DETECTION RAY
        Ray Forward_Ray = new Ray(KiswaModel_GO.transform.position, KiswaModel_GO.transform.forward * 1);
        RaycastHit Forward_RayHit = new RaycastHit();
        if (Physics.Raycast(Forward_Ray, out Forward_RayHit, 2F))
        {
            if (Forward_RayHit.transform.tag == "Platform" 
                || Forward_RayHit.transform.tag == "Wall"
                || Forward_RayHit.transform.tag == "Enemy"
                || Forward_RayHit.transform.tag == "Breakable")
            {
                ObstacleDetected = true;
            }
            if (Forward_RayHit.transform.tag == "Player")
            {
                Invoke("Attack", 1);
                myAnimator.SetTrigger("Attack");
            }
        }
        else
        {
            ObstacleDetected = false;
            CancelInvoke("Attack");
        }
        #endregion
        #endregion

        #region  OBSTACLES, FLOOR AND PLAYER DETECTION BEHAVIOUR
        switch (ObstacleDetected)
        {
            case true:
                ChangeDirection();
                break;
            case false:
                switch (FloorDetected)
                {
                    case true:
                        switch (PlayerDetectedClockWise)
                        {
                            case true:
                                if (FacingClockWise)
                                {
                                    //PLAYER IS IN FRONT OF ZEIA AND IN RANGE
                                    Walk(Speed);
                                    if (ExclamationFX.GetComponent<ParticleSystem>().isStopped) { ExclamationFX.GetComponent<ParticleSystem>().Play(); myAudio.PlayOneShot(Sound[0]); }
                                    //print("EVENT 1");
                                }
                                else
                                {
                                    //PLAYER IS BEHIND ZEIA AND IN RANGE
                                    ChangeDirection();
                                    if (ExclamationFX.GetComponent<ParticleSystem>().isStopped) { ExclamationFX.GetComponent<ParticleSystem>().Play(); myAudio.PlayOneShot(Sound[0]); }
                                    //print("EVENT 2");
                                }
                                break;
                            case false:
                                switch (PlayerDetectedCounterClockWise)
                                {
                                    case true:
                                        if (FacingClockWise)
                                        {
                                            //PLAYER IS BEHIND ZEIA AND IN RANGE
                                            ChangeDirection();
                                            if (ExclamationFX.GetComponent<ParticleSystem>().isStopped) { ExclamationFX.GetComponent<ParticleSystem>().Play(); myAudio.PlayOneShot(Sound[0]); }
                                            //print("EVENT 3");
                                        }
                                        else
                                        {
                                            //PLAYER IS IN FRONT OF ZEIA AND IN RANGE
                                            Walk(-Speed);
                                            if (ExclamationFX.GetComponent<ParticleSystem>().isStopped) { ExclamationFX.GetComponent<ParticleSystem>().Play(); myAudio.PlayOneShot(Sound[0]); }
                                            //print("EVENT 4");
                                        }
                                        break;
                                    case false:
                                        //PLAYER IS NOT DETECTED (CCW & CW)
                                        if (FacingClockWise)
                                        {
                                            Walk(Speed);
                                            //print("EVENT 5");
                                        }
                                        else
                                        {
                                            Walk(-Speed);
                                            //print("EVENT 6");
                                        }
                                        break;
                                }
                                break;
                        }
                        break;
                    case false:
                        ChangeDirection();
                        break;
                }
                break;
        }

        #endregion

        #region CHARACTER POSITION FIX

        transform.forward = KiswaModel_GO.transform.forward; // CHARACTER POSITION FIX
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
            transform.position = new Vector3(centerPos.x + fromOriginToObject.x, transform.position.y, centerPos.z + fromOriginToObject.z);
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
        transform.RotateAround(Vector3.zero, Vector3.up, Speed * Time.deltaTime);
    }

    void ChangeDirection()
    {
        if (!DirChangeTimer)
        {
            DirChangeTimer = true;
            float dctr_time = 2 + ChangedDirectionCounter;
            Invoke("DirChangeTimerReset",dctr_time);
            if (FacingClockWise)
            {
                KiswaModel_GO.transform.localEulerAngles = new Vector3(0, 90, 0);
                CollidersRoot_GO.transform.localEulerAngles = new Vector3(0, -80, 0);
                FacingClockWise = false;
            }
            else
            {
                KiswaModel_GO.transform.localEulerAngles = new Vector3(0, -90, 0);
                CollidersRoot_GO.transform.localEulerAngles = new Vector3(0, 100, 0);
                FacingClockWise = true;
            }
        }
    }
    private void DirChangeTimerReset()
    {
        DirChangeTimer = false;
        ChangedDirectionCounter++;
    }
    private void DirChangeCounterLess()
    {
        if (ChangedDirectionCounter > 0)
        {
            ChangedDirectionCounter--;
        }
    }
    private void Attack()
    {
        Player_GO.GetComponent<Keyboard_Controller>().SetDeadTrue();
        myAudio.PlayOneShot(Sound[2]);
    }
   /* void OnDestroy()
    {
        myAudio.PlayOneShot(Sound[1]);
    }*/
    void PlayIdleSound()
    {
        myAudio.PlayOneShot(Sound[Random.Range(3, 5)]);
        Invoke("PlayIdleSound", Random.Range(3, 10));
	}

	public void DestroySound(){
		myAudio.PlayOneShot(Sound[1]);
	}

}
