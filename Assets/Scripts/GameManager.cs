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
        Game,
        Edit,
    }
    public GameState state = GameState.Game;

    /// <summary>
    /// ���� ���� ����. InputManager.OnEnter() ����
    /// </summary>
    public bool isPlaying = true;
    public string title;
    Coroutine coPlaying;

    public Dictionary<string, Sheet> sheets = new Dictionary<string, Sheet>();
    private Dictionary<int, string> LevelSong = new Dictionary<int, string>(); // ���������� ���� �뷡
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
            // Game UI ����
            canvases[(int)Canvas.Game].SetActive(false);

            // playing timer ����
            if (coPlaying != null)
            {
                StopCoroutine(coPlaying);
                coPlaying = null;
            }
        }
        else
        {
            // Editor UI ����
            canvases[(int)Canvas.Editor].SetActive(false);
            Editor.Instance.Stop();

            FindObjectOfType<GridGenerator>().InActivate();

            // �����Ϳ��� ������ ������Ʈ�� ���� �� �����Ƿ� ��������
            StartCoroutine(Parser.Instance.IEParse(title));
            sheets[title] = Parser.Instance.sheet;
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

        // UIObject���� �ڱ��ڽ��� ĳ���Ҷ����� ������ �ְ� ��Ȱ��ȭ(�ӽ��ڵ�)
        await Task.Delay(2000); // 2�� ���� ���

        DisableCanvases();

        // ����ȭ�� ������ ����
        await WaitUntilSheetLoaded();
        ItemGenerator.Instance.Init();

        // Ÿ��Ʋ ȭ�� ����
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
                // ��ٸ�
            }
        });
    }

    private void IESelect()
    {
        // Select UI �ѱ�
        canvases[(int)Canvas.Select].SetActive(true);

        canvases[(int)Canvas.Title].SetActive(false);

        canvases[(int)Canvas.SFX].SetActive(false);

        // �� ������ ������ �� �ְ� ����
        isPlaying = false;
    }

    IEnumerator IEInitPlay()
    {        
        // �� ������ ������ �� ���� ����
        isPlaying = true;

        // ȭ�� ���̵� �ƿ�
        canvases[(int)Canvas.SFX].SetActive(true);
        //yield return StartCoroutine(AniPreset.Instance.IEAniFade(sfxFade, true, 2f));

        //  Select UI ����
        canvases[(int)Canvas.Select].SetActive(false);

        // Sheet �ʱ�ȭ
        //title = "Feelin Like";

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

        // ȭ�� ���̵� ��

        canvases[(int)Canvas.SFX].SetActive(false);

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
        ResultStage.sprite= Resources.Load<Sprite>("Result/ResultLevel"+CurrentStage);
        while (true)
        {
            if (!AudioManager.Instance.IsPlaying())
            {
                break;
            }
            yield return new WaitForSeconds(1f);
        }

        // ȭ�� ���̵� �ƿ�
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
            Debug.Log(Score.Instance.data.score + "�� ����");
            PlayerPrefs.SetInt(CurrentStage + "MaxScore", Score.Instance.data.score);
        }

        if (Score.Instance.data.score >= 10000)
        {
            Debug.Log("����");
            ResultImage.sprite = Resources.Load<Sprite>("Result/Success");
        }
        else
        {
            Debug.Log("����");
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

        // ȭ�� ���̵� ��
        yield return StartCoroutine(AniPreset.Instance.IEAniFade(sfxFade, false, 2f));
        canvases[(int)Canvas.SFX].SetActive(false);

        // 5�� ���
        yield return new WaitForSeconds(5f);

        // ����â �̵�
        SceneManager.LoadScene("StageSelect");
    }
}
