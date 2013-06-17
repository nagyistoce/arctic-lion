using System;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public interface MovingBehavior
	{
		void Apply (EnemyShip enemyShip, GameTime gameTime);
	}
}