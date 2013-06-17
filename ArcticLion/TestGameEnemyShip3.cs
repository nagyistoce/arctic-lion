using System;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class TestGameEnemyShip3 : EnemyShip
	{
		public TestGameEnemyShip3 (Node target) : base(target)
		{
			Build ();
			MovingBehavior = new CircularMovingBehavior ();
			ShootingBehavior = new ContinuousShootingBehavior ();
		}

		public override void Update (GameTime gameTime)
		{
			base.Update (gameTime);
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

			EnemyShipPart.Connect (body, tail);
			EnemyShipPart.Connect (body, leftJoint);
			EnemyShipPart.Connect (body, rightJoint);
			EnemyShipPart.Connect (body, core);
			EnemyShipPart.Connect (leftJoint, leftArm);
			EnemyShipPart.Connect (rightJoint, rightArm);

			Add (body);
			Add (core);
			Add (tail);
			Add (leftJoint);
			Add (rightJoint);
			Add (leftArm);
			Add (rightArm);
		}
	}
}

