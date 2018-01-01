using UnityEngine;
using System.Collections;

public class Done_DestroyByBoundary : MonoBehaviour
{
	void OnTriggerExit (Collider other) 
	{
		// 2017-12-26 RMG: Added logic to ignore Player objects.
		if (other.tag != "Player")
		{
			Destroy(other.gameObject);
		}
	}
}