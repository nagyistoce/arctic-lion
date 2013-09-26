using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NuclearWinter.GameFlow;
using NuclearWinter.UI;

namespace ArcticLion
{
	public enum InGameStates
	{
		Running,
		Paused,
		Restarting
	}

	public class GameStateInGame : GameState<Game1>
	{
		Screen screen;
		public Camera2D Camera { get; private set;}
		Random random;

		InGameStates inGameState;
		float gameRestartTimeAccumulator = 0f;
		const float gameRestartDelay = 2f;
		Panel pauseMenuPanel;

		Level1 level1;
		const float EnemyGroupChangeDelay = 2f;
		float enemyGroupChangeTimeAccumulator = 2f; //TODO: put back to zero

		Layer mainLayer;
		Layer effectLayer;

		public Ship Ship { get; private set;}

		ProjectileManager projectileManager;
		const double FireDelay = 0.05d;
		double fireDelayAccumulator = 0;

		private List<EnemyShip> enemyShips;

		Texture2D enemyLocator;
		Vector2 enemyLocatorPosition = Vector2.Zero;
		double enemyLocatorAngle = 0;
		bool enemyLocatorVisible = false;

		public GameStateInGame (Game1 game) : base(game)
		{
		}

		public override void Start ()
		{			
			enemyShips = new List<EnemyShip> ();

			BuildUI ();
			random = new Random ();

			Ship = new Ship ();
			Ship.Position = new Vector2 (0, 0);

			Camera = new Camera2D (Game);
			Camera.Focus = Ship;
			Camera.MoveSpeed = 20f;
			Game.Components.Add (Camera);

			level1 = new Level1 (Ship);
			level1.Build (Game.Content);

			mainLayer = new Layer();
			effectLayer = new Layer ();

			mainLayer.Add (Ship);

			mainLayer.LoadContent (Game.Content);

			projectileManager = new ProjectileManager ();
			projectileManager.LoadContent (Game.Content);

			enemyLocator = Content.Load<Texture2D> (Assets.EnemyLocator);

			inGameState = InGameStates.Running;

			base.Start ();
		}

		public override void Stop ()
		{
			base.Stop ();
		}

		public override void Update (GameTime gameTime)
		{
			screen.IsActive = Game.IsActive;
			screen.HandleInput();
			screen.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

			if(Game.InputMgr.WasKeyJustPressed(Keys.Escape)){
				if (inGameState == InGameStates.Running) {
					PauseGame ();
				} else {
					UnpauseGame ();
				}
			}

			switch (inGameState){
			case InGameStates.Running:
				UpdateGame (gameTime);
				break;

			case InGameStates.Restarting:
				UpdateGame (gameTime);
				gameRestartTimeAccumulator += (float)gameTime.ElapsedGameTime.TotalSeconds;
				if(gameRestartTimeAccumulator >= gameRestartDelay){
					gameRestartTimeAccumulator = 0;
					inGameState = InGameStates.Running;
					Start ();
				}
				break;

			case InGameStates.Paused:
				break;
			}
		}

		public override void Draw ()
		{
			if (!Game.IsActive)
				return;

			Game.SpriteBatch.Begin (
				SpriteSortMode.Deferred,
				BlendState.AlphaBlend,
				SamplerState.LinearWrap,
				DepthStencilState.Default,
				null,
				null,
				Camera.Transform);

			mainLayer.Draw (Game.SpriteBatch);
			projectileManager.Draw (Game.SpriteBatch);
			effectLayer.Draw (Game.SpriteBatch);

			DrawEnemyLocator (Game.SpriteBatch);

			Game.SpriteBatch.End ();

			screen.Draw ();
		}

