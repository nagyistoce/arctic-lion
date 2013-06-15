using System;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class TestGameEnemyShip3 : EnemyShip
	{
		public TestGameEnemyShip3 ()
		{
			EnemyShipPart body = new EnemyShipPart ("test_part_body");
			EnemyShipPart core = new EnemyShipPart ("test_part_core");
			EnemyShipPart leftJoint = new EnemyShipPart ("part");
			EnemyShipPart rightJoint = new EnemyShipPart ("part");
			EnemyShipPart leftArm = new EnemyShipPart ("test_part_arm");
			EnemyShipPart rightArm = new EnemyShipPart ("test_part_arm");
			EnemyShipPart tail = new EnemyShipPart ("test_part_tail");

			body.Health = 100;
			core.Health = 100;
			leftJoint.Health = 100;
			rightJoint.Health = 100;
			leftArm.Health = 100;
			rightArm.Health = 100;
			tail.Health = 100;

			core.Position = new Vector2(64,0);
			leftJoint.Position = new Vector2 (0,-64);
			rightJoint.Position = new Vector2 (0,64);
			leftArm.Position = new Vector2 (32, -104);
			rightArm.Position = new Vector2 (32, 104);
			tail.Position = new Vector2 (-64,0);

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

