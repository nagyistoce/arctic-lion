using System;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class PulseShootingBehavior : ShootingBehavior
	{
		const float pulseDelay = 1.5f;
		float pulseDelayAccumulator;
		const float fireDelay = 0.20f;
		float fireDelayAccumulator;
		const int nbShots = 6;
		int timesShot = 0;

		void ShootingBehavior.Apply(EnemyShip enemyShip, GameTime gameTime){

			float elapsedTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

			if (pulseDelayAccumulator >= pulseDelay) {
				if (timesShot < nbShots) {
					if (fireDelayAccumulator >= fireDelay) {
						foreach (EnemyShipPart p in enemyShip.Parts) {
							if (p.Weapon != null) {
								enemyShip.Shoot (p);
							}
						}
						timesShot ++;
						fireDelayAccumulator = 0;
					} else {
						fireDelayAccumulator += elapsedTime;
					}
				} else {
					pulseDelayAccumulator = 0;
					timesShot = 0;
				}
			} else {
				pulseDelayAccumulator += elapsedTime;
			}
		}
	}
}

