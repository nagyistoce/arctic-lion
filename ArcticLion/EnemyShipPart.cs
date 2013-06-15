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
		//TODO: Optimize by comparing with all the bullets instead of just one
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
			double rot = this.Rotation;
			rotationMatrix.Right = new Vector3 ((float)Math.Cos(rot), (float)-Math.Sin(rot), 0);
			rotationMatrix.Up = new Vector3 ((float)Math.Sin(rot), (float)Math.Cos (rot), 0);
	
			Matrix vertexMatrix;
			List<Vector2> rotatedVertices = new List<Vector2> (4);
			foreach (Vector2 v in vertices) {
				vertexMatrix = new Matrix ();
				vertexMatrix.M11 = v.X;
				vertexMatrix.M21 = v.Y;
				Matrix rotatedVertexMatrix = rotationMatrix * vertexMatrix;
				rotatedVertices.Add(new Vector2(rotatedVertexMatrix.M11, rotatedVertexMatrix.M21));
			}
	
			for (int k=0; k<rotatedVertices.Count-1; k++) {
				bool isInProjection = IsBulletProjectionInsideEdgeProjection (bullet,
				                                                              rotatedVertices [k],
				                                                              rotatedVertices [k+1]);
				if (!isInProjection)
					return false;
			}

			//Testing last pair of vertices
			return IsBulletProjectionInsideEdgeProjection (bullet,
			                                               rotatedVertices [0],
			                                               rotatedVertices [rotatedVertices.Count-1]);

			//TODO: last and first vertices

			// Between bullet's last and current position
			//TODO: NOT WORKING!!
//			Vector2 bulletLastPosition = bullet.Position - bullet.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
//			Vector2 bulletInterpolatedPosition = bullet.Position - 0.5f * (bulletLastPosition - bullet.Position);
//
//			if (bulletInterpolatedPosition.X - bullet.Radius < maxX && bulletInterpolatedPosition.X + bullet.Radius > minX) {
//				if (bulletInterpolatedPosition.Y - bullet.Radius < maxY && bulletInterpolatedPosition.Y + bullet.Radius> minY) {
//					return true;
//				}
//			}

		}

		private bool IsBulletProjectionInsideEdgeProjection(Bullet bullet, Vector2 v1, Vector2 v2){
			Vector2 axis = v1 - v2;

			float vp1 = Vector2.Dot (v1, axis);
			float vp2 = Vector2.Dot (v2, axis);
			float bp = Vector2.Dot (bullet.Position, axis);
			float radius = bullet.Radius;

			float max =0, min =0;
			if (vp1 > vp2) {
				min = vp1;
				max = vp2;
			} else {
				min = vp2;
				max = vp1;
			}

			return (bp - radius > min && bp + radius < max);
		}
	}
}