		public void BuildUI(){
			screen = new Screen (Game, new CustomStyle (Game.Content), 
			                          Game.GraphicsDevice.Viewport.Width, 
			                          Game.GraphicsDevice.Viewport.Height); 

			//Menu
			pauseMenuPanel = new Panel (screen, Game.Content.Load<Texture2D> (Assets.Button), 4);
			pauseMenuPanel.AnchoredRect = AnchoredRect.CreateTopRightAnchored (30, 30, 200, 100);

			TextlessButton quitGameButton = new TextlessButton (screen);
			quitGameButton.Icon = Game.Content.Load<Texture2D> (Assets.TextQuitGame);
			quitGameButton.AnchoredRect = AnchoredRect.CreateTopAnchored (10, 20, 10, 30);
			quitGameButton.ClickHandler += new Action<TextlessButton> ((TextlessButton sender)=> {
				QuitGame();
			});

			pauseMenuPanel.AddChild (quitGameButton);
		}

		private void PauseGame(){
			inGameState = InGameStates.Paused;
			screen.Root.AddChild (pauseMenuPanel);
		}

		private void UnpauseGame(){
			if(inGameState == InGameStates.Paused){
				inGameState = InGameStates.Running;
				screen.Root.RemoveChild (pauseMenuPanel);
			}
		}

		private void QuitGame(){
			Game.Exit ();
		}

		private void DetectCollisions (GameTime gameTime)
		{
			List<EnemyShip> enemyShipsCopy = new List<EnemyShip> (enemyShips);

			foreach (EnemyShip es in enemyShipsCopy) {

				List<EnemyShipPart> enemyShipPartsCopy = new List<EnemyShipPart> (es.Parts);

				foreach (EnemyShipPart part in enemyShipPartsCopy) {
					// Enemy ship parts colliding with projectiles
					List<Projectile> collidingProjectiles = part.FindCollidingProjectiles (projectileManager.ShipBullets);
					foreach(Projectile p in collidingProjectiles){
						p.IsFlying = false;
						part.Health -= 1; //TODO: remove damage 
						if (part.Health <= 0) {
							List<EnemyShip> newEnemies = es.DestroyPart (part);

							foreach (EnemyShip newEnemyShip in newEnemies) {
								newEnemyShip.Target = Ship;
								newEnemyShip.PartDestroyed += new PartDestroyedHandler (HandlePartDestroyed);
								newEnemyShip.WeaponFire += new WeaponFiredHandler (HandleEnemyShipWeaponFire);
								enemyShips.Add (newEnemyShip);
								mainLayer.Add (newEnemyShip);
							}			

							enemyShips.Remove (es);
							es.Kill ();
						}
					}

					// Ship colliding with enemy projectiles
					if (Ship.State == ShipStates.Alive) {
						IEnumerable<EnemyBullet> enemyProjectiles = projectileManager.EnemyBullets;
						foreach (Projectile p in enemyProjectiles) {
							if (p.IsFlying) {
								Rectangle shipHitBox = Ship.GetHitBox ();
								if (shipHitBox.Contains (new Point ((int)p.Position.X, (int)p.Position.Y))) {
									Animation explosion = new Animation (Game.Content.Load<Texture2D> (Assets.Explosion),
									                                    Assets.Explosion1Frames);
									explosion.Position = Ship.Position;
									effectLayer.Add (explosion);

									p.IsFlying = false;
									Ship.State = ShipStates.Dead;
									inGameState = InGameStates.Restarting;
								}
							}
						}
					}
				}
			}	
		}

		private void HandlePartDestroyed(EnemyShipPart destroyedPart, Vector2 position){
			Animation explosion = new Animation (Game.Content.Load<Texture2D>(Assets.Explosion),
			                                     Assets.Explosion1Frames);
			explosion.Position = position;

			effectLayer.Add (explosion);
		}

		private void HandleEnemyShipWeaponFire(EnemyShip enemyShip, EnemyShipPart part){
			Vector2 newBulletVelocity = 200f * Vector2.Normalize (enemyShip.Target.Position - part.Weapon.GetAbsolutePosition());
			newBulletVelocity += enemyShip.Velocity;
			projectileManager.ShootEnemyBullet(enemyShip, part.Weapon.GetAbsolutePosition(), newBulletVelocity);
		}

