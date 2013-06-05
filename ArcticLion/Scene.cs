using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArcticLion
{
	public class Scene
	{
		public SortedList<int, Layer> Layers{ get; set; }

		public Scene ()
		{
			Layers = new SortedList<int, Layer> (2);

			//TODO: Temp
			Layers.Add (0, new Layer (0)); //Background
			Layers.Add (1, new Layer (1)); //Main
		}

		public void Update(GameTime gameTime)
		{
			foreach (KeyValuePair<int, Layer> kvp in Layers) {
				kvp.Value.Update (gameTime);
			}
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			foreach (KeyValuePair<int, Layer> kvp in Layers) {
				kvp.Value.Draw (gameTime, spriteBatch);
			}
		}
	}
}

