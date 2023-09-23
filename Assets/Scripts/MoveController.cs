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

	private Animator animator;
	private bool isMoving = false;
	private float startTime;
	private float journeyLength;

	public Image TitleUI;
	public Image StageUI;
	public int UISpeed=2; // UI 변경 속도
	private int isChange = 0; // UI 변경 여부 (0은 변경 없음, 1은 사라짐, 2는 채워짐)
	private int TitleUIposition=-399;
	private int StageUIposition=370;

    public Vector2 Stage1 = new Vector2(1.66f, 1.25f);
    public Vector2 Stage2 = new Vector2(8.18f, 1.25f);
    public Vector2 Stage3 = new Vector2(14.56f, 1.25f);
    public Vector2 Stage4 = new Vector2(20.3f, 2.49f);
    public Vector2 Stage5 = new Vector2(25.41f, 2.49f);

    public static MoveController Instance { get; private set; }

    private void Awake()
    {
		Instance = this;
		LevelLocationInit();
		animator = GetComponent<Animator>();

		CurrentLevel =PlayerPrefs.GetInt("Level", 1 );
		PlayerPrefs.SetInt("Level", CurrentLevel);
		transform.position = TransformPositions[CurrentLevel];

		TitleUI.sprite = Resources.Load<Sprite>("UI/SongTitle0" + CurrentLevel);
		StageUI.sprite = Resources.Load<Sprite>("UI/Stage0" + CurrentLevel);
	}

	//Graphic & Input Updates	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.RightArrow) && CurrentLevel+1<=5 && !isMoving)
        {
			SetIsWalk(true, false);
			StartMoving();
		}
		else if(Input.GetKeyDown(KeyCode.LeftArrow) && CurrentLevel - 1 > 0 && !isMoving)
        {
			SetIsWalk(true,true);
			StartMoving();
		}

		CharacterMove();

		UIChange();

		if (collisionDetected && Input.GetKeyDown(KeyCode.Return))
		{
			Debug.Log("충돌되었습니다");
			SceneManager.LoadScene("Game");
		} // 스테이지 입장
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
			scale.x = -Mathf.Abs(scale.x); // Flip x scale
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
			Debug.Log(CurrentLevel + "진입");
			collisionDetected = true;
		}
	} 

	private void OnTriggerExit2D(Collider2D other)
	{
		// 충돌이 해제되었을 때 상태를 초기화합니다.
		if (other.CompareTag("Level"))
		{
			Debug.Log(PreviousLevel + "벗어남");
			collisionDetected = false;
			isChange = 1;
		}
	}
	//480
	private void UIChange()
    {
		if (isChange == 1)
		{
			if(TitleUI.rectTransform.anchoredPosition.x>=-399)
			{
				float UITitleRecX = TitleUI.rectTransform.anchoredPosition.x+ Time.deltaTime * 5000;
				TitleUI.rectTransform.anchoredPosition = new Vector2(UITitleRecX, TitleUI.rectTransform.anchoredPosition.y);

                float UIStageRecX = StageUI.rectTransform.anchoredPosition.x - Time.deltaTime * 4000;
                StageUI.rectTransform.anchoredPosition = new Vector2(UIStageRecX, StageUI.rectTransform.anchoredPosition.y);
            }
		}
		else if (isChange == 2)
		{
            if (TitleUI.rectTransform.anchoredPosition.x >= 399)
            {
                TitleUI.rectTransform.anchoredPosition = new Vector2(399, TitleUI.rectTransform.anchoredPosition.y);
                StageUI.rectTransform.anchoredPosition = new Vector2(-370, StageUI.rectTransform.anchoredPosition.y);
            }
            if (TitleUI.rectTransform.anchoredPosition.x <= 399)
            {
                float UITitleRecX = TitleUI.rectTransform.anchoredPosition.x - Time.deltaTime * 5000;
                TitleUI.rectTransform.anchoredPosition = new Vector2(UITitleRecX, TitleUI.rectTransform.anchoredPosition.y);

                float UIStageRecX = StageUI.rectTransform.anchoredPosition.x + Time.deltaTime * 4000;
                StageUI.rectTransform.anchoredPosition = new Vector2(UIStageRecX, StageUI.rectTransform.anchoredPosition.y);

            }

            if (TitleUI.rectTransform.anchoredPosition.x <= -399)
            {
                isChange = 0;
                TitleUI.rectTransform.anchoredPosition = new Vector2(-399, TitleUI.rectTransform.anchoredPosition.y);
                StageUI.rectTransform.anchoredPosition = new Vector2(370, StageUI.rectTransform.anchoredPosition.y);
            }
        }
	} // UI 변경

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
				isMoving = false;
				isChange = 2;
				animator.SetBool("IsWalk", false);
				TitleUI.sprite = Resources.Load<Sprite>("UI/SongTitle0" + CurrentLevel);
				StageUI.sprite = Resources.Load<Sprite>("UI/Stage0" + CurrentLevel);
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
}