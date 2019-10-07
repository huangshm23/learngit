using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mygame;

public class Model : MonoBehaviour
{

    Stack<GameObject> start_priests = new Stack<GameObject>();
    Stack<GameObject> end_priests = new Stack<GameObject>();
    Stack<GameObject> start_devils = new Stack<GameObject>();
    Stack<GameObject> end_devils = new Stack<GameObject>();
    
    GameObject[] boat = new GameObject[2];
    GameObject boat_obj;
    public float speed = 50;

    SSDirector one;

    //对象的位置
    Vector3 boatStartPos = new Vector3(-2, 0.25f, 0);
    Vector3 boatEndPos = new Vector3(2, 0.25f, 0);
    Vector3 sideStartPos = new Vector3(-5, 0, 0);
    Vector3 sideEndPos = new Vector3(5, 0, 0);

    Vector3 riverPos = new Vector3(0, -0.25f, 0); 

    float gap = 1.1f;
    Vector3 priestsStartPos = new Vector3(-3.25f, 0.75f, 0);
    Vector3 priestsEndPos = new Vector3(3.25f, 0.75f, 0);
    Vector3 devilsStartPos = new Vector3(-5, 1, 0);
    Vector3 devilsEndPos = new Vector3(5, 1, 0);


    // Start is called before the first frame update
    void Start()
    {
        one = SSDirector.GetInstance();
        one.setModel(this);
        loadSrc();
    }

    // Update is called once per frame
    void Update()
    {
        setpositionS(start_priests, priestsStartPos);
        setpositionE(end_priests, priestsEndPos);
        setpositionS(start_devils, devilsStartPos);
        setpositionE(end_devils, devilsEndPos);

        if (one.state == State.SE_move)
        {
            boat_obj.transform.position = Vector3.MoveTowards(boat_obj.transform.position, boatEndPos, Time.deltaTime * speed);
            if (boat_obj.transform.position == boatEndPos)
            {
                one.state = State.End;
            }
            checkD();
        }
        else if (one.state == State.ES_move)
        {
            boat_obj.transform.position = Vector3.MoveTowards(boat_obj.transform.position, boatStartPos, Time.deltaTime * speed);
            if (boat_obj.transform.position == boatStartPos)
            {
                one.state = State.Start;
            }
            checkD();
        }
        checkV();
    }

    //加载游戏对象
    void loadSrc()
    {
        //sides
        Instantiate(Resources.Load("side"), sideStartPos, Quaternion.identity);
        Instantiate(Resources.Load("side"), sideEndPos, Quaternion.identity);

        //river
        Instantiate(Resources.Load("river"), riverPos, Quaternion.identity);

        //boat
        boat_obj = Instantiate(Resources.Load("boat"), boatStartPos, Quaternion.identity) as GameObject;

        //prisets and devils
        for (int i = 0; i < 3; i++)
        {
            start_priests.Push(Instantiate(Resources.Load("Priest")) as GameObject);
            start_devils.Push(Instantiate(Resources.Load("Devil")) as GameObject);
        }
    }

    //设置起点对象位置
    void setpositionS(Stack<GameObject> aaa, Vector3 pos)
    {
        GameObject[] temp = aaa.ToArray();
        for (int i = 0; i < aaa.Count; i++)
        {
            temp[i].transform.position = pos + new Vector3(-gap * i * 0.5f, 0, 0);
        }
    }

    //设置终点对象位置
    void setpositionE(Stack<GameObject> aaa, Vector3 pos)
    {
        GameObject[] temp = aaa.ToArray();
        for (int i = 0; i < aaa.Count; i++)
        {
            temp[i].transform.position = pos + new Vector3(gap * i * 0.5f, 0, 0);
        }
    }

        //上船
        void getOnTheBoat(GameObject obj)
    {
        obj.transform.parent = boat_obj.transform;
        if (boatNum() != 0)
        {
            if (boat[0] == null)
            {
                boat[0] = obj;
                obj.transform.position = boat_obj.transform.position + new Vector3(0.3f, obj.transform.position.y - 0.25f, 0);
            }
            else
            {
                boat[1] = obj;
                obj.transform.position = boat_obj.transform.position + new Vector3(-0.3f, obj.transform.position.y - 0.25f, 0);
            }
        }
    }
    //判断船上的空位数
    int boatNum()
    {
        int num = 0;
        for (int i = 0; i < 2; i++)
        {
            if (boat[i] == null)
            {
               ++ num;
            }
        }
        return num;
    }

