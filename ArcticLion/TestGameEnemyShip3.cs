using System;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class TestGameEnemyShip3 : EnemyShip
	{
		Node target;

		public TestGameEnemyShip3 (Node target)
		{
			this.target = target;

			Build ();
		}

		public override void Update (GameTime gameTime)
		{
			base.Update (gameTime);

			double totalTime = gameTime.TotalGameTime.TotalSeconds;

			Velocity = new Vector2 (-(float)Math.Sin(totalTime), 
			                        (float)Math.Cos (totalTime));
			Velocity *= 200f;

			Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
			Position += 100f*Vector2.Normalize(target.Position - Position)*(float)gameTime.ElapsedGameTime.TotalSeconds;

			double rotationAngle = Math.Atan2 ((target.Position.Y - Position.Y),
			                                   (target.Position.X - Position.X));

			Rotation = rotationAngle;
		}

		private void Build(){
			EnemyShipPart body = new EnemyShipPart ("test_part_body");
			EnemyShipPart core = new EnemyShipPart ("test_part_core");
			EnemyShipPart leftJoint = new EnemyShipPart ("part");
			EnemyShipPart rightJoint = new EnemyShipPart ("part");
			EnemyShipPart leftArm = new EnemyShipPart ("test_part_arm");
			EnemyShipPart rightArm = new EnemyShipPart ("test_part_arm");
			EnemyShipPart tail = new EnemyShipPart ("test_part_tail");

			body.Health = 100;
			core.Health = 150;
			leftJoint.Health = 50;
			rightJoint.Health = 50;
			leftArm.Health = 100;
			rightArm.Health = 100;
			tail.Health = 100;

			core.Position = new Vector2 (64, 0);
			leftJoint.Position = new Vector2 (0, -64);
			rightJoint.Position = new Vector2 (0, 64);
			leftArm.Position = new Vector2 (32, -104);
			rightArm.Position = new Vector2 (32, 104);
			tail.Position = new Vector2 (-64, 0);

			body.ConnectedParts.Add (tail);
			body.ConnectedParts.Add (leftJoint);
			body.ConnectedParts.Add (rightJoint);
			body.ConnectedParts.Add (core);
			tail.ConnectedParts.Add (body);
			core.ConnectedParts.Add (body);
			leftJoint.ConnectedParts.Add (body);
			leftJoint.ConnectedParts.Add (leftArm);
			rightJoint.ConnectedParts.Add (body);
			rightJoint.ConnectedParts.Add (rightArm);
			leftArm.ConnectedParts.Add (leftJoint);
			rightArm.ConnectedParts.Add (rightJoint);

			Add (body);
			Add (core);
			Add (tail);
			Add (leftJoint);
			Add (rightJoint);
			Add (leftArm);
			Add (rightArm);

			Parts.Add (body);
			Parts.Add (core);
			Parts.Add (tail);
			Parts.Add (leftJoint);
			Parts.Add (rightJoint);
			Parts.Add (leftArm);
			Parts.Add (rightArm);
		}
	}
}
