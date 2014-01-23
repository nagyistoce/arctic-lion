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
			//TODO; FOR TESTING ONLY!!!!!!!!!!!!!!!!
			EnemyShip = EnemyShipFactory.GetInstance ().CreateTestGameEnemyShip3 (null);
		}

		public override bool ReadFromData (NSData data, string typeName, out NSError outError)
		{
			bool readSuccess = false;
			NSDictionary options = new NSDictionary ();
			NSAttributedString fileContents = new NSAttributedString(data, null, out options ,out outError);

			if (fileContents == null && outError !=null) {
				outError = new NSError();
			}

			if (fileContents != null) {
				readSuccess = true;
				SerializableEnemyShip serializableEnemyShip = JsonConvert.DeserializeObject<SerializableEnemyShip> (fileContents.ToString());
				this.EnemyShip = serializableEnemyShip.ToEnemyShip ();
			}
			return readSuccess;
		}

		public override NSData GetAsData (string typeName, out NSError outError)
		{
			outError = null;
			string json = JsonConvert.SerializeObject (new SerializableEnemyShip (EnemyShip), Formatting.Indented);
			return NSData.FromString (json);
		}
	}
}
