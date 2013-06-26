using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArcticLion
{
	public class Animation : Node
	{
		Texture2D animSheet;
		int width;
		int height;
		int frames;
		bool isPingPong;
		bool isOneShot;
		float fps;

		private int currentFrame;
		private float elapsedSeconds;
		private int direction = 1;

		public Animation (Texture2D animSheet,
		                  int frames,
		                  float fps = 30f,
		                  bool isPingPong = false, bool isOneShot = true)
		{
			this.animSheet = animSheet;
			this.width = animSheet.Width / frames;
			this.height = animSheet.Height;
			this.frames = frames;
			this.fps = fps;
			this.isPingPong = isPingPong;
			this.isOneShot = isOneShot;
		}

		public override void Update (Microsoft.Xna.Framework.GameTime gameTime)
		{
			base.Update (gameTime);

			elapsedSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;

			if (elapsedSeconds >= 1/fps) {
				currentFrame += direction;
				if (currentFrame > frames - 1) {
					if (isOneShot) {
						Kill ();
					} else {
						if (isPingPong) {
							currentFrame = frames - 2;
							direction = -1;
						} else {
							currentFrame = 0;
						}
					}
				}else if(currentFrame < 0){
					currentFrame = 1;
					direction = 1;
				}
				elapsedSeconds = 0;
			}
		}

		public override void Draw (SpriteBatch spriteBatch)
		{
			base.Draw (spriteBatch);

			if (IsDead)
				return;

			Rectangle des = new Rectangle ((int)(Position.X - width / 2f), 
			                               (int)(Position.Y - height / 2f), 
			                               width, 
			                               height);

			Rectangle src = new Rectangle ();
			src.X = currentFrame * width;
			src.Width = width;
			src.Height = height;

			spriteBatch.Draw (animSheet, des, src, Color.White);
		}
	}
}