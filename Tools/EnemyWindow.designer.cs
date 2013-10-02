// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace Tools
{
	[Register ("EnemyWindowController")]
	partial class EnemyWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSView mainView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (mainView != null) {
				mainView.Dispose ();
				mainView = null;
			}
		}
	}

	[Register ("EnemyWindow")]
	partial class EnemyWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
