using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveController : MonoBehaviour
{
	private Animator animator; 
	private Dictionary<int, Vector2> TransformPositions = new Dictionary<int, Vector2>(); // 스테이지에 따른 위치


	Rigidbody2D rigid;

	Vector3 movement;
	bool isJumping = false;
    private void Awake()
    {
		LevelLocationInit();
		animator = GetComponent<Animator>();
		// Prefabs으로 현재 위치 결정
	}

    void Start()
	{
	}

	//Graphic & Input Updates	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.RightArrow))
        {
			SetIsWalk(true, false);
		}
		else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
			SetIsWalk(true,true);
		}
	}

	public void SetIsWalk(bool walk, bool flip)
	{
		// Set the "IsWalk" parameter of the Animator
		animator.SetBool("IsWalk", walk);

		if(flip)
        {
			Vector2 scale = transform.localScale;
			scale.x = -Mathf.Abs(scale.x); // Flip x scale
			transform.localScale = scale;
		}
		else
        {
			Vector2 scale = transform.localScale;
			scale.x = +Mathf.Abs(scale.x); // Flip x scale
			transform.localScale = scale;
		}
	}

	private void LevelLocationInit()
	{
		TransformPositions[0] = new Vector2(1.66f, 1.25f);
		TransformPositions[1] = new Vector2(8.18f, 1.25f);
		TransformPositions[2] = new Vector2(14.56f, 1.25f);
		TransformPositions[3] = new Vector2(20.3f, 2.49f);
		TransformPositions[4] = new Vector2(25.41f, 2.49f);
	}
}