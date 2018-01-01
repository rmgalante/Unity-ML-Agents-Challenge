using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Done_GameController : MonoBehaviour
{
	public GameObject[] hazards;
	public Vector3 spawnValues;
	public int hazardCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;
	
	public GUIText scoreText;
	public GUIText restartText;
	public GUIText gameOverText;

	public SpaceShooterNavigatorAgent agent = null;
	public GameObject playerPrefab = null;

	private bool gameStarted = false;
	private bool gameOver;
	private bool restart;
	private int score;

	private Vector3 playerSpawnPosition;
	private GameObject player;

	public float restartTimer = 5f;
	private float restartTimerTime = 0f;

	private SpaceShooterAcademy academy = null;
	private int numHazards = 1;
	private int numHazardsCount;

	private System.Guid waveCoroutineId = System.Guid.NewGuid();

	void Start ()
	{
		numHazardsCount = hazardCount;
		playerSpawnPosition = transform.position;
		if (agent != null)
		{
			playerSpawnPosition = new Vector3(agent.transform.position.x, playerSpawnPosition.y, agent.transform.position.z);
		}
		// Instantiate instance of Player
		Quaternion playerRotation = Quaternion.identity;
		player = Instantiate<GameObject>(playerPrefab, playerSpawnPosition, playerRotation);
		player.transform.SetParent(transform);
		ResetGame();
		if (academy == null)
		{
			// Get a reference to the SpaceShooterAcademy.
			academy = Object.FindObjectOfType<SpaceShooterAcademy>();
			numHazards = academy.hazards;
			numHazardsCount = academy.hazardsCount;
		}
	}
	
	void Update ()
	{
		if (restart)
		{
			if (agent == null)
			{
				if (Input.GetKeyDown(KeyCode.R))
				{
					SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
				}
			}
			else
			{
				// Need a delay timer so we don't restart while the asteroids and aliens are falling.
				restartTimerTime -= Time.deltaTime;
				if (restartTimerTime <= 0f)
				{
					// Notify the Agent that the player died.
					if (agent != null)
					{
						// Call the agent's PlayerDied function. This will reset the game.
						agent.PlayerDied();
					}
				}
			}
		}
	}
	
	IEnumerator SpawnWaves (System.Guid waveId)
	{
		System.Guid thisWaveId = waveId;

		yield return new WaitForSeconds (startWait);
		while (true)
		{
			// Failsafe to stop this coroutine.
			if (thisWaveId != waveCoroutineId)
			{
				break;
			}

			// See if the Academy has changed our hazards and hazards counts.
			if (academy != null)
			{
				numHazards = academy.hazards;
				numHazardsCount = academy.hazardsCount;
				//Debug.Log("Academy found - " + numHazards + ", " + numHazardsCount);
			}

			if (!gameOver)
			{
				player.gameObject.SetActive(true);
				for (int i = 0; i < numHazardsCount; i++)
				{
					GameObject hazard = hazards[Random.Range(0, numHazards)];
					Vector3 spawnPosition = new Vector3(Random.Range(transform.position.x - spawnValues.x, transform.position.x + spawnValues.x), spawnValues.y, transform.position.z + spawnValues.z);
					Quaternion spawnRotation = Quaternion.identity;
					GameObject go = Instantiate(hazard, spawnPosition, spawnRotation);
					go.transform.SetParent(gameObject.transform);
					yield return new WaitForSeconds(spawnWait);

					if (gameOver || thisWaveId != waveCoroutineId)
					{
						// Break out of for loop
						break;
					}
				}
				if (!gameOver && thisWaveId == waveCoroutineId)
				{
					yield return new WaitForSeconds(waveWait);
				}
			}
			if (gameOver)
			{
				if (agent == null)
				{
					restartText.text = "Press 'R' for Restart";
					restart = true;
				}
				else
				{
					restartTimerTime = restartTimer;
					restart = true;
				}
				break;
			}
		}
		Debug.Log(agent.gameObject.name + " exiting SpawnWaves.");
	}
	
	public void AddScore (int newScoreValue)
	{
		if (agent == null)
		{
			score += newScoreValue;
			UpdateScore();
		}
		else
		{
			SpaceShooterAcademy.score += newScoreValue;
			agent.EnemyDied(newScoreValue);
		}
	}
	
	void UpdateScore ()
	{
		if (agent == null)
		{
			scoreText.text = "Score: " + score;
		}
	}
	
	public void GameOver ()
	{
		gameOver = true;
		gameStarted = false;

		StartCoroutine(DestroyPlayerAndEnemies());
		if (agent == null)
		{
			gameOverText.text = "Game Over!";
		}
	}

	private IEnumerator DestroyPlayerAndEnemies()
	{
		yield return new WaitForSeconds(0.1f);
		try
		{
			/*
			* 2017-12-31 RMG: We no longer need to instantiate the player. Just activate him.
			DestroyImmediate(player, true);
			*/
			player.gameObject.SetActive(false);
		}
		catch
		{
			// Ignore errors. This player may have been destroyed already.
		}
		Done_DestroyByContact[] enemies = transform.GetComponentsInChildren<Done_DestroyByContact>();
		for(int i=0; i < enemies.Length; i++)
		{
			if (enemies[i].CompareTag("Enemy"))
			{
				try
				{
					DestroyImmediate(enemies[i].gameObject, true);
				}
				catch
				{
					// Ignore errors. This enemy may have been destroyed already.
				}
			}
		}
	}
	public void ResetGame()
	{
		if (gameStarted)
			return;

		Debug.Log(agent.gameObject.name + " resetting game!");

		gameStarted = true;
		gameOver = false;
		restart = false;
		restartText.text = "";
		gameOverText.text = "";
		restartTimerTime = restartTimer;
		score = 0;
		//UpdateScore();
		/*
		 * 2017-12-31 RMG: We no longer need to instantiate the player. Just activate him.
		// Instantiate instance of Player
		Quaternion playerRotation = Quaternion.identity;
		player = Instantiate<GameObject>(playerPrefab, playerSpawnPosition, playerRotation);
		player.transform.SetParent(transform);
		 */
		player.transform.position = playerSpawnPosition;
		player.gameObject.SetActive(true);
		// Start waves
		waveCoroutineId = System.Guid.NewGuid();
		StartCoroutine(SpawnWaves(waveCoroutineId));
	}
}