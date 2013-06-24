using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ArcticLion
{
	public class Bullet : Projectile
	{
		protected Texture2D bulletTexture;
		public float Radius { get; set;}

		public Bullet ()
		{
			Radius = 2f;
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

		public override void Draw (SpriteBatch spriteBatch)
		{
			base.Draw (spriteBatch);

			if (IsAlive) {
				//TODO: Clean this!
				spriteBatch.Draw (bulletTexture, 
				                  new Vector2(Position.X - bulletTexture.Width/2, Position.Y - bulletTexture.Height/2), 
				                  Color.White);
			}
		}

		public void Shoot(Vector2 initPos, Vector2 initVel)
		{
			Position = initPos;
			Velocity = initVel;
			IsAlive = true;
		}
	}

	public class EnemyBullet : Bullet{

		public EnemyBullet()
		{
			Radius = 4f;
		}

		public override void LoadContent (ContentManager content)
		{
			bulletTexture = content.Load<Texture2D> ("bullet_e");
		}
	}
}
