using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuController : MonoBehaviour
{
    int SelectionIndex;
    public int currentSelection;
    public Button B_0, B_1, B_2, B_3, B_4;
	public HSController online;
    [SerializeField] string nickname;
    private string uniqueID; // WILL BE GIVEN BY SYSTEM
	public Vector2 TouchpadAxis;
    public GameObject Tutorial, MenuCanvas;
    void Start()
    {
		this.gameObject.SetActive (false);
        currentSelection = 2;
        HighlighAction(currentSelection);
        //        if (online == null)
        //        {
        //            Debug.LogError("HIGHSCORE CONTROLLER IS NOT ASSIGNED");
        //            online = GameObject.Find("OnlineHighscore").GetComponent<HSController>();
        //        }
        //
        //		online.gameObject.SetActive (false);
    }
    private void Update()
    {

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.DownArrow))
        {
            online.MoveScrollList(true);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            online.MoveScrollList(false);
        }
#endif

		if (TouchpadAxis.y < 0)
		{
			online.MoveScrollList(true);
		}
		if (TouchpadAxis.y > 0)
		{
			online.MoveScrollList(false);
		}

    }
    public void MenuAction(int _action)
    {
        switch (_action)
        {

            case 1: //go to calibration scene
                print("Calibrate Selected");
                SceneManager.LoadScene("Play");
                break;

            case 2: // instantiate scoreboard
                print("Scoreboard selected");
                #region HIGHSCORE BEHAVIOUR
                if (!online.gameObject.activeSelf)
                {
                    online.gameObject.SetActive(true);
				    MenuCanvas.SetActive(false);
                }
                // SHOW ONLINE HIGHSCORES
                online.startGetScores();
                
                #endregion
                break;

            case 3: // go to tutorial scene
                SceneManager.LoadScene("Tutorial");
                print("Tutorial Selected");
                break;
            case 4: //quit
                print("Exit Selected");
                Application.Quit();
                break;
            default:
                Debug.LogError("NO ACTION ASSIGNED");
                break;
        }

    }


    private void HighlighAction(int _action)
    {
        switch (_action)
        {

            case 1: //go to calibration scene
                B_1.Select();
                currentSelection = 1;
                break;

            case 2: // instantiate scoreboard
                B_2.Select();
                currentSelection = 2;
                break;

            case 3: // go to tutorial scene
                B_3.Select();
                currentSelection = 3;
                break;
            case 4: //quit
                B_4.Select();
                currentSelection = 4;
                break;
            default:
                Debug.LogError("NO ACTION ASSIGNED");
                break;
        }
    }
		
    public void SetSelection(int value)
    {
        /*if (online.gameObject.activeSelf)
        {
            bool _bool;
            if (value < 0)
            {
                _bool = true;
            }
            else
            {
                _bool = false;
            }
            online.MoveScrollList(_bool);
            return;
        }*/
        if (currentSelection >= 1 && currentSelection <= 4)
        {
            currentSelection += value;
        }

        if (currentSelection < 1)
        {
            currentSelection = 1;
        }

        if (currentSelection > 4)
        {
            currentSelection = 4;
        }
        print(currentSelection);

        HighlighAction(currentSelection);

    }
    public void PressedSelection()
    {
        //when pressed the touchpad, do the action
        MenuAction(currentSelection);
    }


    public void PrintInfo()
    {
        print("BUTTONPRESSED");
    }

    public void NewOnlineScore(float _score)
    {
        nickname = PlayerPrefs.GetString("localuser");
        if (_score > PlayerPrefs.GetFloat("bestlocalscore")) //check if the last score is higher than the best one, if so, update it.
        {
            PlayerPrefs.SetFloat("bestlocalscore", _score); // set the local player pref for the score 
            
          
        }
		uniqueID = SystemInfo.deviceUniqueIdentifier + "_" + nickname + "_"; // creation of the unique identifier for the player
		online.updateOnlineHighscoreData(uniqueID, nickname, _score); //this will be the information of the player in the database
		online.startPostScores(); // will create a new entry in the database with the info given before.
		online.Invoke("startGetScores", 1); //update what the player see in the list, invoke in 1f in case update take more time than estimated
		print("NEW ONLINE SCORE UPDATED");
    }
}
