using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoveController : MonoBehaviour
{ 
	private Dictionary<int, Vector2> TransformPositions = new Dictionary<int, Vector2>(); // ���������� ���� ��ġ
	public string Level; // ���������� �̸�
	private bool collisionDetected = false; //�ݸ��� ���� ����
	public int CurrentLevel;
	public int PreviousLevel;
	public int WalkSpeed=2;

	private Animator animator;
	private bool isMoving = false;
	private float startTime;
	private float journeyLength;

	public Image TitleUI;
	public Image StageUI;
	public int UISpeed=2; // UI ���� �ӵ�
	private int isChange = 0; // UI ���� ���� (0�� ���� ����, 1�� �����, 2�� ä����)

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
			Debug.Log("�浹�Ǿ����ϴ�");
			SceneManager.LoadScene("Game");
		} // �������� ����
	}

	private void StartMoving()
	{
		isMoving = true;
		startTime = Time.time;
		Vector2 targetPosition = TransformPositions[CurrentLevel];
		journeyLength = Vector2.Distance(transform.position, targetPosition);
	} // �����̱�

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

	} // �ȱ� �ִϸ��̼� �� �������� ����

	private void LevelLocationInit()
	{
		TransformPositions[1] = new Vector2(1.66f, 1.25f); 
		TransformPositions[2] = new Vector2(8.18f, 1.25f);
		TransformPositions[3] = new Vector2(14.56f, 1.25f);
		TransformPositions[4] = new Vector2(20.3f, 2.49f);
		TransformPositions[5] = new Vector2(25.41f, 2.49f);
	} // ���� ��ġ

	private void OnTriggerStay2D(Collider2D other)
	{
		// �浹 ����� ��Ȯ�� �±����� Ȯ���մϴ�.
		if (other.CompareTag("Level"))
		{
			Debug.Log(CurrentLevel + "����");
			collisionDetected = true;
		}
	} 

	private void OnTriggerExit2D(Collider2D other)
	{
		// �浹�� �����Ǿ��� �� ���¸� �ʱ�ȭ�մϴ�.
		if (other.CompareTag("Level"))
		{
			Debug.Log(PreviousLevel + "���");
			collisionDetected = false;
			isChange = 1;
		}
	}

	private void UIChange()
    {
		if (isChange == 1)
		{
			if (!(TitleUI.fillAmount <= 0))
			{
				float fillAmount = TitleUI.fillAmount;
				TitleUI.fillAmount = fillAmount - Time.deltaTime * UISpeed;
				StageUI.fillAmount = fillAmount - Time.deltaTime * UISpeed;
			}
		}
		else if (isChange == 2)
		{
			if (!(TitleUI.fillAmount >= 1))
			{
				float fillAmount = TitleUI.fillAmount;
				TitleUI.fillAmount = fillAmount + Time.deltaTime * UISpeed;
				StageUI.fillAmount = fillAmount + Time.deltaTime * UISpeed;
			}

			if (TitleUI.fillAmount >= 1)
			{
				isChange = 0;
			}
		}
	} // UI ����

	private void CharacterMove()
    {
		if (isMoving)
		{
			float distanceCovered = (Time.time - startTime) * WalkSpeed; // WalkSpeed�� �ȴ� �ӵ� 
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
	} // ĳ���� ������
}