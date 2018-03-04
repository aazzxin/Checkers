using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;

public class GameRunManager : MonoBehaviour,IGameManager {

    #region IGameMangerData
    public ManagerStatus status { get; private set; }
    public void Startup()
    {
        status = ManagerStatus.Started;


        if (Globe.gameMode == GameMode.OnePerson)
            SelectFirst();

        //if (whichFirst == 0)
        //{
        //    whoseTurn = "Blue";
        //}
        //else
        //{
        //    whoseTurn = "Red";
        //}


        //blueCheckersNum = 12;
        //redCheckersNum = 12;

        playerState = PlayerState.Red_ChooseChecker;

        boardPos = new GameObject[64];
        GetBoardPos();

        checkers = new Checker[64];
        pointers = new List<int>[64];

        possibleDestinationPos = new List<GameObject>();

        //初始化AI
        InitializeRobot();
        //初始化棋盘
        InitializeCheckerboard();
        //初始化记录
        InitializeRecorder();

        GetCheckerNum();
    }

    /// <summary>
    /// 重置棋盘/棋子
    /// 
    /// </summary>
    public void Reset()
    {
        if (Globe.gameMode == GameMode.OnePerson)
            SelectFirst();

        checkers = new Checker[64];
        pointers = new List<int>[64];
        possibleDestinationPos = new List<GameObject>();

        whoseTurn = "Blue";
        whichFirst = 0;
        whichWin = -1;

        clearAll();

        InitializeRobot();

        InitializeCheckerboard();
        InitializeRecorder();

        redCheckersNum = 12;
        blueCheckersNum = 12;
    }

    public void ShutDown()
    {
        status = ManagerStatus.Shutdown;
    }
    #endregion

    public int whichFirst = 0;//哪方优先设定，默认为0（蓝方优先，即玩家优先），为1时表示（红方优先，即电脑优先）。当全局Globe的Mode为双人模式时，此变量无用，默认为蓝方优先。
    public int whichWin = -1;//哪方赢得胜利。0为蓝方赢，1为红方赢
    
    public GameObject pointPrefab;
    public GameObject checkerPrefab;
    public GameObject ROrBCheckers; //红色棋子还是蓝色棋子
    public GameObject redCheckersParent;
    public GameObject blueCheckersParent;
    public GameObject leftUpPos;
    public GameObject rightUpPos;

    public GameObject Position;//棋盘格位置父物体
    public GameObject pointerParent;//落点父对象
    public GameObject sourceChecker;//被点击的棋子
    public GameObject newChecker;
    public GameObject RedChess;
    public GameObject BlueChess;
    public RectTransform chessTempRect;
    public RectTransform pointerTempRect;

    public List<GameObject> possibleDestinationPos;//可能的终点
    //private GameObject[] jumppedCheckers;//各个可能的终点对应的中间格
    //private GameObject[] _middleStep;//中间格,存在列表中
    public GameObject[] boardPos;//棋盘格子物体的position
    public Checker[] checkers;//相应位置boardPos[i]的棋子

    public GameObject destinationPos;//终点坐标
    public GameObject sourcePos;//起点坐标
    public List<int>[] pointers;//pointers数组，存放吃子信息

    public int blueCheckersNum;
    public int redCheckersNum;
    public bool canEat;
    public string checkerType = "Red";//红还是蓝
    public string gridType = "Black";//黑格还是白格
    public string myCheckerType;
    public string yourCheckerType;
    public string whoseTurn="Blue";

    public PlayerState playerState;


    public Robot robot;
    GameObject robotGameObject;

    public void InitializeRobot()
    {
        if(Globe.gameMode==GameMode.OnePerson)
        {
            if(robotGameObject==null)
            {
                robotGameObject = new GameObject("robot");
                robotGameObject.AddComponent<Robot>();
                robot = robotGameObject.GetComponent<Robot>();
            }
            
            robot.InitializeRobot();
        }
    }

