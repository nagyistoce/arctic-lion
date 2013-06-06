using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace ArcticLion
{
	public class Ship : Node, IFocusable
	{
		Texture2D shipTexture;
		Vector2 acceleration;
		public double RotationAngle;

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

			float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

			if(Keyboard.GetState().IsKeyDown(Keys.W)){
				acceleration += new Vector2(0, -20f*elapsedTime);
			}
			if(Keyboard.GetState().IsKeyDown(Keys.S)){
				acceleration += new Vector2(0, 20f*elapsedTime);
			}
			if(Keyboard.GetState().IsKeyDown(Keys.A)){
				acceleration += new Vector2(-20f*elapsedTime, 0);
			}
			if(Keyboard.GetState().IsKeyDown(Keys.D)){
				acceleration += new Vector2(20f*elapsedTime, 0);
			}

			acceleration *= 0.95f;

			Position += acceleration;
		}

		public override void Draw (GameTime gameTime, SpriteBatch spriteBatch)
		{
			base.Draw (gameTime, spriteBatch);

			spriteBatch.Draw(shipTexture, Position, null, Color.White, 
			                 (float)RotationAngle,
			                 new Vector2(shipTexture.Bounds.Center.X, shipTexture.Bounds.Center.Y), 
			                 1.0f, SpriteEffects.None, 0f);
		}
	}
}
