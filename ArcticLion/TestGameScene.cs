using System;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class TestGameScene : Scene
	{
		Ship ship;

		public TestGameScene ()
		{
			ship = new Ship ();
			ship.Position = new Vector2 (100, 100);

			Layer mainLayer; 
			Layers.TryGetValue (1, out mainLayer);

			mainLayer.Add (ship);
		}
	}
}