		private void UpdateGame(GameTime gameTime){
			projectileManager.Update (gameTime);
			projectileManager.CleanProjectiles (Camera);

			DetectCollisions (gameTime);

			enemyGroupChangeTimeAccumulator += (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (enemyGroupChangeTimeAccumulator >= EnemyGroupChangeDelay) {
				enemyGroupChangeTimeAccumulator = 0;
				if (enemyShips.Count == 0 && level1.HasNextGroup ()) {
					enemyShips.AddRange (level1.MoveToNextGroup ());
					foreach (EnemyShip e in enemyShips) {
						e.PartDestroyed += new PartDestroyedHandler (HandlePartDestroyed);
						e.WeaponFire += new WeaponFiredHandler (HandleEnemyShipWeaponFire);
						e.Position += Ship.Position;
						mainLayer.Add (e);
					}
				}
			}

			UpdateShip (gameTime);
			UpdateEnemyLocator (gameTime);
			mainLayer.Update (gameTime);
			effectLayer.Update (gameTime);
		}

		private void UpdateShip(GameTime gameTime){

			switch (Ship.State) {
				case ShipStates.Alive:
					Vector2 mousePositionWorld = GetMousePositionWorld ();
					double rotationAngle = Math.Atan2 ((mousePositionWorld.Y - Ship.Position.Y),
				                                   (mousePositionWorld.X - Ship.Position.X));

					Ship.Rotation = rotationAngle;

					fireDelayAccumulator += gameTime.ElapsedGameTime.TotalSeconds;
					MouseState ms = Mouse.GetState ();
					if (ms.LeftButton == ButtonState.Pressed && fireDelayAccumulator >= FireDelay) {
						Vector2 newBulletVelocity = 600f * Vector2.Normalize (mousePositionWorld - Ship.Position);
						newBulletVelocity += new Vector2 (random.Next(-20,20), random.Next(-20,20));
						newBulletVelocity += Ship.Velocity;
						Vector2 shipYaw = new Vector2 ((float)Math.Cos (Ship.Rotation), 
						                               (float)Math.Sin (Ship.Rotation));

						projectileManager.ShootShipBullet (Ship, Ship.Position + 20 * shipYaw, newBulletVelocity);
						fireDelayAccumulator = 0;
					}
					break;

				case ShipStates.Dead:
					break;
			}
		}

		private void UpdateEnemyLocator(GameTime gameTime){
			EnemyShip closestShip = null;
			float minDistance = 0;
			float distance = 0;

			foreach (EnemyShip es in enemyShips) {
				distance = Vector2.Distance (Ship.Position, es.Position);
				if (closestShip == null) {
					closestShip = es;
					minDistance = distance;
				} else {
					if (distance < minDistance) {
						closestShip = es;
						minDistance = distance;
					}
				}
			}

			if (closestShip != null && 
			    !Camera.IsInView(
							closestShip.Position, 
			                new Rectangle(
								(int)closestShip.Position.X,
								(int)closestShip.Position.Y,
								50,50) //TODO: Get Bounds of enemyShip!!
							)
			    ){

				float w = screen.Bounds.Width / 2f - 40;
				float h = screen.Bounds.Height / 2f - 40;
				float angle = (float) Math.Atan2 ((double)(closestShip.Position.Y - Ship.Position.Y),
				                           		  (double)(closestShip.Position.X - Ship.Position.X));
				enemyLocatorPosition = Ship.Position + new Vector2 (w*(float)Math.Cos(angle), h*(float)Math.Sin(angle));
				enemyLocatorAngle = angle;
				enemyLocatorVisible = true;
			} else {
				enemyLocatorVisible = false;
			}
		}

		private void DrawEnemyLocator(SpriteBatch spriteBatch){
			if (!enemyLocatorVisible)
				return;

			spriteBatch.Draw (enemyLocator, 
			                  enemyLocatorPosition,
			                 null, 
			                 Color.White, 
			                 (float)enemyLocatorAngle,
			                  new Vector2(enemyLocator.Bounds.Center.X,enemyLocator.Bounds.Center.Y),
			                 1f,
			                 SpriteEffects.None, 
			                 1);
		}

		private Vector2 GetMousePositionWorld ()
		{
			Vector2 mousePosition = new Vector2 (Mouse.GetState().X, Mouse.GetState ().Y);

			return Camera.GetUpperLeftPosition () + mousePosition; 
		}
	}
}