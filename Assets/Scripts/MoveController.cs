using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoveController : MonoBehaviour
{ 
	private Dictionary<int, Vector2> TransformPositions = new Dictionary<int, Vector2>(); // 스테이지에 따른 위치
	public string Level; // 스테이지의 이름
	private bool collisionDetected = false; //콜리전 감지 여부
	public int CurrentLevel=1;
	public int PreviousLevel=1;
	public int WalkSpeed=2;

    public string BGMName;
	public string SFXMove;
	public string SFXEnter;

    private Animator animator;
	public bool isMoving = false;
	private float startTime;
	private float journeyLength;

    public Vector2 Stage1 = new Vector2(1.66f, 1.25f);
    public Vector2 Stage2 = new Vector2(8.18f, 1.25f);
    public Vector2 Stage3 = new Vector2(14.56f, 1.25f);
    public Vector2 Stage4 = new Vector2(20.3f, 2.49f);
    public Vector2 Stage5 = new Vector2(25.41f, 2.49f);

	public GameObject[] StageLight; // 스테이지 불

	public Button PlayButton;

    public static MoveController Instance { get; private set; }

    private void Awake()
    {
		Instance = this;
		LevelLocationInit();
		animator = GetComponent<Animator>();

		CurrentLevel =PlayerPrefs.GetInt("Level", 1 );
		PlayerPrefs.SetInt("Level", CurrentLevel);
        transform.position = TransformPositions[CurrentLevel];
    }
    private void Start() 
    {
        PlayButton.interactable = true;
        MainAudioManager.Instance.PlayBGM(BGMName);
    }

    //Graphic & Input Updates	
    void Update()
	{
		if (Input.GetKeyDown(KeyCode.RightArrow) && CurrentLevel+1<=5 && !isMoving && UIStage.Instance.isChange==0)
        {
			MoveState(false);
        }
		else if(Input.GetKeyDown(KeyCode.LeftArrow) && CurrentLevel - 1 > 0 && !isMoving && UIStage.Instance.isChange == 0)
        {
            MoveState(true);
        }

		CharacterMove();

		if (collisionDetected && Input.GetKeyDown(KeyCode.Return) && UIStage.Instance.IsEnter[CurrentLevel-1])
		{
            MainAudioManager.Instance.StopMusicBGM();
            MainAudioManager.Instance.PlaySFX(SFXEnter);
            PlayerPrefs.SetInt("StoryLevel", 0);
            PlayerPrefs.SetString("State", "Before");
            PlayerPrefs.SetString("Scene", "StageSelect");
            SceneManager.LoadScene("Loading(Imsi)");
		} // 스테이지 입장
		else if(collisionDetected && Input.GetKeyDown(KeyCode.Return) && !UIStage.Instance.IsEnter[CurrentLevel - 1])
		{
            MainAudioManager.Instance.PlaySFX(SFXEnter);
            UIOption.Instance.WarningDisplay.SetActive(true);
        }
	}

	private void StartMoving()
	{
		isMoving = true;
		startTime = Time.time;
		Vector2 targetPosition = TransformPositions[CurrentLevel];
		journeyLength = Vector2.Distance(transform.position, targetPosition);
	} // 움직이기

	public void SetIsWalk(bool walk, bool flip)
	{
		// Set the "IsWalk" parameter of the Animator
		animator.SetBool("IsWalk", walk);

		PreviousLevel = CurrentLevel;
		if (flip)
        {
			Vector2 scale = transform.localScale;
			scale.x = -Mathf.Abs(scale.x);
			transform.localScale = scale;
			CurrentLevel--;
		}
		else
        {
			Vector2 scale = transform.localScale;
			scale.x = +Mathf.Abs(scale.x); // Flip x scale
			transform.localScale = scale;
			CurrentLevel++;
		}
		PlayerPrefs.SetInt("Level", CurrentLevel);

	} // 걷기 애니메이션 및 스테이지 변경
    private void OnTriggerStay2D(Collider2D other)
	{
		// 충돌 대상이 정확한 태그인지 확인합니다.
		if (other.CompareTag("Level"))
		{
			collisionDetected = true;
        }
	} 

	private void OnTriggerExit2D(Collider2D other)
	{
		// 충돌이 해제되었을 때 상태를 초기화합니다.
		if (other.CompareTag("Level"))
		{
            collisionDetected = false;
			UIStage.Instance.isChange = 1;
            Debug.Log("스테이지에 벗어났습니다 "+UIStage.Instance.isChange);
        }
	}

	private void CharacterMove()
    {
		if (isMoving)
		{
			float distanceCovered = (Time.time - startTime) * WalkSpeed; // WalkSpeed는 걷는 속도 
			float fractionOfJourney = distanceCovered / journeyLength;

			Vector2 originalPosition = TransformPositions[PreviousLevel];
			Vector2 targetPosition = TransformPositions[CurrentLevel];
			transform.position = Vector3.Lerp(originalPosition, targetPosition, fractionOfJourney);

			if (fractionOfJourney >= 1.0f)
			{
                PlayButton.interactable = true;
                isMoving = false;
                UIStage.Instance.isChange = 2;
                animator.SetBool("IsWalk", false);
				UIStage.Instance.TitleUI.sprite = Resources.Load<Sprite>("UI/SongTitle0" + CurrentLevel);
                UIStage.Instance.StageUI.sprite = Resources.Load<Sprite>("UI/Stage0" + CurrentLevel);
                Debug.Log("스테이지가 바뀌었습니다 " + UIStage.Instance.isChange);
            }
		}
	} // 캐릭터 움직임

    private void LevelLocationInit()
    {
        TransformPositions[1] = Stage1;
        TransformPositions[2] = Stage2;
        TransformPositions[3] = Stage3;
        TransformPositions[4] = Stage4;
        TransformPositions[5] = Stage5;
    } // 발판 위치

    private void MoveState(bool flip)
    {
        PlayButton.interactable = false;
        UIOption.Instance.PrePlayImage.color = new Color(255, 255, 255, 255);
        MainAudioManager.Instance.StopMusicBGM();
        MainAudioManager.Instance.PlaySFX(SFXMove);
        UIStage.Instance.StageLight[CurrentLevel - 1].SetActive(false);
        SetIsWalk(true, flip);
        StartMoving();
    } // 캐릭터가 움직일 때
}