    //船移动
    public void moveBoat()
    {
        if (boatNum() != 2)
        {
            if (one.state == State.Start)
            {
                one.state = State.SE_move;
            }
            else if (one.state == State.End)
            {
                one.state = State.ES_move;
            }
        }
    }

    //下船
    public void getOffTheBoat(int flag)
    {
        if (flag == 0 || flag == 2)
        {
            for (int i = 0; i < 2; ++i)
            {
                if (boat[i] != null && boat[i].tag == "Priest")
                {
                    Debug.Log("enter1");
                    if (flag == 0)
                    {
                        start_priests.Push(boat[i]);
                    }
                    else
                    {
                        end_priests.Push(boat[i]);
                    }
                    boat[i] = null;
                    break;
                }
            }
        }
        else if (flag == 1 || flag == 3)
        {
            for (int i = 0; i < 2; ++i)
            {
                if (boat[i] != null && boat[i].tag == "Devil")
                {
                    if (flag == 1)
                    {
                        start_devils.Push(boat[i]);
                    }
                    else
                    {
                        end_devils.Push(boat[i]);
                    }
                    boat[i] = null;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < 2; ++i)
            {
                if (boat[i] != null)
                {   
                    if (boat[i].tag == "Devil")
                    {
                        start_devils.Push(boat[i]);
                    }
                    else
                        start_priests.Push(boat[i]);
                    boat[i] = null;
                }
            }
        }
    }

    //检查是否失败
    void checkD()
    {
        int bp = 0, bd = 0;
        for (int i = 0; i < 2; i++)
        {
            if (boat[i] != null && boat[i].tag == "Priest")
            {
                bp++;
            }
            else if (boat[i] != null && boat[i].tag == "Devil")
            {
                bd++;
            }
        }

        int sp = start_priests.Count, sd = start_devils.Count, ep = end_priests.Count, ed = end_devils.Count;
        if (one.state == State.Start)
        {
            int sum = sp + bp;
            if ((sum != 0 && sum < (sd + bd)) || (ep != 0 && ep < ed))
                one.state = State.Lose;
        }
        else
        {
            int sum = ep + bp;
            if ((sp != 0 && sp < sd) || (sum != 0 && sum < ed + bd))
                one.state = State.Lose;
        }
    }

    //检查是否胜利
    void checkV()
    {
        if (end_devils.Count == 3 && end_priests.Count == 3)
        {
            one.state = State.Win;
            return;
        }
    }

    //游戏对象从岸上到船上的变化
    public void priS()
    {
        if (start_priests.Count != 0 && boatNum() != 0 && one.state == State.Start)
        {
            getOnTheBoat(start_priests.Pop());
        }
    }
    public void priE()
    {
        if (end_priests.Count != 0 && boatNum() != 0 && one.state == State.End)
        {
            getOnTheBoat(end_priests.Pop());
        }
    }
    public void delS()
    {
        if (start_devils.Count != 0 && boatNum() != 0 && one.state == State.Start)
        {
            getOnTheBoat(start_devils.Pop());
        }
    }
    public void delE()
    {
        if (end_devils.Count != 0 && boatNum() != 0 && one.state == State.End)
        {
            getOnTheBoat(end_devils.Pop());
        }
    }

    //重置游戏
    public void Reset()
    {
        boat_obj.transform.position = boatStartPos;
        one.state = State.Start;

        int num1 = end_devils.Count, num2 = end_priests.Count;
        for (int i = 0; i < num1; i++)
        {
            Debug.Log(i);
            start_devils.Push(end_devils.Pop());
        }

        for (int i = 0; i < num2; i++)
        {
            start_priests.Push(end_priests.Pop());
        }
        if (boatNum() != 2)
        {
            getOffTheBoat(4);
        }
    }
}
