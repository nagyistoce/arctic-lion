using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace ArcticLion
{
	public abstract class Level
	{
		protected List<List<EnemyShip>> enemyShipGroups;
		public int CurrentGroup { get; protected set;}

		public Level ()
		{
			enemyShipGroups = new List<List<EnemyShip>> ();
			CurrentGroup = -1;
		}

		public abstract void Build(ContentManager content);

		public List<EnemyShip> MoveToNextGroup(){
			if (CurrentGroup < enemyShipGroups.Count - 1) {
				CurrentGroup ++;
				return enemyShipGroups [CurrentGroup];
			} else {
				return null;
			}
		}

		public bool HasNextGroup(){
			return CurrentGroup < enemyShipGroups.Count - 1;
		}
	}
}