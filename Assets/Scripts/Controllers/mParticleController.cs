using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class mParticleController : MonoBehaviour
{
	void OnEnable()
	{
		StartCoroutine("CheckIfAlive");
	}

	IEnumerator CheckIfAlive()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.8f);
			if (!GetComponent<ParticleSystem>().IsAlive(true))
			{
				gameObject.SetActive(false);				

			}
		}
	}
}


