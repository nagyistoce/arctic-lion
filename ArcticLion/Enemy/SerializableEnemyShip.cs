using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace ArcticLion
{
	public class SerializableEnemyShip
	{
		[JsonProperty(PropertyName = "id")]
		public string Id { get; set;}

		[JsonProperty(PropertyName = "parts")]
		public List<SerializableEnemyShipPart> Parts { get; set; }

		[JsonProperty(PropertyName = "connections")]
		public List<SerializableEnemyShipPartConnection> Connections { get; set; }

		[JsonProperty(PropertyName = "moving_behavior")]
		public string MovingBehavior { get; set;}

		[JsonProperty(PropertyName = "shooting_behavior")]
		public string ShootingBehavior { get; set;}

		public SerializableEnemyShip(){
		}

		public SerializableEnemyShip(EnemyShip enemyShip){
			this.Id = enemyShip.ID.ToString();
			this.Parts = new List<SerializableEnemyShipPart> ();
			this.Connections = new List<SerializableEnemyShipPartConnection> ();
			this.MovingBehavior = enemyShip.MovingBehavior.GetType().Name;
			this.ShootingBehavior = enemyShip.MovingBehavior.GetType().Name;

			foreach (EnemyShipPart part in enemyShip.Parts) {
				SerializableEnemyShipPart sesp = new SerializableEnemyShipPart ();
				sesp.Id = part.ID.ToString();
				sesp.Asset = part.Asset;
				sesp.Health = part.Health;
				sesp.Weight = part.Weight;
				//TODO sesp.Weapon =
				sesp.Rotation = part.Rotation;
				sesp.PositionX = (int)part.Position.X;
				sesp.PositionY = (int)part.Position.Y;
				sesp.PreferredMovingBehavior = enemyShip.MovingBehavior.GetType().Name;
				sesp.PreferredShootingBehavior = enemyShip.MovingBehavior.GetType().Name;

				this.Parts.Add (sesp);
			}

			//TODO: find connections
		}

		public EnemyShip ToEnemyShip(){
			EnemyShip enemyShip = new EnemyShip ();
			Dictionary<string, EnemyShipPart> parts = new Dictionary<string, EnemyShipPart>();

			enemyShip.MovingBehavior = CreateMovingBehavior(this.MovingBehavior);
			enemyShip.ShootingBehavior = CreateShootingBehavior(this.ShootingBehavior);

			foreach (SerializableEnemyShipPart p in this.Parts) 
			{
				EnemyShipPart enemyShipPart = new EnemyShipPart (p.Asset);
				enemyShipPart.Health = p.Health;
				enemyShipPart.Weight = p.Weight;
				enemyShipPart.Weapon = new Weapon (); //TODO: deserialize weapon
				enemyShipPart.Position = new Vector2 (p.PositionX, p.PositionY);
				enemyShipPart.Rotation = p.Rotation;
				enemyShipPart.PreferredMovingBehavior = CreateMovingBehavior(p.PreferredMovingBehavior);
				enemyShipPart.PreferredShootingBehavior = CreateShootingBehavior(p.PreferredShootingBehavior);

				parts.Add (p.Id, enemyShipPart);
				enemyShip.Add (enemyShipPart);
			}

			foreach (SerializableEnemyShipPartConnection con in this.Connections) 
			{
				EnemyShipPart.Connect (parts [con.LeftId], parts [con.RightId]);
			}
			return enemyShip;
		}

		//TODO: Find a way to instantiate behaviors with the class name or something (BehaviorFactory?, Dictionary?)
		private MovingBehavior CreateMovingBehavior(string name){
			switch (name) {
				case "CircularMovingBehavior":
				return new CircularMovingBehavior ();
			}
			return null;
		}

		private ShootingBehavior CreateShootingBehavior(string name){
			switch (name) {
				case "ContinuousShootingBehavior":
				return new ContinuousShootingBehavior ();
			}
			return null;
		}
	}


	public class SerializableEnemyShipPart
	{
		[JsonProperty(PropertyName = "id")]
		public string Id {get;set;}

		[JsonProperty(PropertyName = "asset")]
		public string Asset { get; set;}

		[JsonProperty(PropertyName = "pos_x")]
		public int PositionX {get;set;}

		[JsonProperty(PropertyName = "pos_y")]
		public int PositionY {get;set;}

		[JsonProperty(PropertyName = "rotation")]
		public double Rotation {get;set;}

		[JsonProperty(PropertyName = "health")]
		public int Health { get; set;}

		[JsonProperty(PropertyName = "weight")]
		public int Weight { get; set;}

		[JsonProperty(PropertyName = "behavior_behavior")]
		public string PreferredMovingBehavior { get; set;}

		[JsonProperty(PropertyName = "shooting_behavior")]
		public string PreferredShootingBehavior { get; set;}
	}
	
	public class SerializableEnemyShipPartConnection
	{
		[JsonProperty(PropertyName = "left_id")]
		public string LeftId {get;set;}

		[JsonProperty(PropertyName = "right_id")]
		public string RightId { get; set; }
	}
}