using System;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class PendulumMovingBehavior : MovingBehavior
	{
		void MovingBehavior.Apply(EnemyShip enemyShip, GameTime gameTime){
			if (enemyShip.Target != null) {
				enemyShip.Velocity = Vector2.Normalize((enemyShip.Target.Position - enemyShip.Position));
				enemyShip.Velocity *= 150f;
				if (enemyShip.ID % 2 == 0) {
					enemyShip.Velocity *= (float)Math.Cos (gameTime.TotalGameTime.TotalSeconds);
				} else {
					enemyShip.Velocity *= (float)Math.Sin (gameTime.TotalGameTime.TotalSeconds);
				}
				enemyShip.Position += enemyShip.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
		}
	}
}

