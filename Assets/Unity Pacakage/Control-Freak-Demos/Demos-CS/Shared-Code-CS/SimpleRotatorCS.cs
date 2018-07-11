using UnityEngine;


[AddComponentMenu("ControlFreak-Demos-CS/SimpleRotatorCS")]
public class SimpleRotatorCS : MonoBehaviour 
	{
	public Vector3 rotSpeed = new Vector3(0, 90, 0);	// degrees per second

	private Quaternion 	initialRot;
	private Vector3 	curRot; 


	// ---------------------
	private void Start()
		{	
		this.initialRot = this.transform.localRotation;
		}

	// ---------------------
	private void Update()
		{
		this.curRot += this.rotSpeed * Time.deltaTime;
		this.transform.localRotation = Quaternion.Euler(this.curRot) * this.initialRot;
		}
	}
