using System;
using ArcticLion;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.Collections.Generic;

namespace Tools
{
	public class EnemyView : NSView
	{
		EnemyShip enemy;
		Dictionary<String, NSImage> images;

		public EnemyView() : this(new EnemyShip()){
		}

		public EnemyView (EnemyShip enemy) :base(new RectangleF(0,0,100,100))
		{
			this.enemy = enemy;
			images = new Dictionary<string, NSImage> ();
			this.Hidden = false;
			this.NeedsDisplay = true;
		}

		public override void ViewWillDraw ()
		{
			foreach (EnemyShipPart p in enemy.Parts) {
				if (!images.ContainsKey(p.Asset)) {
					NSImage i = new NSImage (NSBundle.MainBundle.BundlePath + "/Contents/Resources/" + p.Asset + ".png");
					images.Add (p.Asset, i);
				}
			}

			base.ViewWillDraw ();
		}

		public override void DrawRect (System.Drawing.RectangleF dirtyRect)
		{
			base.DrawRect (dirtyRect);

			foreach (EnemyShipPart p in enemy.Parts) {

				NSImage i = images [p.Asset];

				RectangleF src = new RectangleF (0, 0, i.Size.Width, i.Size.Height);
				RectangleF dest = new RectangleF (p.Position.X, p.Position.Y, i.Size.Width, i.Size.Height);

				i.DrawInRect (dest, src, NSCompositingOperation.SourceOver, 1f);
			}
		}
	}
}