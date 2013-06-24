using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NuclearWinter.GameFlow;

namespace ArcticLion
{
	public class GameStateInGame : GameState<Game1>
	{
		Game1 game;
		ContentManager content;
		SpriteBatch spritebatch;

		public Camera2D Camera { get; private set;}

		public Ship Ship { get; private set;}

		TestGameMainLayer testLayer;

		public GameStateInGame (Game1 game) : base(game)
		{
			this.game = game;
			this.content = new ContentManager (game.Services, "Content");
			this.spritebatch = new SpriteBatch (game.GraphicsDevice);

			Ship = new Ship ();
			Ship.Position = new Vector2 (0, 0);

			Camera = new Camera2D (game);
			Camera.Focus = Ship;
			Camera.MoveSpeed = 20f;
			game.Components.Add (Camera);

			testLayer = new TestGameMainLayer(this);
			testLayer.Add (Ship);
		}

		public override void Start ()
		{
			testLayer.LoadContent (content);
			base.Start ();
		}

		public override void Stop ()
		{
			base.Stop ();
		}

		public override void Update (GameTime gameTime)
		{
			testLayer.Update (gameTime);
		}

		public override void Draw ()
		{
			spritebatch.Begin (
				SpriteSortMode.FrontToBack,
				BlendState.AlphaBlend,
				SamplerState.LinearWrap,
				DepthStencilState.Default,
				null,
				null,
				Camera.Transform);

			testLayer.Draw (spritebatch);

			spritebatch.End ();
		}
	}
}