using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace ArcticLion
{
	public class Ship : Node
	{
		Texture2D shipTexture;
		Vector2 acceleration;

		public Ship ()
		{
			acceleration = new Vector2 ();
		}

		public override void LoadContent(ContentManager content)
		{
			shipTexture = content.Load<Texture2D> ("ship");
		}

		public override void Update (GameTime gameTime)
		{
			base.Update (gameTime);

			int elapsedTime = gameTime.ElapsedGameTime.Milliseconds;

			if(Keyboard.GetState().IsKeyDown(Keys.W)){
				acceleration += new Vector2(0, -0.5f*elapsedTime);
			}
			if(Keyboard.GetState().IsKeyDown(Keys.S)){
				acceleration += new Vector2(0, 0.5f*elapsedTime);
			}
			if(Keyboard.GetState().IsKeyDown(Keys.A)){
				acceleration += new Vector2(-0.5f*elapsedTime, 0);
			}
			if(Keyboard.GetState().IsKeyDown(Keys.D)){
				acceleration += new Vector2(0.5f*elapsedTime, 0);
			}

			acceleration *= 0.70f;

			Position += acceleration;
		}

		public override void Draw (GameTime gameTime, SpriteBatch spriteBatch)
		{
			base.Draw (gameTime, spriteBatch);

			spriteBatch.Draw (shipTexture, Position, Color.White);
		}
	}
}

