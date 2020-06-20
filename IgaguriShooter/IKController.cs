using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour
{
	[Header("LookTerget")]
	[SerializeField]private GameObject lookTarget;

	[Header("HandAnchors")]
	[SerializeField]public Transform rightHandAnchor = null;
	[SerializeField]public Transform leftHandAnchor = null;

	[Header("LookWeight")]
	[SerializeField, Range(0.0f, 1.0f)]
	private float lookTotalWeight = 1.0f;
	[SerializeField, Range(0.0f, 1.0f)]
	private float bodyWeight = 0.15f;
	[SerializeField, Range(0.0f, 1.0f)]
	private float headWeight = 0.85f;
	[SerializeField, Range(0.0f, 1.0f)]
	private float eyeWeight = 0.0f;

	private Animator animator;

	void Start()
	{
		animator = GetComponent<Animator>();
	}

	void OnAnimatorIK()
	{
		//アニメーターが無いときは処理しない
		if (!animator)
			return;

		//右手を固定
		if (rightHandAnchor != null)
		{
			animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
			animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
			animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandAnchor.position);
			animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandAnchor.rotation);
		}

		//右手を固定
		if (leftHandAnchor != null)
		{
			animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
			animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
			animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandAnchor.position);
			animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandAnchor.rotation);
		}


		//敵が自分の方向を向くようにする
		if (lookTarget != null)
		{
			animator.SetLookAtWeight(lookTotalWeight, bodyWeight, headWeight, eyeWeight);
			animator.SetLookAtPosition(lookTarget.transform.position);
		}
	}
}
