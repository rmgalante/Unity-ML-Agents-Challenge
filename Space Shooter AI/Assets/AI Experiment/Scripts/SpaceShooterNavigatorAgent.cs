using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShooterNavigatorAgent : SpaceShooterAgentBase
{
	public float movement = 0.5f;

	private Collider boundaryCollider;
	private Done_GameController gameController = null;

	void Start()
	{
		// Find the boundary. We need to use it to keep the player in bounds.
		Collider[] colliders = transform.GetComponentsInChildren<Collider>();
		if (colliders != null && colliders.Length > 0)
		{
			foreach(Collider collider in colliders)
			{
				if (collider.tag == "Boundary")
				{
					boundaryCollider = collider;
					break;
				}
			}
		}
		// Find the GameController
		if (brain.brainType != BrainType.Heuristic)
		{
			gameController = transform.GetComponentInChildren<Done_GameController>();
			if (gameController == null)
			{
				Debug.Log("GameController not found!");
			}
		}
	}

	public override void AgentStep(float[] act)
	{
		if (act != null && act.Length > 0)
		{
			// Get the agent's player
			Done_PlayerController player = gameObject.GetComponentInChildren<Done_PlayerController>();
			if (player != null)
			{
				if (brain.brainParameters.actionSpaceType == StateType.continuous)
				{
					HandleContinuousAction(player, act);
				}
				else
				{
					HandleDiscreteAction(player, act);
				}
			}
		}
	}

	public override void AgentReset()
	{
		if (gameController != null)
		{
			gameController.ResetGame();
		}
	}

	public override void AgentOnDone()
	{
		
	}

	public void EnemyDied(int score)
	{
		// Reward the player when an enemy dies.
		reward += 0.25f;
	}

	public void PlayerDied()
	{
		if (!done)
		{
			// Penalize the player when he dies.
			reward += -0.5f;
			done = true;
		}
	}

	private void HandleContinuousAction(Done_PlayerController player, float[] act)
	{
		float factor;
		// Discourage lack of movement.
		reward -= 0.001f;

		if (act[0] == 1)
		{
			// Encourage movement.
			//// But we want to encourage it to the center.
			//// So we calculate a factor that is 1 in the center and 0 on the bounds.
			//float zHalf = (boundaryCollider.bounds.max.z - boundaryCollider.bounds.min.z) * 0.5f;
			//factor = 1 - Mathf.Abs((zHalf - player.transform.position.z + boundaryCollider.bounds.min.z) / zHalf);
			factor = 1.0f;
			reward += factor * 0.002f;
			// Forward and Backward
			float z = Mathf.Lerp(player.transform.position.z, player.transform.position.z + act[1], player.speed * Time.deltaTime);
			if (boundaryCollider != null && z < boundaryCollider.bounds.center.z - boundaryCollider.bounds.extents.z)
			{
				z = boundaryCollider.bounds.center.z - boundaryCollider.bounds.extents.z;
			}
			if (boundaryCollider != null && z > boundaryCollider.bounds.center.z + boundaryCollider.bounds.extents.z)
			{
				z = boundaryCollider.bounds.center.z + boundaryCollider.bounds.extents.z;
			}
			Vector3 newPosition = new Vector3(player.transform.position.x, player.transform.position.y, z);
			player.transform.position = newPosition;
		}
		else if (act[0] == 2)
		{
			// Encourage movement.
			//// But we want to encourage it to the center.
			//// So we calculate a factor that is 1 in the center and 0 on the bounds.
			//float xHalf = (boundaryCollider.bounds.max.x - boundaryCollider.bounds.min.x) * 0.5f;
			//factor = 1 - Mathf.Abs((xHalf - player.transform.position.x + boundaryCollider.bounds.min.x) / xHalf);
			factor = 1.0f;
			reward += factor * 0.002f;
			// Left and Right
			float x = Mathf.Lerp(player.transform.position.x, player.transform.position.x + act[1], player.speed * Time.deltaTime);
			if (boundaryCollider != null && x < boundaryCollider.bounds.center.x - boundaryCollider.bounds.extents.x)
			{
				x = boundaryCollider.bounds.center.x - boundaryCollider.bounds.extents.x;
			}
			if (boundaryCollider != null && x > boundaryCollider.bounds.center.x + boundaryCollider.bounds.extents.x)
			{
				x = boundaryCollider.bounds.center.x + boundaryCollider.bounds.extents.x;
			}
			Vector3 newPosition = new Vector3(x, player.transform.position.y, player.transform.position.z);
			player.transform.position = newPosition;
		}
		else if (act[0] == 3)
		{
			// Fire
			if (HasTarget(GameState.ObjectCategory.Asteroid) && act[1] == 1f)
			{
				// Fire at asteroid
				reward += 0.005f;
				player.FireMissile();
			}
			else if (HasTarget(GameState.ObjectCategory.Alien) && act[1] == 2f)
			{
				// Fire at alien
				reward += 0.006f;
				player.FireMissile();
			}
			else if (HasTarget(GameState.ObjectCategory.Missile) && act[1] == 3f)
			{
				// Fire at alien missile
				// These are more dangerous and should be avoided, but we still want to fire at them.
				reward += 0.007f;
				player.FireMissile();
			}
			else
			{
				// Discourage firing at nothing
				reward -= 0.0005f;
			}
		}
	}

	private void HandleDiscreteAction(Done_PlayerController player, float[] act)
	{
		float factor = 1.0f;
		float movementLocal = movement;

		if (act[0] == 1f || act[0] == 2f)
		{
			// Assume forward movement
			if (act[0] == 2f)
			{
				// Adjust for backward movement.
				movementLocal *= -1f;
			}
			// Encourage movement.
			// But we want to encourage it to the center.
			// So we calculate a factor that is 1 in the center and 0 on the bounds.
			float zHalf = (boundaryCollider.bounds.max.z - boundaryCollider.bounds.min.z) * 0.5f;
			factor = 1 - Mathf.Abs((zHalf - player.transform.position.z + boundaryCollider.bounds.min.z) / zHalf);
			reward += 0.8f * factor * 0.02f;
			// Forward and Backward
			float z = Mathf.Lerp(player.transform.position.z, player.transform.position.z + movementLocal, player.speed * Time.deltaTime);
			if (boundaryCollider != null && z < boundaryCollider.bounds.center.z - boundaryCollider.bounds.extents.z)
			{
				z = boundaryCollider.bounds.center.z - boundaryCollider.bounds.extents.z;
			}
			if (boundaryCollider != null && z > boundaryCollider.bounds.center.z + boundaryCollider.bounds.extents.z)
			{
				z = boundaryCollider.bounds.center.z + boundaryCollider.bounds.extents.z;
			}
			Vector3 newPosition = new Vector3(player.transform.position.x, player.transform.position.y, z);
			player.transform.position = newPosition;
		}
		else if (act[0] == 3f || act[0] == 4f)
		{
			// Assume right movement
			if (act[0] == 4f)
			{
				// Adjust for left movement.
				movementLocal *= -1f;
			}
			// Encourage movement.
			// But we want to encourage it to the center.
			// So we calculate a factor that is 1 in the center and 0 on the bounds.
			float xHalf = (boundaryCollider.bounds.max.x - boundaryCollider.bounds.min.x) * 0.5f;
			factor = 1 - Mathf.Abs((xHalf - player.transform.position.x + boundaryCollider.bounds.min.x) / xHalf);
			reward += factor * 0.02f;
			// Left and Right
			float x = Mathf.Lerp(player.transform.position.x, player.transform.position.x + movementLocal, player.speed * Time.deltaTime);
			if (boundaryCollider != null && x < boundaryCollider.bounds.center.x - boundaryCollider.bounds.extents.x)
			{
				x = boundaryCollider.bounds.center.x - boundaryCollider.bounds.extents.x;
			}
			if (boundaryCollider != null && x > boundaryCollider.bounds.center.x + boundaryCollider.bounds.extents.x)
			{
				x = boundaryCollider.bounds.center.x + boundaryCollider.bounds.extents.x;
			}
			Vector3 newPosition = new Vector3(x, player.transform.position.y, player.transform.position.z);
			player.transform.position = newPosition;
		}
		else if (act[0] == 5f)
		{
			if (HasTarget(GameState.ObjectCategory.Asteroid))
			{
				// Fire at asteroid
				reward += 0.05f;
				player.FireMissile();
			}
		}
		else if (act[0] == 6f)
		{
			if (HasTarget(GameState.ObjectCategory.Alien))
			{
				// Fire at alien
				reward += 0.06f;
				player.FireMissile();
			}
		}
		else if (act[0] == 7f)
		{
			if (HasTarget(GameState.ObjectCategory.Missile))
			{
				// Fire at alien missile
				// These are more dangerous and should be avoided, but we still want to fire at them.
				reward += 0.07f;
				player.FireMissile();
			}
		}
	}

}
