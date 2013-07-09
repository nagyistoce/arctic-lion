using System;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class SprayShootingBehavior : ShootingBehavior
	{
		private const double delay = 0.5d;
		private double timeElapsed = 0;

		void ShootingBehavior.Apply(EnemyShip enemyShip, GameTime gameTime){

			throw new NotImplementedException ();

			timeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
			if (timeElapsed >= delay) {
				foreach (EnemyShipPart p in enemyShip.Parts) {
					if (p.Weapon != null) {
						enemyShip.Shoot (p);
					}
				}
				timeElapsed = 0d;
			}
		}
	}
}

