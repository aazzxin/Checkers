using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Robot : MonoBehaviour {

    public int[] checkers;
    public int redCheckersNum, blueCheckersNum;

    GameObject pointerParent;
    GameObject checkerParent;

    int[] cantCheckersPos;
    CantWay[] cantWays;

    public List<int> lastChecker;//上一步的棋子位置
    public List<int> lastPoint;//上一步的落点位置

    float delay;//伪思考时间

    struct CantWay
    {
        public int check;//棋子位置
        public List<int> points;//可能的落点位置
        public List<int> eatenNum;//对应落点位置的被吃子数
    }
    private void Start()
    {
        //PlayerPrefs.DeleteAll();

        delay = 1.0f;

        checkerParent = GameSceneManager.runManager.redCheckersParent;
        pointerParent = GameSceneManager.runManager.pointerParent;
    }
    public void InitializeRobot()
    {
        lastChecker = new List<int>();
        lastPoint = new List<int>();

        checkers = new int[64];
        checkers[0] = -10;//表示仍未录入棋盘信息，电脑仍未下第一步棋
        redCheckersNum = blueCheckersNum = 12;
    }

    public void SetCheckers()
    {
        for (int i = 0; i < 64; i++)
        {
            if (GameSceneManager.runManager.checkers[i] == null)
                checkers[i] = -1;
            else if (GameSceneManager.runManager.checkers[i].checkerType == "Red")
                checkers[i] = 1;
            else if (GameSceneManager.runManager.checkers[i].checkerType == "Blue")
                checkers[i] = 0;
        }
    }

    private string GetCheckersString()
    {
        string checkersString = "";
        for (int i = 0; i < 64; i++)
        {
            checkersString += checkers[i];
            checkersString += ",";
        }
        checkersString = checkersString.Substring(0, checkersString.Length - 1);//去掉最后的','
        return checkersString;
    }
    private string GetCheckersWayString()
    {
        if (cantWays != null)
        {
            string checkersWayString = "";
            foreach (CantWay canWay in cantWays)
            {
                checkersWayString += canWay.check.ToString();
                for (int i = 0; i < canWay.points.Count; i++)
                {
                    checkersWayString += '-';
                    checkersWayString += canWay.points[i].ToString();
                    checkersWayString += '_';
                    checkersWayString += canWay.eatenNum[i].ToString();
                }
                checkersWayString += ',';
            }
            checkersWayString = checkersWayString.Substring(0, checkersWayString.Length - 1);//去掉最后的','
            return checkersWayString;
        }
        else
            return null;
    }

    private void SetCantCheckersPos(string cantCheckersString)
    {
        string[] cantCheckers = cantCheckersString.Split(',');
        cantCheckersPos = new int[cantCheckers.Length];
        for (int i = 0; i < cantCheckers.Length; i++)
        {
            cantCheckersPos[i] = (System.Convert.ToInt32(cantCheckers[i]));
        }
    }
    private void SetCantWays(string cantCheckersWayString)
    {
        //例如"12-16_1-18_2,44-36_0",表示12位置的棋子被否认的落点为16（将被吃1个棋子）和18（将被吃2个棋子），44位置棋子被否认的落点为36（将不被吃掉棋子）
        if (cantCheckersWayString[0] == '~')//若为最佳解决方案
        {
            cantWays = new CantWay[1];
            string[] checkWayString = cantCheckersWayString.Substring(1).Split('-');
            cantWays[0].check = System.Convert.ToInt32(checkWayString[0]);
            cantWays[0].points = new List<int>();
            cantWays[0].points.Add(System.Convert.ToInt32(checkWayString[1]));
            cantWays[0].eatenNum = new List<int>();
        }
        else
        {
            string[] cantCheckers = cantCheckersWayString.Split(',');
            cantWays = new CantWay[cantCheckers.Length];
            for (int i = 0; i < cantCheckers.Length; i++)
            {
                string[] checkWayString = cantCheckers[i].Split('-');
                cantWays[i].check = System.Convert.ToInt32(checkWayString[0]);
                cantWays[i].points = new List<int>();
                cantWays[i].eatenNum = new List<int>();
                for (int j = 1; j < checkWayString.Length; j++)
                {
                    string[] checkAndWay = checkWayString[j].Split('_');
                    cantWays[i].points.Add(System.Convert.ToInt32(checkAndWay[0]));
                    cantWays[i].eatenNum.Add(System.Convert.ToInt32(checkAndWay[1]));
                }
            }
        }
       
    }

    /// <summary>
    /// 思考的主函数
    /// </summary>
    public IEnumerator Consider()
    {
        if(checkers[0]==-10)
        {
            SetCheckers();//录入棋盘
            StartCoroutine(SelectOneWay());
        }
        else
        {
            if (redCheckersNum > GameSceneManager.runManager.redCheckersNum)//被吃子
            {
                int eatenNum = redCheckersNum - GameSceneManager.runManager.redCheckersNum;//吃子数
                redCheckersNum = GameSceneManager.runManager.redCheckersNum;

                string checkerString = GetCheckersString();
                string wayString = PlayerPrefs.GetString(checkerString, "");
                string[] stringAray = wayString.Split(';');

                string cantCheckersString = stringAray[0];
                if (cantCheckersString != "")
                    SetCantCheckersPos(cantCheckersString);

                string cantCheckersWayString = stringAray[1];
                if (cantCheckersWayString != "")
                    SetCantWays(cantCheckersWayString);

                if (cantCheckersWayString != "")
                {
                    if (cantCheckersWayString[0] == '~')//若为最佳解决方案,则不Ban掉
                    {
                        SetCheckers();//录入棋盘
                        StartCoroutine(SelectOneWay());
                        yield break;
                    }
                }

                //否则ban掉该落点
                if(cantCheckersWayString != "")
                {
                    bool isFindAnOldCheck = false;//是否找到该棋子的另外路线被Ban掉
                    foreach (CantWay cantWay in cantWays)
                    {
                        if (cantWay.check == lastChecker[lastChecker.Count - 1]) //若已经存在
                        {
                            //Debug.Log(cantCheckersWayString);
                            //Debug.Log(lastPoint[lastPoint.Count-1]);
                            //foreach(int p in cantWay.points)
                            //{
                            //    Debug.Log("cantWayCount:" + cantWay.points.Count);
                            //    Debug.Log("cantWay:" + p);
                            //}
                            cantWay.points.Add(lastPoint[lastPoint.Count - 1]);
                            cantWay.eatenNum.Add(eatenNum);
                            isFindAnOldCheck = true;
                            break;
                        }
                    }
                    if (!isFindAnOldCheck)//若不存在
                    {
                        CantWay[] temp = new CantWay[cantWays.Length + 1];
                        cantWays.CopyTo(temp, 0);
                        temp[cantWays.Length].check = lastChecker[lastChecker.Count - 1];
                        temp[cantWays.Length].points = new List<int>();
                        temp[cantWays.Length].points.Add(lastPoint[lastPoint.Count - 1]);
                        temp[cantWays.Length].eatenNum = new List<int>();
                        temp[cantWays.Length].eatenNum.Add(eatenNum);
                        cantWays = temp;
                    }
                }
                else
                {
                    cantWays = new CantWay[1];
                    cantWays[0].check = lastChecker[lastChecker.Count - 1];
                    cantWays[0].points = new List<int>();
                    cantWays[0].points.Add(lastPoint[lastPoint.Count - 1]);
                    cantWays[0].eatenNum = new List<int>();
                    cantWays[0].eatenNum.Add(eatenNum);
                }
                
                cantCheckersWayString = GetCheckersWayString();
                wayString = cantCheckersString + ';' + cantCheckersWayString;
                PlayerPrefs.SetString(checkerString, wayString);

                yield return new WaitForSeconds(4 * delay);
                //GameSceneManager.runManager.MoveBack();//悔棋
                SetCheckers();//然后录入棋盘
                StartCoroutine(SelectOneWay());
            }
            else
            {
                SetCheckers();//录入棋盘
                StartCoroutine(SelectOneWay());
            }
        }
    }

    private IEnumerator SelectOneWay()
    {
        string checkerString = GetCheckersString();
        string wayString = PlayerPrefs.GetString(checkerString, "");
        if (wayString == "")//找不到解决方案
        {
            List<Checker> checkList = new List<Checker>();
            string cantCheckersString = "";
            foreach (Checker child in checkerParent.GetComponentsInChildren<Checker>())
            {
                List<GameObject> possiblePos = GameSceneManager.runManager.Analyse(child, "Red");
                if (possiblePos.Count <= 0)
                {
                    cantCheckersString += child.positionNum;
                    cantCheckersString += ",";
                }
                else
                    checkList.Add(child);
            }
            wayString += cantCheckersString;
            if (cantCheckersString != "")
                wayString = wayString.Substring(0, wayString.Length - 1);//去掉最后的','

            wayString += ";";
            PlayerPrefs.SetString(checkerString, wayString);

            int ranNum = Random.Range(0, checkList.Count);
            GameSceneManager.runManager.OnClickChecker(checkList[ranNum].gameObject);
            yield return new WaitForSeconds(delay);
            lastChecker.Add(checkList[ranNum].positionNum);

            ranNum = Random.Range(0, pointerParent.transform.childCount);
            GameObject pointer = pointerParent.transform.GetChild(ranNum).gameObject;//选择某个落点
            GameSceneManager.runManager.OnClickPointer(pointer);//执行落点
            lastPoint.Add(pointer.GetComponent<Pointer>().positionNum);
            yield break;
        }
        else
        {
            string[] stringAray = wayString.Split(';');

            string cantCheckersString = stringAray[0];
            if (cantCheckersString != "")
                SetCantCheckersPos(cantCheckersString);

            string cantCheckersWayString = stringAray[1];
            if (cantCheckersWayString != "")
                SetCantWays(cantCheckersWayString);

            if (cantCheckersWayString != "")
            {
                if (cantCheckersWayString[0] == '~')//若为最佳解决方案
                {
                    GameSceneManager.runManager.OnClickChecker(GameSceneManager.runManager.checkers[cantWays[0].check].gameObject);
                    yield return new WaitForSeconds(delay);
                    lastChecker.Add(cantWays[0].check);

                    foreach (Pointer point in pointerParent.GetComponentsInChildren<Pointer>())
                    {
                        if (point.positionNum == cantWays[0].points[0])
                        {
                            GameSceneManager.runManager.OnClickPointer(point.gameObject);
                            lastPoint.Add(point.positionNum);
                            yield break;
                        }
                    }
                }
            }

            List<Checker> checkList = new List<Checker>();

            foreach (Checker child in checkerParent.GetComponentsInChildren<Checker>())
            {
                int takeoff = 0;
                foreach (int cantPos in cantCheckersPos)
                {
                    if (child.positionNum == cantPos)//若该棋子无路走
                    {
                        takeoff = 1;
                        break;
                    }
                }
                if (takeoff == 0)//否则
                {
                    //Debug.Log("checkers" + child.positionNum);
                    checkList.Add(child);
                }

            }
            //bool onlyOneCheck = false;
            //if (checkList.Count == 1)
            //    onlyOneCheck = true;
            while (true)
            {
                int ranNum = Random.Range(0, checkList.Count);
                if (checkList.Count >= 1)
                    GameSceneManager.runManager.OnClickChecker(checkList[ranNum].gameObject);
                else
                {
                    //当所有落点均被ban掉时，选择最少被吃子的路线作为最优选择
                    int bestCheck = cantWays[0].check, bestPoint = cantWays[0].points[0];
                    int leastEatenCount = cantWays[0].points[0];
                    foreach (CantWay cantway in cantWays)
                    {
                        for (int i = 0; i < cantway.points.Count; i++)
                        {
                            int count = cantway.eatenNum[i];
                            if (count < leastEatenCount)
                            {
                                leastEatenCount = count;
                                bestCheck = cantway.check;
                                bestPoint = cantway.points[i];
                            }
                        }
                    }
                    wayString = "";
                    wayString += cantCheckersString;
                    wayString += ";~";
                    wayString += bestCheck.ToString();
                    wayString += "-";
                    wayString += bestPoint.ToString();
                    PlayerPrefs.SetString(checkerString, wayString);
                    //再进行一次选择路线
                    StartCoroutine(SelectOneWay());
                    yield break;
                }
                yield return new WaitForSeconds(delay);
                //lastChecker.Add(checkList[ranNum].positionNum);

                int takeoff = 0;
                if (cantCheckersWayString != "")
                {
                    foreach (CantWay cantWay in cantWays)
                    {
                        if (checkList[ranNum].positionNum == cantWay.check) //若该棋子有某些路数被否认
                        {
                            takeoff = 1;

                            bool isAllBan = true;//是否该棋子的所有落点都被否认
                            foreach (Pointer point in pointerParent.GetComponentsInChildren<Pointer>())
                            {
                                //Debug.Log(cantCheckersWayString);
                                //Debug.Log(point.positionNum);
                                //foreach (int p in cantWay.points)
                                //{
                                //    Debug.Log("cantWayPointCount:" + cantWay.points.Count);
                                //    Debug.Log("cantWayPoint:" + p);
                                //}

                                if (!cantWay.points.Contains(point.positionNum))//不包含该落点
                                {
                                    isAllBan = false;

                                    //ranNum = Random.Range(0, pointerParent.transform.childCount);
                                    //Pointer pointer = pointerParent.transform.GetChild(ranNum).GetComponent<Pointer>();

                                    lastChecker.Add(checkList[ranNum].positionNum);
                                    GameSceneManager.runManager.OnClickPointer(point.gameObject);//执行落点
                                    lastPoint.Add(point.positionNum);
                                    yield break;
                                }
                                else
                                {
                                    //point.transform.SetParent(this.transform);
                                    //Destroy(point.gameObject);//若包含该落点，则清除该落点
                                }
                            }
                            if (isAllBan)
                            {
                                checkList.Remove(checkList[ranNum]);//清除该棋子选择
                                break;
                            }
                            //break;
                        }
                    }
                }
                if (takeoff == 0)//否则
                {
                    if(pointerParent.transform.childCount!=0)//还存在可能落点
                    {
                        lastChecker.Add(checkList[ranNum].positionNum);

                        ranNum = Random.Range(0, pointerParent.transform.childCount);
                        GameObject pointer = pointerParent.transform.GetChild(ranNum).gameObject;//选择某个落点

                        if (pointerParent.transform.childCount == 1 && checkList.Count == 1) //只有唯一该落点可能,设置为最优解决方案
                        {
                            wayString = "";
                            wayString += cantCheckersString;
                            wayString += ";~";
                            wayString += checkList[0].positionNum.ToString();
                            wayString += "-";
                            wayString += pointer.GetComponent<Pointer>().positionNum;
                            PlayerPrefs.SetString(checkerString, wayString);
                        }

                        GameSceneManager.runManager.OnClickPointer(pointer);//执行落点
                       
                        lastPoint.Add(pointer.GetComponent<Pointer>().positionNum);
                        yield break;
                    }
                }
            }
        }
    }
}
