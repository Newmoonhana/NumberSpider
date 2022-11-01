using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchManager : MonoBehaviour
{
    ScoreManager Score_M;
    public GameObject hitBefore_obj = null;    //전에 클릭한 오브젝트.
    public GameObject hit_obj = null;   //클릭한 오브젝트.
    public GameObject hit_parent = null;
    public int ispush = -1;  //처리 된 마우스 입력 여부(-1: null, 0: 다운, 1:클릭, 2: 업).
    public int douClick = 1;   //더블클릭까지의 클릭 여부(1: 클릭 X(한번 클릭), n: n번째 클릭).
    public bool dontClick = false;  //클릭 금지 상태(false:꺼짐, true: 켜짐).

    public void Awake()
    {
        Score_M = GameManager.inst.Score_M;
        StartCoroutine("UpdateCor");
    }

    public IEnumerator UpdateCor()
    {

        if (dontClick == false)
        {
            if (ispush == 2)
                ispush = -1;
#if UNITY_ANDROID
            if (Input.touchCount > 0)
            {
                TouchDown();
                Touch();
                TouchUp();
            }
#else
            MouseDown();
            if (!GameManager.inst.Pause_M.pauseOn)
            {
                Mouse();
                MouseUp();
            }
#endif
        }

        yield return null;
        StartCoroutine("UpdateCor");
    }

#if UNITY_ANDROID
    void TouchDown()
    {
        int a = 0;
        //for (int a = 0; a < Input.touchCount; a++)
        if (Input.GetTouch(a).phase == TouchPhase.Began)
        {
            if (ispush == -1)
            {
                RaycastHit hit = new RaycastHit();
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray.origin, ray.direction, out hit))
                {
                    //사운드.
                    if (hit.transform.gameObject.tag != null)
                    {
                        if (GameManager.inst.Pause_M.pauseOn)
                        {
                            RaycastHit[] hits = Physics.RaycastAll(ray);
                            for (int i = 0; i < hits.Length; i++)
                                if (hits[i].transform.gameObject.tag == "Button")
                                    GameManager.inst.Sound_M.Play(GameManager.inst.Sound_M.ClickPlayer, GameManager.inst.Sound_M.ClickButtonAudio);
                        }
                        else
                        {
                            if (hit.transform.gameObject.tag == "Button")
                                GameManager.inst.Sound_M.Play(GameManager.inst.Sound_M.ClickPlayer, GameManager.inst.Sound_M.ClickButtonAudio);
                            else if (hit.transform.gameObject.tag == "MoveBlock")
                                GameManager.inst.Sound_M.Play(GameManager.inst.Sound_M.ClickPlayer, GameManager.inst.Sound_M.MoveBlockAudio);
                        }
                    }
                    if (!GameManager.inst.Pause_M.pauseOn)
                    {
                        if (hit.transform.gameObject.tag == "MoveBlock")
                        {
                            int beforeNum = -1;
                            for (int i = hit.transform.parent.childCount - 1; i >= 0; i--)
                            {
                                if (beforeNum != -1)
                                    if (int.Parse(hit.transform.parent.GetChild(i).GetChild(0).GetComponent<Text>().text) - 1 < beforeNum)
                                        break;
                                if (hit.transform == hit.transform.parent.GetChild(i))
                                {
                                    hitBefore_obj = hit_obj;
                                    hit_obj = hit.transform.gameObject;
                                    hit_parent = hit_obj.transform.parent.gameObject;
                                    if (beforeNum != -1)
                                        for (int j = i + 1; j < hit.transform.parent.childCount;)
                                        {
                                            GameObject moveChildBlock = hit.transform.parent.GetChild(j).gameObject;
                                            moveChildBlock.transform.SetParent(hit_obj.transform);
                                            Vector3 vec = moveChildBlock.transform.localPosition;
                                            vec.z = 0;
                                            moveChildBlock.transform.localPosition = vec;
                                        }

                                    hit_obj.transform.SetParent(GameManager.inst.GameScenes_M.gamePanal.transform.parent);
                                    break;
                                }
                                beforeNum = int.Parse(hit.transform.parent.GetChild(i).GetChild(0).GetComponent<Text>().text);
                            }
                        }

                        if (hitBefore_obj == hit_obj && hit_obj != null)
                            douClick += 1;
                        else
                            douClick = 1;
                    }
                }
                ispush = 0;
            }
        }
    }

    void Touch()
    {
        //int i = 0;
        for (int i = 0; i < Input.touchCount; i++)
            if (Input.GetTouch(i).phase == TouchPhase.Moved || Input.GetTouch(i).phase == TouchPhase.Stationary)
        {
            if ((ispush == 0 || ispush == 1) && hit_obj != null)
            {
                if (hit_obj.tag == "MoveBlock")
                {
                    followMouseObj(hit_obj);
                }
                ispush = 1;
            }
        }
    }

    private void TouchUp()
    {
        int a = 0;
        //for (int a = 0; a < Input.touchCount; a++)
            if (Input.GetTouch(a).phase == TouchPhase.Ended)
            {
                if ((ispush == 0 || ispush == 1) && hit_obj != null)
                {
                    if (hit_obj.tag == "MoveBlock")
                    {
                        GameObject gamePanal = GameManager.inst.GameScenes_M.gamePanal;
                        GridLayoutGroup gamePanalGroup = gamePanal.GetComponent<GridLayoutGroup>();
                        for (int i = 0; i < GameManager.inst.GameScenes_M.LineXY.x; i++)
                            if (GameManager.inst.ScreenX / 2 - gamePanalGroup.cellSize.x * GameManager.inst.GameScenes_M.LineXY.x / 2 + gamePanalGroup.cellSize.x * (i + 1) > hit_obj.transform.position.x - 0.01f)
                            {
                                if (gamePanal.transform.GetChild(i).childCount == 0)
                                    hit_obj.transform.SetParent(gamePanal.transform.GetChild(i));
                                else if (int.Parse(hit_obj.transform.GetChild(0).GetComponent<Text>().text) == int.Parse(gamePanal.transform.GetChild(i).GetChild(gamePanal.transform.GetChild(i).childCount - 1).GetChild(0).GetComponent<Text>().text) && hit_obj.transform.childCount - 2 + gamePanal.transform.GetChild(i).childCount < GameManager.inst.GameScenes_M.LineXY.y)
                                {
                                    Score_M.ScorePluse(int.Parse(hit_obj.transform.GetChild(0).GetComponent<Text>().text) * 2);
                                    GameManager.inst.GameScenes_M.DestroyBlock(gamePanal.transform.GetChild(i).GetChild(gamePanal.transform.GetChild(i).childCount - 1).gameObject);
                                    GameManager.inst.GameScenes_M.ChangeBlock(hit_obj, int.Parse(hit_obj.transform.GetChild(0).GetComponent<Text>().text) * 2);
                                    hit_obj.transform.SetParent(gamePanal.transform.GetChild(i));
                                    GameManager.inst.Sound_M.Play(GameManager.inst.Sound_M.ClickPlayer, GameManager.inst.Sound_M.FusionBlockAudio);
                                }
                                else if (hit_obj.transform.childCount + gamePanal.transform.GetChild(i).childCount > GameManager.inst.GameScenes_M.LineXY.y)
                                    hit_obj.transform.SetParent(hit_parent.transform);
                                else if (int.Parse(hit_obj.transform.GetChild(0).GetComponent<Text>().text) < int.Parse(gamePanal.transform.GetChild(i).GetChild(gamePanal.transform.GetChild(i).childCount - 1).GetChild(0).GetComponent<Text>().text))
                                {
                                    hit_obj.transform.SetParent(gamePanal.transform.GetChild(i));
                                }
                                else
                                    hit_obj.transform.SetParent(hit_parent.transform);
                                while (hit_obj.transform.childCount > 1)
                                    hit_obj.transform.GetChild(1).SetParent(hit_obj.transform.parent);

                                //블럭 생성.
                                for (int j = 0; j < gamePanal.transform.childCount; j++)
                                    if (gamePanal.transform.GetChild(j).childCount == 0)
                                        GameManager.inst.GameScenes_M.AddBlock(gamePanal.transform.GetChild(j).gameObject);
                                GameManager.inst.GameScenes_M.ReachCheck();
                                break;
                            }
                    }
                }
                hit_obj = null;
                ispush = 2;
            }
    }
    
