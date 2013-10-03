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
		Dictionary<String, EnemyPartView> partViews;

		public EnemyView() : this(new EnemyShip()){
		}

		public EnemyView (EnemyShip enemy) :base(new RectangleF(0,0,500,500))
		{
			this.enemy = enemy;
			partViews = new Dictionary<string, EnemyPartView> ();
			this.Hidden = false;
		}

		public override void ViewDidMoveToSuperview ()
		{
			Frame = Superview.Bounds;

			base.ViewDidMoveToSuperview ();
		}

		public override void ViewDidEndLiveResize ()
		{
			Frame = Superview.Bounds;
		
			foreach (EnemyPartView partView in partViews.Values) {
				partView.ViewDidEndLiveResize ();
			}
		
			base.ViewDidEndLiveResize ();
		}

		public override void ViewWillDraw ()
		{
			foreach (EnemyShipPart p in enemy.Parts) {
				if (!partViews.ContainsKey(p.Asset)) {
					EnemyPartView partView = new EnemyPartView (p);
					partViews.Add (p.Asset, partView);
					this.AddSubview (partView);
				}
			}

			base.ViewWillDraw ();
		}
	}
}