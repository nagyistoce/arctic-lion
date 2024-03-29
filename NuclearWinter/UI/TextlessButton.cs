﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using NuclearWinter.Animation;
using Microsoft.Xna.Framework.Input;

namespace NuclearWinter.UI
{
    /*
     * A clickable Button containing a Label and an optional Image icon
     */
    public class TextlessButton: Widget
    {
        //----------------------------------------------------------------------
        public struct ButtonStyle
        {
            //------------------------------------------------------------------
            public int              CornerSize;
            public Texture2D        Frame;
            public Texture2D        DownFrame;
            public Texture2D        HoverOverlay;
            public Texture2D        DownOverlay;
            public Texture2D        FocusOverlay;
            public int              VerticalPadding;
            public int              HorizontalPadding;

            //------------------------------------------------------------------
            public ButtonStyle(
                int         _iCornerSize,
                Texture2D   _buttonFrame,
                Texture2D   _buttonFrameDown,
                Texture2D   _buttonFrameHover,
                Texture2D   _buttonFramePressed,
                Texture2D   _buttonFrameFocused,
                int         _iVerticalPadding,
                int         _iHorizontalPadding
            )
            {
                CornerSize      = _iCornerSize;
                Frame           = _buttonFrame;
                DownFrame       = _buttonFrameDown;
                HoverOverlay    = _buttonFrameHover;
                DownOverlay     = _buttonFramePressed;
                FocusOverlay    = _buttonFrameFocused;

                VerticalPadding     = _iVerticalPadding;
                HorizontalPadding   = _iHorizontalPadding;
            }
        }

        //----------------------------------------------------------------------
        Image                   mIcon;

        bool                    mbIsHovered;

        AnimatedValue           mPressedAnim;
        bool                    mbIsPressed;

        protected Box           mMargin;
        public Box              Margin
        {
            get { return mMargin; }

            set {
                mMargin = value;
                UpdateContentSize();
            }
        }

        public Texture2D        Icon
        {
            get {
                return mIcon.Texture;
            }

            set
            {
                mIcon.Texture = value;
                UpdateContentSize();
            }
        }

        public Color            IconColor
        {
            get {
                return mIcon.Color;
            }

            set
            {
                mIcon.Color = value;
            }
        }


        Anchor mAnchor;
        public Anchor Anchor
        {
            get {
                return mAnchor;
            }

            set
            {
                mAnchor = value;
            }
        }

        public ButtonStyle Style;

        public Action<TextlessButton>   ClickHandler;
        public object           Tag;

        public string       TooltipText
        {
            get { return mTooltip.Text; }
            set { mTooltip.Text = value; }
        }

        //----------------------------------------------------------------------
        Tooltip             mTooltip;

        //----------------------------------------------------------------------
        public TextlessButton( Screen _screen, ButtonStyle _style, Texture2D _iconTex = null, Anchor _anchor = Anchor.Center, string _strTooltipText="", object _tag=null )
        : base( _screen )
        {
            Style = _style;

            mPadding    = new Box(0);
            mMargin     = new Box(0);

            mIcon           = new Image( _screen );
            mIcon.Texture   = _iconTex;
            mIcon.Padding   = new Box( Style.VerticalPadding, 0, Style.VerticalPadding, Style.HorizontalPadding );

            Anchor          = _anchor;

            mPressedAnim    = new SmoothValue( 1f, 0f, 0.2f );
            mPressedAnim.SetTime( mPressedAnim.Duration );

            mTooltip        = new Tooltip( Screen, "" );

            TooltipText     = _strTooltipText;
            Tag             = _tag;

            UpdateContentSize();
        }

        //----------------------------------------------------------------------
        public TextlessButton( Screen _screen, Texture2D _iconTex = null, Anchor _anchor = Anchor.Center, string _strTooltipText="", object _tag=null )
        : this( _screen, new ButtonStyle(
                _screen.Style.ButtonCornerSize,
                _screen.Style.ButtonFrame,
                _screen.Style.ButtonDownFrame,
                _screen.Style.ButtonHoverOverlay,
                _screen.Style.ButtonDownOverlay,
                _screen.Style.ButtonFocusOverlay,
                _screen.Style.ButtonVerticalPadding,
                _screen.Style.ButtonHorizontalPadding
            ), _iconTex, _anchor, _strTooltipText, _tag )
        {
        }

        //----------------------------------------------------------------------
        protected internal override void UpdateContentSize()
        {
			if (mIcon.Texture != null) {
				ContentWidth = mIcon.ContentWidth + Padding.Horizontal + mMargin.Horizontal;
				ContentHeight = mIcon.ContentHeight + Padding.Vertical + mMargin.Vertical;
			} else {
				ContentWidth = this.Style.Frame.Width + Padding.Vertical + mMargin.Vertical;
				ContentHeight = this.Style.Frame.Height + Padding.Vertical + mMargin.Vertical;
			}

            base.UpdateContentSize();
        }

