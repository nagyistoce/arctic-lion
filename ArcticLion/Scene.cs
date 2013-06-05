using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ArcticLion
{
	public class Scene : Node
	{
		public SortedList<int, Layer> Layers{ get; set; }

		public Scene ()
		{
			Layers = new SortedList<int, Layer> (2);

			//TODO: Temp
			Layers.Add (0, new Layer (0)); //Background
			Layers.Add (1, new Layer (1)); //Main
		}

		public override void LoadContent (ContentManager content)
		{
			base.LoadContent (content);

			foreach (KeyValuePair<int, Layer> kvp in Layers) {
				kvp.Value.LoadContent (content);
			}
		}

		public override void Update(GameTime gameTime)
		{
			foreach (KeyValuePair<int, Layer> kvp in Layers) {
				kvp.Value.Update (gameTime);
			}
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			foreach (KeyValuePair<int, Layer> kvp in Layers) {
				kvp.Value.Draw (gameTime, spriteBatch);
			}
		}
	}
}

