﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using NuclearWinter;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using NuclearWinter.Xna;

#if !MONOGAME
using OSKey = System.Windows.Forms.Keys;
#elif !MONOMAC
using OSKey = OpenTK.Input.Key;
#else
using OSKey = MonoMac.AppKit.NSKey;
#endif

namespace NuclearWinter.UI
{
    /*
     * A Screen handles passing on user input, updating and drawing
     * a bunch of widgets
     */
    public class Screen
    {
        //----------------------------------------------------------------------
        public NuclearGame  Game                { get; private set; }
        public bool         IsActive;

        public Style        Style               { get; private set; }

        public int          Width               { get; private set; }
        public int          Height              { get; private set; }
        public Rectangle    Bounds              { get { return new Rectangle( 0, 0, Width, Height ); } }

        public Group        Root                { get; private set; }

        public Widget       FocusedWidget       { get; private set; }
        bool                mbHasActivatedFocusedWidget;

        Widget              mClickedWidget;
        int                 miClickedWidgetMouseButton;

        public Widget       HoveredWidget       { get; private set; }
        Point               mPreviousMouseHitPoint;

        int                 miIgnoreClickFrames;

        //----------------------------------------------------------------------
        public Screen( NuclearWinter.NuclearGame _game, Style _style, int _iWidth, int _iHeight )
        {
            Game    = _game;
            Style   = _style;
            Width   = _iWidth;
            Height  = _iHeight;

            Root    = new Group( this );
        }

        //----------------------------------------------------------------------
        public void Resize( int _iWidth, int _iHeight )
        {
            Width = _iWidth;
            Height = _iHeight;
            Root.DoLayout( Bounds );

            // This will prevent accidental clicks when maximizing the window
            miIgnoreClickFrames = 3;
        }

