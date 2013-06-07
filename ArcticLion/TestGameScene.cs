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
		private Camera2D camera;
		private SpriteBatch testGameSceneSpriteBatch;
		Ship ship;
		Texture2D logo;

		public TestGameScene (Game game)
		{
			this.game = game;
		
			ship = new Ship ();
			ship.Position = new Vector2 (0, 0);

			camera = new Camera2D (game);
			camera.Focus = ship;
			camera.MoveSpeed = 20f;
			game.Components.Add (camera);

			TestGameMainLayer mainLayer = new TestGameMainLayer(1,ship, camera);
			Layers.Add (mainLayer.Z, mainLayer);
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
				camera.Transform);

			base.Draw (gameTime, testGameSceneSpriteBatch);

			testGameSceneSpriteBatch.Draw (logo, new Vector2 (25, 25), Color.White);

			testGameSceneSpriteBatch.End ();
		}
	}
}