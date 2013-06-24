using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ArcticLion
{
	public abstract class Node
	{
		#region Fields
		private static long static_id;
		public long ID { get; private set;}
		List<Node> children;
		#endregion

		#region Properties
		public Node Parent { get; set; }
		public Vector2 Position { get; set; }
		public double Rotation { get; set;}
		#endregion

		public Node ()
		{
			ID = ++static_id;
			children = new List<Node> ();
		}

		public virtual void LoadContent (ContentManager content)
		{
			foreach (Node n in children) {
				n.LoadContent (content);
			}
		}

		public virtual void Update (GameTime gameTime)
		{
			foreach (Node n in children) {
				n.Update (gameTime);
			}
		}

		public virtual void Draw (SpriteBatch spriteBatch)
		{
			foreach (Node n in children) {
				n.Draw (spriteBatch);
			}
		}

		public virtual void Add(Node newNode)
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
			this.Parent = null;
		}

		public override bool Equals (object obj)
		{
			if (obj != null) {
				if (obj is Node) {
					Node other = (Node)obj;
					return other.ID == this.ID;
				}
			}
			return false;
		}

		public override int GetHashCode ()
		{
			return (int)ID;
		}
	}
}