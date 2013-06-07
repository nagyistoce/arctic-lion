using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ArcticLion
{
	public class Bullet : Projectile
	{
		private Texture2D bulletTexture;

		public Bullet ()
		{
		}

		public override void LoadContent (ContentManager content)
		{
			base.LoadContent (content);

			bulletTexture = content.Load<Texture2D> ("bullet");
		}

		public override void Update (GameTime gameTime)
		{
			base.Update (gameTime);

			if (IsAlive) {
				Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
		}

		public override void Draw (GameTime gameTime, SpriteBatch spriteBatch)
		{
			base.Draw (gameTime, spriteBatch);

			if (IsAlive) {
				spriteBatch.Draw (bulletTexture, Position, Color.White);
			}
		}

		public void Shoot(Vector2 initPos, Vector2 initVel)
		{
			Position = initPos;
			Velocity = initVel;
			IsAlive = true;
		}
	}
}
