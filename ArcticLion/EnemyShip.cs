using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class EnemyShip : Node
	{
		//TODO:Remove this shit!
		public int direction = 1;

		public List<EnemyShipPart> Parts { get; protected set; }
		public Vector2 Velocity { get; set;}

		public EnemyShip ()
		{
			Parts = new List<EnemyShipPart> (3);
		}

		//TODO: Move to TestGameEnemyShip
		public override void Update (GameTime gameTime)
		{
			base.Update (gameTime);

			double totalTime = gameTime.TotalGameTime.TotalSeconds;

			//TODO: remove this shit
			Velocity = new Vector2 ((float)Math.Cos(totalTime), 
			                        (float)Math.Sin (totalTime));
			Velocity *= 100f * Math.Abs((float)Math.Sin(totalTime));

			Position += direction*Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
		}

		public List<EnemyShip> DestroyPart(EnemyShipPart destroyedPart){

			List<EnemyShip> newEnemies = new List<EnemyShip> ();

			foreach (EnemyShipPart p in Parts) {
				p.isVisited = false;
				p.ConnectedParts.Remove (destroyedPart);
			}

			Parts.Remove(destroyedPart);
			destroyedPart.Kill ();

			foreach (EnemyShipPart p in Parts) {
				if (!p.isVisited) {
					//TODO: Create a ship type depending on the parts
					EnemyShip newEnemyShip = new EnemyShip ();
					newEnemyShip.Position = this.Position;

					VisitPartRecursive(newEnemyShip, p);
					newEnemies.Add (newEnemyShip);
				}
			}

			if(newEnemies.Count > 0){
				return newEnemies;
			}else{
				return null;
			}
		}

		private void VisitPartRecursive(EnemyShip ship, EnemyShipPart part){
			if (!part.isVisited) {
				ship.Add (part);
				ship.Parts.Add (part);
				part.isVisited = true;
				foreach (EnemyShipPart neighbor in part.ConnectedParts) {
					if(!neighbor.isVisited){
						VisitPartRecursive (ship, neighbor);
					}
				}
			}
		}
	}
}