        //----------------------------------------------------------------------
        public override void DoLayout( Rectangle _rect )
        {
            base.DoLayout( _rect );
            HitBox = LayoutRect;
            Point pCenter = LayoutRect.Center;

            switch( mAnchor )
            {
                case UI.Anchor.Start:
                    if( mIcon.Texture != null )
                    {
                        mIcon.DoLayout( new Rectangle( LayoutRect.X + Padding.Left + Margin.Left, pCenter.Y - mIcon.ContentHeight / 2, mIcon.ContentWidth, mIcon.ContentHeight ) );
                    }

                    break;
                case UI.Anchor.Center: {
                    if( mIcon.Texture != null )
                    {
                        mIcon.DoLayout( new Rectangle( pCenter.X - ContentWidth / 2 + Padding.Left + Margin.Left, pCenter.Y - mIcon.ContentHeight / 2, mIcon.ContentWidth, mIcon.ContentHeight ) );
                    }
                    break;
                }
            }
        }

        //----------------------------------------------------------------------
        public override void Update( float _fElapsedTime )
        {
            if( ! mPressedAnim.IsOver )
            {
                mPressedAnim.Update( _fElapsedTime );
            }

            mTooltip.EnableDisplayTimer = mbIsHovered;
            mTooltip.Update( _fElapsedTime );
        }

        //----------------------------------------------------------------------
        public override void OnMouseEnter( Point _hitPoint )
        {
            base.OnMouseEnter( _hitPoint );
            mbIsHovered = true;
        }

        public override void OnMouseOut( Point _hitPoint )
        {
            base.OnMouseOut( _hitPoint );
            mbIsHovered = false;
            mTooltip.EnableDisplayTimer = false;
        }

        //----------------------------------------------------------------------
        protected internal override bool OnMouseDown( Point _hitPoint, int _iButton )
        {
            if( _iButton != Screen.Game.InputMgr.PrimaryMouseButton ) return false;

            Screen.Focus( this );
            OnActivateDown();

            return true;
        }

        protected internal override void OnMouseUp( Point _hitPoint, int _iButton )
        {
            if( _iButton != Screen.Game.InputMgr.PrimaryMouseButton ) return;

            if( HitTest( _hitPoint ) == this )
            {
                OnActivateUp();
            }
            else
            {
                ResetPressState();
            }
        }

        //----------------------------------------------------------------------
        protected internal override bool OnActivateDown()
        {
            mbIsPressed = true;
            mPressedAnim.SetTime( 0f );
            return true;
        }

        protected internal override void OnActivateUp()
        {
            mPressedAnim.SetTime( 0f );
            mbIsPressed = false;
            if( ClickHandler != null ) ClickHandler( this );
        }

        protected internal override void OnBlur()
        {
            ResetPressState();
        }

        //----------------------------------------------------------------------
        internal void ResetPressState()
        {
            mPressedAnim.SetTime( 1f );
            mbIsPressed = false;
        }

        //----------------------------------------------------------------------
        public override void Draw()
        {
            Texture2D frame = (!mbIsPressed) ? Style.Frame : Style.DownFrame;

            if( frame != null )
            {
                Screen.DrawBox( frame, LayoutRect, Style.CornerSize, Color.White );
            }

            Rectangle marginRect = new Rectangle( LayoutRect.X + Margin.Left, LayoutRect.Y + Margin.Top, LayoutRect.Width - Margin.Left - Margin.Right, LayoutRect.Height - Margin.Top - Margin.Bottom );

            if( mbIsHovered && ! mbIsPressed && mPressedAnim.IsOver )
            {
                if( Screen.IsActive && Style.HoverOverlay != null )
                {
                    Screen.DrawBox( Style.HoverOverlay, marginRect, Style.CornerSize, Color.White );
                }
            }
            else
            if( mPressedAnim.CurrentValue > 0f )
            {
                if( Style.DownOverlay != null )
                {
                    Screen.DrawBox( Style.DownOverlay, marginRect, Style.CornerSize, Color.White * mPressedAnim.CurrentValue );
                }
            }

            if( Screen.IsActive && HasFocus && ! mbIsPressed )
            {
                if( Style.FocusOverlay != null )
                {
                    Screen.DrawBox( Style.FocusOverlay, marginRect, Style.CornerSize, Color.White );
                }
            }

            mIcon.Draw();
        }

        //----------------------------------------------------------------------
        protected internal override void DrawHovered()
        {
            mTooltip.Draw();
        }
    }
}