        //----------------------------------------------------------------------
        public void HandleInput()
        {
            //------------------------------------------------------------------
            // Make sure we don't hold references to orphaned widgets
            if( FocusedWidget != null && FocusedWidget.IsOrphan )
            {
                FocusedWidget = null;
                mbHasActivatedFocusedWidget = false;
            }

            if( HoveredWidget != null && HoveredWidget.IsOrphan )
            {
#if !MONOGAME
                Game.Form.Cursor = System.Windows.Forms.Cursors.Default;
#endif
                HoveredWidget = null;
            }

            if( mClickedWidget != null && mClickedWidget.IsOrphan ) mClickedWidget = null;

            //------------------------------------------------------------------
            Point mouseHitPoint = new Point(
                (int)( Game.InputMgr.MouseState.X / Resolution.ScaleFactor ),
                (int)( ( Game.InputMgr.MouseState.Y - Game.GraphicsDevice.Viewport.Y ) / Resolution.ScaleFactor )
            );

            if( ! IsActive )
            {
                if( HoveredWidget != null )
                {
                    HoveredWidget.OnMouseOut( mouseHitPoint );
                    HoveredWidget = null;
                }

                if( mClickedWidget != null )
                {
                    mClickedWidget.OnMouseUp( mouseHitPoint, miClickedWidgetMouseButton );
                    mClickedWidget = null;
                }

                return;
            }

#if WINDOWS || LINUX || MACOSX
            //------------------------------------------------------------------
            // Mouse buttons
            bool bHasMouseEvent = false;

            if( miIgnoreClickFrames == 0 )
            {
                for( int iButton = 0; iButton < 3; iButton++ )
                {
                    if( Game.InputMgr.WasMouseButtonJustPressed( iButton ) )
                    {
                        bHasMouseEvent = true;

                        if( mClickedWidget == null )
                        {
                            miClickedWidgetMouseButton = iButton;

                            Widget hitWidget = null;

                            if( FocusedWidget != null )
                            {
                                hitWidget = FocusedWidget.HitTest( mouseHitPoint );
                            }

                            if( hitWidget == null )
                            {
                                hitWidget = Root.HitTest( mouseHitPoint );
                            }

                            while( hitWidget != null && ! hitWidget.OnMouseDown( mouseHitPoint, iButton ) )
                            {
                                hitWidget = hitWidget.Parent;
                            }

                            mClickedWidget = hitWidget;
                        }
                    }
                }

                if( Game.InputMgr.WasMouseJustDoubleClicked() )
                {
                    bHasMouseEvent = true;

                    Widget widget  = FocusedWidget == null ? null : FocusedWidget.HitTest( mouseHitPoint );
                    if( widget != null )
                    {
                        bool bPressed;

                        switch( Game.InputMgr.PrimaryMouseButton )
                        {
                            case 0:
                                bPressed = Game.InputMgr.MouseState.LeftButton == ButtonState.Pressed;
                                break;
                            case 2:
                                bPressed = Game.InputMgr.MouseState.RightButton == ButtonState.Pressed;
                                break;
                            default:
                                throw new NotSupportedException();
                        }

                        if( bPressed )
                        {
                            mClickedWidget = widget;
                            miClickedWidgetMouseButton = Game.InputMgr.PrimaryMouseButton;
                        }

                        if( widget.OnMouseDoubleClick( mouseHitPoint ) )
                        {
                            miIgnoreClickFrames = 3;
                        }
                    }
                }
            }
            else
            {
                miIgnoreClickFrames--;
            }

            for( int iButton = 0; iButton < 3; iButton++ )
            {
                if( Game.InputMgr.WasMouseButtonJustReleased( iButton ) )
                {
                    bHasMouseEvent = true;

                    if( mClickedWidget != null && iButton == miClickedWidgetMouseButton )
                    {
                        mClickedWidget.OnMouseUp( mouseHitPoint, iButton );
                        mClickedWidget = null;
                    }
                }
            }
            
            if( ! bHasMouseEvent )
            {
                Widget hoveredWidget = ( FocusedWidget == null ? null : FocusedWidget.HitTest( mouseHitPoint ) ) ?? Root.HitTest( mouseHitPoint );

                if( mouseHitPoint != mPreviousMouseHitPoint )
                {
                    if( mClickedWidget == null )
                    {
                        if( hoveredWidget != null && hoveredWidget == HoveredWidget )
                        {
                            HoveredWidget.OnMouseMove( mouseHitPoint );
                        }
                        else
                        {
                            if( HoveredWidget != null )
                            {
                                HoveredWidget.OnMouseOut( mouseHitPoint );
                            }
                        
                            HoveredWidget = hoveredWidget;
                            if( HoveredWidget != null )
                            {
                                HoveredWidget.OnMouseEnter( mouseHitPoint );
                            }
                        }
                    }
                    else
                    {
                        mClickedWidget.OnMouseMove( mouseHitPoint );
                    }
                }
            }

            // Mouse wheel
            int iWheelDelta = Game.InputMgr.GetMouseWheelDelta();
            if( iWheelDelta != 0 )
            {
                Widget hoveredWidget = ( FocusedWidget == null ? null : FocusedWidget.HitTest( mouseHitPoint ) ) ?? Root.HitTest( mouseHitPoint );

                if( hoveredWidget != null )
                {
                    hoveredWidget.OnMouseWheel( mouseHitPoint, iWheelDelta );
                }
            }

            mPreviousMouseHitPoint = mouseHitPoint;

            //------------------------------------------------------------------
            // Keyboard
            if( FocusedWidget != null )
            {
                foreach( char character in Game.InputMgr.EnteredText )
                {
                    FocusedWidget.OnTextEntered( character );
                }

#if !MACOSX
                if( Game.InputMgr.JustPressedOSKeys.Contains( OSKey.Enter ) || Game.InputMgr.JustPressedOSKeys.Contains( OSKey.Space ) )
#else
                if( Game.InputMgr.JustPressedOSKeys.Contains( OSKey.Return ) || Game.InputMgr.JustPressedOSKeys.Contains( OSKey.Space ) )
#endif

                {
                    if( FocusedWidget.OnActivateDown() )
                    {
                        mbHasActivatedFocusedWidget = true;
                    }
                }
                else
#if !MACOSX
                if( Game.InputMgr.JustReleasedOSKeys.Contains( OSKey.Enter ) || Game.InputMgr.JustReleasedOSKeys.Contains( OSKey.Space ) )
#else
                if( Game.InputMgr.JustReleasedOSKeys.Contains( OSKey.Return ) || Game.InputMgr.JustReleasedOSKeys.Contains( OSKey.Space ) )
#endif
                {
                    if( mbHasActivatedFocusedWidget )
                    {
                        FocusedWidget.OnActivateUp();
                        mbHasActivatedFocusedWidget = false;
                    }
                }

                if( Game.InputMgr.WasKeyJustPressed( Keys.Left ) )  FocusedWidget.OnPadMove( UI.Direction.Left );
                if( Game.InputMgr.WasKeyJustPressed( Keys.Right ) ) FocusedWidget.OnPadMove( UI.Direction.Right );
                if( Game.InputMgr.WasKeyJustPressed( Keys.Up ) )    FocusedWidget.OnPadMove( UI.Direction.Up );
                if( Game.InputMgr.WasKeyJustPressed( Keys.Down ) )  FocusedWidget.OnPadMove( UI.Direction.Down );

                foreach( Keys key in Game.InputMgr.JustPressedKeys )
                {
                    FocusedWidget.OnKeyPress( key );
                }

                foreach( var key in Game.InputMgr.JustPressedOSKeys )
                {
                    FocusedWidget.OnOSKeyPress( key );
                }
            }
#endif
        }

