using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SelectHUD : MonoBehaviour {


    public Action<int> selectCallback;

    [SerializeField]
    private Text Txt_Message;
    [SerializeField]
    private GameObject[] selectBtn;

    public string message
    {
        get { return Txt_Message.text; }
        set { Txt_Message.text = value; }
    }

    public void buttonDo(int select)
    {
        if (selectCallback != null)
        {
            selectCallback(select);
        }
    }
}
