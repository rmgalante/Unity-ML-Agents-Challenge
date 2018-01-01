using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShooterDecision : MonoBehaviour, Decision
{
	public float MaxFireDistance = 7.0f;
	public float[] Decide(List<float> state, List<Camera> observation, float reward, bool done, float[] memory)
	{
		if (gameObject.GetComponent<Brain>().brainParameters.actionSpaceType == StateType.continuous)
		{
			return HandleContinuousAction(state, observation, reward, done, memory);
		}
		else
		{
			return HandleDiscreteAction(state, observation, reward, done, memory);
		}
	}

	public float[] MakeMemory(List<float> state, List<Camera> observation, float reward, bool done, float[] memory)
	{
		return new float[0];
	}

	private float[] HandleContinuousAction(List<float> state, List<Camera> observation, float reward, bool done, float[] memory)
	{
		// Enemy state starts at index 3
		int enemyIndex = 3;

		// Decide whether or not the player should fire his missile.
		if (state != null && state.Count > 0)
		{
			// state[enemyIndex+0] = is boundary
			// state[enemyIndex+1] = is asteroid
			// state[enemyIndex+2] = is alien ship
			// state[enemyIndex+3] = is alien missile
			// state[enemyIndex+4] = distance
			if ((state[enemyIndex + 1] > 0f || state[enemyIndex + 2] > 0f || state[enemyIndex + 3] > 0f) && state[enemyIndex + 4] <= MaxFireDistance)
			{
				if (state[enemyIndex + 1] > 0f)
				{
					// Fire at asteroid
					return new float[2] { 3, 1f };
				}
				else if (state[enemyIndex + 2] > 0f)
				{
					// Fire at alien ship
					return new float[2] { 3, 2f };
				}
				else if (state[enemyIndex + 3] > 0f)
				{
					// Fire at alien missile
					return new float[2] { 3, 3f };
				}
			}
		}
		// Don't fire
		return new float[2] { 0, 0f };
	}

	private float[] HandleDiscreteAction(List<float> state, List<Camera> observation, float reward, bool done, float[] memory)
	{
		// Enemy state starts at index 3
		int enemyIndex = 3;

		// Decide whether or not the player should fire his missile.
		if (state != null && state.Count > 0)
		{
			// state[enemyIndex+0] = is boundary
			// state[enemyIndex+1] = is asteroid
			// state[enemyIndex+2] = is alien ship
			// state[enemyIndex+3] = is alien missile
			// state[enemyIndex+4] = distance
			if ((state[enemyIndex + 1] > 0f || state[enemyIndex + 2] > 0f || state[enemyIndex + 3] > 0f) && state[enemyIndex + 4] <= MaxFireDistance)
			{
				if (state[enemyIndex + 1] > 0f)
				{
					// Fire at asteroid
					return new float[1] { 5f };
				}
				else if (state[enemyIndex + 2] > 0f)
				{
					// Fire at alien ship
					return new float[1] { 6f };
				}
				else if (state[enemyIndex + 3] > 0f)
				{
					// Fire at alien missile
					return new float[1] { 7f };
				}
			}
		}
		// Don't fire
		return new float[1] { 0f };
	}

}