        //----------------------------------------------------------------------
        public void Update( float _fElapsedTime )
        {
            Root.DoLayout( new Rectangle( 0, 0, Width, Height ) );
            Root.Update( _fElapsedTime );
        }

        //----------------------------------------------------------------------
        public void Focus( Widget _widget )
        {
            Debug.Assert( _widget.Screen == this );

            mbHasActivatedFocusedWidget = false;
            if( FocusedWidget != null && FocusedWidget != _widget )
            {
                FocusedWidget.OnBlur();
            }

            if( FocusedWidget != _widget )
            {
                FocusedWidget = _widget;
                FocusedWidget.OnFocus();
            }
        }

        //----------------------------------------------------------------------
        public void DrawBox( Texture2D _tex, Rectangle _extents, int _cornerSize, Color _color )
        {
            // Corners
            Game.SpriteBatch.Draw( _tex, new Rectangle( _extents.Left,                  _extents.Top,                   _cornerSize, _cornerSize ), new Rectangle( 0,                           0,                          _cornerSize, _cornerSize ), _color );
            Game.SpriteBatch.Draw( _tex, new Rectangle( _extents.Right - _cornerSize,   _extents.Top,                   _cornerSize, _cornerSize ), new Rectangle( _tex.Width - _cornerSize,    0,                          _cornerSize, _cornerSize ), _color );
            Game.SpriteBatch.Draw( _tex, new Rectangle( _extents.Left,                  _extents.Bottom - _cornerSize,  _cornerSize, _cornerSize ), new Rectangle( 0,                           _tex.Height - _cornerSize,  _cornerSize, _cornerSize ), _color );
            Game.SpriteBatch.Draw( _tex, new Rectangle( _extents.Right - _cornerSize,   _extents.Bottom - _cornerSize,  _cornerSize, _cornerSize ), new Rectangle( _tex.Width - _cornerSize,    _tex.Height - _cornerSize,  _cornerSize, _cornerSize ), _color );

            // Content
            Game.SpriteBatch.Draw( _tex, new Rectangle( _extents.Left + _cornerSize,    _extents.Top + _cornerSize,     _extents.Width - _cornerSize * 2, _extents.Height - _cornerSize * 2 ), new Rectangle( _cornerSize, _cornerSize, _tex.Width - _cornerSize * 2, _tex.Height - _cornerSize * 2 ), _color );

            // Border top / bottom
            Game.SpriteBatch.Draw( _tex, new Rectangle( _extents.Left + _cornerSize,    _extents.Top,                   _extents.Width - _cornerSize * 2, _cornerSize ), new Rectangle( _cornerSize, 0, _tex.Width - _cornerSize * 2, _cornerSize ), _color );
            Game.SpriteBatch.Draw( _tex, new Rectangle( _extents.Left + _cornerSize,    _extents.Bottom - _cornerSize,  _extents.Width - _cornerSize * 2, _cornerSize ), new Rectangle( _cornerSize, _tex.Height - _cornerSize, _tex.Width - _cornerSize * 2, _cornerSize ), _color );

            // Border left / right
            Game.SpriteBatch.Draw( _tex, new Rectangle( _extents.Left,                  _extents.Top + _cornerSize,     _cornerSize, _extents.Height - _cornerSize * 2 ), new Rectangle( 0, _cornerSize, _cornerSize, _tex.Height - _cornerSize * 2 ), _color );
            Game.SpriteBatch.Draw( _tex, new Rectangle( _extents.Right - _cornerSize,   _extents.Top + _cornerSize,     _cornerSize, _extents.Height - _cornerSize * 2 ), new Rectangle( _tex.Width - _cornerSize, _cornerSize, _cornerSize, _tex.Height - _cornerSize * 2 ), _color );
        }

