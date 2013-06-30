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
	public class GameStateInGame : GameState<Game1>
	{
		Screen screen;
		public Camera2D Camera { get; private set;}
		Random random;

		bool isPaused = false;
		Panel pauseMenuPanel;

		Level1 level1;

		Layer mainLayer;
		Layer effectLayer;

		public Ship Ship { get; private set;}

		ProjectileManager projectileManager;
		const double FireDelay = 0.05d;
		double fireDelayAccumulator = 0;

		private List<EnemyShip> enemyShips;

		public GameStateInGame (Game1 game) : base(game)
		{
			enemyShips = new List<EnemyShip> ();
		}

		public override void Start ()
		{			
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
				if (!isPaused) {
					PauseGame ();
				} else {
					UnpauseGame ();
				}
			}

			if (!isPaused) {
				projectileManager.Update (gameTime);
				projectileManager.CleanProjectiles (Camera);

				DetectCollisions (gameTime);

				if (enemyShips.Count == 0 && level1.HasNextGroup()) {
					enemyShips.AddRange (level1.MoveToNextGroup ());
					foreach (EnemyShip e in enemyShips) {
						e.PartDestroyed += new PartDestroyedHandler (HandlePartDestroyed);
						e.WeaponFire += new WeaponFiredHandler (HandleEnemyShipWeaponFire);
						e.Position += Ship.Position;
						mainLayer.Add (e);
					}
				}

				UpdateShip (gameTime);
				mainLayer.Update (gameTime);
				effectLayer.Update (gameTime);
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
			isPaused = true;
			screen.Root.AddChild (pauseMenuPanel);
		}

		private void UnpauseGame(){
			isPaused = false;
			screen.Root.RemoveChild (pauseMenuPanel);
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
				}
			}	
		}

		private void HandlePartDestroyed(EnemyShipPart destroyedPart, Vector2 position){
			Animation explosion = new Animation (Game.Content.Load<Texture2D>(Assets.Explosion),
			                                     Assets.Explosion1Frames);
			explosion.Position = position;

			effectLayer.Add (explosion);
		}

		private void HandleEnemyShipWeaponFire(EnemyShip enemyShip){
			Vector2 newBulletVelocity = 300f * Vector2.Normalize (enemyShip.Target.Position - enemyShip.Position);
			projectileManager.ShootEnemyBullet(enemyShip, enemyShip.Position, newBulletVelocity);
		}

		private void UpdateShip(GameTime gameTime){
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
		}

		private Vector2 GetMousePositionWorld ()
		{
			Vector2 mousePosition = new Vector2 (Mouse.GetState().X, Mouse.GetState ().Y);

			return Camera.GetUpperLeftPosition () + mousePosition; 
		}
	}
}