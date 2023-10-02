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
	public int CurrentLevel=1;
	public int PreviousLevel=1;
	public int WalkSpeed=2;

	private Animator animator;
	private bool isMoving = false;
	private float startTime;
	private float journeyLength;

    public Vector2 Stage1 = new Vector2(1.66f, 1.25f);
    public Vector2 Stage2 = new Vector2(8.18f, 1.25f);
    public Vector2 Stage3 = new Vector2(14.56f, 1.25f);
    public Vector2 Stage4 = new Vector2(20.3f, 2.49f);
    public Vector2 Stage5 = new Vector2(25.41f, 2.49f);

	public GameObject[] StageLight; // �������� ��

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

		if (collisionDetected && Input.GetKeyDown(KeyCode.Return) && UIStage.Instance.IsEnter[CurrentLevel-1])
		{
			SceneManager.LoadScene("Game");
		} // �������� ����
		else if(collisionDetected && Input.GetKeyDown(KeyCode.Return) && !UIStage.Instance.IsEnter[CurrentLevel - 1])
		{
			UIStage.Instance.WarningDisplay.SetActive(true);
        }
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

    private void OnTriggerStay2D(Collider2D other)
	{
		// �浹 ����� ��Ȯ�� �±����� Ȯ���մϴ�.
		if (other.CompareTag("Level"))
		{
			collisionDetected = true;
		}
	} 

	private void OnTriggerExit2D(Collider2D other)
	{
		// �浹�� �����Ǿ��� �� ���¸� �ʱ�ȭ�մϴ�.
		if (other.CompareTag("Level"))
		{
            UIStage.Instance.StageLight[PreviousLevel-1].SetActive(false);
            collisionDetected = false;
			UIStage.Instance.isChange = 1;
		}
	}

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
                UIStage.Instance.isChange = 2;
                animator.SetBool("IsWalk", false);
				UIStage.Instance.TitleUI.sprite = Resources.Load<Sprite>("UI/SongTitle0" + CurrentLevel);
                UIStage.Instance.StageUI.sprite = Resources.Load<Sprite>("UI/Stage0" + CurrentLevel);
            }
		}
	} // ĳ���� ������

    private void LevelLocationInit()
    {
        TransformPositions[1] = Stage1;
        TransformPositions[2] = Stage2;
        TransformPositions[3] = Stage3;
        TransformPositions[4] = Stage4;
        TransformPositions[5] = Stage5;
    } // ���� ��ġ
}