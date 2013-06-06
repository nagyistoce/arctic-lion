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

			Layer mainLayer; 
			Layers.TryGetValue (1, out mainLayer);

			mainLayer.Add (ship);

			camera = new Camera2D (game);
			camera.Focus = ship;
			camera.MoveSpeed = 20f;
			game.Components.Add (camera);
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

			Vector2 mousePositionWorld = GetMousePositionWorld ();

			double rotationAngle = Math.Atan2 ((mousePositionWorld.Y - ship.Position.Y),
			                                   (mousePositionWorld.X - ship.Position.X));

			rotationAngle += Math.PI / 2;

			ship.RotationAngle = rotationAngle;
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

		private Vector2 GetMousePositionWorld()
		{
			Vector2 mousePosition = new Vector2 (Mouse.GetState().X, Mouse.GetState ().Y);

			return camera.GetUpperLeftPosition() + mousePosition; 
		}
	}
}