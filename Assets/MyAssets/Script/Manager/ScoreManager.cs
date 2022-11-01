using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public InputField highScore;
    public InputField Row;
    public InputField Colu;
    public InputField SoundOn;
    public int score = 0;
    public Text scoreText_tx;
    public Text highScoreText_tx;

    public int limitHighBase = 99;
    public int limitHigh;

    private void Awake()
    {
        //하이스코어 불러오기.
        limitHigh = limitHighBase;
        Load();
    }

    public void ScorePluse(int num)
    {
        limitHigh++;
        score += num;
        scoreText_tx.text = score.ToString();

        if (score > int.Parse(highScore.text))
        {
            highScore.text = score.ToString();
            highScoreText_tx.text = highScore.text;
            Save();
        }
    }

    public void Save()
    {
        PlayerPrefs.SetInt("HighScore", int.Parse(highScore.text));
        PlayerPrefs.SetInt("Row", GameManager.inst.GameScenes_M.LineXY.x);
        PlayerPrefs.SetInt("Colu", GameManager.inst.GameScenes_M.LineXY.y);
        PlayerPrefs.SetInt("SoundOn", GameManager.inst.Sound_M.SoundOn == false ? 0 : 1);
    }

    public void Load()
    {
        scoreText_tx.text = score.ToString();
        if (!PlayerPrefs.HasKey("HighScore") || !PlayerPrefs.HasKey("Row"))
            ResetPP();
        
        highScore.text = PlayerPrefs.GetInt("HighScore").ToString();
        highScoreText_tx.text = highScore.text;

        GameManager.inst.GameScenes_M.LineXY.x = PlayerPrefs.GetInt("Row");
        GameManager.inst.GameScenes_M.LineXY.y = PlayerPrefs.GetInt("Colu");
        GameManager.inst.Sound_M.SoundOn = PlayerPrefs.GetInt("SoundOn") == 0 ? false : true;
        GameManager.inst.buttonSoundObj.transform.GetChild(0).GetComponent<Text>().text = GameManager.inst.Sound_M.SoundOn == true ? "On" : "Off";
    }

    //모든 값 리셋.
    public void ResetPP()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("HighScore", 0);
        PlayerPrefs.SetInt("Row", 4);
        PlayerPrefs.SetInt("Colu", 5);
        PlayerPrefs.SetInt("SoundOn", 1);
    }
}
