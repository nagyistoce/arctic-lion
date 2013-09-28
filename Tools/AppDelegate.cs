using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace Tools
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		NSDocumentController sharedDocumentController;

		public AppDelegate ()
		{
		}

		public override void FinishedLaunching (NSObject notification)
		{
			sharedDocumentController = (NSDocumentController)NSDocumentController.SharedDocumentController;
			newDocument (this);
		}

		partial void newDocument (NSObject sender)
		{
			EnemyDocument enemyDocument = new EnemyDocument();
			EnemyWindowController enemyWindowController = new EnemyWindowController();
			enemyDocument.AddWindowController(enemyWindowController);
			sharedDocumentController.AddDocument (enemyDocument);
			enemyWindowController.Window.MakeKeyAndOrderFront(enemyWindowController);
		}

		partial void openDocument (NSObject sender)
		{
			sharedDocumentController.OpenDocument(sender);
		}

		partial void saveDocument (NSObject sender)
		{
			if(sharedDocumentController.CurrentDocument.FileUrl != null){
				sharedDocumentController.CurrentDocument.SaveDocument(sender);
			}else{
				saveDocumentAs(sender);
			}
		}

		partial void saveDocumentAs (NSObject sender)
		{
			sharedDocumentController.CurrentDocument.SaveDocumentAs(sender);
		}
	}
}