using UnityEngine;
using System.Collections;

public class Done_DestroyByContact : MonoBehaviour
{
	public GameObject explosion;
	public GameObject playerExplosion;
	public int scoreValue;
	private Done_GameController gameController;

	void Start ()
	{
		// 2017-12-24 RMG: Changed the logic so that it gets the GameController in the parent.
		gameController = gameObject.GetComponentInParent<Done_GameController>();
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Boundary" || other.tag == "Enemy")
		{
			return;
		}

		if (explosion != null)
		{
			Instantiate(explosion, transform.position, transform.rotation);
		}

		if (other.tag == "Player")
		{
			if (gameObject.tag == "Player Missile")
			{
				// Player from another boundary must have shot a missile through the boundary collider.
				return;
			}
			// 2017-12-26 RMG: Added try/catch block
			try
			{
				Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
				// 2017-12-23 RMG: Added test for gameController != null
				if (gameController != null)
				{
					gameController.GameOver();
				}
			}
			catch (System.Exception ex)
			{
				Debug.Log("Exception: " + ex.Message);
			}
		}

		// 2017-12-23 RMG: Added test for gameController != null
		if (gameController != null)
		{
			try
			{ 
				gameController.AddScore(scoreValue);
			}
			catch
			{ }
		}
		// 2017-12-23 RMG: Added test for other.tag != "Player"
		if (other.tag != "Player")
		{
			Destroy(other.gameObject);
		}
		Destroy (gameObject);
	}
}