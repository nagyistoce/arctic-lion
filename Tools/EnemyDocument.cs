using System;
using System.Collections.Generic;
using Microsoft.Xna;
using ArcticLion;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Newtonsoft.Json;
using System.IO;

namespace Tools
{
	public class EnemyDocument : NSDocument
	{
		public EnemyShip EnemyShip {get;set;}

		public EnemyDocument ()
		{
			EnemyShip = new EnemyShip ();
			this.FileType = "enemy";
		}

		public override bool WriteToUrl (MonoMac.Foundation.NSUrl url, string typeName, out NSError outError)
		{
			outError = null;

			NSError error = null;
			base.WriteToUrl (url, typeName, out error);

			SerializableEnemyShip ses = new SerializableEnemyShip();
			ses.Id = FileUrl.RelativePath;
			ses.Parts = new List<SerializableEnemyShipPart> ();
			ses.Connections = new List<SerializableEnemyShipPartConnection> ();
			ses.MovingBehavior = EnemyShip.MovingBehavior.GetType().Name;
			ses.ShootingBehavior = EnemyShip.ShootingBehavior.GetType().Name;

			foreach (EnemyShipPart p in EnemyShip.Parts) {
				SerializableEnemyShipPart sesp = new SerializableEnemyShipPart ();
				sesp.Id = p.ID.ToString();
				sesp.Asset = Assets.PartCore;
				sesp.Health = p.Health;
				sesp.Weight = p.Weight;
				sesp.Rotation = p.Rotation;
				sesp.PositionX = (int)p.Position.X;
				sesp.PositionY = (int)p.Position.Y;
				sesp.PreferredMovingBehavior = p.PreferredMovingBehavior.GetType().Name;
				sesp.PreferredShootingBehavior = p.PreferredShootingBehavior.GetType().Name;

				ses.Parts.Add (sesp);
			}

			string json = JsonConvert.SerializeObject (ses, Formatting.Indented);

			File.WriteAllText (url.Path + typeName, json);

			if (!File.Exists (url.Path + typeName)) {
				outError = new NSError ();
				return false;
			} else {
				return true;
			}
		}
	}
}

