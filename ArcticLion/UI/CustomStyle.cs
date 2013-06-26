using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NuclearWinter.UI;

namespace ArcticLion
{
	public class CustomStyle : Style
	{
		public CustomStyle(ContentManager content){
			this.ButtonCornerSize = 1;;
			this.ButtonFrame = content.Load<Texture2D>("btn");
			this.ButtonDownFrame = content.Load<Texture2D> ("btn_down");
			this.ButtonVerticalPadding = 0;
			this.ButtonHorizontalPadding = 0;
		}
	}
}