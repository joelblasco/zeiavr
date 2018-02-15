using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XhelasBehaviour : MonoBehaviour
{
    #region VARIABLES
    [SerializeField]
    public GameObject Player_GO, Model_GO, ModelDestination_GO;
    public LineRenderer SpiderWire;
    public Vector3 RootPosition, ModelPosition, ModelOriginalPosition, ModelLerpDestination;
    [SerializeField] enum States { GoingDown, GoingUp, DownPosition, UpPosition };//
    [Header("IA"), Space(10)]
    [SerializeField]
    private States MyState = States.UpPosition;
    bool PlayerOnTrigger;
    private float StateTimer;
    private float LerpPoint;
    [SerializeField] [Range(0, 10)] public float LerpSpeed;
    [SerializeField] Animator myAnimator;
    [SerializeField] bool AbleToChangeState;
    [SerializeField] AudioClip ScrollSound, AttackSound;
    private AudioSource myAudio;
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
        #region CHARACTER POSITION FIX
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
        transform.LookAt(new Vector3(0, this.transform.position.y, 0));
        ModelOriginalPosition = Model_GO.transform.position;
        RootPosition = this.transform.position;
        SpiderWire.SetPosition(0, RootPosition);
        #endregion
        ModelLerpDestination = ModelDestination_GO.transform.position;
        if (!myAnimator)
        {
            myAnimator = transform.GetComponentInChildren<Animator>();
        }
        myAudio = this.gameObject.GetComponent<AudioSource>();
        AbleToChangeState = true;
    }

    void Update()
    {
        #region DEBUG
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.X)) //testing purposes, delete before alpha
        {
            ChangeState();
        }
#endif
        #endregion

        #region LINE RENDERER FROM ROOT TO MODEL ACTUAL POSITION
        ModelPosition = Model_GO.transform.position;
        SpiderWire.SetPosition(1, ModelPosition);
        #endregion

        #region PLAYER HIT THE WIRE, SO KILLS THE XHELAS
        RaycastHit SpiderWireHit = new RaycastHit();
        if (Physics.Linecast(RootPosition, ModelPosition, out SpiderWireHit))
        {
            if (SpiderWireHit.transform.tag == "Player")
            {
                SpiderWire.enabled = false;
                Destroy(this.gameObject, 1); //Provisional death.
            }
        }
        #endregion

        #region PLAYER TIMING
        switch (MyState)
        {
            case States.DownPosition:
                StateTimer += 1f * Time.deltaTime;
                if (StateTimer > 1)//the spider will be 1 second waiting in down position
                {
                    AbleToChangeState = true;
                    ChangeState();
                }
                break;
            case States.UpPosition:
                if (PlayerOnTrigger)
                {
                    StateTimer += 1f * Time.deltaTime;
                    if (StateTimer > 2)//the spider will be 2 seconds waiting in upper position
                    {
                        AbleToChangeState = true;
                        ChangeState();
                    }
                }
                break;
            case States.GoingDown:
                if (LerpPoint + (1 * Time.deltaTime * LerpSpeed) <= 1)
                {
                    LerpPoint += 1 * Time.deltaTime * LerpSpeed;
                }
                Model_GO.transform.position = Vector3.Lerp(ModelOriginalPosition, ModelLerpDestination, LerpPoint);
                if (LerpPoint < 0.95f) return;
                ChangeState();
                break;
            case States.GoingUp:
                if (LerpPoint - (1 * Time.deltaTime * LerpSpeed) >= 0)
                {
                    LerpPoint -= 1 * Time.deltaTime * LerpSpeed;
                }
                Model_GO.transform.position = Vector3.Lerp(ModelOriginalPosition, ModelLerpDestination, LerpPoint);
                if (LerpPoint > 0.05f) return;
                ChangeState();
                break;
        }
        #endregion
    }

    public void NewTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.transform.position.y < RootPosition.y + 1)
        {
            PlayerOnTrigger = true;
            if (MyState == States.UpPosition) ChangeState();
        }
    }
    public void NewTriggerExit(Collider other)
    {
        if (other.tag == "Player" && other.transform.position.y < RootPosition.y + 1)
        {
            PlayerOnTrigger = false;
            if (MyState == States.DownPosition) ChangeState();
        }
    }
    private void ChangeState()
    {
        if (!AbleToChangeState) return;
        switch (MyState)
        {
            case States.UpPosition:
                myAnimator.SetTrigger("Attack");
                myAudio.PlayOneShot(AttackSound);
                MyState = States.GoingDown;
                break;
            case States.DownPosition:
                myAudio.PlayOneShot(ScrollSound);
                MyState = States.GoingUp;
                break;
            case States.GoingUp:
                StateTimer = 0;
                MyState = States.UpPosition;
                break;
            case States.GoingDown:
                StateTimer = 0;
                myAudio.PlayOneShot(ScrollSound);
                MyState = States.DownPosition;
                break;
        }

    }
    public void PlayerCollisioned()
    {
        Player_GO.GetComponent<Keyboard_Controller>().SetDeadTrue();
    }
}
