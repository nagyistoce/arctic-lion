using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ArcticLion
{
	public class EnemyShipFactory
	{
		private static EnemyShipFactory instance;
		private ContentManager content;

		private EnemyShipFactory (){
		}

		public void Initialize(ContentManager content){
			this.content = content;
		}

		public static EnemyShipFactory GetInstance(){
			if (instance == null) {
				instance = new EnemyShipFactory ();
			}
			return instance;
		}

		public EnemyShip CreateTestGameEnemyShip1(Node target){
			EnemyShip newEnemyShip = new EnemyShip(target);

			EnemyShipPart part1 = new EnemyShipPart ("part");
			EnemyShipPart part2 = new EnemyShipPart ("part");
			EnemyShipPart part3 = new EnemyShipPart ("part");

			part1.Health = 50;
			part2.Health = 10;
			part3.Health = 50;

			part2.Rotation = Math.PI / 2;
			part1.Position = new Vector2 (-32,0);
			part3.Position = new Vector2 (32,0);

			EnemyShipPart.Connect (part1, part2);
			EnemyShipPart.Connect (part2, part3);

			newEnemyShip.Add (part1);
			newEnemyShip.Add (part2);
			newEnemyShip.Add (part3);

			return newEnemyShip;
		}

		public EnemyShip CreateTestGameEnemyShip2(Node target){
			EnemyShip newEnemyShip = new EnemyShip(target);

			newEnemyShip.MovingBehavior = new PendulumMovingBehavior ();
			newEnemyShip.ShootingBehavior = new ContinuousShootingBehavior ();

			return newEnemyShip;
		}

		public EnemyShip CreateTestGameEnemyShip3(Node target){
			EnemyShip newEnemyShip = new EnemyShip(target);

			EnemyShipPart body = new EnemyShipPart ("test_part_body");
			EnemyShipPart core = new EnemyShipPart ("test_part_core");
			EnemyShipPart leftJoint = new EnemyShipPart ("part");
			EnemyShipPart rightJoint = new EnemyShipPart ("part");
			EnemyShipPart leftArm = new EnemyShipPart ("test_part_arm");
			EnemyShipPart rightArm = new EnemyShipPart ("test_part_arm");
			EnemyShipPart tail = new EnemyShipPart ("test_part_tail");

			body.Health = 20;
			core.Health = 15;
			leftJoint.Health = 3;
			rightJoint.Health = 3;
			leftArm.Health = 10;
			rightArm.Health = 10;
			tail.Health = 5;

			core.Position = new Vector2 (64, 0);
			leftJoint.Position = new Vector2 (0, -64);
			rightJoint.Position = new Vector2 (0, 64);
			leftArm.Position = new Vector2 (32, -104);
			rightArm.Position = new Vector2 (32, 104);
			tail.Position = new Vector2 (-64, 0);

			body.Weight = 100;
			core.Weight = 50;
			leftJoint.Weight = 1;
			rightJoint.Weight = 1;
			leftArm.Weight = 5;
			rightArm.Weight = 5;
			tail.Weight = 3;

			EnemyShipPart.AssignBehavior (new CircularMovingBehavior(), body, core, tail);
			EnemyShipPart.AssignBehavior (new PendulumMovingBehavior(), tail, leftJoint, rightJoint);
			EnemyShipPart.AssignBehavior (new SuicidalMovingBehavior(target), leftArm, rightArm);

			EnemyShipPart.AssignBehavior (new ContinuousShootingBehavior(), body, core, leftArm, rightArm);

			EnemyShipPart.Connect (body, tail);
			EnemyShipPart.Connect (body, leftJoint);
			EnemyShipPart.Connect (body, rightJoint);
			EnemyShipPart.Connect (body, core);
			EnemyShipPart.Connect (leftJoint, leftArm);
			EnemyShipPart.Connect (rightJoint, rightArm);

			newEnemyShip.Add (body);
			newEnemyShip.Add (core);
			newEnemyShip.Add (tail);
			newEnemyShip.Add (leftJoint);
			newEnemyShip.Add (rightJoint);
			newEnemyShip.Add (leftArm);
			newEnemyShip.Add (rightArm);

			newEnemyShip.MovingBehavior = new CircularMovingBehavior ();
			newEnemyShip.ShootingBehavior = new ContinuousShootingBehavior ();

			return newEnemyShip;
		}

		public void LoadContentFor(EnemyShip enemyShip){
			enemyShip.LoadContent (content);
		}
	}
}