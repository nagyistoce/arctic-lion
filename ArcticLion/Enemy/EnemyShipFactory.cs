using System;
using System.Xml;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;

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

		public EnemyShip CreateEnemyShipAplha1(Node target){
			EnemyShip newEnemyShip = new EnemyShip (target);

			EnemyShipPart arm = new EnemyShipPart (Assets.PartArm);
			arm.Health = 10;
			arm.Position = Vector2.Zero;
			arm.Weapon = new Weapon ();
			arm.Weapon.Position = new Vector2 (29, 0);

			newEnemyShip.Add (arm);

			newEnemyShip.MovingBehavior = new SuicidalMovingBehavior ();
			newEnemyShip.ShootingBehavior = new PulseShootingBehavior ();

			return newEnemyShip;
		}

		public EnemyShip CreateEnemyShipAlpha2(Node target){
			EnemyShip newEnemyShip = new EnemyShip (target);

			EnemyShipPart body = new EnemyShipPart (Assets.PartBody);
			EnemyShipPart arm = new EnemyShipPart (Assets.PartArm);

			body.Health = 20;
			arm.Health = 10;

			body.Position = Vector2.Zero;
			arm.Position = new Vector2 (64, 0);

			arm.Weapon = new Weapon ();
			arm.Weapon.Position = new Vector2 (29, 0);

			body.PreferredMovingBehavior = new PendulumMovingBehavior ();
			arm.PreferredMovingBehavior = new SuicidalMovingBehavior ();
			EnemyShipPart.AssignBehavior (new ContinuousShootingBehavior (), body, arm);

			newEnemyShip.Add (body);
			newEnemyShip.Add (arm);

			newEnemyShip.MovingBehavior = new CircularMovingBehavior ();
			newEnemyShip.ShootingBehavior = new ContinuousShootingBehavior ();

			return newEnemyShip;
		}

		public EnemyShip CreateTestGameEnemyShip3(Node target){
			EnemyShip newEnemyShip = new EnemyShip(target);

			EnemyShipPart body = new EnemyShipPart (Assets.PartBody);
			EnemyShipPart core = new EnemyShipPart (Assets.PartCore);
			EnemyShipPart leftJoint = new EnemyShipPart (Assets.PartLeftJoint);
			EnemyShipPart rightJoint = new EnemyShipPart (Assets.PartRightJoint);
			EnemyShipPart leftArm = new EnemyShipPart (Assets.PartArm);
			EnemyShipPart rightArm = new EnemyShipPart (Assets.PartArm);
			EnemyShipPart tail = new EnemyShipPart (Assets.PartTail);

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

			leftArm.Weapon = new Weapon ();
			leftArm.Weapon.Position = new Vector2 (29,0);
			rightArm.Weapon = new Weapon ();
			rightArm.Weapon.Position = new Vector2 (29,0);

			EnemyShipPart.AssignBehavior (new CircularMovingBehavior(), body, core, tail);
			EnemyShipPart.AssignBehavior (new PendulumMovingBehavior(), tail, leftJoint, rightJoint);
			EnemyShipPart.AssignBehavior (new SuicidalMovingBehavior(), leftArm, rightArm);

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

		public EnemyShip TestDeserialize(Node target){

			SerializableEnemyShip ses = new SerializableEnemyShip();
			ses.Id = "test";
			ses.Parts = new List<SerializableEnemyShipPart> ();
			ses.Connections = new List<SerializableEnemyShipPartConnection> ();
			ses.MovingBehavior = "CircularMovingBehavior";
			ses.ShootingBehavior = "ContinuousShootingBehavior";

			SerializableEnemyShipPart sesp = new SerializableEnemyShipPart ();
			sesp.Id = "core";
			sesp.Asset = Assets.PartCore;
			sesp.Health = 20;
			sesp.Weight = 1;
			sesp.Rotation = Math.PI/4f;
			sesp.PositionX = 0;
			sesp.PositionY = 0;
			sesp.PreferredMovingBehavior = "CircularMovingBehavior";
			sesp.PreferredShootingBehavior = "ContinuousShootingBehavior";

			ses.Parts.Add (sesp);

			string json = JsonConvert.SerializeObject (ses);
	
			var deserialized = JsonConvert.DeserializeObject<SerializableEnemyShip> (json);

			if(deserialized is SerializableEnemyShip){
				SerializableEnemyShip serializableEnemyShip = (SerializableEnemyShip)deserialized;
				EnemyShip enemyShip =  serializableEnemyShip.ToEnemyShip ();
				enemyShip.Target = target;
				return enemyShip;
			}

			return null;
		}

		public void LoadContentFor(EnemyShip enemyShip){
			enemyShip.LoadContent (content);
		}
	}
}