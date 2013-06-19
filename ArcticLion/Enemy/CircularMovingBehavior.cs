using System;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class CircularMovingBehavior : MovingBehavior
	{
		void MovingBehavior.Apply(EnemyShip enemyShip, GameTime gameTime){
			double totalTime = gameTime.TotalGameTime.TotalSeconds;

			enemyShip.Velocity = new Vector2 (-(float)Math.Sin(totalTime), 
			                        (float)Math.Cos (totalTime));
			enemyShip.Velocity *= 200f;

			enemyShip.Position += enemyShip.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
			enemyShip.Position += 100f*
				Vector2.Normalize(enemyShip.Target.Position - enemyShip.Position)*
					(float)gameTime.ElapsedGameTime.TotalSeconds;

			double rotationAngle = Math.Atan2 ((enemyShip.Target.Position.Y - enemyShip.Position.Y),
			                                   (enemyShip.Target.Position.X - enemyShip.Position.X));

			enemyShip.Rotation = rotationAngle;
		}
	}
}

