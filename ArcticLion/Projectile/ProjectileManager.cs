using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ArcticLion
{
	public class ProjectileManager : Node
	{
		public Queue<Bullet> ShipBullets { get; private set;}
		public Queue<EnemyBullet> EnemyBullets { get; private set;}

		private const int MaxShipBullets = 40;
		private const int MaxEnemyBullets = 100;

		public ProjectileManager ()
		{
			ShipBullets = new Queue<Bullet> ();
			EnemyBullets = new Queue<EnemyBullet> ();

			for(int k =0; k < MaxShipBullets; k++){
				Bullet b = new Bullet ();
				ShipBullets.Enqueue (b);
				Add (b);
			}

			for(int k =0; k < MaxEnemyBullets; k++){
				EnemyBullet b = new EnemyBullet ();
				EnemyBullets.Enqueue (b);
				Add (b);
			}
		}

		public void ShootShipBullet(Ship ship, Vector2 initPos, Vector2 initVel){
			if (!ShipBullets.Peek ().IsFlying) {
				Bullet newBullet = ShipBullets.Dequeue ();

				newBullet.Position = initPos;
				newBullet.Velocity = initVel;
				newBullet.IsFlying = true;

				ShipBullets.Enqueue (newBullet);
			}
		}

		public void ShootEnemyBullet(EnemyShip enemyShip, Vector2 initPos, Vector2 initVel){
			if (!EnemyBullets.Peek ().IsFlying) {
				EnemyBullet newEnemyBullet = EnemyBullets.Dequeue ();

				newEnemyBullet.Position = initPos;
				newEnemyBullet.Velocity = initVel;
				newEnemyBullet.IsFlying = true;

				EnemyBullets.Enqueue (newEnemyBullet);
			}
		}

		public void CleanProjectiles(Camera2D camera){
			foreach (Node n in children) {
				if (!(n is Projectile))
					continue;

				Projectile p = (Projectile)n;

				if (p.IsFlying && !camera.IsInView (p.Position, new Rectangle (0, 0, 8, 8))) {
					p.IsFlying = false;
				}
			}
		}
	}
}