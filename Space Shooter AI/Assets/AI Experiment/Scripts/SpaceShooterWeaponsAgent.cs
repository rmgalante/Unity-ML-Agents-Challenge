using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShooterWeaponsAgent : SpaceShooterAgentBase
{
	public override void AgentStep(float[] act)
	{
		if (act != null && act.Length > 0)
		{
			// Get the agent's player
			Done_PlayerController player = GetComponentInChildren<Done_PlayerController>();
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

	}

	public override void AgentOnDone()
	{

	}

	private void HandleContinuousAction(Done_PlayerController player, float[] act)
	{
		if (act[0] == 3)
		{
			// Fire
			if (HasTarget(GameState.ObjectCategory.Asteroid) && act[1] == 1f)
			{
				// Fire at asteroid
				player.FireMissile();
			}
			else if (HasTarget(GameState.ObjectCategory.Alien) && act[1] == 2f)
			{
				// Fire at alien
				player.FireMissile();
			}
			else if (HasTarget(GameState.ObjectCategory.Missile) && act[1] == 3f)
			{
				// Fire at alien missile
				player.FireMissile();
			}
		}
	}

	private void HandleDiscreteAction(Done_PlayerController player, float[] act)
	{
		if (HasTarget(GameState.ObjectCategory.Asteroid) && act[0] == 5f)
		{
			// Fire at asteroid
			player.FireMissile();
		}
		else if (HasTarget(GameState.ObjectCategory.Alien) && act[0] == 6f)
		{
			// Fire at alien
			player.FireMissile();
		}
		else if (HasTarget(GameState.ObjectCategory.Missile) && act[0] == 7f)
		{
			// Fire at alien missile
			player.FireMissile();
		}
	}
}