        //----------------------------------------------------------------------
        Stack<Rectangle> mlScissorRects = new Stack<Rectangle>();

        Rectangle TransformRect( Rectangle _rect, Matrix _matrix )
        {
            Vector2 vMin = new Vector2( _rect.X, _rect.Y );
            Vector2 vMax = new Vector2( _rect.Right, _rect.Bottom );
            vMin = Vector2.Transform( vMin, _matrix );
            vMax = Vector2.Transform( vMax, _matrix );
            Rectangle bounds = new Rectangle(
                (int)vMin.X,
                (int)vMin.Y,
                (int)( vMax.X - vMin.X  ),
                (int)( vMax.Y - vMin.Y )
                );

            return bounds;
        }

        public void PushScissorRectangle( Rectangle _scissorRect )
        {
            Rectangle rect = TransformRect( _scissorRect, Game.SpriteMatrix );
            rect.Offset( 0, NuclearWinter.Resolution.Viewport.Y );
            Rectangle parentRect = ScissorRectangle;

            Rectangle newRect = parentRect.Clip( rect ).Clip( Game.GraphicsDevice.Viewport.Bounds );

            SuspendBatch();
            mlScissorRects.Push( newRect );
            ResumeBatch();
        }

        public void PopScissorRectangle()
        {
            SuspendBatch();
            mlScissorRects.Pop();
            ResumeBatch();
        }

        public Rectangle ScissorRectangle
        {
            get { return mlScissorRects.Count > 0 ? mlScissorRects.Peek() : Game.GraphicsDevice.Viewport.Bounds; }
        }

        //----------------------------------------------------------------------
        public void Draw()
        {
            Game.SpriteBatch.Begin( SpriteSortMode.Deferred, null, null, null, null, null, Game.SpriteMatrix );

            Root.Draw();

            if( FocusedWidget != null && IsActive )
            {
                FocusedWidget.DrawFocused();
            }

            if( HoveredWidget != null && IsActive )
            {
                HoveredWidget.DrawHovered();
            }

            Game.SpriteBatch.End();

            Debug.Assert( mlScissorRects.Count == 0, "Unbalanced calls to PushScissorRectangles" );
        }

        //----------------------------------------------------------------------
        public void SuspendBatch()
        {
            Game.SpriteBatch.End();
            Game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        }

        //----------------------------------------------------------------------
        public void ResumeBatch()
        {
            Game.GraphicsDevice.RasterizerState = Game.ScissorRasterizerState;

            if( mlScissorRects.Count > 0 )
            {
                var rect = mlScissorRects.Peek();
                if( rect.Width > 0 && rect.Height > 0 )
                {
                    Game.GraphicsDevice.ScissorRectangle = rect;
                }

                Game.SpriteBatch.Begin( SpriteSortMode.Deferred, null, null, null, Game.ScissorRasterizerState, null, Game.SpriteMatrix );
            }
            else
            {
                Game.SpriteBatch.Begin( SpriteSortMode.Deferred, null, null, null, null, null, Game.SpriteMatrix );
            }
        }
    }
}