    public struct Record
    {
        public int[] checkers;//-1为空格子，0为蓝棋子，1为红棋子。不用Checker类型，是因为Checker对象是实际的（需要实例化），而int表示虚拟的
        public bool[] isKing;//对应位置是否为王

        public int blueCheckersNum;
        public int redCheckersNum;
    }
    public List<Record> record;

    public enum PlayerState
    {
        Red_ChooseChecker,
        Red_ChoosePointer,
        Blue_ChooseChecker,
        Blue_ChoosePointer
    }

    // Use this for initialization
    void Start()
    {
    }

    public void GetBoardPos()
    {
        //获取棋盘位置并排序好放在boardPos中
        for(int i=0; i<64;i++)
        {
            boardPos[i] = Position.transform.GetChild(i).gameObject;
        }
    }

    /// <summary>
    /// 红色&蓝色棋子位置\剩余数目(用于UI显示数目
    /// </summary>
    public void GetCheckerNum()
    {
        redCheckersNum = redCheckersParent.transform.childCount;
        blueCheckersNum = blueCheckersParent.transform.childCount;
    }
    // Update is called once per frame
    void Update()
    {
        //SetChessNumFrame();
    }
   
    public void PrintPos(GameObject[] Pos)
    {
        foreach (GameObject child in Pos)
        {
            Debug.Log(child.GetComponent<Checker>().checkerPostion);
        }
    }

    
  
