using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using ArcticLion;

namespace Tools
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		NSDocumentController documentController;
	
		public AppDelegate ()
		{
		}
	
		public override bool ApplicationShouldOpenUntitledFile (NSApplication sender)
		{
			return false;
		}

		public override void FinishedLaunching (NSObject notification)
		{
			documentController = (NSDocumentController) NSDocumentController.SharedDocumentController;
		}

		partial void newDocument (NSObject sender)
		{
			EnemyDocument enemyDocument = new EnemyDocument();

			EnemyWindowController enemyWindowController = new EnemyWindowController();
			enemyDocument.AddWindowController(enemyWindowController);

			//TODO: FOR TEST PURPOSE ONLY
			EnemyShip enemy = new EnemyShip();
			EnemyShipPart part = new EnemyShipPart("part_core");
			enemy.Add(part);
			enemyWindowController.SetEnemy(enemy);

			documentController.AddDocument (enemyDocument);

			enemyWindowController.ShowWindow(sender);
			enemyWindowController.Window.MakeKeyAndOrderFront(enemyWindowController);
		}

		partial void openDocument (NSObject sender)
		{
			documentController.OpenDocument(sender);
		}

		partial void saveDocument (NSObject sender)
		{
			if(documentController.CurrentDocument.FileUrl != null){
				documentController.CurrentDocument.SaveDocument(sender);
			}else{
				saveDocumentAs(sender);
			}
		}

		partial void saveDocumentAs (NSObject sender)
		{
			documentController.CurrentDocument.SaveDocumentAs(sender);
		}
	}
}