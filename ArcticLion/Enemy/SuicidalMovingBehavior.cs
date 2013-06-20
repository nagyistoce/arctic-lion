using System;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class SuicidalMovingBehavior : MovingBehavior
	{
		private Node target;

		public SuicidalMovingBehavior(Node target){
			this.target = target;
		}

		void MovingBehavior.Apply(EnemyShip enemyShip, GameTime gameTime){
			enemyShip.Acceleration = 200f * Vector2.Normalize(target.Position - enemyShip.Position);
			enemyShip.Velocity += enemyShip.Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
			enemyShip.Velocity *= 0.99f;
			enemyShip.Position += enemyShip.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
		}
	}
}

