using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace NuclearWinter.GameFlow
{
    public abstract class GameStateFadeTransition<T>: GameState<T> where T:NuclearGame
    {
        static float     sfTransitionDuration = 0.3f;
        float            mfTransition;

        //----------------------------------------------------------------------
        public GameStateFadeTransition( T _game )
        :   base ( _game )
        {
        }

        //----------------------------------------------------------------------
        public override bool UpdateFadeIn( GameTime gameTime )
        {
            bool bFadeInDone = ( mfTransition >= sfTransitionDuration );
            mfTransition = Math.Min( mfTransition + (float)gameTime.ElapsedGameTime.TotalSeconds, sfTransitionDuration );

            Update( gameTime );

            return bFadeInDone;
        }

        //----------------------------------------------------------------------
		public override bool UpdateFadeOut( GameTime gameTime )
        {
            bool bFadeOutDone = ( mfTransition <= 0f );
			mfTransition = Math.Max( mfTransition - (float)gameTime.ElapsedGameTime.TotalSeconds, 0f );

			Update( gameTime );

            return bFadeOutDone;
        }

        //----------------------------------------------------------------------
        public override void DrawFadeIn()
        {
            Draw( mfTransition / sfTransitionDuration );
        }

        //----------------------------------------------------------------------
        public override void DrawFadeOut()
        {
            Draw( mfTransition / sfTransitionDuration );
        }

        //----------------------------------------------------------------------
        public void Draw( float _fTransition )
        {
            Draw();

            Color fadeColor = Color.Black * ( 1f - _fTransition );

            Game.SpriteBatch.Begin();
            Game.SpriteBatch.Draw( Game.WhitePixelTex, new Rectangle( 0, 0, NuclearWinter.Resolution.InternalMode.Width + 1, NuclearWinter.Resolution.InternalMode.Height + 1 ), fadeColor );
            Game.SpriteBatch.End();
        }
    }
}
