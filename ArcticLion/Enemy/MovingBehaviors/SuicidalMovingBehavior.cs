using System;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class SuicidalMovingBehavior : MovingBehavior
	{
		void MovingBehavior.Apply(EnemyShip enemyShip, GameTime gameTime){
			enemyShip.Acceleration = 200f * Vector2.Normalize(enemyShip.Target.Position - enemyShip.Position);
			enemyShip.Velocity += enemyShip.Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
			enemyShip.Velocity *= 0.99f;
			enemyShip.Position += enemyShip.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

			double rotationAngle = Math.Atan2 ((enemyShip.Target.Position.Y - enemyShip.Position.Y),
			                                   (enemyShip.Target.Position.X - enemyShip.Position.X));

			enemyShip.Rotation = rotationAngle;
		}
	}
}

