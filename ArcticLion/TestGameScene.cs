using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace ArcticLion
{
	public class TestGameScene : Scene
	{
		private Game game;
		public Camera2D Camera { get; private set;}
		private SpriteBatch testGameSceneSpriteBatch;
		public Ship Ship { get; private set;}
		Texture2D logo;

		public TestGameScene (Game game)
		{
			this.game = game;
		
			Ship = new Ship ();
			Ship.Position = new Vector2 (0, 0);

			Camera = new Camera2D (game);
			Camera.Focus = Ship;
			Camera.MoveSpeed = 20f;
			game.Components.Add (Camera);

			TestGameMainLayer mainLayer = new TestGameMainLayer(this);
			mainLayer.Add (Ship);
			Layers.Add (1, mainLayer);
		}

		public override void LoadContent (ContentManager content)
		{	
			base.LoadContent (content);

			testGameSceneSpriteBatch = new SpriteBatch (game.GraphicsDevice);
			logo = content.Load<Texture2D> ("logo");
		}

		public override void Update (GameTime gameTime)
		{
			base.Update (gameTime);
		}

		public override void Draw (GameTime gameTime, SpriteBatch spriteBatch)
		{
			testGameSceneSpriteBatch.Begin (
				SpriteSortMode.FrontToBack,
				BlendState.AlphaBlend,
				SamplerState.LinearWrap,
				DepthStencilState.Default,
				null,
				null,
				Camera.Transform);

			base.Draw (gameTime, testGameSceneSpriteBatch);

			testGameSceneSpriteBatch.Draw (logo, new Vector2 (25, 25), Color.White);

			testGameSceneSpriteBatch.End ();
		}
	}
}