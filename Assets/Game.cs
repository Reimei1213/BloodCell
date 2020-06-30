using Sequence = System.Collections.IEnumerator;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

//音楽の長さは time = 5700
public sealed class Game : GameBase
{
    // 変数の宣言
    private int height = 1080;
    private int width = 1920;
    //private int gameScene = -1;
    private int gameScene = 1;
    private int time = 0;
    private int notes_idx = 0;
    
    private const int music_end = 5700;
    private int x = 1720;
    

    public override void InitGame()
    {
        // キャンバスの大きさを設定します
        gc.SetResolution(width, height);
        gc.SetSoundVolume(1);
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
            x -= 30;
            if (x < -300)
                x = 1720;
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
            int note = 0;

            CreateMainScreen();

            gc.DrawImage(0, x, 100);

            /*
            notes_idx = gc.Load(-1);
            gc.DrawString("idx" + notes_idx, 300, 0);
            for (int i = 0; i <= notes_idx; i++)
            {
                note = gc.Load(i);
                gc.DrawString("load" + note, 0, i * 50);
            }
            */
        }

        if (gameScene == 2)
        {
            
        }
    }

    void CreateNotes()
    {
        if (gc.GetPointerFrameCount(0) == 1)
        {
            gc.Save(notes_idx, time);
            gc.Save(-1, notes_idx);
            notes_idx++;
        }
    }

    void CreateMainScreen()
    {
        gc.SetColor(140, 0, 0, 150);
        gc.DrawImage(6, 0, 0);
        gc.FillRect(0, 100, width, 200);
        gc.SetColor(0, 0, 0);
        gc.FillRect(100, 100, 10, 200);
        gc.DrawImage(1, 150, 550);
    }

}