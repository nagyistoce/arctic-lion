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
		public int Health { get; set; }
		public bool isVisited;

		public Vector2 Velocity{ get; set;}

		public EnemyShipPart (String assetName)
		{
			this.assetName = assetName;
			this.Health = 1;
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

		//TODO: Move to a collision detector?
		public bool IsCollidingWith(Bullet bullet, GameTime gameTime){
			Rectangle bounds = enemyShipPartTexture.Bounds;
			List<Vector2> vertices = new List<Vector2> (4);
			float halfWidth = bounds.Width / 2;
			float halfHeight = bounds.Height / 2;
			vertices.Add (Position + new Vector2(-halfWidth, halfHeight)); //Upper Left
			vertices.Add (Position + new Vector2(halfWidth, halfHeight)); //Upper Right
			vertices.Add (Position + new Vector2(-halfWidth, -halfHeight)); //Lower Left
			vertices.Add (Position + new Vector2(-halfWidth, -halfHeight)); //Lower Right

			for (int k =0; k < 4; k++) {
				vertices[k] += this.Parent.Position;
			}

			Matrix rotationMatrix = new Matrix ();
			double r = this.Rotation;
			rotationMatrix.Right = new Vector3 ((float)Math.Cos(r), (float)-Math.Sin(r), 0);
			rotationMatrix.Up = new Vector3 ((float)Math.Sin(r), (float)Math.Cos (r), 0);
	
			Matrix vertexMatrix;
			List<Vector2> rotatedVertices = new List<Vector2> (4);
			foreach (Vector2 v in vertices) {
				vertexMatrix = new Matrix ();
				vertexMatrix.M11 = v.X;
				vertexMatrix.M21 = v.Y;
				Matrix rotatedVertexMatrix = rotationMatrix * vertexMatrix;
				rotatedVertices.Add(new Vector2(rotatedVertexMatrix.M11, rotatedVertexMatrix.M21));
			}

			float maxX = rotatedVertices [0].X;
			float minX = rotatedVertices [0].X;
			float maxY = rotatedVertices [0].Y;
			float minY = rotatedVertices [0].Y;
		
			foreach (Vector2 v in rotatedVertices) {
				if(v.X > maxX) {maxX = v.X;}
				if(v.X < minX) {minX = v.X;}
				if(v.Y > maxY) {maxY = v.Y;}
				if (v.Y < minY) {minY = v.Y;}
			}

			//Current bullet's current
			if (bullet.Position.X - bullet.Radius < maxX && bullet.Position.X + bullet.Radius > minX) {
				if (bullet.Position.Y - bullet.Radius < maxY && bullet.Position.Y + bullet.Radius > minY) {
					return true;
				}
			}

			// Between bullet's last and current position
			//TODO: NOT WORKING!!
			Vector2 bulletLastPosition = bullet.Position - bullet.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
			Vector2 bulletInterpolatedPosition = bullet.Position - 0.5f * (bulletLastPosition - bullet.Position);

			if (bulletInterpolatedPosition.X - bullet.Radius < maxX && bulletInterpolatedPosition.X + bullet.Radius > minX) {
				if (bulletInterpolatedPosition.Y - bullet.Radius < maxY && bulletInterpolatedPosition.Y + bullet.Radius> minY) {
					return true;
				}
			}

			return false;
		}
	}
}