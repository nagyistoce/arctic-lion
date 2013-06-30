using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public delegate void PartDestroyedHandler(EnemyShipPart destroyedPart, Vector2 position);
	public delegate void WeaponFiredHandler(EnemyShip enemyShip);

	public class EnemyShip : Node
	{
		public Node Target { get; set;}
		public List<EnemyShipPart> Parts { get; protected set; }
		public Vector2 Velocity {get; set;}
		public Vector2 Acceleration { get; set; }
		public MovingBehavior MovingBehavior { get; set;}
		public ShootingBehavior ShootingBehavior { get; set;}

		public event PartDestroyedHandler PartDestroyed;
		public event WeaponFiredHandler WeaponFire;

		public EnemyShip (Node target)
		{
			this.Target = target;
			Parts = new List<EnemyShipPart>();
		}

		public override void Update (GameTime gameTime)
		{
			base.Update (gameTime);
			if(ShootingBehavior != null) ShootingBehavior.Apply (this, gameTime);
			if(MovingBehavior != null) MovingBehavior.Apply (this, gameTime);
		}

		public override void Draw (Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
		{
			base.Draw (spriteBatch);
		}

		public override void Add (Node newNode)
		{
			base.Add (newNode);
			if (newNode is EnemyShipPart) {
				Parts.Add((EnemyShipPart) newNode);
			}
		}

		public void Shoot(){
			OnWeaponFired (this);
		}

		public List<EnemyShip> DestroyPart(EnemyShipPart destroyedPart)
		{
			List<EnemyShip> newEnemies = new List<EnemyShip> ();

			foreach (EnemyShipPart p in Parts) {
				p.isVisited = false;
				p.ConnectedParts.Remove (destroyedPart);
			}

			destroyedPart.Kill ();
			Parts.Remove (destroyedPart);

			float cos = (float)Math.Cos (Rotation);
			float sin = (float)Math.Sin (Rotation);

			Vector2 destroyedPartPosition = new Vector2 (destroyedPart.Position.X * cos - destroyedPart.Position.Y * sin, 
			                                             destroyedPart.Position.X * sin + destroyedPart.Position.Y * cos);
			destroyedPartPosition += this.Position;
			OnPartDestroyed (destroyedPart, destroyedPartPosition);

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

		protected virtual void OnPartDestroyed(EnemyShipPart destroyedPart, Vector2 position) 
		{
			if (PartDestroyed != null)
				PartDestroyed(destroyedPart, position);
		}

		protected virtual void OnWeaponFired(EnemyShip enemyShip){
			if (WeaponFire != null)
				WeaponFire (enemyShip);
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