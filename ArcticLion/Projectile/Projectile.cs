using System;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public abstract class Projectile : Node
	{
		public Vector2 Acceleration {get; set;}
		public Vector2 Velocity { get; set;}
		public float Radius { get; set;}
		public bool IsFlying { get; set;}

		public abstract Projectile Clone();
	}
}