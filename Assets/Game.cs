using Sequence = System.Collections.IEnumerator;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

public sealed class Game : GameBase
{
    // 変数の宣言
    private int height = 1080;
    private int width = 1920;
    //private int gameScene = -1;
    private int gameScene = 1;
    //private int gameScene = 2;
    private int time = 0;
    private const int music_end = 5820;
    private int hanbetuBarX = 200;  //タイミングが合っているか判断するための棒の横幅

    //ノーツの情報
    //楽譜(gc.Save, gc.Loadが異なるデバイスでうまくできないから脳死で代入)
    private int[] notes = new int[125]
    {
        71, 112, 156, 201, 247, 272, 293, 339, 383, 427, 473, 517, 562, 607, 619, 653, 697, 709, 743, 790, 815, 876,
        902, 968, 1013, 1058, 1105, 1150, 1173, 1239, 1262, 1285, 1329, 1374, 1419, 1464, 1476, 1515, 1559, 1602, 1649,
        1693, 1738, 1784, 1825, 1871, 1917, 1960, 2007, 2051, 2093, 2142, 2164, 2188, 2236, 2280, 2327, 2369, 2415,
        2456, 2501, 2547, 2596, 2640, 2686, 2732, 2774, 2816, 2861, 2904, 2918, 2985, 3021, 3033, 3089, 3138, 3201,
        3225, 3247, 3269, 3315, 3366, 3418, 3474, 3497, 3521, 3563, 3608, 3657, 3679, 3702, 3744, 3769, 3790, 3857,
        3901, 3947, 3993, 4039, 4084, 4127, 4172, 4218, 4289, 4300, 4355, 4401, 4445, 4490, 4536, 4578, 4625, 4668,
        4713, 4724, 4762, 4817, 4868, 4918, 4940, 4964, 4987, 5029, 5073, 5120
    };
    private int[] notes_x = new int[0];  //現在使われている各ノーツのx座標
    private int notes_idx = 0;  //現在使われているノーツの合計
   private int notes_lost = 0;  //ミスしたノーツの数
    private int notes_sum = 124;  //すべてのノーツの個数
    private const int note_x = 1960;
    private int note_y = 175;
    int note_w = 50;
    int note_h = 50;

    //プレイヤー情報
    private int playerHP = 10;
    private int player_x = 150;
    private int player_y = 550;
    private int player_action = 0;  //プレイヤーアクションの処理が必要か
    private int player_time = 0;  //プレイヤーアクションの時間計測


    public override void InitGame()
    {
        // キャンバスの大きさを設定します
        gc.SetResolution(width, height);
        gc.SetSoundVolume(1);
        
        if (gameScene != -1)
        {
            //notes_sum = gc.Load(-1);
            //Array.Resize(ref notes, notes_sum+1);
            //for(int i=0; i<=notes_sum; i++)
                //notes[i] = gc.Load(i) - 62;  //生成場所から判定場所まで１フレーム30px進むとしたら62フレーム必要
        }
    }

    public override void UpdateGame()
    {
        if (gameScene == -1)  //楽譜の作成
        {
            CreateNotes();
            time++;
        }

        if (gameScene == 1)
        {
            for (int i = 0; i <= notes_sum; i++)
            {
                if (time == notes[i])
                    AddNotes();
            }

            if (time == music_end)
                gameScene = 2;

            time++;
        }
       
        
    }

    public override void DrawGame()
    {
        gc.ClearScreen();

        if (gameScene == -1)
        {
            gc.PlaySound(0, false);
            //gc.DrawImage(1, 0, 0);
        }

        if (gameScene == 0)
        {
            
        }
        
        if (gameScene == 1)
        {
            gc.PlaySound(0, false);
            CreateMainScreen();
            
            for (int i = 0; i < notes_idx; i++)  //ノーツの移動
            {
                gc.DrawImage(0, notes_x[i]-106, note_y-75);  //赤血球
                gc.FillRect(notes_x[i], note_y, note_w, note_h);  //判定用の正方形
                notes_x[i] -= 30;
            }
            
            if (gc.GetPointerFrameCount(0) == 1)
                //ノーツをタイミングよくたっぷできたとき
                for(int i=0; i<notes_idx; i++)
                    if (gc.CheckHitRect(notes_x[i], note_y, note_w, note_h, 100, 100, hanbetuBarX, 200))
                    {
                        notes_x[i] = notes_x[notes_idx - 1];
                        notes_idx--;
                        player_action = 1;
                        player_time = 0;
                    }

            if (player_action == 1)  //プレイヤーのアクション処理
            {
                if (player_time % 3 == 0)
                    player_x += 50;

                if (player_time == 9)
                {
                    player_action = 0;
                    player_x = 150;
                }
            }
            player_time++;

            for (int i = 0; i < notes_idx; i++)  //ノーツをタイミングよくタッチできなかった時の処理
                if (notes_x[i] <= -156)
                {
                    notes_x[i] = notes_x[notes_idx - 1];
                    notes_idx--;
                    notes_lost++;
                    playerHP--;
                }

            gc.DrawImage(1, player_x, player_y);  //プレイヤー(白血球)
            DrawHPbar("player");
        }

        if (gameScene == 2)
        {
            gc.ClearScreen();
        }
    }

    void CreateNotes()
    {
        if (gc.GetPointerFrameCount(0) == 1)
        {
            gc.Save(notes_idx, time);  //ノーツの生成タイミングを記憶
            gc.Save(-1, notes_idx);  //ノーツの数を記憶
            notes_idx++;
        }
    }

    void CreateMainScreen()
    {
        gc.SetColor(140, 0, 0, 150);  //濃い赤
        gc.DrawImage(6, 0, 0);  //背景画像
        gc.FillRect(0, 100, width, 200);  //ノーツが流れてくるレーン
        gc.SetColor(0, 0, 0);  //黒
        gc.FillRect(100, 100, hanbetuBarX, 200);  //タイミングが合っているかあっていないか判断する棒
    }

    void AddNotes()
    {
        notes_idx++;
        Array.Resize(ref notes_x, notes_idx);
        notes_x[notes_idx - 1] = note_x;
    }

    void DrawHPbar(string mode)
    {
        if (mode == "player")
        {
            gc.SetColor(125, 125, 125);
            gc.FillRect(150, 480, 500, 30);
            gc.SetColor(0, 255, 0);
            gc.FillRect(150, 480, playerHP*50, 30);
        }
    }
}