using System;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class TestGameEnemyShip2 : EnemyShip
	{
		public TestGameEnemyShip2 (Node target) : base(target)
		{
		}

		public override void Update (GameTime gameTime)
		{
			base.Update (gameTime);

			if (Target != null) {
				Velocity = Vector2.Normalize((Target.Position - this.Position));
				Velocity *= 150f;
				if (id % 2 == 0) {
					Velocity *= (float)Math.Cos (gameTime.TotalGameTime.TotalSeconds);
				} else {
					Velocity *= (float)Math.Sin (gameTime.TotalGameTime.TotalSeconds);
				}
				Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
		}
	}
}

