using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager instance;

    private int[] MaxScore = new int[5]; // 스테이지별 최고 점수
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    public enum GameState
    {
        Game,
        Edit,
    }
    public GameState state = GameState.Game;

    /// <summary>
    /// 게임 진행 상태. InputManager.OnEnter() 참고
    /// </summary>
    public bool isPlaying = true;
    public string title;
    Coroutine coPlaying;

    public Dictionary<string, Sheet> sheets = new Dictionary<string, Sheet>();
    private Dictionary<int, string> LevelSong = new Dictionary<int, string>(); // 스테이지에 따른 노래
    private int CurrentStage;

    float speed = 1.0f;
    public Image ResultImage;
    public Image ResultStage;
    public float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = Mathf.Clamp(value, 1.0f, 5.0f);
        }
    }

    public List<GameObject> canvases = new List<GameObject>();
    enum Canvas
    {
        Title,
        Select,
        SFX,
        GameBGA,
        Game,
        Result,
        Editor,
    }
    CanvasGroup sfxFade;

    void Awake()
    {
        if (instance == null)
            instance = this;

        LevelSongInit();
        CurrentStage = PlayerPrefs.GetInt("Level", 1);
        title = LevelSong[CurrentStage];
        Debug.Log("곡 이름 : " + title);

        for(int i=0;i<5;i++)
        {
            MaxScore[i]=PlayerPrefs.GetInt((i + 1) + "MaxScore", 0);
            PlayerPrefs.SetInt((i + 1) + "MaxScore", MaxScore[i]);
        }
        // 곡 선택
    }

    void Start()
    {
        InitializeGame();
    }

    private void LevelSongInit()
    {
        LevelSong[1] = "Feelin Like";
        LevelSong[2] = "Do or Not";
        LevelSong[3] = "Consolation";
        LevelSong[4] = "Splendid Circus";
        LevelSong[5] = "Feelin Like";
    }

    public void ChangeMode(UIObject uiObject)
    {
        if (state == GameState.Game)
        {
            state = GameState.Edit;
            TextMeshProUGUI text = uiObject.transform.GetComponentInChildren<TextMeshProUGUI>();
            text.text = "Edit\nMode";
        }
        else
        {
            state = GameState.Game;
            TextMeshProUGUI text = uiObject.transform.GetComponentInChildren<TextMeshProUGUI>();
            text.text = "Game\nMode";
        }
    }

    public void Select()
    {
        IESelect();
    }

    public void Play()
    {
        StartCoroutine(IEInitPlay());
    }

    public void Stop()
    {
        if (state == GameState.Game)
        {
            // Game UI 끄기
            canvases[(int)Canvas.Game].SetActive(false);

            // playing timer 끄기
            if (coPlaying != null)
            {
                StopCoroutine(coPlaying);
                coPlaying = null;
            }
        }
        else
        {
            // Editor UI 끄기
            canvases[(int)Canvas.Editor].SetActive(false);
            Editor.Instance.Stop();

            FindObjectOfType<GridGenerator>().InActivate();

            // 에디터에서 수정된 오브젝트가 있을 수 있으므로 갱신해줌
            StartCoroutine(Parser.Instance.IEParse(title));
            sheets[title] = Parser.Instance.sheet;
        }

        // 노트 Gen 끄기
        NoteGenerator.Instance.StopGen();

        // 음악 끄기
        AudioManager.Instance.progressTime = 0f;
        AudioManager.Instance.Stop();

        SceneManager.LoadScene("StageSelect");
    }

    private async void InitializeGame()
    {
        SheetLoader.Instance.Init();

        foreach (GameObject go in canvases)
        {
            go.SetActive(true);
        }

        UIController.Instance.Init();
        Score.Instance.Init();

        // UIObject들이 자기자신을 캐싱할때까지 여유를 주고 비활성화(임시코드)
        await Task.Delay(2000); // 2초 동안 대기

        DisableCanvases();

        // 선택화면 아이템 생성
        await WaitUntilSheetLoaded();
        ItemGenerator.Instance.Init();

        // 타이틀 화면 시작
        canvases[(int)Canvas.Title].SetActive(false);
        Play();
    }

    private void DisableCanvases()
    {
        canvases[(int)Canvas.Game].SetActive(false);
        canvases[(int)Canvas.GameBGA].SetActive(false);
        canvases[(int)Canvas.Result].SetActive(false);
        canvases[(int)Canvas.Select].SetActive(false);
        canvases[(int)Canvas.Editor].SetActive(false);
    }

    private async Task WaitUntilSheetLoaded()
    {
        await Task.Run(() => {
            while (!SheetLoader.Instance.bLoadFinish)
            {
                // 기다림
            }
        });
    }

    private void IESelect()
    {
        // Select UI 켜기
        canvases[(int)Canvas.Select].SetActive(true);

        canvases[(int)Canvas.Title].SetActive(false);

        canvases[(int)Canvas.SFX].SetActive(false);

        // 새 게임을 시작할 수 있게 해줌
        isPlaying = false;
    }

    IEnumerator IEInitPlay()
    {        
        // 새 게임을 시작할 수 없게 해줌
        isPlaying = true;

        // 화면 페이드 아웃
        canvases[(int)Canvas.SFX].SetActive(true);
        //yield return StartCoroutine(AniPreset.Instance.IEAniFade(sfxFade, true, 2f));

        //  Select UI 끄기
        canvases[(int)Canvas.Select].SetActive(false);

        // Sheet 초기화
        //title = "Feelin Like";

        sheets[title].Init();

        // Audio 삽입
        AudioManager.Instance.Insert(sheets[title].clip);

        // Game UI 켜기
        canvases[(int)Canvas.Game].SetActive(true);

        // BGA 켜기
        canvases[(int)Canvas.GameBGA].SetActive(true);

        // 판정 초기화
        FindObjectOfType<Judgement>().Init();

        // 점수 초기화
        Score.Instance.Clear();

        // 판정 이펙트 초기화
        JudgeEffect.Instance.Init();

        // 화면 페이드 인

        canvases[(int)Canvas.SFX].SetActive(false);

        // Note 생성
        NoteGenerator.Instance.StartGen();

        // 3초 대기
        yield return new WaitForSeconds(3f);

        // Audio 재생
        AudioManager.Instance.progressTime = 0f;
        AudioManager.Instance.Play();

        // End 알리미
        coPlaying = StartCoroutine(IEEndPlay());
    }

    // 게임 끝
    IEnumerator IEEndPlay()
    {
        ResultStage.sprite= Resources.Load<Sprite>("Result/ResultLevel"+CurrentStage);
        while (true)
        {
            if (!AudioManager.Instance.IsPlaying())
            {
                break;
            }
            yield return new WaitForSeconds(1f);
        }

        // 화면 페이드 아웃
        canvases[(int)Canvas.SFX].SetActive(true);
        yield return StartCoroutine(AniPreset.Instance.IEAniFade(sfxFade, true, 2f));
        canvases[(int)Canvas.Game].SetActive(false);
        canvases[(int)Canvas.GameBGA].SetActive(false);
        canvases[(int)Canvas.Result].SetActive(true);

        UIText rscore = UIController.Instance.FindUI("UI_R_Score").uiObject as UIText;
        UIText rcombo = UIController.Instance.FindUI("UI_R_Combo").uiObject as UIText;
        UIText rperfect = UIController.Instance.FindUI("UI_R_Perfect").uiObject as UIText;
        UIText rgreat = UIController.Instance.FindUI("UI_R_Great").uiObject as UIText;
        UIText rgood = UIController.Instance.FindUI("UI_R_Good").uiObject as UIText;
        UIText rmiss = UIController.Instance.FindUI("UI_R_Miss").uiObject as UIText;

        if (MaxScore[CurrentStage - 1] <= Score.Instance.data.score)
        {
            Debug.Log(Score.Instance.data.score + "를 받음");
            PlayerPrefs.SetInt(CurrentStage + "MaxScore", Score.Instance.data.score);
        }

        if (Score.Instance.data.score >= 10000)
        {
            Debug.Log("성공");
            ResultImage.sprite = Resources.Load<Sprite>("Result/Success");
        }
        else
        {
            Debug.Log("실패");
            ResultImage.sprite = Resources.Load<Sprite>("Result/Fail");
        }

        rscore.SetText(Score.Instance.data.score.ToString());
        rcombo.SetText(Score.Instance.data.combo.ToString());
        rperfect.SetText(Score.Instance.data.perfect.ToString());
        rgreat.SetText(Score.Instance.data.great.ToString());
        rgood.SetText(Score.Instance.data.good.ToString());
        rmiss.SetText(Score.Instance.data.miss.ToString());

        //UIImage rBG = UIController.Instance.FindUI("UI_R_BG").uiObject as UIImage;
        //rBG.SetSprite(sheets[title].img);

        NoteGenerator.Instance.StopGen();
        AudioManager.Instance.Stop();

        // 화면 페이드 인
        yield return StartCoroutine(AniPreset.Instance.IEAniFade(sfxFade, false, 2f));
        canvases[(int)Canvas.SFX].SetActive(false);

        // 5초 대기
        yield return new WaitForSeconds(5f);

        // 선택창 이동
        SceneManager.LoadScene("StageSelect");
    }
}
