using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class YesOrNoHUD : MonoBehaviour {

    public Action<bool> yesOrNoCallback;

    [SerializeField]
    private Text Txt_Message;
    [SerializeField]
    private GameObject comfirmBtn;
    [SerializeField]
    private GameObject cancelBtn;
    [SerializeField]
    private GameObject onlyComfirmBtn;

    private bool _canCancel = true;
    public bool canCancel
    {
        set
        {
            _canCancel = value;
            comfirmBtn.SetActive(_canCancel);
            cancelBtn.SetActive(_canCancel);
            onlyComfirmBtn.SetActive(!_canCancel);
        }
        get { return _canCancel; }
    }


    public string message
    {
        get { return Txt_Message.text; }
        set { Txt_Message.text = value; }
    }

    public void buttonDo(bool yesOrNo)
    {
        if (yesOrNoCallback != null)
        {
            yesOrNoCallback(yesOrNo);
        }
    }

}
