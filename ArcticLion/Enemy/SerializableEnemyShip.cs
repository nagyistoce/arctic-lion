using System;
using System.Collections.Generic;
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