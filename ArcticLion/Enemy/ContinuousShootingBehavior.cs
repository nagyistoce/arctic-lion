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

		void ShootingBehavior.Apply(EnemyShip enemyShip, Queue<EnemyBullet> enemyBullets, GameTime gameTime){
			timeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
			if (timeElapsed >= delay) {
				if (!enemyBullets.Peek ().IsAlive) {
					EnemyBullet newEnemyBullet = enemyBullets.Dequeue ();
					Vector2 newBulletVelocity = Vector2.Normalize (enemyShip.Target.Position - enemyShip.Position);
					newBulletVelocity *= 300f;
					newEnemyBullet.Shoot (enemyShip.Position, newBulletVelocity);
					enemyBullets.Enqueue (newEnemyBullet);
					timeElapsed = 0d;
				}
			}
		}
	}
}

