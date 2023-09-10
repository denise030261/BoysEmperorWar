using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
	private Dictionary<int, Vector3> CameraPositions = new Dictionary<int, Vector3>();

	private bool isMoving = false;
    private float startTime;
    private float journeyLength;

	public int CameraSpeed = 1;
	private int CameraLevel;
	private int PreviousCameraLevel;

    private void Awake()
    {
		CameraLevel = PlayerPrefs.GetInt("CameraLevel", 1);
		PlayerPrefs.SetInt("CameraLevel", CameraLevel);

		LevelLocationInit();
		transform.position= CameraPositions[CameraLevel];
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.RightArrow) && MoveController.Instance.CurrentLevel + 1 == 4 && !isMoving)
		{
			PreviousCameraLevel = CameraLevel;
			CameraLevel++;
			PlayerPrefs.SetInt("CameraLevel", CameraLevel);
			StartMoving();
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow) && MoveController.Instance.CurrentLevel - 1 == 3 && !isMoving)
		{
			PreviousCameraLevel = CameraLevel;
			CameraLevel--;
			PlayerPrefs.SetInt("CameraLevel", CameraLevel);
			StartMoving();
		}

		if (isMoving)
		{
			float distanceCovered = (Time.time - startTime) * CameraSpeed; // WalkSpeed´Â °È´Â ¼Óµµ 
			float fractionOfJourney = distanceCovered / journeyLength;

			Vector3 originalPosition = CameraPositions[PreviousCameraLevel];
			Vector3 targetPosition = CameraPositions[CameraLevel];
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
        Vector3 targetPosition = CameraPositions[CameraLevel];
        journeyLength = Vector3.Distance(transform.position, targetPosition);
    }

	private void LevelLocationInit()
	{
		CameraPositions[1] = new Vector3(8.24f, 0.339f,-10);
		CameraPositions[2] = new Vector3(20.47f, 0.339f,-10);
	}
}
