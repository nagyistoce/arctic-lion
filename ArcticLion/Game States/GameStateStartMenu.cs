using System;
using Microsoft.Xna.Framework.Graphics;
using NuclearWinter.GameFlow;
using NuclearWinter.UI;

namespace ArcticLion
{
	public class GameStateStartMenu : GameState<Game1>
	{
		Screen screen;
		Panel mainPanel;

		public GameStateStartMenu (Game1 game) : base(game)
		{
		}

		public override void Start ()
		{
			base.Start ();

			BuildUI ();
		}

		public override void Update (Microsoft.Xna.Framework.GameTime gameTime)
		{
			screen.IsActive = Game.IsActive;
			screen.HandleInput();
			screen.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
		}

		public override void Draw ()
		{
			screen.Draw ();
		}

		private void BuildUI(){
			screen = new Screen (Game, new CustomStyle (Game.Content), 
			                          Game.GraphicsDevice.Viewport.Width, 
			                          Game.GraphicsDevice.Viewport.Height); 

			mainPanel = new Panel (screen, Game.Content.Load<Texture2D> (Assets.Button), 4);

			TextlessButton startGameButton = new TextlessButton (screen);
			startGameButton.AnchoredRect = AnchoredRect.CreateTopAnchored ((int)(screen.Bounds.Width/2f - 70),
			                                                              100,
			                                                              (int)(screen.Bounds.Width / 2f - 70),
			                                                              50);
			startGameButton.ClickHandler += new Action<TextlessButton> ((TextlessButton obj) => {
				Game.GameStateMgr.SwitchState(Game.InGame);
			});
			mainPanel.AddChild (startGameButton);

			screen.Root.AddChild (mainPanel);
		}
	}
}
