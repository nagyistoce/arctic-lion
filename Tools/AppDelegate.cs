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

			//TODO: FOR TEST PURPOSE ONLY
			EnemyShip enemy = new EnemyShip();
			EnemyShipPart part = new EnemyShipPart("part_core");
			enemy.Add(part);
			enemyWindowController.SetEnemy(enemy);

			sharedDocumentController.AddDocument (enemyDocument);

			enemyWindowController.ShowWindow(sender);
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