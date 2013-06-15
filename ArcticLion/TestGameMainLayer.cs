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
		TestGameScene scene;

		List<EnemyShip> enemyShips;
		private double enemyShipFireDelay = 0.25d; //TODO: move logic to enemy ship?
		Queue<Bullet> bullets;
//TODO: change to List?
		Queue<EnemyBullet> enemyBullets;

		public TestGameMainLayer (TestGameScene scene) : base(scene)
		{
			this.scene = scene;

			enemyShips = new List<EnemyShip> ();

			bullets = new Queue<Bullet> ();
			for (int i=0; i<20; i++) {
				Bullet b = new Bullet ();
				bullets.Enqueue (b);
				Add (b);
			}

			enemyBullets = new Queue<EnemyBullet> ();
			for (int i=0; i<20; i++) {
				EnemyBullet b = new EnemyBullet ();
				enemyBullets.Enqueue (b);
				Add (b);
			}

//			EnemyShip testEnemy = new TestGameEnemyShip ();
//			enemyShips.Add (testEnemy);
//			Add (testEnemy);

			EnemyShip testEnemy3 = new TestGameEnemyShip3 ();
			enemyShips.Add (testEnemy3);
			Add (testEnemy3);
		}

		public override void Update (GameTime gameTime)
		{
			base.Update (gameTime);

			DetectCollisions (gameTime);

			Vector2 mousePositionWorld = GetMousePositionWorld ();

			double rotationAngle = Math.Atan2 ((mousePositionWorld.Y - scene.Ship.Position.Y),
			                                   (mousePositionWorld.X - scene.Ship.Position.X));

			scene.Ship.Rotation = rotationAngle;

			foreach (Bullet b in bullets) {
				//TODO: get the bounds?
				if (b.IsAlive && !scene.Camera.IsInView (b.Position, new Rectangle (0, 0, 8, 8))) {
					b.IsAlive = false;
				}
			}

			foreach (Bullet b in enemyBullets) {
				//TODO: get the bounds?
				if (b.IsAlive && !scene.Camera.IsInView (b.Position, new Rectangle (0, 0, 8, 8))) {
					b.IsAlive = false;
				}
			}

			MouseState ms = Mouse.GetState ();
			if (ms.LeftButton == ButtonState.Pressed) {
				if (!bullets.Peek ().IsAlive) {
					Bullet newBullet = bullets.Dequeue ();
					Vector2 newBulletVelocity = Vector2.Normalize (mousePositionWorld - scene.Ship.Position);
					newBulletVelocity *= 1000f;
					newBulletVelocity += scene.Ship.Velocity;
					Vector2 shipYaw = new Vector2 ((float)Math.Cos (scene.Ship.Rotation), 
					                               (float)Math.Sin (scene.Ship.Rotation));
					newBullet.Shoot (scene.Ship.Position + 45 * shipYaw, newBulletVelocity);
					bullets.Enqueue (newBullet);
				}
			}

			//TODO: move shooting logic to enemy AI?
			enemyShipFireDelay -= gameTime.ElapsedGameTime.TotalSeconds;
			if (enemyShipFireDelay <= 0) {
				foreach (EnemyShip es in enemyShips) {
					if (!enemyBullets.Peek ().IsAlive) {
						EnemyBullet newEnemyBullet = enemyBullets.Dequeue ();
						Vector2 newBulletVelocity = Vector2.Normalize (scene.Ship.Position - es.Position);
						newBulletVelocity *= 300f;
						newBulletVelocity += es.Velocity;
						newEnemyBullet.Shoot (es.Position, newBulletVelocity);
						enemyBullets.Enqueue (newEnemyBullet);
						enemyShipFireDelay = 0.25d;
					}
				}
			}
		}

		private void DetectCollisions (GameTime gameTime)
		{
			foreach (Bullet b in bullets) {
				List<EnemyShip> enemyShipsCopy = new List<EnemyShip> ();
				enemyShipsCopy.AddRange (enemyShips);
				foreach (EnemyShip es in enemyShipsCopy) {
					if (b.IsAlive) {
						foreach(EnemyShipPart p in es.Parts){
							if(p.IsCollidingWith(b)){
								b.IsAlive = false;
								p.Health -= 1; //TODO: remove damage 
								if(p.Health <= 0){
									List<EnemyShip> newEnemies = es.DestroyPart (p);
								
									foreach (EnemyShip newEnemyShip in newEnemies) {
										((TestGameEnemyShip2)newEnemyShip).Target = scene.Ship;
										enemyShips.Add (newEnemyShip);
										Add (newEnemyShip);
									}						

									enemyShips.Remove (es);
									es.Kill ();
								}
								break;
							}
						}

						if (!b.IsAlive)
							break;
					}
				}
			}
		}

		private Vector2 GetMousePositionWorld ()
		{
			Vector2 mousePosition = new Vector2 (Mouse.GetState().X, Mouse.GetState ().Y);

			return scene.Camera.GetUpperLeftPosition () + mousePosition; 
		}
	}
}