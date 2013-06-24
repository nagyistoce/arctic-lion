using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Text;

namespace NuclearWinter.GameFlow
{
    public interface IGameState<out T> where T:NuclearGame
    {
        void Start();
        void Stop();
        void OnActivated();
        void OnExiting();
        bool UpdateFadeIn( GameTime gameTime );
        void DrawFadeIn();
		bool UpdateFadeOut( GameTime gameTime );
        void DrawFadeOut();
		void Update( GameTime gameTime );
        void Draw();
    }
}
