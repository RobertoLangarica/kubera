using DG.Tweening;
using UnityEngine;
using System.Collections;

public class PiecesControllAnimation : MonoBehaviour {
	
	public Vector3 explodePosition;

	void Start () {
		//this.transform.localRotation = Quaternion.Euler(0, 0, Random.Range (0, 360));

		//this.transform.DOShakeRotation (2);
		this.transform.DORotate (new Vector3 (0, 0, Random.Range (0, 360)), 0);

	}

	public void setPosition(Vector3 position)
	{
		this.transform.position = position;
	}

	public void startRotate(float speed)
	{
		this.transform.DORotate (new Vector3 (0, 0, this.transform.localRotation.eulerAngles.z + Random.Range(520,720)), speed,RotateMode.FastBeyond360).OnComplete(()=>{
			this.transform.DORotate (new Vector3 (0, 0, this.transform.localRotation.eulerAngles.z + 720),speed,RotateMode.FastBeyond360);
			this.transform.DOMove(explodePosition,speed).OnComplete(()=>{
				this.transform.DORotate (new Vector3 (0, 0, this.transform.localRotation.eulerAngles.z + 720),speed,RotateMode.FastBeyond360);
				this.transform.DOMove(new Vector3(0,0,0),speed).OnComplete(()=>{
					this.transform.DOMove(explodePosition,0);
				});
			});
		});
	}

	public void ultimatePosition(Vector3 ultimatePosition, float speed)
	{
		this.transform.DORotate (new Vector3 (0, 0, this.transform.localRotation.eulerAngles.z + 720),speed,RotateMode.FastBeyond360);
		this.transform.DOMove (ultimatePosition, speed);
	}
}
