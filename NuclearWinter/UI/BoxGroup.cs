﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace NuclearWinter.UI
{
    /*
     * BoxGroup allows packing widgets in one direction
     * It takes care of positioning each child widget properly
     */
    public class BoxGroup: Group
    {
        //----------------------------------------------------------------------
        Orientation     mOrientation;
        int             miSpacing;

        List<bool>      mlExpandedChildren;

        Anchor          mContentAnchor;

        //----------------------------------------------------------------------
        public BoxGroup( Screen _screen, Orientation _orientation, int _iSpacing, Anchor _contentAnchor = Anchor.Center )
        : base( _screen )
        {
            mlExpandedChildren = new List<bool>();

            mOrientation    = _orientation;
            miSpacing       = _iSpacing;
            mContentAnchor = _contentAnchor;
        }

        //----------------------------------------------------------------------
        public void AddChild( Widget _widget, int _iIndex, bool _bExpand )
        {
            Debug.Assert( _widget.Parent == null );
            Debug.Assert( _widget.Screen == Screen );

            _widget.Parent = this;
            mlExpandedChildren.Insert( _iIndex, _bExpand );
            mlChildren.Insert( _iIndex, _widget );
            UpdateContentSize();
        }

        public override void AddChild( Widget _widget, int _iIndex )
        {
            AddChild( _widget, _iIndex, false );
        }

        public void AddChild( Widget _widget, bool _bExpand )
        {
            AddChild( _widget, mlChildren.Count, _bExpand );
        }

        public override void RemoveChild( Widget _widget )
        {
            Debug.Assert( _widget.Parent == this );

            _widget.Parent = null;

            mlExpandedChildren.RemoveAt( mlChildren.IndexOf( _widget ) );

            mlChildren.Remove( _widget );
            UpdateContentSize();
        }

        //----------------------------------------------------------------------
        public override Widget GetFirstFocusableDescendant( Direction _direction )
        {
            if( mlChildren.Count == 0 ) return null;

            if( mOrientation == Orientation.Vertical )
            {
                return mlChildren[0].GetFirstFocusableDescendant( _direction );
            }
            else
            {
                return mlChildren[0].GetFirstFocusableDescendant( _direction );
            }
        }

        //----------------------------------------------------------------------
        public override Widget GetSibling( Direction _direction, Widget _child )
        {
            int iIndex = mlChildren.IndexOf( _child );

            if( mOrientation == Orientation.Horizontal )
            {
                if( _direction == Direction.Right )
                {
                    if( iIndex < mlChildren.Count - 1 )
                    {
                        return mlChildren[iIndex + 1];
                    }
                }
                else
                if( _direction == Direction.Left )
                {
                    if( iIndex > 0 )
                    {
                        return mlChildren[iIndex - 1];
                    }
                }
            }
            else
            {
                if( _direction == Direction.Down )
                {
                    if( iIndex < mlChildren.Count - 1 )
                    {
                        return mlChildren[iIndex + 1];
                    }
                }
                else
                if( _direction == Direction.Up )
                {
                    if( iIndex > 0 )
                    {
                        return mlChildren[iIndex - 1];
                    }
                }
            }

            return base.GetSibling( _direction, this );
        }

        //----------------------------------------------------------------------
        protected internal override void UpdateContentSize()
        {
            if( mOrientation == Orientation.Horizontal )
            {
                ContentWidth    = Padding.Horizontal;
                ContentHeight   = 0;
                foreach( Widget child in mlChildren )
                {
                    ContentWidth += child.AnchoredRect.HasWidth ? child.AnchoredRect.Width : child.ContentWidth;
                    ContentHeight = Math.Max( ContentHeight, child.ContentHeight );
                }

                ContentHeight += Padding.Vertical;

                if( mlChildren.Count > 1 )
                {
                    ContentWidth += miSpacing * ( mlChildren.Count - 1 );
                }
            }
            else
            {
                ContentHeight   = Padding.Vertical;
                ContentWidth    = 0;
                foreach( Widget child in mlChildren )
                {
                    ContentHeight += child.AnchoredRect.HasHeight ? child.AnchoredRect.Height : child.ContentHeight;
                    ContentWidth = Math.Max( ContentWidth, child.ContentWidth );
                }

                ContentWidth += Padding.Horizontal;
                if( mlChildren.Count > 1 )
                {
                    ContentHeight += miSpacing * ( mlChildren.Count - 1 );
                }
            }

            base.UpdateContentSize();
        }

        //----------------------------------------------------------------------
        public override void DoLayout( Rectangle _rect )
        {
            base.DoLayout( _rect );
        }

        protected override void LayoutChildren()
        {
            int iWidth = LayoutRect.Width - Padding.Horizontal;
            int iHeight = LayoutRect.Height - Padding.Vertical;

            int iUnexpandedSize = 0;
            int iExpandedChildrenCount = 0;
            for( int iIndex = 0; iIndex < mlChildren.Count; iIndex++ )
            {
                if( ! mlExpandedChildren[iIndex] )
                {
                    iUnexpandedSize += ( mOrientation == Orientation.Horizontal ) ?
                        ( mlChildren[iIndex].AnchoredRect.HasWidth ? mlChildren[iIndex].AnchoredRect.Width : mlChildren[iIndex].ContentWidth ) :
                        ( mlChildren[iIndex].AnchoredRect.HasHeight ? mlChildren[iIndex].AnchoredRect.Height : mlChildren[iIndex].ContentHeight );
                }
                else
                {
                    iExpandedChildrenCount++;
                }
            }

            iUnexpandedSize += ( mlChildren.Count - 1 ) * miSpacing;

            float fExpandedWidgetSize = 0f;
            if( iExpandedChildrenCount > 0 )
            {
                fExpandedWidgetSize = ( ( ( mOrientation == Orientation.Horizontal ) ? iWidth : iHeight ) - iUnexpandedSize ) / (float)iExpandedChildrenCount;
            }

            int iActualSize = iExpandedChildrenCount > 0 ? ( ( mOrientation == Orientation.Horizontal ) ? iWidth : iHeight ) : iUnexpandedSize;

            Point pWidgetPosition;
            
            switch( mOrientation )
            {
                case Orientation.Horizontal:
                    switch( mContentAnchor )
                    {
                        case Anchor.Start:
                            pWidgetPosition = new Point( LayoutRect.Left + Padding.Left, LayoutRect.Top + Padding.Top );
                            break;
                        case Anchor.Center:
                            pWidgetPosition = new Point( LayoutRect.Center.X - iActualSize / 2, LayoutRect.Top + Padding.Top );
                            break;
                        case Anchor.End:
                            pWidgetPosition = new Point( LayoutRect.Right - Padding.Right - iActualSize, LayoutRect.Top + Padding.Top );
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                    break;
                case Orientation.Vertical:
                    switch( mContentAnchor )
                    {
                        case Anchor.Start:
                            pWidgetPosition = new Point( LayoutRect.Left + Padding.Left, LayoutRect.Top + Padding.Top );
                            break;
                        case Anchor.Center:
                            pWidgetPosition = new Point( LayoutRect.X + Padding.Left, LayoutRect.Center.Y - iActualSize / 2 );
                            break;
                        case Anchor.End:
                            pWidgetPosition = new Point( LayoutRect.X + Padding.Left, LayoutRect.Bottom - Padding.Bottom - iActualSize );
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }

            float fOffset = 0;
            for( int iIndex = 0; iIndex < mlChildren.Count; iIndex++ )
            {
                Widget widget = mlChildren[iIndex];

                int iWidgetSize = ( mOrientation == Orientation.Horizontal ) ? 
                    ( mlChildren[iIndex].AnchoredRect.HasWidth ? mlChildren[iIndex].AnchoredRect.Width : mlChildren[iIndex].ContentWidth ) :
                    ( mlChildren[iIndex].AnchoredRect.HasHeight ? mlChildren[iIndex].AnchoredRect.Height : mlChildren[iIndex].ContentHeight );

                if( mlExpandedChildren[iIndex] )
                {
                    if( iIndex < mlChildren.Count - 1 )
                    {
                        iWidgetSize = (int)Math.Floor( fExpandedWidgetSize + fOffset - Math.Floor( fOffset ) );
                    }
                    else
                    {
                        iWidgetSize = (int)( ( ( mOrientation == Orientation.Horizontal ) ? iWidth : iHeight ) - Math.Floor( fOffset ) );
                    }
                    fOffset += fExpandedWidgetSize + miSpacing;
                }
                else
                {
                    fOffset += iWidgetSize + miSpacing;
                }

                widget.DoLayout( new Rectangle( pWidgetPosition.X, pWidgetPosition.Y, mOrientation == Orientation.Horizontal ? iWidgetSize : iWidth, mOrientation == Orientation.Horizontal ? iHeight : iWidgetSize ) );
                
                switch( mOrientation )
                {
                    case Orientation.Horizontal:
                        pWidgetPosition.X += iWidgetSize + miSpacing;
                        break;
                    case Orientation.Vertical:
                        pWidgetPosition.Y += iWidgetSize + miSpacing;
                        break;
                }
            }
        }
    }
}
