using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HUDManager : MonoBehaviour,IGameManager {

    #region IGameMangerData
    public ManagerStatus status { get; private set; }
    public void Startup()
    {
        status = ManagerStatus.Started;

        HideYONHUD();
        if (Globe.gameMode==GameMode.TwoPerson)//双人模式默认蓝方优先，不需要选择先手
            HideSelectHUD();
    }

    public void Reset()
    {
        Startup();
    }

    public void ShutDown()
    {
        status = ManagerStatus.Shutdown;
    }
    #endregion

    #region 是否提示窗口
    [SerializeField]
    private YesOrNoHUD _yesOrNoHUD;
    public YesOrNoHUD yonHUD { get { return _yesOrNoHUD; } }

    
    /// <summary>
    /// YesOrNoHUD显示与隐藏
    /// </summary>
    public void ShowYONHUD()
    {
        _yesOrNoHUD.gameObject.SetActive(true);
    }
    public void HideYONHUD()
    {
        _yesOrNoHUD.gameObject.SetActive(false);
    }

    public void ShowWinner(string side)
    {
        switch (side)
        {
            case "Blue":
                GameSceneManager.runManager.whichWin = 1;
                yonHUD.message = "-红方获胜-";
                break;
            case "Red":
                GameSceneManager.runManager.whichWin = 0;
                yonHUD.message = "-蓝方获胜-";
                break;
            default:
                break;
        }
        //显示输赢
        yonHUD.canCancel = false;
        yonHUD.yesOrNoCallback = new Action<bool>((yesOrNo) =>
        {
            HideYONHUD();
            StartCoroutine(GameSceneManager.Instance.ResetManagers());
        });
        ShowYONHUD();
    }
    #endregion

    #region 选择窗口
    [SerializeField]
    private SelectHUD _selectHUD;
    public SelectHUD selectHUD { get { return _selectHUD; } }


    /// <summary>
    /// YesOrNoHUD显示与隐藏
    /// </summary>
    public void ShowSelectHUD()
    {
        _selectHUD.gameObject.SetActive(true);
    }
    public void HideSelectHUD()
    {
        _selectHUD.gameObject.SetActive(false);
    }
    #endregion
}
