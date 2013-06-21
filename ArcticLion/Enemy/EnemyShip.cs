using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class EnemyShip : Node
	{
		public Node Target { get; set;}
		public List<EnemyShipPart> Parts { get; protected set; }
		public Vector2 Velocity {get; set;}
		public Vector2 Acceleration { get; set; }
		public MovingBehavior MovingBehavior { get; set;}
		public ShootingBehavior ShootingBehavior { get; set;}

		public Queue<EnemyBullet> EnemyBullets;
		private const int MaxNumberOfBullets = 40;

		public EnemyShip (Node target)
		{
			this.Target = target;
			Parts = new List<EnemyShipPart>();

			EnemyBullets = new Queue<EnemyBullet> ();
			for (int i=0; i<MaxNumberOfBullets; i++) {
				EnemyBullet b = new EnemyBullet ();
				EnemyBullets.Enqueue (b);
				Add (b);
			}
		}

		public override void Update (GameTime gameTime)
		{
			base.Update (gameTime);
			if(ShootingBehavior != null) ShootingBehavior.Apply (this, EnemyBullets, gameTime);
			if(MovingBehavior != null) MovingBehavior.Apply (this, gameTime);
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
					EnemyShip newEnemyShip = new EnemyShip (Target);

					VisitPartRecursive(newEnemyShip, p); //This adds parts to the ship

					EnemyShipFactory.GetInstance ().LoadContentFor (newEnemyShip); //TODO: Super Bad Design LOL

					//TODO: test check this crap, supposed to reorganize parts positions
					foreach (EnemyShipPart newEnemyShipPart in newEnemyShip.Parts) {
						if (newEnemyShipPart != p) {
							newEnemyShipPart.Position = newEnemyShipPart.Position - p.Position; 
						}
					}

					float cos = (float)Math.Cos (Rotation);
					float sin = (float)Math.Sin (Rotation);

					newEnemyShip.Position = new Vector2 (p.Position.X * cos - p.Position.Y * sin,
					                                     p.Position.X * sin + p.Position.Y * cos);

					newEnemyShip.Position += this.Position;

					p.Position = Vector2.Zero;

					newEnemyShip.Rotation = Rotation;

					newEnemyShip.ResetBehavior ();

					newEnemies.Add (newEnemyShip);
				}
			}

			return newEnemies;
		}

		//TODO: clean this shit
		private void VisitPartRecursive(EnemyShip ship, EnemyShipPart part)
		{
			if (!part.isVisited) {
				ship.Add (part);
				part.isVisited = true;
				foreach (EnemyShipPart neighbor in part.ConnectedParts) {
					if(!neighbor.isVisited){
						VisitPartRecursive (ship, neighbor);
					}
				}
			}
		}

		private void ResetBehavior(){
			EnemyShipPart dominantPart = Parts[0];

			foreach (EnemyShipPart p in Parts) {
				if (p.Weight > dominantPart.Weight) {
					dominantPart = p;
				}
			}

			MovingBehavior = dominantPart.PreferredMovingBehavior;
			ShootingBehavior = dominantPart.PreferredShootingBehavior;
		}
	}
}