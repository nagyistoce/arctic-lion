#region File Description
//-----------------------------------------------------------------------------
// ArcticLionGame.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion
#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using NuclearWinter;
using NuclearWinter.GameFlow;

#endregion
namespace ArcticLion
{
	/// <summary>
	/// Default Project Template
	/// </summary>
	public class Game1 : NuclearGame
	{

	#region Fields
//		public GameStateMgr<Game1> GameStateManager { get; private set; }
	#endregion

	#region Game States
		public GameStateStartMenu StartMenu { get; private set; }
		public GameStateInGame InGame { get; private set; }
	#endregion

	#region Initialization

		public Game1 () : base(true)
		{
			Content.RootDirectory = "Content";

			Graphics.IsFullScreen = false;

			Graphics.PreferredBackBufferHeight = 640;
			Graphics.PreferredBackBufferWidth = 960;

			//TODO: draw a cursor
			this.IsMouseVisible = true;

			EnemyShipFactory.GetInstance ().Initialize (Content);
		}

		protected override void Initialize ()
		{
			base.Initialize ();

			StartMenu = new GameStateStartMenu (this);
			InGame = new GameStateInGame (this);

			GameStateMgr.SwitchState (StartMenu);
		}

		protected override void LoadContent ()
		{
			//TODO: LOL
		}
	#endregion

	#region Update and Draw
		protected override void Update (GameTime gameTime)
		{
			base.Update (gameTime);
		}
	
		protected override void Draw (GameTime gameTime)
		{
			Graphics.GraphicsDevice.Clear (Color.Gainsboro);

			base.Draw (gameTime);
		}
	#endregion
	}
}
