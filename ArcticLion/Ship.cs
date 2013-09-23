using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace ArcticLion
{
	public enum ShipStates{
		Alive,
		Dead
	}

	public class Ship : Node, IFocusable
	{
		Texture2D shipTexture;
		public ShipStates State { get;set; }
		public Vector2 Velocity { get; set;}

		public Ship(){
			State = ShipStates.Alive;
		}

		public override void LoadContent(ContentManager content)
		{
			shipTexture = content.Load<Texture2D> ("ship");
		}

		public override void Update (GameTime gameTime)
		{
			base.Update (gameTime);

			switch (State) {
			case ShipStates.Alive:
				float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

				if(Keyboard.GetState().IsKeyDown(Keys.W)){
					Velocity += new Vector2(0, -20f*elapsedTime);
				}
				if(Keyboard.GetState().IsKeyDown(Keys.S)){
					Velocity += new Vector2(0, 20f*elapsedTime);
				}
				if(Keyboard.GetState().IsKeyDown(Keys.A)){
					Velocity += new Vector2(-20f*elapsedTime, 0);
				}
				if(Keyboard.GetState().IsKeyDown(Keys.D)){
					Velocity += new Vector2(20f*elapsedTime, 0);
				}

				Velocity *= 0.95f;

				Position += Velocity;
				break;
			case ShipStates.Dead:
				break;
			}
		}

		public override void Draw (SpriteBatch spriteBatch)
		{
			base.Draw (spriteBatch);

			spriteBatch.Draw(shipTexture, Position, null, Color.Black, 
			                 (float)Rotation,
			                 new Vector2(shipTexture.Bounds.Center.X, shipTexture.Bounds.Center.Y), 
			                 1.0f, SpriteEffects.None, 0f);
		}

		public Rectangle GetHitBox(){
			Rectangle hitbox = shipTexture.Bounds;
			hitbox.Offset (new Point ((int)GetAbsolutePosition ().X, (int)GetAbsolutePosition ().Y));
			return hitbox;
		}
	}
}
