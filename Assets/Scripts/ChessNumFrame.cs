using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessNumFrame : MonoBehaviour {

    public int blueCheckerNum;
    public int redCheckerNum;

    public GameObject redTurnPointer, blueTrunPointer;

    public GameObject RedCheckerNumFrame;
    public GameObject BlueCheckerNumFrame;

    // Use this for initialization
    void Start () {
   
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    //public void isLose(string loser)
    //{
    //    //依据loser弹结束框
    //}
    public void SetNum(int red_num,int blue_num)
    {
        redCheckerNum = red_num;
        blueCheckerNum = blue_num;

        RedCheckerNumFrame.GetComponent<Text>().text = red_num.ToString();
        BlueCheckerNumFrame.GetComponent<Text>().text = blue_num.ToString();

        //if (red_num == 0) isLose("Red");
        //else if (blue_num == 0) isLose("Blue");
    }
    public void InitializeChessNumFrame()
    {
        blueCheckerNum = redCheckerNum = 12;

        RedCheckerNumFrame.GetComponent<Text>().text = "12";
        BlueCheckerNumFrame.GetComponent<Text>().text = "12";
    }
}
