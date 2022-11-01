using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;

    public int ScreenX = 720;
    public int ScreenY = 1280;

    public GameObject GameScenes_Obj;
    public GameScenesManager GameScenes_M;
    public ScoreManager Score_M;
    public PauseManager Pause_M;
    public SoundManager Sound_M;
    public ADManager AD_M;

    public GameObject canvas_obj;
    public GameObject buttonSoundObj;

    public WaitForSeconds wait00_05 = new WaitForSeconds(0.5f);
    public Color32 blue_rgb;
    public Color32 white_rgb;
    public Color32 gray_rgb;

    private void Awake()
    {
        if (inst != null)   //싱글톤.
        {
            Destroy(inst.gameObject);
            return;
        }
        inst = this;
        DontDestroyOnLoad(this);    //씬 이동 후 오브젝트 보존.

        //해상도 설정.
#if UNITY_ANDROID
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.SetResolution(Screen.width, Screen.height, true);
        //Screen.SetResolution(Screen.width, Screen.width * 9 / 16, true);
        //Screen.SetResolution(Screen.width, Screen.width * 16 / 9, true);
#else
        if (Screen.width >= 720)
            Screen.SetResolution(Screen.width / 2, Screen.width / 2 * 16 / 9, false);
        else
            Screen.SetResolution(Screen.width, Screen.width * 16 / 9, false);
#endif

        if (SceneManager.GetActiveScene().name == "GameScenes")
        {
            GameScenes_Obj.SetActive(true);
        }
        else
            GameScenes_Obj.SetActive(false);
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Pause_M.GameExit();
    }

    
}
