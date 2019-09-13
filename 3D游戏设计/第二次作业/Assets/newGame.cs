using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newGame : MonoBehaviour
{   
    //引入图片
    public Texture letterX;
    public Texture circle;
    public Texture2D backGround;

    private int[,] board = new int[3, 3];           //储存棋盘信息,0为X，1为O，
    private int turn = 1;               //标记轮到谁,1为O，0为X
    private int count = 0;              //统计棋盘棋子数量
    private bool finished = false;      //标记游戏是否结束
    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(100, 50, 350, 100), "Tic - Tac - Toe");   //游戏标题
        if (GUI.Button(new Rect(400, 50, 100, 50), "RESET")) {   //重置游戏按钮
            Reset();
        }

        //游戏过程
        for (int i = 0; i < 3; ++ i) {
            for (int j = 0; j < 3; ++ j) {
                //模拟轮次点击过程
                if (board[i, j] == 1) {
                    GUI.Button(new Rect(400 + 100 * i, 150 + 100 * j, 100, 100), circle);
                }
                else if (board[i, j] == 0) {
                    GUI.Button(new Rect(400 + 100 * i, 150 + 100 * j, 100, 100), letterX);
                }
                else {
                    if (GUI.Button(new Rect(400 + 100 * i, 150 + 100 * j, 100, 100), backGround)) {
                        if (finished == true)       //游戏结束便保持现状
                            continue;
                        count++;
                        if (turn == 1) {
                            board[i, j] = 1;
                            turn = 0;
                        }
                        else if (turn == 0) {
                            board[i, j] = 0;
                            turn = 1;
                        }
                    }
                }
            }
        }

        //得到结果并输出
        if (count >= 5)
        {
            int State = getResult();        //-2表示游戏未结束，-1表示平局，0表示X胜利，1表示O胜利
            if (State == 0) {
                GUI.Label(new Rect(400, 100, 100, 50), "X WIN!");
                finished = true;
            }
            else if (State == 1) {
                GUI.Label(new Rect(400, 100, 100, 50), "O WIN!");
                finished = true;
            }
            else if (State == -1 && count == 9) {
                GUI.Label(new Rect(400, 100, 210, 50), "NO ONE WIN");
            }
        }
    }

    int getResult()
    {
        //检验行是否存在满足胜利的条件
        for (int i = 0; i < 3; ++ i) {
            if (board[i, 0] == board[i, 1] && board[i, 0] == board[i, 2] && board[i, 0] != -1) {
                return board[i, 0]; 
            }
        }
        //检验列是否存在满足胜利的条件
        for (int j = 0; j < 3; ++ j) {
            if (board[0, j] == board[1, j] && board[0, j] == board[2, j] && board[0, j] != -1) {
                return board[0, j]; 
            }
        }

        //检验对角线是否存在满足胜利的条件
        if (board[0, 0] == board[1, 1] && board[0, 0] == board[2, 2] && board[0, 0] != -1)
            return board[0, 0];

        if (board[0, 2] == board[1, 1] && board[0, 2] == board[2, 0] && board[0, 2] != -1)
            return board[0, 2];

        //判断是否平局
        int counts = 0;
        for (int i = 0; i < 3; ++ i) {
            for (int j = 0; j < 3; ++ j) {
                if (board[i, j] != -1) {
                    counts++;
                }
                else
                    return -2;
            }
        }
        if (counts == 9) {
            return -1;
        }
        return -2;
    }

    // Update is called once per frame
    void Update()
    {

    }
    //重置游戏
    void Reset()
    {
        turn = 1;
        count = 0;
        finished = false;
        for (int i = 0; i < 3; ++ i) {
            for (int j = 0; j < 3; ++ j) {
                board[i, j] = -1;
            }
        }
    }
}
