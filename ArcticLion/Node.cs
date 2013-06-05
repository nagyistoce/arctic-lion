using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArcticLion
{
	public abstract class Node
	{
		#region Fields
			List<Node> children;
		#endregion

		#region Properties
			public Node Parent;
		#endregion

		public Node ()
		{
			children = new List<Node> ();
		}

		public virtual void Update (GameTime gameTime)
		{
			foreach (Node n in children) {
				n.Update (gameTime);
			}
		}

		public virtual void Draw (GameTime gameTime, SpriteBatch spriteBatch)
		{
			foreach (Node n in children) {
				n.Draw (gameTime, spriteBatch);
			}
		}

		public void Add(Node newNode)
		{
			newNode.Parent = this;
			children.Add (newNode);
		}

		public bool Remove(Node node)
		{
			if (children.Remove (node)) {
				node.Parent = null;
				return true;
			} else {
				return false;
			}
		}

		public void Kill()
		{
			Parent.Remove (this);
		}
	}
}