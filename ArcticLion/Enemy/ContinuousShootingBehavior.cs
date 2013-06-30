using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class ContinuousShootingBehavior : ShootingBehavior
	{
		private const double delay = 0.5d;
		private double timeElapsed = 0;

		//TODO: Solve problem -> Bullet disapearing when enemy dies, should bullets be handled outside of the ship?

		void ShootingBehavior.Apply(EnemyShip enemyShip, GameTime gameTime){
			timeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
			if (timeElapsed >= delay) {
				enemyShip.Shoot ();
				timeElapsed = 0d;
			}
		}
	}
}