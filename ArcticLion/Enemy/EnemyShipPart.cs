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
		public string Name {get;set;}
		public int Health { get; set;}
		public int Weight { get; set;}
		public MovingBehavior PreferredMovingBehavior { get; set;}
		public ShootingBehavior PreferredShootingBehavior { get; set;}

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

			float cos = (float)Math.Cos (Parent.Rotation);
			float sin = (float)Math.Sin (Parent.Rotation);

			Vector2 worldPosition = new Vector2 (Position.X * cos - Position.Y * sin,
			                                     Position.X * sin + Position.Y * cos);

			worldPosition += Parent.Position;

			spriteBatch.Draw(enemyShipPartTexture, 
			                 worldPosition, 
			                 null, 
			                 Color.White,
			                 (float)Parent.Rotation,
			                 new Vector2(enemyShipPartTexture.Bounds.Center.X, enemyShipPartTexture.Bounds.Center.Y), 
			                 1.0f,
			                 SpriteEffects.None, 0f);
		}

		//TODO: Move to a collision detector?
		//TODO: Optimize by comparing with all the bullets instead of just one
		//TODO: Replace rotation matrices by simple Sin and Cos
		public bool IsCollidingWith(Bullet bullet){
			List<Vector2> vertices = new List<Vector2> (4);
			float halfWidth = enemyShipPartTexture.Bounds.Width / 2;
			float halfHeight = enemyShipPartTexture.Bounds.Height / 2;
			vertices.Add (Position + new Vector2(-halfWidth, halfHeight)); //Lower Left
			vertices.Add (Position + new Vector2(halfWidth, halfHeight)); //Lower Right
			vertices.Add (Position + new Vector2(halfWidth, -halfHeight)); //Upper Right
			vertices.Add (Position + new Vector2(-halfWidth, -halfHeight)); //Upper Left
	
			float cos = (float)Math.Cos (Parent.Rotation);
			float sin = (float)Math.Sin (Parent.Rotation);
		
			List<Vector2> rotatedVertices = new List<Vector2> (4);
			for(int k=0; k<vertices.Count;k++){
				rotatedVertices.Add(new Vector2 (vertices[k].X * cos - vertices[k].Y * sin,
				                                 vertices[k].X * sin + vertices[k].Y * cos));
			}

			for (int k =0; k < 4; k++) {
				rotatedVertices[k] += this.Parent.Position;
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
			                                               rotatedVertices [rotatedVertices.Count-1],
			                                               rotatedVertices [0]);
		}

		public static void Connect(EnemyShipPart part1, EnemyShipPart part2){
			part1.ConnectedParts.Add (part2);
			part2.ConnectedParts.Add (part1);
		}

		public static void AssignBehavior(MovingBehavior mb, params EnemyShipPart[] parts){
			foreach (EnemyShipPart p in parts) {
				p.PreferredMovingBehavior = mb;
			}
		}

		public static void AssignBehavior(ShootingBehavior sb, params EnemyShipPart[] parts){
			foreach (EnemyShipPart p in parts) {
				p.PreferredShootingBehavior = sb;
			}
		}

		private bool IsBulletProjectionInsideEdgeProjection(Bullet bullet, Vector2 v1, Vector2 v2){
			Vector2 axis = Vector2.Normalize(v1 - v2);

			float vp1 = Vector2.Dot (v1, axis);
			float vp2 = Vector2.Dot (v2, axis);
			float bp = Vector2.Dot (bullet.Position, axis);
			float radius = bullet.Radius;

			float max =0, min =0;
			if (vp1 > vp2) {
				max = vp1;
				min = vp2;
			} else {
				min = vp2;
				max = vp1;
			}
		
			return bp + radius > min && bp - radius < max;
		}
	}
}