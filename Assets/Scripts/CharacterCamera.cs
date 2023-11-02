using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
	private Dictionary<int, Vector3> CameraPositions = new Dictionary<int, Vector3>();

	private bool isMoving = false;
    private float startTime;
    private float journeyLength;

	public int CameraSpeed = 1;
	private int CurrentCamera = 1;
	private int PreviousCamera = 1;

	public Vector3 Stage1 = new Vector3(1.92f, 0.6f, -10);
    public Vector3 Stage2 = new Vector3(9.2f, 0.6f, -10);
    public Vector3 Stage3 = new Vector3(15.2f, 1f, -10);
    public Vector3 Stage4 = new Vector3(20.7f, 1f, -10);
    public Vector3 Stage5 = new Vector3(26f, 1f, -10);

    private void Awake()
    {
        CurrentCamera = PlayerPrefs.GetInt("Level", 1);
        PlayerPrefs.SetInt("Level", CurrentCamera);

        LevelLocationInit();
        Debug.Log(CurrentCamera);
        transform.position= CameraPositions[CurrentCamera];
	}

    private void Start()
    {
        PreviousCamera = MoveController.Instance.PreviousLevel;
        CurrentCamera = MoveController.Instance.CurrentLevel;
    }

    void Update()
	{
        if (Input.GetKeyDown(KeyCode.RightArrow) && CurrentCamera + 1 <= 5 
            && !isMoving && !MoveController.Instance.isMoving)
		{
            PreviousCamera = CurrentCamera;
			CurrentCamera++;
            StartMoving();
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow) && CurrentCamera - 1 > 0 
            && !isMoving && !MoveController.Instance.isMoving)
		{
            PreviousCamera = CurrentCamera;
            CurrentCamera--;
            StartMoving();
        }

		if (isMoving)
		{
			float distanceCovered = (Time.time - startTime) * CameraSpeed; // WalkSpeed´Â °È´Â ¼Óµµ 
			float fractionOfJourney = distanceCovered / journeyLength;

			Vector3 originalPosition = CameraPositions[PreviousCamera];
			Vector3 targetPosition = CameraPositions[CurrentCamera];
			transform.position = Vector3.Lerp(originalPosition, targetPosition, fractionOfJourney);

			if (fractionOfJourney >= 1.0f)
			{
				isMoving = false;
			}
		}
	}

    private void StartMoving()
    {
        isMoving = true;
        startTime = Time.time;
        Vector3 targetPosition = CameraPositions[CurrentCamera];
        journeyLength = Vector3.Distance(transform.position, targetPosition);
    }

	private void LevelLocationInit()
	{
		CameraPositions[1] = Stage1;
		CameraPositions[2] = Stage2;
        CameraPositions[3] = Stage3;
        CameraPositions[4] = Stage4;
        CameraPositions[5] = Stage5;
    }
}
