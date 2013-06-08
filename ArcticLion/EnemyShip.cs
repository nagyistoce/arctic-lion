using System;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class EnemyShip : Node
	{
		EnemyShipPart part1;
		EnemyShipPart part2;
		EnemyShipPart part3;

		public Vector2 Velocity { get; set;}

		public EnemyShip ()
		{
			part1 = new EnemyShipPart ("part");
			part1.Rotation = Math.PI / 4;

			part2 = new EnemyShipPart ("part");
			part2.Rotation = 3*Math.PI / 4;

			part3 = new EnemyShipPart ("part");
			part3.Position = new Vector2 (0, -20);

			Add (part1);
			Add (part2);
			Add (part3);
		}

		public override void Update (GameTime gameTime)
		{
			base.Update (gameTime);

			double totalTime = gameTime.TotalGameTime.TotalSeconds;

			//TODO: remove this shit
			Velocity = new Vector2 ((float)Math.Cos(totalTime), 
			                        (float)Math.Sin (totalTime));
			Velocity *= Math.Abs(5f * (float)Math.Sin(totalTime));

			Position += Velocity;
		}
	}
}

