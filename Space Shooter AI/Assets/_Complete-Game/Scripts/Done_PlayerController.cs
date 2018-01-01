using UnityEngine;
using System.Collections;

[System.Serializable]
public class Done_Boundary 
{
	public float xMin, xMax, zMin, zMax;
}

public class Done_PlayerController : MonoBehaviour
{
	public float speed;
	public float tilt;
	public Done_Boundary boundary;

	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;

	private SpaceShooterNavigatorAgent agent = null;
	private Done_Boundary boundaryLocal;
	private float nextFire;
	private SpaceShooterWeaponsAgent weaponsAgent = null;

	void Start()
	{
		agent = transform.gameObject.GetComponentInParent<SpaceShooterNavigatorAgent>();
		weaponsAgent = GetComponentInChildren<SpaceShooterWeaponsAgent>();

		boundaryLocal = boundary;
		if (agent != null)
		{
			boundaryLocal.xMin = boundary.xMin + agent.transform.position.x;
			boundaryLocal.xMax = boundary.xMax + agent.transform.position.x;
			boundaryLocal.zMin = boundary.zMin + agent.transform.position.z;
			boundaryLocal.zMax = boundary.zMax + agent.transform.position.z;
		}
		if (weaponsAgent != null)
		{
			//Debug.Log(gameObject.name + "has weapons agent!");
			GameObject go = GameObject.Find("SpaceShooterBrain");
			if (go != null)
			{
				//Debug.Log("Found SpaceShooterBrain!");
				weaponsAgent.GiveBrain(go.GetComponentInChildren<Brain>());
				//Debug.Log("Assigned brain to player.");
			}
		}
	}

	void Update ()
	{
		if (agent == null)
		{
			if (Input.GetButton("Fire1") && Time.time > nextFire)
			{
				nextFire = Time.time + fireRate;
				Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
				GetComponent<AudioSource>().Play();
			}
		}
	}

	void FixedUpdate ()
	{
		if (agent == null)
		{
			float moveHorizontal = Input.GetAxis("Horizontal");
			float moveVertical = Input.GetAxis("Vertical");

			Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
			GetComponent<Rigidbody>().velocity = movement * speed;

			GetComponent<Rigidbody>().position = new Vector3
			(
				Mathf.Clamp(GetComponent<Rigidbody>().position.x, boundaryLocal.xMin, boundaryLocal.xMax),
				0.0f,
				Mathf.Clamp(GetComponent<Rigidbody>().position.z, boundaryLocal.zMin, boundaryLocal.zMax)
			);

			GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
		}
	}

	public void FireMissile()
	{
		if (Time.time > nextFire)
		{
			nextFire = Time.time + fireRate;
			Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
			GetComponent<AudioSource>().Play();
		}
	}
}