    /// <summary>
    /// 初始化棋盘
    /// 把现在棋盘上所有的棋子destory
    /// 在初始位置生成新的棋子
    /// </summary>
    public void InitializeCheckerboard()
    {
        for (int i = 0; i < 4; i++)
        {
            InstantiateNewChecker("Red",2 * i + 1);
            InstantiateNewChecker("Blue", 62 - 2 * i);
            InstantiateNewChecker("Red", 2 * i + 8);
            InstantiateNewChecker("Blue", 63 - 8 - 2 * i);
            InstantiateNewChecker("Red", 2 * i + 17);
            InstantiateNewChecker("Blue", 63 - 2 * i - 17);
        }

        //GetCheckerBoard();
        //SortPos(BlueCheckers, blueCheckersNum);
        //SortPos(RedCheckers, redCheckersNum);
    }
    /// <summary>
    /// 实例化一个棋子并移到新位置
    /// </summary>
    public void InstantiateNewChecker(string checkerType, int n)
    {
        GameObject newCheckerPos = boardPos[n];

        if (checkerType == "Blue")
        {
            checkerPrefab = BlueChess;
            ROrBCheckers = blueCheckersParent;
        }

        else if (checkerType == "Red")
        {
            checkerPrefab = RedChess;
            ROrBCheckers = redCheckersParent;
        }

        //在newCheckerPos处实例化一个棋子
        GameObject newChecker = Instantiate(checkerPrefab) as GameObject;
        newChecker.transform.position = newCheckerPos.transform.position;

        //以该类型的棋子种类作为父对象
        newChecker.transform.SetParent(ROrBCheckers.transform);
        newChecker.GetComponent<RectTransform>().sizeDelta = new Vector2(chessTempRect.sizeDelta.x, chessTempRect.sizeDelta.x);
        newChecker.GetComponent<RectTransform>().localScale = chessTempRect.localScale;

        newChecker.GetComponent<Button>().onClick.AddListener(OnClickChecker);
        if (Globe.gameMode == GameMode.OnePerson && robot.checkers[0] == -10) //在单人模式一开始，先屏蔽棋子按钮。在选择先手方后恢复按钮
            newChecker.GetComponent<Button>().enabled = false;
        //设置棋子位置
        newChecker.GetComponent<Checker>().SetCheckerPos(newCheckerPos);

        checkers[n] = newChecker.GetComponent<Checker>();
        checkers[n].positionNum = n;
        //checkers[n].checkerPostion = boardPos[n].name;

    }
    /// <summary>
    /// 悔棋按钮相应事件
    /// </summary>
    public void OnClickBackMove()
    {
        if (record.Count >= 2 && (Globe.gameMode == GameMode.TwoPerson || whoseTurn == "Blue")) //当已经下了两个棋子时，且在双人模式或者是蓝方出手时
        {
            MoveBack();
        }
    }
    public void MoveBack()
    {
        record.RemoveAt(record.Count - 1);
        Record rcd = record[record.Count - 1];

        redCheckersNum = rcd.redCheckersNum;
        blueCheckersNum = rcd.blueCheckersNum;

        if (Globe.gameMode == GameMode.OnePerson)
        {
            if (record.Count == 1)
            {
                robot.InitializeRobot();
            }
            else
            {
                robot.lastChecker.RemoveAt(robot.lastChecker.Count - 1);
                robot.lastPoint.RemoveAt(robot.lastPoint.Count - 1);
                robot.redCheckersNum = redCheckersNum;
                robot.blueCheckersNum = blueCheckersNum;
            }
        }

        for (int i = 0; i < 64; i++)
        {
            if (rcd.checkers[i] == -1) //空格子
            {
                if (checkers[i] != null)
                {
                    checkers[i].transform.SetParent(this.transform);//立即移出队列
                    Destroy(checkers[i].gameObject);
                    checkers[i] = null;
                }
            }
            else if (rcd.checkers[i] == 0)//蓝棋子
            {
                if (checkers[i] == null)
                {
                    InstantiateNewChecker("Blue", i);
                    if (rcd.isKing[i])
                    {
                        checkers[i].isKing = true;
                        checkers[i].gameObject.GetComponent<Image>().sprite = GameSceneManager.uiManager.blueKingSprite;
                    }

                }
                else if (checkers[i].checkerType == "Red")
                {
                    checkers[i].transform.SetParent(this.transform);//立即移出队列
                    Destroy(checkers[i].gameObject);
                    InstantiateNewChecker("Blue", i);
                    if (rcd.isKing[i])
                    {
                        checkers[i].isKing = true;
                        checkers[i].gameObject.GetComponent<Image>().sprite = GameSceneManager.uiManager.blueKingSprite;
                    }
                }
            }
            else if (rcd.checkers[i] == 1) //红棋子
            {
                if (checkers[i] == null)
                {
                    InstantiateNewChecker("Red", i);
                    if (rcd.isKing[i])
                    {
                        checkers[i].isKing = true;
                        checkers[i].gameObject.GetComponent<Image>().sprite = GameSceneManager.uiManager.redKingSprite;
                    }
                }
                else if (checkers[i].checkerType == "Blue")
                {
                    checkers[i].transform.SetParent(this.transform);//立即移出队列
                    Destroy(checkers[i].gameObject);
                    InstantiateNewChecker("Red", i);
                    if (rcd.isKing[i])
                    {
                        checkers[i].isKing = true;
                        checkers[i].gameObject.GetComponent<Image>().sprite = GameSceneManager.uiManager.redKingSprite;
                    }
                }
            }
        }
        record.RemoveAt(record.Count - 1);

        ClearPointer();

        GameSceneManager.uiManager.SetChessNumFrame(redCheckersNum, blueCheckersNum);
    }

    /// <summary>
    /// 初始化记录器
    /// 记录器是用于悔棋功能
    /// </summary>
    public void InitializeRecorder()
    {
        record = new List<Record>();
    }

