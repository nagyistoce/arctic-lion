using System;

namespace ArcticLion
{
	public class Director
	{
		public Scene CurrentScene{ get; set; }

		public Director ()
		{
			//TODO: Temp
			CurrentScene = new TestGameScene ();
		}
	}
}