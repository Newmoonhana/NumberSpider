using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    GameScenesManager GameScenes_M;
    public Button pauseButton;
    public Button retryButton;

    public GameObject pause_obj;
    public GameObject rowPanal_obj;
    public GameObject coluPanal_obj;
    public GameObject soundButton_obj;

    public GameObject selectPanal_obj;

    int tempX, tempY;

    private void Awake()
    {
        GameScenes_M = GameManager.inst.GameScenes_M;
    }

    //일시정지 관련함수.
    public bool pauseOn = false;
    bool restart = false;

    public void Pause()
    {
        GameManager.inst.AD_M.ClosedAD();
        pauseButton.enabled = false;
        retryButton.enabled = false;
        pause_obj.SetActive(true);
        pauseOn = true;
        Time.timeScale = 0f;
        tempX = GameScenes_M.LineXY.x;
        tempY = GameScenes_M.LineXY.y;
        rowPanal_obj.transform.GetChild(tempX - 3).GetChild(0).GetComponent<Text>().color = GameManager.inst.blue_rgb;
        coluPanal_obj.transform.GetChild(tempY - 3).GetChild(0).GetComponent<Text>().color = GameManager.inst.blue_rgb;
        soundButton_obj.transform.GetChild(0).GetComponent<Text>().color = GameManager.inst.Sound_M.SoundOn == true ? GameManager.inst.blue_rgb : GameManager.inst.gray_rgb;
    }

    public void ChangeRow(int row)
    {
        rowPanal_obj.transform.GetChild(GameScenes_M.LineXY.x - 3).GetChild(0).GetComponent<Text>().color = GameManager.inst.white_rgb;
        rowPanal_obj.transform.GetChild(row - 3).GetChild(0).GetComponent<Text>().color = GameManager.inst.blue_rgb;
        GameScenes_M.LineXY.x = row;
        if (tempX != row || tempY != GameScenes_M.LineXY.y)
        {
            restart = true;
        }
        else
            restart = false;
    }
    public void ChangeColu(int colu)
    {
        coluPanal_obj.transform.GetChild(GameScenes_M.LineXY.y - 3).GetChild(0).GetComponent<Text>().color = GameManager.inst.white_rgb;
        coluPanal_obj.transform.GetChild(colu - 3).GetChild(0).GetComponent<Text>().color = GameManager.inst.blue_rgb;
        GameScenes_M.LineXY.y = colu;
        if (tempY != colu || tempX != GameScenes_M.LineXY.x)
        {
            restart = true;
        }
        else
            restart = false;
    }

    public void SoundOn()
    {
        GameManager.inst.Sound_M.SoundOn = !GameManager.inst.Sound_M.SoundOn;
        soundButton_obj.transform.GetChild(0).GetComponent<Text>().text = GameManager.inst.Sound_M.SoundOn == true ? "On" : "Off";
        soundButton_obj.transform.GetChild(0).GetComponent<Text>().color = GameManager.inst.Sound_M.SoundOn == true ? GameManager.inst.blue_rgb : GameManager.inst.gray_rgb;
        if (GameManager.inst.Sound_M.SoundOn)
            GameManager.inst.Sound_M.Play(GameManager.inst.Sound_M.ClickPlayer, GameManager.inst.Sound_M.ClickButtonAudio);
    }

    public void Continue()
    {
        pauseButton.enabled = true;
        retryButton.enabled = true;
        Time.timeScale = 1f;
        pause_obj.SetActive(false);
        selectPanal_obj.SetActive(false);
        GameManager.inst.Score_M.Save();
        pauseOn = false;
        if (restart)
        {
            GameManager.inst.AD_M.ShowVidioAD();
            GameScenes_M.Restart();
        }
        GameManager.inst.AD_M.ShowAD();
            
        restart = false;
    }

    public void GameExit()
    {
        GameManager.inst.Score_M.Save();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void AllReset()
    {
        Continue();
        GameManager.inst.Score_M.ResetPP();
        GameManager.inst.GameScenes_M.Restart();
        GameManager.inst.AD_M.ShowRewardVidioAD();
    }

    //경고 창(OK, Cancel 선택) 띄위기.
    string whatthing = "";
    public void SelectPause(string _whatthing)
    {
        whatthing = _whatthing;
        if (whatthing == "restart")
        {
            pauseButton.enabled = false;
            retryButton.enabled = false;
            selectPanal_obj.SetActive(true);
            selectPanal_obj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "-RETRY-\n게임을 다시 시작하겠습니까?";
            pauseOn = true;
            Time.timeScale = 0f;
        }
        else if (whatthing == "reset")
        {
            selectPanal_obj.SetActive(true);
            selectPanal_obj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "-RESET-\n지금까지의 모든 기록을 완전히 초기화하겠습니까?";
        }
    }
    bool reset = false;
    public void SelectInputOK()
    {
        if (whatthing == "restart")
            restart = true;
        else if (whatthing == "reset")
            reset = true;
        SelectContinue();
    }
    public void SelectInputCancel()
    {
        SelectContinue();
    }
    void SelectContinue()
    {
        if (whatthing == "restart")
        {
            pauseButton.enabled = true;
            retryButton.enabled = true;
            Time.timeScale = 1f;
            selectPanal_obj.SetActive(false);
            GameManager.inst.Score_M.Save();
            pauseOn = false;

            if (restart)
            {
                GameScenes_M.Restart();
            }

            restart = false;
        }
        else if (whatthing == "reset")
        {
            if (reset)
            {
                AllReset();
            }
            else
            {
                selectPanal_obj.SetActive(false);
            }
            reset = false;
        }
        whatthing = "";
    }
}
