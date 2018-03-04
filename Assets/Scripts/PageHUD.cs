using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PageHUD : MonoBehaviour {
    [SerializeField]
    private Text Txt_Title;
    [SerializeField]
    private Image image;
    [SerializeField]
    private Sprite[] sprites;
    [SerializeField]
    private GameObject leftBtn, rightBtn;

    [SerializeField]
    private Text Txt_page;
    [SerializeField]
    private int maxPage;

    private void Start()
    {
        maxPage = sprites.Length;
        page = 1;
        SetButton();
    }

    public string title
    {
        get { return Txt_Title.text; }
        set { Txt_Title.text = value; }
    }
    public int page
    {
        get { return System.Convert.ToInt32(Txt_page.text.Split('/')[0]); }
        set { Txt_page.text = value.ToString() + '/' + maxPage.ToString(); }
    }

    public void buttonDo(int leftOrRight)
    {
        if (leftOrRight == 0 && page != 1) //向左翻转
        {
            page -= 1;
            
        }
        if(leftOrRight == 1 && page != maxPage)//向右翻转
        {
            page += 1;
        }
        image.sprite = sprites[page - 1];
        SetButton();
    }
    public void SetButton()
    {
        if(page==1)
        {
            leftBtn.SetActive(false);
        }
        else
            leftBtn.SetActive(true);
        if (page==maxPage)
        {
            rightBtn.SetActive(false);
        }
        else
            rightBtn.SetActive(true);
    }
}