#else

    void MouseDown()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            if (ispush == -1)
            { 
                RaycastHit hit = new RaycastHit();
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray.origin, ray.direction, out hit))
                {
                    //사운드.
                    if (hit.transform.gameObject.tag != null)
                    {
                        if (GameManager.inst.Pause_M.pauseOn)
                        {
                            RaycastHit[] hits = Physics.RaycastAll(ray);
                            for (int i = 0; i < hits.Length; i++)
                                if (hits[i].transform.gameObject.tag == "Button")
                                    GameManager.inst.Sound_M.Play(GameManager.inst.Sound_M.ClickPlayer, GameManager.inst.Sound_M.ClickButtonAudio);
                        }
                        else
                        {
                            if (hit.transform.gameObject.tag == "Button")
                                GameManager.inst.Sound_M.Play(GameManager.inst.Sound_M.ClickPlayer, GameManager.inst.Sound_M.ClickButtonAudio);
                            else if (hit.transform.gameObject.tag == "MoveBlock")
                                GameManager.inst.Sound_M.Play(GameManager.inst.Sound_M.ClickPlayer, GameManager.inst.Sound_M.MoveBlockAudio);
                        }
                    }
                    if (!GameManager.inst.Pause_M.pauseOn)
                    {
                        if (hit.transform.gameObject.tag == "MoveBlock")
                        {
                            int beforeNum = -1;
                            for (int i = hit.transform.parent.childCount - 1; i >= 0; i--)
                            {
                                if (beforeNum != -1)
                                    if (int.Parse(hit.transform.parent.GetChild(i).GetChild(0).GetComponent<Text>().text) - 1 < beforeNum)
                                        break;
                                if (hit.transform == hit.transform.parent.GetChild(i))
                                {
                                    hitBefore_obj = hit_obj;
                                    hit_obj = hit.transform.gameObject;
                                    hit_parent = hit_obj.transform.parent.gameObject;
                                    if (beforeNum != -1)
                                        for (int j = i + 1; j < hit.transform.parent.childCount;)
                                        {
                                            GameObject moveChildBlock = hit.transform.parent.GetChild(j).gameObject;
                                            moveChildBlock.transform.SetParent(hit_obj.transform);
                                            Vector3 vec = moveChildBlock.transform.localPosition;
                                            vec.z = 0;
                                            moveChildBlock.transform.localPosition = vec;
                                        }

                                    hit_obj.transform.SetParent(GameManager.inst.GameScenes_M.gamePanal.transform.parent);
                                    break;
                                }
                                beforeNum = int.Parse(hit.transform.parent.GetChild(i).GetChild(0).GetComponent<Text>().text);
                            }
                        }

                        if (hitBefore_obj == hit_obj && hit_obj != null)
                            douClick += 1;
                        else
                            douClick = 1;
                    }
                }
                ispush = 0;
            }
        }
    }

    void Mouse()
    {
        if (Input.GetMouseButton(0) == true)
        {
            if ((ispush == 0 || ispush == 1) && hit_obj != null)
            //if ((ispush == -1 || ispush == 1) && hit_obj != null)
            {
                if (hit_obj.tag == "MoveBlock")
                {
                    followMouseObj(hit_obj);
                }
                ispush = 1;
            }
        }
    }

    private void MouseUp()
    {
        if (Input.GetMouseButtonUp(0) == true)
        {
            if ((ispush == 0 || ispush == 1) && hit_obj != null)
            {
                if (hit_obj.tag == "MoveBlock")
                {
                    GameObject gamePanal = GameManager.inst.GameScenes_M.gamePanal;
                    GridLayoutGroup gamePanalGroup = gamePanal.GetComponent<GridLayoutGroup>();
                    for (int i = 0; i < GameManager.inst.GameScenes_M.LineXY.x; i++)
                        if (GameManager.inst.ScreenX / 2 - gamePanalGroup.cellSize.x * GameManager.inst.GameScenes_M.LineXY.x / 2 + gamePanalGroup.cellSize.x * (i + 1) > hit_obj.transform.position.x - 0.01f)
                        {
                            if (gamePanal.transform.GetChild(i).childCount == 0)
                                hit_obj.transform.SetParent(gamePanal.transform.GetChild(i));
                            else if (int.Parse(hit_obj.transform.GetChild(0).GetComponent<Text>().text) == int.Parse(gamePanal.transform.GetChild(i).GetChild(gamePanal.transform.GetChild(i).childCount - 1).GetChild(0).GetComponent<Text>().text) && hit_obj.transform.childCount - 2 + gamePanal.transform.GetChild(i).childCount < GameManager.inst.GameScenes_M.LineXY.y)
                            {
                                Score_M.ScorePluse(int.Parse(hit_obj.transform.GetChild(0).GetComponent<Text>().text) * 2);
                                GameManager.inst.GameScenes_M.DestroyBlock(gamePanal.transform.GetChild(i).GetChild(gamePanal.transform.GetChild(i).childCount - 1).gameObject);
                                GameManager.inst.GameScenes_M.ChangeBlock(hit_obj, int.Parse(hit_obj.transform.GetChild(0).GetComponent<Text>().text) * 2);
                                hit_obj.transform.SetParent(gamePanal.transform.GetChild(i));
                                GameManager.inst.Sound_M.Play(GameManager.inst.Sound_M.ClickPlayer, GameManager.inst.Sound_M.FusionBlockAudio);
                            }
                            else if (hit_obj.transform.childCount + gamePanal.transform.GetChild(i).childCount > GameManager.inst.GameScenes_M.LineXY.y)
                                hit_obj.transform.SetParent(hit_parent.transform);
                            else if (int.Parse(hit_obj.transform.GetChild(0).GetComponent<Text>().text) < int.Parse(gamePanal.transform.GetChild(i).GetChild(gamePanal.transform.GetChild(i).childCount - 1).GetChild(0).GetComponent<Text>().text))
                            {
                                hit_obj.transform.SetParent(gamePanal.transform.GetChild(i));
                            }
                            else
                                hit_obj.transform.SetParent(hit_parent.transform);
                            while (hit_obj.transform.childCount > 1)
                                hit_obj.transform.GetChild(1).SetParent(hit_obj.transform.parent);

                            //블럭 생성.
                            for (int j = 0; j < gamePanal.transform.childCount; j++)
                                if (gamePanal.transform.GetChild(j).childCount == 0)
                                    GameManager.inst.GameScenes_M.AddBlock(gamePanal.transform.GetChild(j).gameObject);
                            GameManager.inst.GameScenes_M.ReachCheck();
                            break;
                        }
                }
            }
            hit_obj = null;
            ispush = 2;
        }
    }
