using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace ArcticLion
{
	public class TestGameMainLayer : Layer
	{
		Ship ship;
		List<EnemyShip> enemyShips;
		private double enemyShipFireDelay = 1d;
//TODO: Remove
		Camera2D camera;
		Queue<Bullet> bullets;
//TODO: change to List?
		Queue<Bullet> enemyBullets;

		public TestGameMainLayer (int z, Ship ship, Camera2D camera) : base(z)
		{
			this.ship = ship;
			this.camera = camera;
			this.Add (ship);

			enemyShips = new List<EnemyShip> ();

			bullets = new Queue<Bullet> ();
			for (int i=0; i<20; i++) {
				Bullet b = new Bullet ();
				bullets.Enqueue (b);
				Add (b);
			}

			enemyBullets = new Queue<Bullet> ();
			for (int i=0; i<20; i++) {
				Bullet b = new Bullet ();
				enemyBullets.Enqueue (b);
				Add (b);
			}

			EnemyShip testEnemy = new TestGameEnemyShip ();

			enemyShips.Add (testEnemy);
			Add (testEnemy);
		}

		public override void Update (GameTime gameTime)
		{
			base.Update (gameTime);

			DetectCollisions ();

			Vector2 mousePositionWorld = GetMousePositionWorld ();

			double rotationAngle = Math.Atan2 ((mousePositionWorld.Y - ship.Position.Y),
			                                   (mousePositionWorld.X - ship.Position.X));

			ship.Rotation = rotationAngle;

			foreach (Bullet b in bullets) {
				//TODO: get the bounds?
				if (b.IsAlive && !camera.IsInView (b.Position, new Rectangle (0, 0, 8, 8))) {
					b.IsAlive = false;
				}
			}

			foreach (Bullet b in enemyBullets) {
				//TODO: get the bounds?
				if (b.IsAlive && !camera.IsInView (b.Position, new Rectangle (0, 0, 8, 8))) {
					b.IsAlive = false;
				}
			}

			MouseState ms = Mouse.GetState ();
			if (ms.LeftButton == ButtonState.Pressed) {
				if (!bullets.Peek ().IsAlive) {
					Bullet newBullet = bullets.Dequeue ();
					Vector2 newBulletVelocity = Vector2.Normalize (mousePositionWorld - ship.Position);
					newBulletVelocity *= 1500f;
					newBulletVelocity += ship.Velocity;
					Vector2 shipYaw = new Vector2 ((float)Math.Cos (ship.Rotation), 
					                               (float)Math.Sin (ship.Rotation));
					newBullet.Shoot (ship.Position + 45 * shipYaw, newBulletVelocity);
					bullets.Enqueue (newBullet);
				}
			}

			//TODO: move shooting logic to enemy AI?
			enemyShipFireDelay -= gameTime.ElapsedGameTime.TotalSeconds;
			if (enemyShipFireDelay <= 0) {
				foreach (EnemyShip es in enemyShips) {
					if (!enemyBullets.Peek ().IsAlive) {
						Bullet newEnemyBullet = enemyBullets.Dequeue ();
						Vector2 newBulletVelocity = Vector2.Normalize (ship.Position - es.Position);
						newBulletVelocity *= 500f;
						newBulletVelocity += es.Velocity;
						newEnemyBullet.Shoot (es.Position, newBulletVelocity);
						enemyBullets.Enqueue (newEnemyBullet);
						enemyShipFireDelay = 1d;
					}
				}
			}
		}

		public void DetectCollisions ()
		{
			foreach (Bullet b in bullets) {
				List<EnemyShip> enemyShipsCopy = new List<EnemyShip> ();
				enemyShipsCopy.AddRange (enemyShips);
				foreach (EnemyShip es in enemyShipsCopy) {
					if (b.IsAlive && /*TODO: Loop thru enemies*/ es.Parent != null &&
						(b.Position - es.Position).Length () < 30f) {
						b.IsAlive = false;

						//TODO: REMOVE!! FOR TESTING PURPOSES
						if (es.Parts.Count > 1) {
							List<EnemyShip> newEnemies = es.DestroyPart (es.Parts[1]);
							if (newEnemies != null) {
								foreach (EnemyShip newEnemyShip in newEnemies) {
									enemyShips.Add (newEnemyShip);
									Add (newEnemyShip);
								}

								newEnemies [0].direction = -1;
								newEnemies [1].direction = 1;

								enemyShips.Remove (es);
								es.Kill ();						
							}
						}
					}
				}
				enemyShips.RemoveAll ((EnemyShip s)=> {return s.Parent == null;});
			}
		}

		private Vector2 GetMousePositionWorld ()
		{
			Vector2 mousePosition = new Vector2 (Mouse.GetState().X, Mouse.GetState ().Y);

			return camera.GetUpperLeftPosition () + mousePosition; 
		}
	}
}