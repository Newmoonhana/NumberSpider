using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScenesManager : MonoBehaviour
{
    public GameObject mainCamera;
    ScoreManager Score_M;
    public GameObject gamePanal;
    public GameObject sumUI_obj;
    public GameObject reachUI_obj;

    public GameObject gameLine_pre;
    public GameObject gameBlock_pre;
    public GameObject sumText_pre;
    public GameObject reachText_pre;
    public List<GameObject> widthLine = new List<GameObject>();

    GridLayoutGroup gamePanalGroup;

    public Point LineXY;

    //오브젝트 풀링 용 공간.
    public GameObject gameBlockEmpty_obj;

    private void Awake()
    {
        Score_M = GameManager.inst.Score_M;
        gamePanalGroup = gamePanal.GetComponent<GridLayoutGroup>();
        GameRun();
    }

    void GameRun()
    {
        mainCamera.transform.position = new Vector3(GameManager.inst.ScreenX / 2, GameManager.inst.ScreenY / 2, -10);
        Score_M.Load();

        widthLine.Clear();

        for (int a = 0; a < gameBlockEmpty_obj.transform.childCount; a++)
            Destroy(gameBlockEmpty_obj.transform.GetChild(a).gameObject);
        for (int a = 0; a < gamePanal.transform.childCount; a++)
            Destroy(gamePanal.transform.GetChild(a).gameObject);
        for (int a = 0; a < sumUI_obj.transform.childCount; a++)
            Destroy(sumUI_obj.transform.GetChild(a).gameObject);
        for (int a = 0; a < reachUI_obj.transform.childCount; a++)
            Destroy(reachUI_obj.transform.GetChild(a).gameObject);

        float Xfloat = (GameManager.inst.ScreenX + gamePanal.GetComponent<RectTransform>().offsetMax.x - gamePanal.GetComponent<RectTransform>().offsetMin.x) / LineXY.x;
        gamePanalGroup.cellSize = new Vector2(Xfloat - (Xfloat % 2 == 0 ? 0 : 1), GameManager.inst.ScreenY + gamePanal.GetComponent<RectTransform>().offsetMax.y - gamePanal.GetComponent<RectTransform>().offsetMin.y);
        gamePanalGroup.spacing = new Vector2(LineXY.x % 2 == 0 ? 2 : 1, 0);
        //라인 생성.
        int lineN = 0;
        while (lineN < LineXY.x)
        {
            GameObject line = Instantiate<GameObject>(gameLine_pre);
            line.transform.SetParent(gamePanal.transform);
            line.name = "line" + lineN.ToString() + "_obj";
            line.transform.localScale = Vector3.one;
            line.GetComponent<GridLayoutGroup>().cellSize = new Vector2(gamePanalGroup.cellSize.x, gamePanalGroup.cellSize.y / LineXY.y);
            widthLine.Add(line);

            lineN++;
        }
        //오브젝트 풀링 블록 생성.
        int i = 0;
        while (i < LineXY.x * LineXY.y)
        {
            GameObject block = Instantiate<GameObject>(gameBlock_pre);
            block.transform.SetParent(gameBlockEmpty_obj.transform);
            block.transform.localScale = Vector3.one;
            i++;
        }
    }

    private void Start()
    {
        GameStartRun();
    }

    public void GameStartRun()
    {
        int lineN = 0;

        reachUI_obj.GetComponent<GridLayoutGroup>().cellSize = new Vector2(gamePanalGroup.cellSize.x - 25, gamePanalGroup.cellSize.y / LineXY.y);
        reachUI_obj.GetComponent<GridLayoutGroup>().spacing = new Vector2(25, 0);
        sumUI_obj.GetComponent<GridLayoutGroup>().cellSize = new Vector2(gamePanalGroup.cellSize.x - 25, gamePanalGroup.cellSize.y / LineXY.y);
        sumUI_obj.GetComponent<GridLayoutGroup>().spacing = new Vector2(25, 0);
        while (lineN < LineXY.x)
        {
            //GameManager.inst.Score_M.limitHigh = GameManager.inst.Score_M.limitHighBase;    //생성 한정으로 기본값(점수에 안오름).
            AddBlock(widthLine[lineN]);

            //목표치 표시창 생성.
            GameObject lineReach = Instantiate<GameObject>(reachText_pre);
            lineReach.transform.SetParent(reachUI_obj.transform);
            //lineReach.GetComponent<RectTransform>().sizeDelta = new Vector2(gamePanalGroup.cellSize.x , lineReach.GetComponent<RectTransform>().sizeDelta.y);
            //lineReach.transform.position = new Vector3(GameManager.inst.ScreenX / 2 - gamePanalGroup.cellSize.x * LineXY.x / 2 + gamePanalGroup.cellSize.x * (lineN + 1) - gamePanalGroup.cellSize.x / 2, gamePanal.transform.position.y + gamePanalGroup.cellSize.y / 2 + gamePanalGroup.cellSize.y / 6 / (Screen.height % 9) + 75, 0);
            //lineReach.transform.position = new Vector3(GameManager.inst.ScreenX / 2 - gamePanalGroup.cellSize.x * LineXY.x / 2 + gamePanalGroup.cellSize.x * (lineN + 1) - gamePanalGroup.cellSize.x / 2, gamePanal.transform.position.y + gamePanalGroup.cellSize.y / 2 + lineReach.GetComponent<RectTransform>().sizeDelta.y / 4 - (Screen.height % 1280) / 10, 0);
            ////lineReach.transform.position = new Vector3((GameManager.inst.ScreenX / 2 - (gamePanalGroup.cellSize.x - (Screen.width / 720 * (10 + (3 * (Screen.width / 720) - LineXY.x)* (Screen.width / 720)))) * LineXY.x / 2) + (gamePanalGroup.cellSize.x - (Screen.width / 720 * (10 + (3 * (Screen.width / 720) - LineXY.x) * (Screen.width / 720)))) / 2 + (gamePanalGroup.cellSize.x - (Screen.width / 720 * (10 + (3 * (Screen.width / 720) - LineXY.x) * (Screen.width / 720)))) * (lineN), gamePanal.transform.position.y + gamePanalGroup.cellSize.y / 2 + lineReach.GetComponent<RectTransform>().sizeDelta.y / 4 - (Screen.height % 1280) / 15, 0);
            ResetReachNum(lineReach);

            //합계 표시창 생성.
            GameObject lineSum = Instantiate<GameObject>(sumText_pre);
            lineSum.transform.SetParent(sumUI_obj.transform);
            //lineSum.transform.position = new Vector3(GameManager.inst.ScreenX / 2 - gamePanalGroup.cellSize.x * LineXY.x / 2 + gamePanalGroup.cellSize.x * (lineN + 1) - gamePanalGroup.cellSize.x / 2, gamePanal.transform.position.y - gamePanalGroup.cellSize.y / 2 + (Screen.height % 9 * 10), 0);
            ////lineSum.transform.position = new Vector3((GameManager.inst.ScreenX / 2 - (gamePanalGroup.cellSize.x - (Screen.width / 720 * (10 + (3 * (Screen.width / 720) - LineXY.x) * (Screen.width / 720)))) * LineXY.x / 2) + (gamePanalGroup.cellSize.x - (Screen.width / 720 * (10 + (3 * (Screen.width / 720) - LineXY.x) * (Screen.width / 720)))) / 2 + (gamePanalGroup.cellSize.x - (Screen.width / 720 * (10 + (3 * (Screen.width / 720) - LineXY.x) * (Screen.width / 720)))) * (lineN), gamePanal.transform.position.y - (gamePanalGroup.cellSize.y / 2 + lineSum.GetComponent<RectTransform>().sizeDelta.y / 4 - (Screen.height % 1280) / 15), 0);

            lineN++;
        }
        GameManager.inst.Score_M.limitHigh = GameManager.inst.Score_M.limitHighBase;    //생성 한정으로 기본값(점수에 안오름).
        SumUpdate();
    }

    public GameObject AddBlock(GameObject parent)
    {
        if (gameBlockEmpty_obj.transform.childCount > 0)
        {
            GameObject block = gameBlockEmpty_obj.transform.GetChild(0).gameObject;

            block.transform.SetParent(parent.transform);
            block.GetComponent<BoxCollider>().size = new Vector3(parent.GetComponent<GridLayoutGroup>().cellSize.x, parent.GetComponent<GridLayoutGroup>().cellSize.y, 0);
            ChangeBlock(block, Random.Range(1, 10));
            Score_M.ScorePluse(int.Parse(block.transform.GetChild(0).GetComponent<Text>().text));

            return block;
        }
        return null;
    }

    Color32 []colorBlock = { new Color32(237, 102, 129, 255), new Color32(246, 167, 107, 255), new Color32(141, 208, 224, 255), new Color32(254, 205, 102, 255), new Color32(108, 176, 147, 255) };
    public void ChangeBlock(GameObject block, int index)
    {
        block.transform.GetChild(0).GetComponent<Text>().text = index.ToString();
        block.name = "block_obj_" + index.ToString();
        block.GetComponent<Image>().color = colorBlock[int.Parse(block.transform.GetChild(0).GetComponent<Text>().text) % 5];
    }

    public void DestroyBlock(GameObject block)
    {
        block.transform.SetParent(gameBlockEmpty_obj.transform);
        block.transform.position = gameBlockEmpty_obj.transform.position;
    }

    public void ResetReachNum(GameObject reach)
    {
        int num = Random.Range(Score_M.limitHigh - 89, Score_M.limitHigh);
        reach.transform.GetComponent<Text>().text = num.ToString();
    }

    public void StringSize()
    {
        int setsize = 99999999;

        //for (int i = 0; i < reachUI_obj.transform.childCount; i++)
        //{
        //    int thisFontsize = reachUI_obj.transform.GetChild(i).GetComponent<Text>().fontSize;
        //    if (setsize > thisFontsize)
        //        setsize = thisFontsize;
        //}
        //if (setsize > 50)
        //    setsize = 50;   //아무리 커도 기본값 폰트 사이즈는 안넘게 설정.
        //for (int i = 0; i < reachUI_obj.transform.childCount; i++)
        //{
        //    reachUI_obj.transform.GetChild(i).GetComponent<Text>().fontSize = setsize;
        //}

        setsize = 99999999;
        for (int i = 0; i < sumUI_obj.transform.childCount; i++)
        {
            int thisFontsize = (int)gamePanalGroup.cellSize.x / (Mathf.FloorToInt(int.Parse(sumUI_obj.transform.GetChild(i).GetComponent<Text>().text) / 100) + 2);
            if (setsize > thisFontsize)
                setsize = thisFontsize;
        }
        if (setsize > 50)
            setsize = 50;   //아무리 커도 기본값 폰트 사이즈는 안넘게 설정.
        for (int i = 0; i < sumUI_obj.transform.childCount; i++)
        {
            sumUI_obj.transform.GetChild(i).GetComponent<Text>().fontSize = setsize;
        }
    }

    public void ReachCheck()
    {
        SumUpdate();

        int lineN = 0;
        while (lineN < LineXY.x)
        {
            if (reachUI_obj.transform.GetChild(lineN).GetComponent<Text>().text == sumUI_obj.transform.GetChild(lineN).GetComponent<Text>().text)
            {
                Score_M.ScorePluse(int.Parse(reachUI_obj.transform.GetChild(lineN).GetComponent<Text>().text));
                while (widthLine[lineN].transform.childCount != 0)
                    DestroyBlock(widthLine[lineN].transform.GetChild(0).gameObject);
                AddBlock(widthLine[lineN]);
                ResetReachNum(reachUI_obj.transform.GetChild(lineN).gameObject);
                GameManager.inst.Sound_M.Play(GameManager.inst.Sound_M.ClickPlayer, GameManager.inst.Sound_M.ScoreUpAudio);
            }

            lineN++;
        }

        SumUpdate();
    }

    public void SumUpdate()
    {
        int lineN = 0;
        while (lineN < LineXY.x)
        {
            int sum = 0;
            if (gamePanal.transform.GetChild(lineN).childCount != 0)
                for (int i = 0; i < gamePanal.transform.GetChild(lineN).childCount; i++)
                    sum += int.Parse(gamePanal.transform.GetChild(lineN).GetChild(i).GetChild(0).GetComponent<Text>().text);
            sumUI_obj.transform.GetChild(lineN).GetComponent<Text>().text = sum.ToString();

            lineN++;
        }
        StringSize();
    }

    public void Restart()
    {
        StartCoroutine(RestartCor());
    }
    public IEnumerator RestartCor()
    {
        Score_M.score = 0;
        Score_M.Load();
        Score_M.limitHigh = Score_M.limitHighBase;
        GameRun();
        yield return null;
        GameStartRun();
    }
}