    #region 点击棋子分析，以及点击落点
    /// <summary>
    /// 点击棋子时的相应
    /// 获取当前点击棋子的位置并调用分析函数
    /// 获得可能的目标位置和中间格
    /// 根据可能的目标位置生成点
    /// </summary>
    public void OnClickChecker()
    {
        GameObject temp = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (Globe.gameMode == GameMode.TwoPerson || whoseTurn == "Blue")//在双人模式或者是蓝方出手时
            OnClickChecker(temp);
    }
    public void OnClickChecker(GameObject temp)
    {
        if (temp.GetComponent<Checker>().checkerType == whoseTurn)//所选棋子类型属于出手方
        {
            if (sourceChecker != null)
            {
                if (sourceChecker == temp && pointerParent.transform.childCount != 0)//原来点击的棋子就是这次点击的棋子，且有显示落点，则不处理
                {
                    return;
                }
            }
            sourceChecker = temp;

            if (sourceChecker.GetComponent<Checker>().checkerType == whoseTurn)//所选棋子类型属于出手方
            {
                ClearPointer();//清除落点
                               //foreach (GameObject grid in boardPos)
                               //{
                               //    if (grid.transform.position == sourceChecker.transform.position)
                               //    {
                               //        sourcePos = grid;
                               //    }
                               //}
                               //Debug.Log(sourceChecker.name + sourcePos.name + "CheckerOnClicked");
                possibleDestinationPos = Analyse(sourceChecker.GetComponent<Checker>(), sourceChecker.GetComponent<Checker>().checkerType);


                InstantiatePointer(possibleDestinationPos);
            }
        }
        else
            ClearPointer();//清除落点
    }

    /// <summary>
    /// 分析可能的终点
    /// 记录中间格
    /// </summary>
    public List<GameObject> Analyse(Checker sourceChecker, string checkerType)
    {

        //获取当前下棋方
        myCheckerType = checkerType;
        if (myCheckerType == "Red") yourCheckerType = "Blue";
        else if (myCheckerType == "Blue") yourCheckerType = "Red";

        //获取棋盘数据:我方棋子位置和敌方棋子位置
        //剩余的就是空位置和没用的格
        //GetCheckerBoard();

        //GetBoardPos();
        //PrintPos(BlueCheckers);
        //PrintPos(RedCheckers);


        possibleDestinationPos = new List<GameObject>();
        pointers = new List<int>[64];
        if (!IsCanEat(sourceChecker, sourceChecker.positionNum)) //若该棋子不能吃子
        {
            if (checkerType == "Red")
                ROrBCheckers = redCheckersParent;
            else if(checkerType=="Blue")
                ROrBCheckers = blueCheckersParent;
            foreach (Checker child in ROrBCheckers.GetComponentsInChildren<Checker>())
            {
                if (child!= sourceChecker)
                {
                    possibleDestinationPos = new List<GameObject>();
                    pointers = new List<int>[64];
                    //对其余棋子判断是否可以吃子
                    if (IsCanEat(child,child.positionNum))
                    {
                        //若存在其余棋子可以吃子，则该棋子不能移动
                        possibleDestinationPos = new List<GameObject>();
                        pointers = new List<int>[64];
                        return new List<GameObject>();
                    }
                }
            }

            //若其他棋子均不能吃子，则正常分析
            possibleDestinationPos = new List<GameObject>();
            pointers = new List<int>[64];
            GetAroundPos(sourceChecker);
        }
        return possibleDestinationPos;
    }
    public void GetAroundPos(Checker check)//获取周围能移动的位置（非吃子）
    {
        int n = check.positionNum;
        if(check.isKing)
        {
            if(n-9>=0)
            {
                if (checkers[n - 9] == null && boardPos[n - 9].tag == "Black") 
                {
                    possibleDestinationPos.Add(boardPos[n - 9]);
                }
            }
            if(n-7>=0)
            {
                if (checkers[n - 7] == null && boardPos[n - 7].tag == "Black")
                {
                    possibleDestinationPos.Add(boardPos[n - 7]);
                }
            }
            if(n+7<64)
            {
                if (checkers[n + 7] == null && boardPos[n + 7].tag == "Black") 
                {
                    possibleDestinationPos.Add(boardPos[n + 7]);
                }
            }
            if(n+9<64)
            {
                if (checkers[n + 9] == null && boardPos[n + 9].tag == "Black")
                {
                    possibleDestinationPos.Add(boardPos[n + 9]);
                }
            }
        }
        else if (check.checkerType == "Red")
        {
            if (n + 7 < 64)
            {
                if (checkers[n + 7] == null && boardPos[n + 7].tag == "Black")
                {
                    possibleDestinationPos.Add(boardPos[n + 7]);
                }
            }
            if (n + 9 < 64)
            {
                if (checkers[n + 9] == null && boardPos[n + 9].tag == "Black")
                {
                    possibleDestinationPos.Add(boardPos[n + 9]);
                }
            }
        }
        else if(check.checkerType == "Blue")
        {
            if (n - 9 >= 0)
            {
                if (checkers[n - 9] == null && boardPos[n - 9].tag == "Black")
                {
                    possibleDestinationPos.Add(boardPos[n - 9]);
                }
            }
            if (n - 7 >= 0)
            {
                if (checkers[n - 7] == null && boardPos[n - 7].tag == "Black")
                {
                    possibleDestinationPos.Add(boardPos[n - 7]);
                }
            }
        }
    }

