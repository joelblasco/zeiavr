using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTrigger : MonoBehaviour {
    [SerializeField][TextArea] string INFO = ("This object will set its Y autom given by the best local score. And a line rendered in the world will show that value.");
    [SerializeField] Vector3[] SCObjectsPositions = new Vector3[26];
    LineRenderer thisLR;
	void Awake () {
        transform.position = new Vector3(0, PlayerPrefs.GetFloat("bestlocalscore"), 0); //the height is equal to the best score.
        thisLR = GetComponent<LineRenderer>();
        for (int i = 0; i < SCObjectsPositions.Length; i++)
        {
            SCObjectsPositions[i] = transform.GetChild(i).GetComponent<Transform>().position;
            thisLR.SetPositions(SCObjectsPositions);
        }
	}
}
