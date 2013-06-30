using System;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class Projectile : Node
	{
		public Vector2 Acceleration {get; set;}
		public Vector2 Velocity { get; set;}
		public float Radius { get; set;}
		public bool IsFlying { get; set;}
	}
}