using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class EnemyShip : Node
	{
		public List<EnemyShipPart> Parts { get; protected set; }
		public Vector2 Velocity { get; set;}

		public EnemyShip ()
		{
			Parts = new List<EnemyShipPart>();
		}

		public override void Add (Node newNode)
		{
			base.Add (newNode);
			if (newNode is EnemyShipPart) {
				Parts.Add((EnemyShipPart) newNode);
			}
		}

		public List<EnemyShip> DestroyPart(EnemyShipPart destroyedPart)
		{
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
					EnemyShip newEnemyShip = new TestGameEnemyShip2 ();
					newEnemyShip.Rotation = Rotation;
	
					VisitPartRecursive(newEnemyShip, p);

					//TODO: test check this crap, supposed to reorganize part positions
					foreach (EnemyShipPart newEnemyShipPart in newEnemyShip.Parts) {
						if (newEnemyShipPart != p) {
							newEnemyShipPart.Position = newEnemyShipPart.Position - p.Position; 
						}
					}
					newEnemyShip.Position = this.Position + p.Position;
					p.Position = Vector2.Zero;

					newEnemies.Add (newEnemyShip);
				}
			}

			return newEnemies;
		}

		private void VisitPartRecursive(EnemyShip ship, EnemyShipPart part)
		{
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