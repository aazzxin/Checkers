using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour {

    public GameObject checkerPrefab;
    public GameObject ROrBCheckers; //红色棋子还是蓝色棋子
    public GameObject _checkerPos;
    public string checkerPostion;
    public int positionNum;

    public bool isKing = false;
    //  public GameObject[] Checkers;
    // private GameObject[] checkerPossiblePos;

    public string checkerType = "Red";//红还是蓝

    //   [SerializeField]
    //  private Vector2 _checkerTransform;

    // Use this for initialization
    void Start()
    {
        //isKing = false;
        //checkerPostion = _checkerPos.name;
        //    //获取棋盘上该类型棋子的所有可能位置
        //    Checkers = GameObject.FindGameObjectsWithTag(checkerType); 
        //
        //    //通过给棋盘上的黑白格添加标签获取棋盘上两种不同颜色棋子的集合
        //    checkerPossiblePos = GameObject.FindGameObjectsWithTag(gridType);
        //
        //    //获取当前此棋子的位置
        //    foreach(GameObject checkerPos in checkerPossiblePos)
        //    {
        //        if (this.transform.position == checkerPos.transform.position)
        //        {
        //            _checkerPos = checkerPos;
        //        }
        //    }
    }

    // Update is called once per frame
    void Update()
    {

    }
    // public void InstantiateNewChecker(GameObject newCheckerPos)
    // {
    //     //实例化一个棋子
    //     GameObject newChecker = Instantiate(checkerPrefab) as GameObject; 
    //
    //     //以该类型的棋子种类作为父对象
    //     newChecker.transform.parent = ROrBCheckers.transform;
    //
    //     //设置棋子位置
    //     SetCheckerPos(newCheckerPos);
    // }

    public void SetCheckerPos(GameObject checkerPos)
    {
        this._checkerPos = checkerPos;
        //this.transform.Translate(checkerPos.transform.position);
    }
    public GameObject GetCheckerPos()
    {
        return _checkerPos;
    }


}
