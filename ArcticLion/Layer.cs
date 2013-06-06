using System;
using Microsoft.Xna.Framework;

namespace ArcticLion
{
	public class Layer : Node
	{
		public int Z;

		public Layer (int z)
		{
			Z = z;
		}
	}
}