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

			part2.Rotation = Math.PI / 2;
			part1.Position = new Vector2 (-32,0);
			part3.Position = new Vector2 (32,0);

			//TODO: connect that shit
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
	}
}