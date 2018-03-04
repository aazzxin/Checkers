using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour {

    public float _time;
    public float maxTime;

  
    public bool timeStart;

    public string time;
    public string whoseTurn;//正在为谁计时...

    // Use this for initialization
    void Start()
    {
        //InitializeClock();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //倒计时
        if (timeStart)
        {
            if (_time > 0 && _time <= maxTime)
            {
                _time -= Time.fixedDeltaTime;
            }
            else if (_time == 0 || _time < 0)
            {
                TimeRunOver();
            }
            time = _time.ToString("f0");
            GameObject.Find("ClockFrame/Text").GetComponent<Text>().text = time;
        }
    }

    public void InitializeClock()
    {
        whoseTurn = "Blue";
        maxTime = 30;
        _time = maxTime;
        time = maxTime.ToString();
        GameObject.Find("ClockFrame/Text").GetComponent<Text>().text = time;
        timeStart = false;
    }

    /// <summary>
    /// 时间结束,游戏结束
    /// 判负条件1:无自己棋子
    /// 判负条件2:所有棋子不能移动
    /// 判负条件3:时间结束
    /// </summary>

    public void TimeRunOver()
    {
        timeStart = false;
        isLose(whoseTurn);
    }
    public void isLose(string loser)
    {
        GameSceneManager.runManager.SetWinner(loser);
    }

    /// <summary>
    /// 前一个棋手下完时调用此函数
    /// 用于重置计时器
    /// </summary>
    public void ResetClock(string whoseTurn)
    {
        this.whoseTurn = whoseTurn;

        _time = maxTime;
        timeStart = true;
    }

}
