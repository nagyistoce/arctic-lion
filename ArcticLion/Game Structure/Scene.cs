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

		public override void Draw(SpriteBatch spriteBatch)
		{
			foreach (KeyValuePair<int, Layer> kvp in Layers) {
				kvp.Value.Draw (spriteBatch);
			}
		}
	}
}

