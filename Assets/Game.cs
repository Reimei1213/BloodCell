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
    private int gameScene = 0;
    //private int gameScene = 2;
    private int time = 0;
    private const int music_end = 5820;
    int music = 0;
    private int gameOverFlag = 0;
    private int gimic_flag = 0;  //タイトル画面のギミック
    private int gimic_x = 1920;
    private Single gimic_deg = 0;
    private int combo = 0;
    private int comboMax = 0;
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
    
    //敵の情報  3:エボラウイルス　4:コロナウイルス  5:MERSウイルス
    private int[] enemy1 = new int[7] {0, 950, 400, 0, 0, 0, 0}; //敵の種類、ｘ、ｙ、ＨＰ、action, time, flag
    private int[] enemy2 = new int[7] {0, 1450, 600, 0, 0, 0, 0}; //敵の種類、ｘ、ｙ、ＨＰ、action, time, flag
    private int[] enemy_kill = new int[3] {0, 0, 0};
    
    //ブドウ糖の情報
    private int enegy_y = 500;  //ブドウ糖のｙ座標
    private int enegy_action = 0;  //ブドウ糖のアクションが必要か
    private int enegy_time = 0;  //ブドウ糖アクションの時間計測
    private int alp = 250;
    private int[] effect_x = new int[30];  //ブドウ糖使用時のエフェクトのx座標
    private int[] effect_y = new int[30];  // //ブドウ糖使用時のエフェクトのy座標
    private int heart_flag = 1; //心臓の画像の判定
    
    

    public override void InitGame()
    {
        // キャンバスの大きさを設定します
        gc.SetResolution(width, height);
        gc.SetSoundVolume(10);

        CreateEffects();
        
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

            if (time == music_end)  //音楽が終了したら結果の画面へ
                gameScene = 2;

            if (enemy1[6] == 0 & enemy2[6] == 0)  //敵の生成
            {
                enemy1[0] = gc.Random(3, 5);
                enemy1[3] = enemy1[0];
                enemy1[6] = 1;
                enemy2[0] = gc.Random(3, 5);
                enemy2[3] = enemy2[0];
                enemy2[6] = 1;
            }

            if (enemy1[3] == 0)
                enemy1[6] = 0;
            if (enemy2[3] == 0)
                enemy2[6] = 0;

            time++;
        }

        if (gameScene == 2)
        {
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
            gc.PlaySound(1, true);
            gc.DrawImage(11, 0, 0);
            if (gc.GetPointerFrameCount(0) == 1)
                gameScene = 1;
            
            //ギミック
            
            if (gc.Random(0, 2500) == 0)
                gimic_flag = 1;
            if (gimic_flag == 1)
            {
                gc.DrawScaledRotateImage(0, gimic_x, 640, 100, 100, gimic_deg);
                gimic_x -= 5;
                gimic_deg+=4;
                if (gimic_x == -210)
                {
                    gimic_flag = 0;
                    gimic_deg = 0;
                    gimic_x = 1920;
                }
            }
            
            
        }

        
        if (gameScene == 1)
        {
            gc.PlaySound(music, false);
            CreateMainScreen();
            
            for (int i = 0; i < notes_idx; i++)  //ノーツの移動
            {
                gc.FillRect(notes_x[i], note_y, note_w, note_h);  //判定用の正方形
                gc.DrawImage(0, notes_x[i]-106, note_y-75);  //赤血球
                notes_x[i] -= 30;
            }

            if (gc.GetPointerFrameCount(0) == 1)
                //ノーツをタイミングよくたっぷできたとき
                for(int i=0; i<notes_idx; i++)
                    if (gc.CheckHitRect(notes_x[i], note_y, note_w, note_h, 100, 100, hanbetuBarX, 200))
                    {
                        notes_x[i] = notes_x[notes_idx - 1];
                        notes_idx--;
                        combo++;
                        player_action = 1;
                        player_time = 0;
                        if (enemy1[6] == 1)
                        {
                            enemy1[3]--;
                            if (enemy1[3] == 0)
                                enemy_kill[enemy1[0] - 3]++;
                        }
                        else if (enemy2[6] == 1)
                        {
                            enemy2[3]--;
                            if (enemy2[3] == 0)
                                enemy_kill[enemy2[0] - 3]++;
                        }
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

            if (enemy1[4] == 1)  //enemy１のアクション
            {
                if (enemy1[5] % 3 == 0)
                    enemy1[1] -= 50;
                if (enemy1[5] == 9)
                {
                    enemy1[4] = 0;
                    enemy1[1] = 950;
                }
            }
            enemy1[5]++;
            
            if (enemy2[4] == 1)  //enemy2のアクション
            {
                if (enemy2[5] % 3 == 0)
                    enemy2[1] -= 50;
                if (enemy2[5] == 9)
                {
                    enemy2[4] = 0;
                    enemy2[1] = 1450;
                }
            }
            enemy2[5]++;

            for (int i = 0; i < notes_idx; i++)  //ノーツをタイミングよくタッチできなかった時の処理
                if (notes_x[i] <= -156)
                {
                    notes_x[i] = notes_x[notes_idx - 1];
                    notes_idx--;
                    notes_lost++;
                    if (enemy1[6] == 1)
                    {
                        playerHP--;
                        enemy1[4] = 1;
                        enemy1[5] = 0;
                    }
                    if (enemy2[6] == 1)
                    {
                        playerHP--;
                        enemy2[4] = 1;
                        enemy2[5] = 0;
                    }
                    if (combo > comboMax)
                        comboMax = combo;
                    combo = 0;
                }

            if (playerHP <= 0)
            {
                //gameOverFlag = 1;
                gameScene = 2;
                time = 0;
                gc.StopSound();
            }

            gc.SetColor(0, 0, 0);
            if (enemy1[6] == 1)
            {
                gc.DrawImage(enemy1[0], enemy1[1], enemy1[2]);
                DrawHPbar("enemy1");
            }
            if (enemy2[6] == 1)
            {
                gc.DrawImage(enemy2[0], enemy2[1], enemy2[2]);
                DrawHPbar("enemy2");
            }
            gc.DrawImage(1, player_x, player_y);  //プレイヤー(白血球)
            DrawHPbar("player");

            if (combo % 10 == 0 & combo != 0 & playerHP < 10 )
            {
                enegy_time = 0;
                enegy_action = 1;
            }
            
            if (enegy_action == 1)  //ブドウ糖のアクション
            {
                if (enegy_time % 2 == 0)
                {
                    enegy_y -= 15;
                    alp -= 5;
                    gc.SetImageAlpha(alp);
                    gc.DrawImage(7, 670, enegy_y);
                    gc.ClearImageMultiplyColor();
                    DrawEffects();
                }

                if (enegy_time == 20)
                {
                    enegy_action = 0;
                    alp = 250;
                    enegy_y = 500;
                    playerHP++;
                    CreateEffects();
                }
            }
            enegy_time++;
        }

        if (gameScene == 2)
        {
            gc.ClearScreen();
            if (gameOverFlag == 1)  //gameover
            {
                gc.SetColor(0, 0, 0);
                gc.FillRect(0, 0, 1920, 1080);
                if (time > 180)
                {
                    gc.PlaySound(2, true);
                    gc.SetColor(255, 255, 255);
                    gc.SetFontSize(250);
                    gc.DrawString("GameOver", 500, 400);

                    if (time > 360)
                    {
                        gc.SetFontSize(70);
                        gc.DrawString("Long Press", 800, 700);

                        if (gc.GetPointerFrameCount(0) == 60)
                            gameScene = 0;
                    }
                }
            }
            else  //clear
            {
                if (time > 180)
                {
                    gc.PlaySound(3, true);
                    gc.SetColor(0, 0, 0);
                    gc.SetFontSize(250);
                    gc.DrawString("Clear!", 0, 0);

                    if (time > 360)
                    {
                        int score = comboMax * 1000 + enemy_kill[0] * 300 + enemy_kill[1] * 400 + enemy_kill[2] * 500;
                        gc.SetFontSize(70);
                        gc.DrawString("Score: " + score, 200, 300);
                        gc.DrawString("Combo: " + comboMax, 200, 400);
                        gc.DrawString("排除したウイルス", 200, 500);
                        gc.DrawString("エボラウイルス X " + enemy_kill[0], 300, 600);
                        gc.DrawString("コロナウイルス X " + enemy_kill[1], 300, 700);
                        gc.DrawString("MERSウイルス X " + enemy_kill[2], 300, 800);
                        gc.DrawString("Long Press", 800, 950);
                        gc.DrawScaledRotateImage(12, 900, 50, 130, 130, 0);
                        if (gc.GetPointerFrameCount(0) == 60)
                            gameScene = 0;
                    }
                }
            }
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
        gc.FillRect(100, 100, hanbetuBarX, 200);  //タイミングが合っているかあっていないか判断する棒
        gc.DrawImage(6, 0, 0);  //背景画像
        gc.FillRect(0, 100, width, 200);  //ノーツが流れてくるレーン
        //gc.SetColor(0, 0, 0);  //黒
        if(heart_flag == 1)
            gc.DrawImage(8, 100, 100);
        else
            gc.DrawImage(9, 50, 50);
        if (time % 30 == 0)
            heart_flag *= -1;
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

        if (mode == "enemy1")
        {
            gc.SetColor(125, 125, 125);
            gc.FillRect(enemy1[1], enemy1[2]-70, 400, 30);
            gc.SetColor(255, 0, 0);
            gc.FillRect(enemy1[1], enemy1[2]-70, enemy1[3]*400/enemy1[0], 30);
        }
        
        if (mode == "enemy2")
        {
            gc.SetColor(125, 125, 125);
            gc.FillRect(enemy2[1], enemy2[2]-70, 400, 30);
            gc.SetColor(255, 0, 0);
            gc.FillRect(enemy2[1], enemy2[2]-70, enemy2[3]*400/enemy2[0], 30);
        }
    }

    void CreateEffects()
    {
        for (int i = 0; i < 30; i++)
        {
            effect_x[i] = gc.Random(150, 650);
            effect_y[i] = gc.Random(550, 1050);
        }
    }

    void DrawEffects()
    {
        int effects_speed = 5;
        gc.SetColor(255, 255, 255);
        for (int i = 0; i < 30; i++)
        {
            gc.FillCircle(effect_x[i], effect_y[i], 5);
            effect_y[i] -= effects_speed;
        }
    }
}