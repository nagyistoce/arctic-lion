using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public interface ShootingBehavior
	{
		void Apply(EnemyShip enemyShip, GameTime gameTime);
	}
}

