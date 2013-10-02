using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using ArcticLion;

namespace Tools
{
	public partial class EnemyWindowController : MonoMac.AppKit.NSWindowController
	{
		EnemyView enemyView;

		#region Constructors
		// Called when created from unmanaged code
		public EnemyWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public EnemyWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		// Call to load from the XIB/NIB file
		public EnemyWindowController () : base ("EnemyWindow")
		{
			Initialize ();
		}
		// Shared initialization code
		void Initialize ()
		{
		}
		#endregion
		//strongly typed window accessor
		public new EnemyWindow Window {
			get {
				return (EnemyWindow)base.Window;
			}
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			if (enemyView == null) {
				enemyView = new EnemyView ();
			}
			mainView.AddSubview (enemyView);
			enemyView.NeedsDisplay = true;
		}

		public void SetEnemy(EnemyShip enemy){
			enemyView = new EnemyView (enemy);
			if (mainView != null) {
				mainView.AddSubview (enemyView);
			}
		}
	}
}