#endif

    void followMouseObj(GameObject obj)
    {
        RectTransform objRect = obj.GetComponent<RectTransform>();
        RectTransform panalRect = GameManager.inst.GameScenes_M.gamePanal.GetComponent<RectTransform>();
        Vector3 followPosition = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 10f);
        //부모 패널 영역 안 넘어가게.
        GridLayoutGroup lineGroup = GameManager.inst.GameScenes_M.widthLine[0].GetComponent<GridLayoutGroup>();
        if (followPosition.x - objRect.sizeDelta.x / 2 < GameManager.inst.ScreenX / 2 - lineGroup.cellSize.x * GameManager.inst.GameScenes_M.LineXY.x / 2)
            followPosition.x = GameManager.inst.ScreenX / 2 - lineGroup.cellSize.x * GameManager.inst.GameScenes_M.LineXY.x / 2 + objRect.sizeDelta.x / 2;
        else if (followPosition.x + objRect.sizeDelta.x / 2 > GameManager.inst.ScreenX / 2 + lineGroup.cellSize.x * GameManager.inst.GameScenes_M.LineXY.x / 2)
            followPosition.x = GameManager.inst.ScreenX / 2 + lineGroup.cellSize.x * GameManager.inst.GameScenes_M.LineXY.x / 2 - objRect.sizeDelta.x / 2;
        if (followPosition.y - objRect.sizeDelta.y / 2 < GameManager.inst.ScreenY / 2 - lineGroup.cellSize.y * GameManager.inst.GameScenes_M.LineXY.y / 2)
            followPosition.y = GameManager.inst.ScreenY / 2 - lineGroup.cellSize.y * GameManager.inst.GameScenes_M.LineXY.y / 2;
        else if (followPosition.y + objRect.sizeDelta.y > GameManager.inst.ScreenY / 2 + lineGroup.cellSize.y * GameManager.inst.GameScenes_M.LineXY.y / 2)
            followPosition.y = GameManager.inst.ScreenY / 2 + lineGroup.cellSize.y * GameManager.inst.GameScenes_M.LineXY.y / 2 - objRect.sizeDelta.y;

        followPosition.z = 0f;
        obj.transform.position = followPosition;
    }
}