    /// <summary>
    /// 在可能的终点生成:点
    /// </summary>
    public void InstantiatePointer(List<GameObject> pointPos)
    {
        foreach (GameObject point in pointPos)
        {
            GameObject newPointer = Instantiate(pointPrefab) as GameObject;
            newPointer.transform.position = point.transform.position;
            newPointer.transform.SetParent(pointerParent.transform);
            newPointer.GetComponent<RectTransform>().sizeDelta = new Vector2(pointerTempRect.sizeDelta.x, pointerTempRect.sizeDelta.x);
            newPointer.GetComponent<RectTransform>().localScale = pointerTempRect.localScale;
            for (int i=0;i<64;i++)
            {
                if(point==boardPos[i])
                {
                    newPointer.GetComponent<Pointer>().positionNum = i;
                    break;
                }
            }
            newPointer.GetComponent<Button>().onClick.AddListener(OnClickPointer);
        }
    }

    /// <summary>
    /// 点:被点击后
    /// 将棋子移动到该终点位置
    /// 清除该终点位置对应的中间格
    /// </summary>
    public void OnClickPointer()
    {
        GameObject point = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

        OnClickPointer(point);
    }
    public void OnClickPointer(GameObject point)
    {
        //录入棋盘记录
        Record rcd;
        rcd.checkers = new int[64];
        rcd.isKing = new bool[64];
        for (int i = 0; i < 64; i++)
        {
            if (checkers[i] == null)
                rcd.checkers[i] = -1;
            else if (checkers[i].checkerType == "Red")
            {
                rcd.checkers[i] = 1;
                if (checkers[i].isKing)
                    rcd.isKing[i] = true;
            }
            else if (checkers[i].checkerType == "Blue")
            {
                rcd.checkers[i] = 0;
                if (checkers[i].isKing)
                    rcd.isKing[i] = true;
            }
        }
        rcd.redCheckersNum = redCheckersNum;
        rcd.blueCheckersNum = blueCheckersNum;
        record.Add(rcd);


        for (int i = 0; i < boardPos.Length; i++)
        {
            GameObject grid = boardPos[i];
            if (point.transform.position == grid.transform.position)
            {
                destinationPos = grid;

                ClearPointer();
                //InstantiateNewChecker(checkerType, i);
                sourceChecker.transform.position = destinationPos.transform.position;

                Checker clickChecker = sourceChecker.GetComponent<Checker>();
                checkers[clickChecker.positionNum] = null;
                clickChecker.positionNum = i;
                clickChecker.SetCheckerPos(destinationPos);
                checkers[i] = clickChecker;

                if (pointers[i] != null)
                {
                    if (pointers[i].Count > 0)
                    {
                        int num = pointers[i].Count;
                        //若有吃子
                        foreach (int pos in pointers[i])
                        {
                            checkers[pos].transform.SetParent(this.transform);//立即移出parent队列,否则robot在检测parent时会检测到被吃的棋子
                            Destroy(checkers[pos].gameObject);
                            checkers[pos] = null;
                        }

                        if (whoseTurn == "Red")
                            blueCheckersNum -= num;
                        else if (whoseTurn == "Blue")
                            redCheckersNum -= num;

                        GameSceneManager.uiManager.SetChessNumFrame(redCheckersNum, blueCheckersNum);
                    }
                }

                if (checkers[i].checkerType == "Red" && i >= 56)//红棋子达到变王条件
                {
                    checkers[i].isKing = true;
                    checkers[i].gameObject.GetComponent<Image>().sprite = GameSceneManager.uiManager.redKingSprite;
                }
                else if (checkers[i].checkerType == "Blue" && i <= 7)//蓝棋子达到变王条件
                {
                    checkers[i].isKing = true;
                    checkers[i].gameObject.GetComponent<Image>().sprite = GameSceneManager.uiManager.blueKingSprite;
                }

                break;
            }
        }

        if (whoseTurn == "Red")
            whoseTurn = "Blue";
        else
            whoseTurn = "Red";

        GameSceneManager.uiManager.clock.ResetClock(whoseTurn);

        bool isWin;
        isWin = SelectLose(whoseTurn);

        GameSceneManager.uiManager.SetTurnPointer();

        if (!isWin && whoseTurn == "Red" && Globe.gameMode == GameMode.OnePerson) 
        {
            StartCoroutine(robot.Consider());
        }
    }
   
