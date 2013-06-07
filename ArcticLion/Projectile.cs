using System;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class Projectile : Node
	{
		public Vector2 Acceleration {get; set;}
		public Vector2 Velocity { get; set;}
		public bool IsAlive { get; set; }

		public Projectile ()
		{
			IsAlive = false;
		}
	}
}