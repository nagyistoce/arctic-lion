using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ArcticLion
{
	public class EnemyShipPart : Node
	{
		String assetName;
		Texture2D enemyShipPartTexture;
		public List<EnemyShipPart> ConnectedParts{ get; private set;}

		public bool isVisited;

		public Vector2 Velocity{ get; set;}

		public EnemyShipPart (String assetName)
		{
			this.assetName = assetName;
			ConnectedParts = new List<EnemyShipPart> (1);
		}

		public override void LoadContent (ContentManager content)
		{	
			enemyShipPartTexture = content.Load<Texture2D> (assetName);
		}

		public override void Update (GameTime gameTime)
		{
			base.Update (gameTime);
		}

		public override void Draw (GameTime gameTime, SpriteBatch spriteBatch)
		{
			base.Draw (gameTime, spriteBatch);

			spriteBatch.Draw(enemyShipPartTexture, 
			                 Position + Parent.Position, 
			                 null, Color.White, 
			                 (float)Rotation,
			                 new Vector2(enemyShipPartTexture.Bounds.Center.X, enemyShipPartTexture.Bounds.Center.Y), 
			                 1.0f,
			                 SpriteEffects.None, 0f);
		}
	}
}

