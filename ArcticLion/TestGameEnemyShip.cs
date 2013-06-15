using System;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class TestGameEnemyShip : EnemyShip
	{
		public TestGameEnemyShip ()
		{
			EnemyShipPart part1 = new EnemyShipPart ("part");
			EnemyShipPart part2 = new EnemyShipPart ("part");
			EnemyShipPart part3 = new EnemyShipPart ("part");

			//TODO: revert to more lower values
			part1.Health = 100;
			part2.Health = 20;
			part3.Health = 100;

			part2.Rotation = Math.PI / 2;
			part1.Position = new Vector2 (-32,0);
			part3.Position = new Vector2 (32,0);

			part1.ConnectedParts.Add (part2);
			part2.ConnectedParts.Add (part1);
			part2.ConnectedParts.Add (part3);
			part3.ConnectedParts.Add (part2);
		
			Add (part1);
			Parts.Add (part1);
			Add (part2);
			Parts.Add (part2);
			Add (part3);
			Parts.Add (part3);
		}

		public override void Update (GameTime gameTime)
		{
			base.Update (gameTime);

			double totalTime = gameTime.TotalGameTime.TotalSeconds;

			Velocity = new Vector2 ((float)Math.Cos(totalTime), 
			                        (float)Math.Sin (totalTime));
			Velocity *= 200f * Math.Abs((float)Math.Sin(totalTime));

			Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
		}
	}
}