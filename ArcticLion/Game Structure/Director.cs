using System;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class Director
	{
		public Scene CurrentScene{ get; set; }

		public Director (Game game)
		{
			//TODO: Temp
			CurrentScene = new TestGameScene (game);
		}
	}
}