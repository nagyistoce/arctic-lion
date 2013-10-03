using System;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Drawing;
using Microsoft.Xna.Framework;
using ArcticLion;

namespace Tools
{
	public class EnemyPartView : NSView
	{
		EnemyShipPart part;
		NSImage image;

		//Convinience property
		private PointF SuperviewCenter {
			get{
				PointF superViewCenter = new PointF();
				if (Superview != null) {
					superViewCenter.X = Superview.Frame.Size.Width/2f;
					superViewCenter.Y = Superview.Frame.Size.Height/2f;
				}
				return superViewCenter;
			}
		}

		public EnemyPartView (EnemyShipPart part)
		{
			this.part = part;
			image = new NSImage (NSBundle.MainBundle.BundlePath + "/Contents/Resources/" + part.Asset + ".png");
			Frame = new RectangleF (0,0,image.Size.Width, image.Size.Height);
		}

		public override void ViewDidMoveToSuperview ()
		{
			base.ViewDidMoveToSuperview ();

			CenterFrameOn(SuperviewCenter);
		}

		public override void ViewDidEndLiveResize ()
		{
			base.ViewDidEndLiveResize ();

			PointF frameOrigin = SuperviewCenter;
			frameOrigin.X += part.Position.X;
			frameOrigin.Y += part.Position.Y;
			CenterFrameOn (frameOrigin);
		}

		public override void DrawRect (System.Drawing.RectangleF dirtyRect)
		{
			base.DrawRect(dirtyRect);

			RectangleF src = new RectangleF (0, 0, image.Size.Width, image.Size.Height);
			RectangleF dest = Bounds;

			image.DrawInRect (dest, src, NSCompositingOperation.SourceOver, 1f);
		}

		public override void MouseDragged (NSEvent e)
		{
			base.MouseDragged (e);

			part.Position = part.Position + new Vector2 (e.DeltaX, -e.DeltaY);

			PointF frameOrigin = SuperviewCenter;
			frameOrigin.X += part.Position.X;
			frameOrigin.Y += part.Position.Y;
			CenterFrameOn (frameOrigin);
		}

		private void CenterFrameOn(PointF point){
			point.X -= Bounds.Size.Width / 2f;
			point.Y -= Bounds.Size.Height / 2f;
			SetFrameOrigin (point);
			this.NeedsDisplay = true;
		}
	}
}

