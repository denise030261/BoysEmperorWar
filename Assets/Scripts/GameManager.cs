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
        Game
    }
    public GameState state = GameState.Game;

    /// <summary>
    /// 게임 진행 상태. InputManager.OnEnter() 참고
    /// </summary>
    public bool isPlaying = true;
    public TextMeshProUGUI CountNum;
    public GameObject Pause;
    public string title;
    Coroutine coPlaying;

    public Dictionary<string, Sheet> sheets = new Dictionary<string, Sheet>();
    private Dictionary<int, string> LevelSong = new Dictionary<int, string>(); // 스테이지에 따른 노래
    public int CurrentStage;

    public Image ResultImage;
    public Image ResultStage;
    public string BGMName;

    public int LoadingTime=5;
    public bool IsPaused=false; // 멈춤 여부
    private bool IsPlay = false; // 플레이 여부
    private bool IsCount = false;
    private float CurrentTime;

    public Image BackGroundImage;

    public List<GameObject> canvases = new List<GameObject>();
    enum Canvas
    {
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
        CountNum.text = "";
        Pause.SetActive(false);
        InitializeGame();
    }

    private void Update()
    {
        VideoManager.Instance.SetResolution();

        if (Input.GetKeyDown(KeyCode.Space) && IsPlay)
        {
            IsPaused = !IsPaused;
            if (IsPaused)
            {
                Pause.SetActive(true);
                Debug.Log("멈춥니다");
                AudioManager.Instance.Pause();
                Time.timeScale = 0;
            }
            else
            {
                Pause.SetActive(false);
                Debug.Log("3초 뒤에 시작합니다");
                CurrentTime = Time.unscaledTime;
                IsCount = true;
            }
        }

        if(IsCount)
        {
            if (Time.unscaledTime-CurrentTime>=3f)
            {
                CountNum.text = "";
                IsCount = false;
                AudioManager.Instance.UnPause();
                Time.timeScale = 1;
            }
            else if(Time.unscaledTime-CurrentTime>=2f)
            {
                CountNum.text = "1";
            }
            else if (Time.unscaledTime - CurrentTime >= 1f)
            {
                CountNum.text = "2";
            }
            else if (Time.unscaledTime - CurrentTime >= 0f)
            {
                CountNum.text = "3";
            }
        }
    }

    private void LevelSongInit()
    {
        LevelSong[1] = "Feelin Like";
        LevelSong[2] = "Do or Not";
        LevelSong[3] = "Dr.BeBe";
        LevelSong[4] = "Humph";
        LevelSong[5] = "Daisy";
    }

    public void Play()
    {
        IsPlay = true;
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
        await Task.Delay(1000 * LoadingTime); // 대기

        DisableCanvases();

        // 선택화면 아이템 생성
        await WaitUntilSheetLoaded();

        Debug.Log(CurrentStage);
        BackGroundImage.sprite = Resources.Load<Sprite>("Play/Stage" + CurrentStage);

        Play();
    }

    private void DisableCanvases()
    {
        canvases[(int)Canvas.Game].SetActive(false);
        canvases[(int)Canvas.GameBGA].SetActive(false);
        canvases[(int)Canvas.Result].SetActive(false);
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

    IEnumerator IEInitPlay()
    {        
        // 새 게임을 시작할 수 없게 해줌
        isPlaying = true;

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

        IsPlay = false;

        // 화면 페이드 아웃
        canvases[(int)Canvas.Game].SetActive(false);
        canvases[(int)Canvas.GameBGA].SetActive(false);
        canvases[(int)Canvas.Result].SetActive(true);

        MainAudioManager.Instance.PlayBGM(BGMName);

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

        if (Score.Instance.data.score >= UIStage.Instance.StandardScore[CurrentStage-1])
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
        rcombo.SetText(Score.Instance.data.maxcombo.ToString());
        rperfect.SetText(Score.Instance.data.perfect.ToString());
        rgreat.SetText(Score.Instance.data.great.ToString());
        rgood.SetText(Score.Instance.data.good.ToString());
        rmiss.SetText(Score.Instance.data.miss.ToString());

        NoteGenerator.Instance.StopGen();
        AudioManager.Instance.Stop();
    }

    public void OnClick_Next()
    {
        PlayerPrefs.SetString("State", "After");

        if (UIStage.Instance.StandardScore[CurrentStage-1] > Score.Instance.data.score)
            SceneManager.LoadScene("StageSelect");
        else
            SceneManager.LoadScene("Story");
    }

    public void OnClick_Retry()
    {
        MainAudioManager.Instance.StopBGM();
        DisableCanvases();
        Play();
    }
}
