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
		protected List<Node> children;
		List<Node> deadChildren;
		#endregion

		#region Properties
		public Node Parent { get; set; }
		public Vector2 Position { get; set; }
		public double Rotation { get; set;}
		public bool IsDead { get; set;}
		#endregion

		public Node ()
		{
			ID = ++static_id;
			children = new List<Node> ();
			deadChildren = new List<Node> ();
			IsDead = false;
			Position = Vector2.Zero;
			Rotation = 0;
		}

		public virtual void LoadContent (ContentManager content)
		{
			foreach (Node n in children) {
				n.LoadContent (content);
			}
		}

		public virtual void Update (GameTime gameTime)
		{
			if (IsDead)
				return;

			foreach (Node n in deadChildren) {
				children.Remove (n);
			}
			deadChildren.Clear ();

			foreach (Node n in children) {
				n.Update (gameTime);
			}
		}

		public virtual void Draw (SpriteBatch spriteBatch)
		{
			if (IsDead)
				return;

			foreach (Node n in children) {
				n.Draw (spriteBatch);
			}
		}

		public virtual void Add(Node newNode)
		{
			newNode.Parent = this;
			children.Add (newNode);
		}

		public virtual bool Remove(Node node)
		{
			return children.Remove (node);
		}

		public void Kill()
		{
			IsDead = true;
			Parent.deadChildren.Add (this);
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