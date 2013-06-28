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
		GameStateInGame gameState;

		List<EnemyShip> enemyShips;
		Queue<Bullet> bullets; //TODO: change to List?
		const double FireDelay = 0.1d;
		double fireDelayAccumulator = 0;

		Layer effectLayer;

		public TestGameMainLayer (GameStateInGame gameState) : base(gameState)
		{
			this.gameState = gameState;

			enemyShips = new List<EnemyShip> ();

			bullets = new Queue<Bullet> ();
			for (int i=0; i<40; i++) {
				Bullet b = new Bullet ();
				bullets.Enqueue (b);
				Add (b);
			}

			//TODO: you know what to do
			EnemyShip testEnemy3 = EnemyShipFactory.GetInstance().CreateTestGameEnemyShip3 (gameState.Ship);
			testEnemy3.Position = new Vector2 (300, 300);
			testEnemy3.PartDestroyed += new PartDestroyedHandler (HandlePartDestroyed);
			enemyShips.Add (testEnemy3);
			Add (testEnemy3);

			effectLayer = new Layer (gameState);
		}

		public override void Update (GameTime gameTime)
		{
			base.Update (gameTime);
			effectLayer.Update (gameTime);

			DetectCollisions (gameTime);

			Vector2 mousePositionWorld = GetMousePositionWorld ();

			double rotationAngle = Math.Atan2 ((mousePositionWorld.Y - gameState.Ship.Position.Y),
			                                   (mousePositionWorld.X - gameState.Ship.Position.X));

			gameState.Ship.Rotation = rotationAngle;

			foreach (Bullet b in bullets) {
				//TODO: get the bounds?
				if (b.IsAlive && !gameState.Camera.IsInView (b.Position, new Rectangle (0, 0, 8, 8))) {
					b.IsAlive = false;
				}
			}

			foreach(EnemyShip es in enemyShips){
				foreach (Bullet b in es.EnemyBullets) {
					//TODO: get the bounds?
					if (b.IsAlive && !gameState.Camera.IsInView (b.Position, new Rectangle (0, 0, 8, 8))) {
						b.IsAlive = false;
					}
				}
			}

			fireDelayAccumulator += gameTime.ElapsedGameTime.TotalSeconds;
			MouseState ms = Mouse.GetState ();
			if (ms.LeftButton == ButtonState.Pressed) {
				if (!bullets.Peek ().IsAlive && fireDelayAccumulator >= FireDelay) {
					Bullet newBullet = bullets.Dequeue ();
					Vector2 newBulletVelocity = Vector2.Normalize (mousePositionWorld - gameState.Ship.Position);
					newBulletVelocity *= 600f;
					newBulletVelocity += gameState.Ship.Velocity;
					Vector2 shipYaw = new Vector2 ((float)Math.Cos (gameState.Ship.Rotation), 
					                               (float)Math.Sin (gameState.Ship.Rotation));
					newBullet.Shoot (gameState.Ship.Position + 45 * shipYaw, newBulletVelocity);
					bullets.Enqueue (newBullet);
					fireDelayAccumulator = 0;
				}
			}
		}

		public override void Draw (SpriteBatch spriteBatch)
		{
			base.Draw (spriteBatch);

			effectLayer.Draw (spriteBatch);
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
										newEnemyShip.Target = gameState.Ship;
										newEnemyShip.PartDestroyed += new PartDestroyedHandler (HandlePartDestroyed);
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

		private void HandlePartDestroyed(EnemyShipPart destroyedPart, Vector2 position){
			Animation explosion = new Animation (gameState.Content.Load<Texture2D>(Assets.Explosion1),
			                                    Assets.Explosion1Frames);
			explosion.Position = position;

			effectLayer.Add (explosion);
		}

		private Vector2 GetMousePositionWorld ()
		{
			Vector2 mousePosition = new Vector2 (Mouse.GetState().X, Mouse.GetState ().Y);

			return gameState.Camera.GetUpperLeftPosition () + mousePosition; 
		}
	}
}