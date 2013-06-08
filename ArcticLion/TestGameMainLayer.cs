using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace ArcticLion
{
	public class TestGameMainLayer : Layer
	{
		Ship ship;
		EnemyShip enemyShip;
		Camera2D camera;
		Queue<Bullet> bullets; //TODO: change to List?

		public TestGameMainLayer (int z, Ship ship, Camera2D camera) : base(z)
		{
			this.ship = ship;
			this.camera = camera;
			this.Add (ship);

			bullets = new Queue<Bullet> ();
			for (int i=0; i<20; i++) {
				Bullet b = new Bullet ();
				bullets.Enqueue (b);
				Add (b);
			}

			enemyShip = new EnemyShip ();
			Add (enemyShip);
		}

		public override void Update (GameTime gameTime)
		{
			base.Update (gameTime);

			DetectCollisions ();

			Vector2 mousePositionWorld = GetMousePositionWorld ();

			double rotationAngle = Math.Atan2 ((mousePositionWorld.Y - ship.Position.Y),
			                                   (mousePositionWorld.X - ship.Position.X));

//			rotationAngle += Math.PI / 2;

			ship.Rotation = rotationAngle;

			foreach(Bullet b in bullets){
				//TODO: get the bounds?
				if(b.IsAlive && !camera.IsInView(b.Position, new Rectangle(0,0,8,8))){
					b.IsAlive = false;
				}
			}

			MouseState ms = Mouse.GetState ();
			if (ms.LeftButton == ButtonState.Pressed) {
				if (!bullets.Peek ().IsAlive) {
					Bullet newBullet = bullets.Dequeue ();
					Vector2 newBulletVelocity = Vector2.Normalize(mousePositionWorld - ship.Position);
					newBulletVelocity *= 1500f;
					newBulletVelocity += ship.Velocity;
					Vector2 shipYaw = new Vector2 ((float)Math.Cos (ship.Rotation), 
					                               (float)Math.Sin (ship.Rotation));
					newBullet.Shoot (ship.Position + 45 * shipYaw, newBulletVelocity);
					bullets.Enqueue (newBullet);
				}
			}
		}

		public void DetectCollisions(){
			foreach(Bullet b in bullets){
				if (b.IsAlive &&
				    (b.Position - enemyShip.Position).Length() < 40f){
					b.IsAlive = false;
				}
			}
		}

		private Vector2 GetMousePositionWorld()
		{
			Vector2 mousePosition = new Vector2 (Mouse.GetState().X, Mouse.GetState ().Y);

			return camera.GetUpperLeftPosition() + mousePosition; 
		}
	}
}