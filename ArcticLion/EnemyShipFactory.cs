using System;
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

