using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManger : MonoBehaviour,IGameManager {

    #region IGameMangerData
    public ManagerStatus status { get; private set; }
    public void Startup()
    {
        status = ManagerStatus.Started;

        InitializeClock();

        SetTurnPointer();
        InitializeChessNumFrame();
    }

    /// <summary>
    /// 重置棋盘/棋子
    /// 
    /// </summary>
    public void Reset()
    {
        Startup();
    }

    public void ShutDown()
    {
        status = ManagerStatus.Shutdown;
    }
    #endregion

    public Clock clock;
    public ChessNumFrame chessNumFrame;

    public Sprite redKingSprite,blueKingSprite;

    public void SetTurnPointer()
    {
        if (GameSceneManager.runManager.whoseTurn == "Red")
        {
            chessNumFrame.blueTrunPointer.SetActive(false);
            chessNumFrame.redTurnPointer.SetActive(true);
        }
        else if(GameSceneManager.runManager.whoseTurn == "Blue")
        {
            chessNumFrame.blueTrunPointer.SetActive(true);
            chessNumFrame.redTurnPointer.SetActive(false);
        }
    }

    /// <summary>
    /// 初始化倒计时
    /// </summary>
    public void InitializeClock()
    {
        clock.ResetClock(GameSceneManager.runManager.whoseTurn);
        clock.InitializeClock();
    }
    public void InitializeChessNumFrame()
    {
        chessNumFrame.InitializeChessNumFrame();
    }
    /// <summary>
    /// 设置棋子数目显示
    /// 在Update()中一直检测
    /// ps:放在OnClickPointer中会滞后
    /// </summary>
    public void SetChessNumFrame(int redCheckersNum,int blueCheckersNum)
    {
        chessNumFrame.SetNum(redCheckersNum, blueCheckersNum);
    }

    
}
