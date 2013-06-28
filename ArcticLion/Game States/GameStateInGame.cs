using System;
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

		public Ship Ship { get; private set;}

		bool isPaused = false;
		Panel pauseMenuPanel;

		TestGameMainLayer testLayer;

		public GameStateInGame (Game1 game) : base(game)
		{
		}

		public override void Start ()
		{
			BuildUI ();

			Ship = new Ship ();
			Ship.Position = new Vector2 (0, 0);

			Camera = new Camera2D (Game);
			Camera.Focus = Ship;
			Camera.MoveSpeed = 20f;
			Game.Components.Add (Camera);

			testLayer = new TestGameMainLayer(this);
			testLayer.Add (Ship);

			testLayer.LoadContent (Game.Content);

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
				testLayer.Update (gameTime);
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

			testLayer.Draw (Game.SpriteBatch);

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
	}
}