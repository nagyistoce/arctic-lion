using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ArcticLion
{
	public class EnemyShipPart : Node
	{
		public String Asset { get; private set;}
		Texture2D enemyShipPartTexture; //TODO: Move to content manager
		public List<EnemyShipPart> ConnectedParts{ get; private set;}
		public int Health { get; set;}
		public int Weight { get; set;}
		public MovingBehavior PreferredMovingBehavior { get; set;}
		public ShootingBehavior PreferredShootingBehavior { get; set;}

		private Weapon _weapon;
		public Weapon Weapon{
			get{ return _weapon; }
			set{ 
				Remove (_weapon);
				_weapon = value;
				Add (_weapon);
			}
		}

		public bool isVisited;

		public Vector2 Velocity{ get; set;}

		public EnemyShipPart (String assetName)
		{
			this.Asset = assetName;
			this.Health = 1;
			ConnectedParts = new List<EnemyShipPart> (1);
		}

		public override void LoadContent (ContentManager content)
		{	
			enemyShipPartTexture = content.Load<Texture2D> (Asset);
		}

		public override void Update (GameTime gameTime)
		{
			base.Update (gameTime);
		}

		public override void Draw (SpriteBatch spriteBatch)
		{
			base.Draw (spriteBatch);

			Vector2 absPosition = GetAbsolutePosition ();

			spriteBatch.Draw(enemyShipPartTexture, 
			                 absPosition, 
			                 null, 
			                 Color.White,
			                 (float)Parent.Rotation,
			                 new Vector2(enemyShipPartTexture.Bounds.Center.X, enemyShipPartTexture.Bounds.Center.Y), 
			                 1.0f,
			                 SpriteEffects.None, 0f);
		}

		//TODO: Move to a collision detector?
		//TODO: Replace rotation matrices by simple Sin and Cos
		public List<Projectile> FindCollidingProjectiles(IEnumerable<Projectile> projectiles){
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

			List<Projectile> collidingProjectiles = new List<Projectile>();

			foreach (Projectile p in projectiles) {
				if (!p.IsFlying)
					continue;

				bool isColliding = true;

				for (int k=0; k<rotatedVertices.Count-1; k++) {
					bool isInProjection = IsBulletProjectionInsideEdgeProjection (p,
					                                                              rotatedVertices [k],
					                                                              rotatedVertices [k+1]);
					if (!isInProjection) {
						isColliding = false;
						break;
					}
				}

				//Testing last pair of vertices
				if(isColliding && IsBulletProjectionInsideEdgeProjection (p,
				                                               rotatedVertices [rotatedVertices.Count-1],
				                                               rotatedVertices [0])){
					collidingProjectiles.Add (p);
				}
			}

			return collidingProjectiles;
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

		private bool IsBulletProjectionInsideEdgeProjection(Projectile projectile, Vector2 v1, Vector2 v2){
			Vector2 axis = Vector2.Normalize(v1 - v2);

			float vp1 = Vector2.Dot (v1, axis);
			float vp2 = Vector2.Dot (v2, axis);
			float pp = Vector2.Dot (projectile.Position, axis);
			float radius = projectile.Radius;

			float max =0, min =0;
			if (vp1 > vp2) {
				max = vp1;
				min = vp2;
			} else {
				min = vp2;
				max = vp1;
			}
		
			return pp + radius > min && pp - radius < max;
		}
	}
}