    private void ClearPointer()
    {
        foreach (Pointer pointerChild in pointerParent.GetComponentsInChildren<Pointer>())
        {
            pointerChild.transform.SetParent(this.gameObject.transform);//立即移出parent队列,否则robot在检测parent时会检测到被吃的棋子
            Destroy(pointerChild.gameObject);
        }
    }
    #endregion


    /// <summary>
    /// 清除所有游戏界面东西-棋盘\棋子 退出游戏界面时使用
    /// </summary>
    public void clearAll()
    {
        foreach(Checker child in redCheckersParent.GetComponentsInChildren<Checker>())
        {
            Destroy(child.gameObject);
        }
        foreach (Checker child in blueCheckersParent.GetComponentsInChildren<Checker>())
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// 是否可以吃子
    /// </summary>
    public bool IsCanEat(Checker checker,int n,int dir=0)//dir为方向，默认为0无方向，1为左上，2为右上，3为左下，4为右下，表示该判断为该方向吃子的再判断
    {
        bool isCanEat = false;
        if(checker!=null)
        {
            if (checker.isKing)
            {
                if (n - 9 - 9 >= 0 && dir != 4)
                {
                    if (boardPos[n - 18].tag == "Black" && checkers[n - 18] == null && checkers[n - 9] != null) 
                    {
                        if (checkers[n - 9].checkerType !=
                    checker.checkerType)
                        {
                            //左上可吃
                            isCanEat = true;
                            possibleDestinationPos.Add(boardPos[n - 18]);

                            pointers[n - 18] = new List<int>();
                            pointers[n - 18].Add(n-9);
                            if(pointers[n]!=null)
                            {
                                pointers[n - 18].AddRange(pointers[n]);

                                pointers[n] = null;
                            }

                            IsCanEat(checker, n - 18, 1);
                        }
                    }
                }
                if (n - 7 - 7 >= 0 && dir != 3)
                {
                    if (boardPos[n - 14].tag == "Black" && checkers[n - 14] == null && checkers[n - 7] != null) 
                    {
                        if (checkers[n - 7].checkerType !=
                    checker.checkerType)
                        {
                            //右上可吃
                            isCanEat = true;
                            possibleDestinationPos.Add(boardPos[n - 14]);

                            pointers[n - 14] = new List<int>();
                            pointers[n - 14].Add(n - 7);
                            if (pointers[n] != null)
                            {
                                pointers[n - 14].AddRange(pointers[n]);

                                pointers[n] = null;
                            }

                            IsCanEat(checker,n - 14, 2);
                        }
                    }
                }
                if (n + 7 + 7 < 64 && dir != 2)
                {
                    if (boardPos[n + 14].tag == "Black" && checkers[n + 14] == null && checkers[n + 7] != null) 
                    {
                        if (checkers[n + 7].checkerType !=
                    checker.checkerType)
                        {
                            //左下可吃
                            isCanEat = true;
                            possibleDestinationPos.Add(boardPos[n + 14]);

                            pointers[n + 14] = new List<int>();
                            pointers[n + 14].Add(n + 7);
                            if (pointers[n] != null)
                            {
                                pointers[n + 14].AddRange(pointers[n]);

                                pointers[n] = null;
                            }

                            IsCanEat(checker,n + 14, 3);
                        }
                    }
                }
                if (n + 9 + 9 < 64 && dir != 1)
                {
                    if (boardPos[n + 18].tag == "Black" && checkers[n + 18] == null && checkers[n + 9] != null) 
                    {
                        if (checkers[n + 9].checkerType !=
                    checker.checkerType)
                        {
                            //右下可吃
                            isCanEat = true;
                            possibleDestinationPos.Add(boardPos[n + 18]);

                            pointers[n + 18] = new List<int>();
                            pointers[n + 18].Add(n + 9);
                            if (pointers[n] != null)
                            {
                                pointers[n + 18].AddRange(pointers[n]);

                                pointers[n] = null;
                            }

                            IsCanEat(checker,n + 18, 4);
                        }
                    }
                }
            }
            else if (checker.checkerType == "Red")
            {
                if (n + 7 + 7 < 64 && dir != 2)
                {
                    if (boardPos[n + 14].tag == "Black" && checkers[n + 14] == null && checkers[n + 7] != null)
                    {
                        if (checkers[n + 7].checkerType !=
                    checker.checkerType)
                        {
                            //左下可吃
                            isCanEat = true;
                            possibleDestinationPos.Add(boardPos[n + 14]);

                            pointers[n + 14] = new List<int>();
                            pointers[n + 14].Add(n + 7);
                            if (pointers[n] != null)
                            {
                                pointers[n + 14].AddRange(pointers[n]);

                                pointers[n] = null;
                            }

                            IsCanEat(checker, n + 14, 3);
                        }
                    }
                }
                if (n + 9 + 9 < 64 && dir != 1)
                {
                    if (boardPos[n + 18].tag == "Black" && checkers[n + 18] == null && checkers[n + 9] != null)
                    {
                        if (checkers[n + 9].checkerType !=
                    checker.checkerType)
                        {
                            //右下可吃
                            isCanEat = true;
                            possibleDestinationPos.Add(boardPos[n + 18]);

                            pointers[n + 18] = new List<int>();
                            pointers[n + 18].Add(n + 9);
                            if (pointers[n] != null)
                            {
                                pointers[n + 18].AddRange(pointers[n]);

                                pointers[n] = null;
                            }

                            IsCanEat(checker, n + 18, 4);
                        }
                    }
                }
            }
            else if (checker.checkerType == "Blue")
            {
                if (n - 9 - 9 >= 0 && dir != 4)
                {
                    if (boardPos[n - 18].tag == "Black" && checkers[n - 18] == null && checkers[n - 9] != null)
                    {
                        if (checkers[n - 9].checkerType !=
                    checker.checkerType)
                        {
                            //左上可吃
                            isCanEat = true;
                            possibleDestinationPos.Add(boardPos[n - 18]);

                            pointers[n - 18] = new List<int>();
                            pointers[n - 18].Add(n - 9);
                            if (pointers[n] != null)
                            {
                                pointers[n - 18].AddRange(pointers[n]);

                                pointers[n] = null;
                            }

                            IsCanEat(checker, n - 18, 1);
                        }
                    }
                }
                if (n - 7 - 7 >= 0 && dir != 3)
                {
                    if (boardPos[n - 14].tag == "Black" && checkers[n - 14] == null && checkers[n - 7] != null)
                    {
                        if (checkers[n - 7].checkerType !=
                    checker.checkerType)
                        {
                            //右上可吃
                            isCanEat = true;
                            possibleDestinationPos.Add(boardPos[n - 14]);

                            pointers[n - 14] = new List<int>();
                            pointers[n - 14].Add(n - 7);
                            if (pointers[n] != null)
                            {
                                pointers[n - 14].AddRange(pointers[n]);

                                pointers[n] = null;
                            }

                            IsCanEat(checker, n - 14, 2);
                        }
                    }
                }
            }
        }
        if (isCanEat && possibleDestinationPos.Contains(boardPos[n]) && dir != 0) //该点能连吃，且可能的点有该位置，则删去该点（能连吃必须连吃）
        {
            possibleDestinationPos.Remove(boardPos[n]);
        }
        return isCanEat;
    }

    /// <summary>
    /// 响应返回主菜单标题界面的按钮
    /// </summary>
    public void ReturnButtonOnClick()
    {
        if(!GameSceneManager.hudManager.selectHUD.isActiveAndEnabled)
        {
            GameSceneManager.hudManager.yonHUD.message = "是否返回主菜单";
            GameSceneManager.hudManager.yonHUD.yesOrNoCallback
                = new Action<bool>(yesOrNo =>
                {
                    if (yesOrNo)
                    {
                    //返回主菜单
                    GameSceneManager.Instance.ReturnTitle();
                    }
                    else
                    {
                    //取消，隐藏HUD
                    GameSceneManager.hudManager.HideYONHUD();
                    }
                });
            GameSceneManager.hudManager.ShowYONHUD();
        }
    }

    /// <summary>
    /// 单人模式，选择先手按钮
    /// </summary>
    public void SelectFirst()
    {
        GameSceneManager.hudManager.selectHUD.message = "选择先手顺序";
        GameSceneManager.hudManager.selectHUD.selectCallback
            = new Action<int>(select =>
            {
                switch(select)
                {
                    case 0:
                        //玩家优先
                        whichFirst = 0;
                        whoseTurn = "Blue";
                        GameSceneManager.uiManager.SetTurnPointer();
                        GameSceneManager.hudManager.HideSelectHUD();
                        foreach(Checker child in blueCheckersParent.GetComponentsInChildren<Checker>())
                        {
                            child.GetComponent<Button>().enabled = true;
                        }
                        break;
                    case 1:
                        //AI优先
                        whichFirst = 1;
                        whoseTurn = "Red";
                        GameSceneManager.uiManager.SetTurnPointer();
                        GameSceneManager.hudManager.HideSelectHUD();
                        foreach (Checker child in blueCheckersParent.GetComponentsInChildren<Checker>())
                        {
                            child.GetComponent<Button>().enabled = true;
                        }
                        StartCoroutine(robot.Consider());
                        break;
                    default:
                        break;
                }
            });
        GameSceneManager.hudManager.ShowSelectHUD();
    }

    #region 判断并选择输赢
    public bool SelectLose(string side)
    {
        //判断输赢
        //side为0时，判断蓝方是否输；side为1时，判断红方是否输
        bool isLose = false;
        switch(side)
        {
            case "Blue":
                if (blueCheckersNum<=0)
                    isLose = true;
                else if (IsNoneWay(blueCheckersParent))
                    isLose = true;
                break;
            case "Red":
                if (redCheckersNum<=0)
                    isLose = true;
                else if (IsNoneWay(redCheckersParent))
                    isLose = true;
                break;
            default:
                break;
        }

        //选择输赢
        if(isLose)
        {
            SetWinner(side);
        }
        return isLose;
    }
    public void SetWinner(string side)
    {
        for (int i = 0; i < 64; i++)
        {
            //对所有棋子去掉button
            if (checkers[i] != null)
                Destroy(checkers[i].GetComponent<Button>());
        }
        GameSceneManager.hudManager.ShowWinner(side);
    }
    //private bool IsClear(GameObject parent)
    //{
    //    //判断红蓝方棋子是否为0
    //    if (parent.transform.childCount == 0)
    //        return true;
    //    else
    //        return false;
    //}
    private bool IsNoneWay(GameObject parent)
    {
        bool isNoneWay = true;
        foreach(Checker child in parent.GetComponentsInChildren<Checker>())
        {
            List<GameObject> possiblePos = Analyse(child, child.checkerType);
            if(possiblePos.Count>0)
            {
                //存在棋子有路
                isNoneWay = false;
                break;
            }
        }
        return isNoneWay;
    }
    #endregion
}
