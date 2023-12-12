using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

public class GameManager : MonoBehaviour
{
    static GameManager instance;

    private int[] MaxScore = new int[5]; // ���������� �ְ� ����
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
    /// ���� ���� ����. InputManager.OnEnter() ����
    /// </summary>
    public bool isPlaying = true;
    public TextMeshProUGUI CountNum;
    public GameObject Pause;
    public string title;
    Coroutine coPlaying;

    public Dictionary<string, Sheet> sheets = new Dictionary<string, Sheet>();
    private Dictionary<int, string> LevelSong = new Dictionary<int, string>(); // ���������� ���� �뷡
    public int CurrentStage;

    public Image ResultImage;
    public Image ResultStage;
    public string BGMName;

    public int LoadingTime=5;
    public bool IsPaused=false; // ���� ����
    private bool IsPlay = false; // �÷��� ����
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
        Debug.Log("�� �̸� : " + title);

        for(int i=0;i<5;i++)
        {
            MaxScore[i]=PlayerPrefs.GetInt((i + 1) + "MaxScore", 0);
            PlayerPrefs.SetInt((i + 1) + "MaxScore", MaxScore[i]);
        }
        // �� ����
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
                Debug.Log("����ϴ�");
                AudioManager.Instance.Pause();
                Time.timeScale = 0;
            }
            else
            {
                Pause.SetActive(false);
                Debug.Log("3�� �ڿ� �����մϴ�");
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
            // Game UI ����
            canvases[(int)Canvas.Game].SetActive(false);

            // playing timer ����
            if (coPlaying != null)
            {
                StopCoroutine(coPlaying);
                coPlaying = null;
            }
        }

        // ��Ʈ Gen ����
        NoteGenerator.Instance.StopGen();

        // ���� ����
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

        string imagePath = Path.Combine(Application.streamingAssetsPath, "Play/Stage" + CurrentStage + ".png");
        StartCoroutine(LoadImage(imagePath, BackGroundImage));

        // UIObject���� �ڱ��ڽ��� ĳ���Ҷ����� ������ �ְ� ��Ȱ��ȭ(�ӽ��ڵ�)
        await Task.Delay(1000 * LoadingTime); // ���

        DisableCanvases();


        // ����ȭ�� ������ ����
        await WaitUntilSheetLoaded();

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
                // ��ٸ�
            }
        });
    }

    IEnumerator IEInitPlay()
    {        
        // �� ������ ������ �� ���� ����
        isPlaying = true;

        sheets[title].Init();

        // Audio ����
        AudioManager.Instance.Insert(sheets[title].clip);

        // Game UI �ѱ�
        canvases[(int)Canvas.Game].SetActive(true);

        // BGA �ѱ�
        canvases[(int)Canvas.GameBGA].SetActive(true);

        // ���� �ʱ�ȭ
        FindObjectOfType<Judgement>().Init();

        // ���� �ʱ�ȭ
        Score.Instance.Clear();

        // ���� ����Ʈ �ʱ�ȭ
        JudgeEffect.Instance.Init();

        // Note ����
        NoteGenerator.Instance.StartGen();

        // 3�� ���
        yield return new WaitForSeconds(3f);

        // Audio ���
        AudioManager.Instance.progressTime = 0f;
        AudioManager.Instance.Play();

        // End �˸���
        coPlaying = StartCoroutine(IEEndPlay());
    }

    // ���� ��
    IEnumerator IEEndPlay()
    {
        while (true)
        {
            if (!AudioManager.Instance.IsPlaying())
            {
                string imagePath = Path.Combine(Application.streamingAssetsPath, "Result/ResultLevel" + CurrentStage + ".png");
                StartCoroutine(LoadImage(imagePath, ResultStage));

                if (MaxScore[CurrentStage - 1] <= Score.Instance.data.score)
                {
                    Debug.Log(Score.Instance.data.score + "�� ����");
                    PlayerPrefs.SetInt(CurrentStage + "MaxScore", Score.Instance.data.score);
                }

                if (Score.Instance.data.score >= UIStage.Instance.StandardScore[CurrentStage - 1])
                {
                    Debug.Log("����");
                    imagePath = Path.Combine(Application.streamingAssetsPath, "Result/Success.png");
                    StartCoroutine(LoadImage(imagePath, ResultImage));
                }
                else
                {
                    Debug.Log("����");
                    imagePath = Path.Combine(Application.streamingAssetsPath, "Result/Fail.png");
                    StartCoroutine(LoadImage(imagePath, ResultImage));
                }
                break;
            }
            yield return new WaitForSeconds(1f);
        }

        IsPlay = false;

        // ȭ�� ���̵� �ƿ�
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

    IEnumerator LoadImage(string path,Image image)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + path);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            // ���� texture�� BackGroundImage.sprite�� �Ҵ��ϸ� �˴ϴ�.
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        else
        {
            Debug.LogError("Failed to load image: " + www.error+" Path : "+path);
        }
    }
}
