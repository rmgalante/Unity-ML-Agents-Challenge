using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShooterAgentBase : Agent
{
	public float MaxFireDistance = 20f;
	public float RayLength = 40.0f;

	protected class GameState
	{
		public enum ObjectCategory
		{
			Boundary,
			Asteroid,
			Alien,
			Missile
		}
		public ObjectCategory Category = ObjectCategory.Boundary;
		public float Distance = 20.0f;
	}
	protected List<float> state = new List<float>();

	public override List<float> CollectState()
	{
		// Clear the state
		state.Clear();

		// Get the agent's player
		Done_PlayerController player = gameObject.GetComponentInChildren<Done_PlayerController>();

		// Where is the player?
		state.Add(player != null ? player.transform.position.x : gameObject.transform.position.x);
		state.Add(player != null ? player.transform.position.y : gameObject.transform.position.y);
		state.Add(player != null ? player.transform.position.z : gameObject.transform.position.z);
		// Where are the enemies?
		AddStateForward(state, Vector3.forward, player);
		return state;
	}

	private void AddStateForward(List<float> list, Vector3 direction, Done_PlayerController player)
	{
		// Notes:	The forward raycast needs to cast a wider net.
		//			Try the ray directly in front of the player.
		//			Then try the ray slightly to the left of the player.
		//			Then try the ray slightly to the right of the player.
		// Create empty game state.
		GameState state = new GameState();
		// If player exists
		if (player != null)
		{
			direction = player.transform.TransformDirection(direction);
			// Cast ray and get distance.
			RaycastHit hit;

			if (Physics.Raycast(player.transform.position, direction, out hit, RayLength))
			{
				// In front
				state.Distance = hit.distance;
				if (hit.collider.gameObject.CompareTag("Boundary"))
				{
					state.Category = GameState.ObjectCategory.Boundary;
				}
				else if (hit.collider.gameObject.name.Contains("Asteroid"))
				{
					state.Category = GameState.ObjectCategory.Asteroid;
				}
				else if (hit.collider.gameObject.name.Contains("Enemy Ship"))
				{
					state.Category = GameState.ObjectCategory.Alien;
				}
				else if (hit.collider.gameObject.name.Contains("Bolt-Enemy"))
				{
					state.Category = GameState.ObjectCategory.Missile;
				}
			}
			else if (Physics.Raycast(new Vector3(player.transform.position.x - 0.35f, player.transform.position.y, player.transform.position.z), direction, out hit, RayLength))
			{
				// Slightly left
				state.Distance = hit.distance;
				if (hit.collider.gameObject.CompareTag("Boundary"))
				{
					state.Category = GameState.ObjectCategory.Boundary;
				}
				else if (hit.collider.gameObject.name.Contains("Asteroid"))
				{
					state.Category = GameState.ObjectCategory.Asteroid;
				}
				else if (hit.collider.gameObject.name.Contains("Enemy Ship"))
				{
					state.Category = GameState.ObjectCategory.Alien;
				}
				else if (hit.collider.gameObject.name.Contains("Bolt-Enemy"))
				{
					state.Category = GameState.ObjectCategory.Missile;
				}
			}
			else if (Physics.Raycast(new Vector3(player.transform.position.x + 0.35f, player.transform.position.y, player.transform.position.z), direction, out hit, RayLength))
			{
				// Slightly right
				state.Distance = hit.distance;
				if (hit.collider.gameObject.CompareTag("Boundary"))
				{
					state.Category = GameState.ObjectCategory.Boundary;
				}
				else if (hit.collider.gameObject.name.Contains("Asteroid"))
				{
					state.Category = GameState.ObjectCategory.Asteroid;
				}
				else if (hit.collider.gameObject.name.Contains("Enemy Ship"))
				{
					state.Category = GameState.ObjectCategory.Alien;
				}
				else if (hit.collider.gameObject.name.Contains("Bolt-Enemy"))
				{
					state.Category = GameState.ObjectCategory.Missile;
				}
			}
		}
		TransferState(list, direction, state);

		//if (gameObject.name == "Agent1" && state.Category != GameState.ObjectCategory.Boundary)
		//{
		//	Debug.Log("Direction: (" + direction + "), Category: " + state.Category + ", Distance: " + state.Distance);
		//}
	}

	protected bool HasTarget(GameState.ObjectCategory category)
	{
		// Enemy state starts at index 3
		int enemyIndex = 3;
		bool hasTarget = false;

		// Decide whether or not the player should fire his missile.
		// state[enemyIndex+0] = is boundary
		// state[enemyIndex+1] = is asteroid
		// state[enemyIndex+2] = is alien ship
		// state[enemyIndex+3] = is alien missile
		// state[enemyIndex+4] = distance
		// Discrete actions
		if (state != null && state.Count > 0)
		{
			switch (category)
			{
				case GameState.ObjectCategory.Asteroid:
					if (state[enemyIndex + 1] > 0f && state[enemyIndex + 4] <= MaxFireDistance)
					{
						hasTarget = true;
					}
					break;
				case GameState.ObjectCategory.Alien:
					if (state[enemyIndex + 2] > 0f && state[enemyIndex + 4] <= MaxFireDistance)
					{
						hasTarget = true;
					}
					break;
				case GameState.ObjectCategory.Missile:
					if (state[enemyIndex + 3] > 0f && state[enemyIndex + 4] <= MaxFireDistance)
					{
						hasTarget = true;
					}
					break;
				default:
					break;
			}
		}
		return hasTarget;
	}

	private void TransferState(List<float> list, Vector3 direction, GameState state)
	{
		// Transfer state to list
		list.Add(state.Category == GameState.ObjectCategory.Boundary ? 1 : 0);
		list.Add(state.Category == GameState.ObjectCategory.Asteroid ? 1 : 0);
		list.Add(state.Category == GameState.ObjectCategory.Alien ? 1 : 0);
		list.Add(state.Category == GameState.ObjectCategory.Missile ? 1 : 0);
		list.Add(state.Distance);
	}
}
