using System;
using System.Xml;
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

		public EnemyShip CreateEnemyShipAplha1(Node target){
			EnemyShip newEnemyShip = new EnemyShip (target);

			EnemyShipPart arm = new EnemyShipPart (Assets.PartArm);
			arm.Health = 10;
			arm.Position = Vector2.Zero;

			newEnemyShip.Add (arm);

			newEnemyShip.MovingBehavior = new SuicidalMovingBehavior ();
			newEnemyShip.ShootingBehavior = new ContinuousShootingBehavior ();

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

		public EnemyShip CreateFromXML(Node target, String xml){
			XmlTextReader reader = new XmlTextReader (xml);

			EnemyShip newEnemyShip = new EnemyShip (target);

			while (reader.Read()) {
				if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals ("EnemyShip")) {

					//Read parts
					while (reader.Read()) {
						if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals ("EnemyShipPart")) {
							string name = reader.GetAttribute ("name");
							string asset = reader.GetAttribute ("asset");
							int weight = int.Parse (reader.GetAttribute ("weight"));
							int health = int.Parse (reader.GetAttribute ("health"));
							float px = float.Parse (reader.GetAttribute ("px"));
							float py = float.Parse (reader.GetAttribute ("py"));

							EnemyShipPart newPart = new EnemyShipPart (asset);
							newPart.Name = name;
							newPart.Weight = weight;
							newPart.Health = health;
							newPart.Position = new Vector2 (px, py);

							//Read behaviors
							//TODO: ALLOW OPTIONAL BEHAVIORS!!!!!!!!!!!!
							while (reader.Read()) {
								if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals ("Behavior")) {
									reader.ReadToFollowing ("Moving");
									string movingBehaviorName = reader.ReadString ();
									reader.ReadToFollowing ("Shooting");
									string shootingBehaviorName = reader.ReadString ();

									var movingBehavior = Activator.CreateInstance (Type.GetType(movingBehaviorName));
									var shootingBehavior = Activator.CreateInstance (Type.GetType(shootingBehaviorName));

									newPart.PreferredMovingBehavior = (MovingBehavior)movingBehavior;
									newPart.PreferredShootingBehavior = (ShootingBehavior)shootingBehavior;
								} else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals ("Behavior")) {
									break;
								}
							}//End read behaviors

							newEnemyShip.Add (newPart);
						}//End read part
					}//End read parts

					//Read links
				} else if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals ("Links")) {
					while (reader.Read()) {
						if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals ("Link")) {
							string part1Name = reader.GetAttribute ("part1");
							string part2Name = reader.GetAttribute ("part2");

							EnemyShipPart part1 = newEnemyShip.Parts.Find ((p) => {return p.Name.Equals(part1Name);});
							EnemyShipPart part2 = newEnemyShip.Parts.Find ((p) => {return p.Name.Equals(part2Name);});

							EnemyShipPart.Connect (part1, part2);

						} else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals ("Links")) {
							break;
						}
					}
				}else if(reader.NodeType == XmlNodeType.Element && reader.Name.Equals ("Behavior")){
					reader.ReadToFollowing ("Moving");
					string movingBehaviorName = reader.ReadString ();
					reader.ReadToFollowing ("Shooting");
					string shootingBehaviorName = reader.ReadString ();

					var movingBehavior = Activator.CreateInstance (Type.GetType(movingBehaviorName));
					var shootingBehavior = Activator.CreateInstance (Type.GetType(shootingBehaviorName));

					newEnemyShip.MovingBehavior = (MovingBehavior)movingBehavior;
					newEnemyShip.ShootingBehavior = (ShootingBehavior)shootingBehavior;
				}//End enemy ship read
			}//End big read loop
			
			return newEnemyShip;
		}

		public void LoadContentFor(EnemyShip enemyShip){
			enemyShip.LoadContent (content);
		}
	}
}