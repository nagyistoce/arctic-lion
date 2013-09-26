using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class Level1 : Level
	{
		Ship ship;

		public Level1 (Ship ship)
		{
			this.ship = ship;
		}

		public override void Build (Microsoft.Xna.Framework.Content.ContentManager content)
		{
			EnemyShip enemyShip;

			//Group 1
			List<EnemyShip> group1 = new List<EnemyShip> ();
			enemyShip = EnemyShipFactory.GetInstance ().CreateEnemyShipAplha1 (ship); 
			enemyShip.Position = new Vector2 (0, -300);
			enemyShip.LoadContent (content);
			group1.Add(enemyShip);

			enemyShip = EnemyShipFactory.GetInstance ().TestDeserialize (ship);
			enemyShip.Position = new Vector2 (300, 0);
			enemyShip.LoadContent (content);
			group1.Add (enemyShip);

			//Group 2
			List<EnemyShip> group2 = new List<EnemyShip> ();
			enemyShip = EnemyShipFactory.GetInstance ().CreateEnemyShipAplha1 (ship);
			enemyShip.Position = new Vector2 (-300, 30);
			enemyShip.LoadContent (content);
			group2.Add(enemyShip);

			enemyShip = EnemyShipFactory.GetInstance ().CreateEnemyShipAplha1 (ship);
			enemyShip.Position = new Vector2 (300, -30);
			enemyShip.LoadContent (content);
			group2.Add(enemyShip);

			enemyShip = EnemyShipFactory.GetInstance ().CreateEnemyShipAlpha2 (ship);
			enemyShip.Position = new Vector2 (0, -300);
			enemyShip.LoadContent (content);
			group2.Add (enemyShip);

			//Group 3
			List<EnemyShip> group3 = new List<EnemyShip> ();
			enemyShip = EnemyShipFactory.GetInstance ().CreateTestGameEnemyShip3 (ship); 
			enemyShip.Position = new Vector2 (0, -300);
			enemyShip.LoadContent (content);
			group3.Add(enemyShip);

			//Add all groups
			enemyShipGroups.Add (group1);
			enemyShipGroups.Add (group2);
			enemyShipGroups.Add (group3);
		}
	}
}

