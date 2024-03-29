using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using NuclearWinter.Animation;
using Microsoft.Xna.Framework.Input;
using NuclearWinter.Collections;
using System.Diagnostics;

#if !MONOGAME
using OSKey = System.Windows.Forms.Keys;
#elif !MONOMAC
using OSKey = OpenTK.Input.Key;
#else
using OSKey = MonoMac.AppKit.NSKey;
#endif

namespace NuclearWinter.UI
{
    //--------------------------------------------------------------------------
    // A ListViewColumn defines a column in a ListView
    public class ListViewColumn
    {
        //----------------------------------------------------------------------
        public Label        Label { get; private set; }
        public Image        Image { get; private set; }
        public int          Width;
        public Anchor       Anchor;

        //----------------------------------------------------------------------
        public ListViewColumn( ListView _listView, string _strText, int _iWidth, Anchor _anchor )
        {
            Width   = _iWidth;
            Label   = new UI.Label( _listView.Screen, _strText );
            Anchor  = _anchor;
        }

        public ListViewColumn( ListView _listView, Texture2D _tex, Color _color, int _iWidth, Anchor _anchor )
        {
            Width   = _iWidth;
            Image   = new Image( _listView.Screen, _tex );
            Image.Color = _color;
            Anchor  = _anchor;
        }
    }

    //--------------------------------------------------------------------------
    abstract public class ListViewCell
    {
        //----------------------------------------------------------------------
        protected ListView                      mListView;

        public string                           Text;
        public Texture2D                        Image;
        public Color                            Color;

        public List<ListViewCellIndicator>      Indicators { get; private set; }
        protected int                           miIndicatorsWidth;

        //----------------------------------------------------------------------
        public ListViewCell( ListView _view, string _strText, Texture2D _image )
        {
            mListView = _view;
            Text    = _strText;
            Image   = _image;

            Indicators = new List<ListViewCellIndicator>();
        }

        //----------------------------------------------------------------------
        protected internal abstract void DoLayout( Rectangle _rect, int _iColumnIndex, ListViewColumn _col, ListViewRow _row );
        protected internal abstract void Draw( Point _location );
    }

    //--------------------------------------------------------------------------
    public class ListViewTextCell: ListViewCell
    {
        string          mstrText;
        float           mfTextWidth;
        Vector2         mvTextOffset;

        //----------------------------------------------------------------------
        public ListViewTextCell( ListView _view, string _strText )
        : base( _view, _strText, null )
        {
            Color   = mListView.TextColor;
        }

        //----------------------------------------------------------------------
        protected internal override void DoLayout( Rectangle _rect, int _iColumnIndex, ListViewColumn _col, ListViewRow _row )
        {
            int iIndicatorRight = _rect.Right;
            if( mListView.ActionButtonsColumn == _iColumnIndex && mListView.HoveredRow == _row && mListView.ActionButtons.Count > 0 )
            {
                iIndicatorRight -= mListView.ActionButtons.Sum( x => x.ContentWidth + mListView.Style.CellHorizontalPadding ) + mListView.Style.ActionButtonsRightPadding;
            }

            miIndicatorsWidth = 0;

            // Indicators
            foreach( ListViewCellIndicator indicator in Indicators )
            {
                miIndicatorsWidth += indicator.ContentWidth;
                indicator.DoLayout( new Rectangle ( iIndicatorRight - miIndicatorsWidth - mListView.Style.CellHorizontalPadding - mListView.Style.ActionButtonsRightPadding, _rect.Top + mListView.Style.IndicatorVerticalPadding, indicator.ContentWidth, mListView.Style.RowHeight - mListView.Style.IndicatorVerticalPadding * 2 ) );
                miIndicatorsWidth += mListView.Style.IndicatorHorizontalSpacing;
            }

            mstrText = Text;
            mfTextWidth = mListView.Screen.Style.MediumFont.MeasureString( mstrText ).X;
            if( mstrText != "" )
            {
                int iOffset = mstrText.Length;

                while( mfTextWidth + mListView.Style.CellHorizontalPadding * 2 > _col.Width - miIndicatorsWidth )
                {
                    iOffset--;
                    mstrText = Text.Substring( 0, iOffset ) + "…";
                    if( iOffset == 0 ) break;

                    mfTextWidth = mListView.Screen.Style.MediumFont.MeasureString( mstrText ).X;
                }
            }

            mvTextOffset = Vector2.Zero;
            switch( _col.Anchor )
            {
                case Anchor.Start:
                    mvTextOffset.X += mListView.Style.CellHorizontalPadding;
                    break;
                case Anchor.Center:
                    mvTextOffset.X += (int)( _col.Width / 2f - mfTextWidth / 2f );
                    break;
                case Anchor.End:
                    mvTextOffset.X += _col.Width - mListView.Style.CellHorizontalPadding - mfTextWidth;
                    break;
            }
        }

        //----------------------------------------------------------------------
        protected internal override void Draw( Point _location )
        {
            Vector2 vTextPos = new Vector2( _location.X, _location.Y + mListView.Style.RowHeight / 2f - (int)( mListView.Screen.Style.MediumFont.LineSpacing * 0.9f / 2f ) );
            vTextPos += mvTextOffset;
            vTextPos.Y += mListView.Screen.Style.MediumFont.YOffset;

            mListView.Screen.Game.SpriteBatch.DrawString( mListView.Screen.Style.MediumFont, mstrText, vTextPos, Color );

            foreach( ListViewCellIndicator indicator in Indicators )
            {
                indicator.Draw();
            }
        }
    }

    //--------------------------------------------------------------------------
    public class ListViewImageCell: ListViewCell
    {
        public int ForcedHeight;

        Vector2     mvOffset;
        Vector2     mvSize;

        //----------------------------------------------------------------------
        public ListViewImageCell( ListView _view, Texture2D _image, int _iForcedHeight=0 )
        : base( _view, null, _image )
        {
            ForcedHeight = _iForcedHeight;
            Color   = Color.White;
        }

        //----------------------------------------------------------------------
        protected internal override void DoLayout( Rectangle _rect, int _iColumnIndex, ListViewColumn _col, ListViewRow _row )
        {
            mvOffset = Vector2.Zero;

            mvSize = new Vector2( Image.Width, Image.Height );
            if( ForcedHeight > 0 )
            {
                mvSize.X *= ForcedHeight / mvSize.Y;
                mvSize.Y = ForcedHeight;
            }

            switch( _col.Anchor )
            {
                case Anchor.Start:
                    mvOffset.X += 10;
                    break;
                case Anchor.Center:
                    mvOffset.X += (int)( _col.Width / 2f - mvSize.X / 2f );
                    break;
                case Anchor.End:
                    mvOffset.X += (int)( _col.Width - mvSize.X - 10 );
                    break;
            }
        }

        //----------------------------------------------------------------------
        protected internal override void Draw( Point _location )
        {
            Vector2 vImagePos = new Vector2( _location.X, _location.Y + mListView.Style.RowHeight / 2 - mvSize.Y / 2f );
            vImagePos += mvOffset;

            if( ForcedHeight == 0 )
            {
                mListView.Screen.Game.SpriteBatch.Draw( Image, vImagePos, Color );
            }
            else
            {
                mListView.Screen.Game.SpriteBatch.Draw( Image, new Rectangle( (int)vImagePos.X, (int)vImagePos.Y, (int)mvSize.X, (int)mvSize.Y ), Color );
            }
        }
    }

    //--------------------------------------------------------------------------
    public class ListViewCellIndicator: Widget
    {
        public Texture2D            Frame;
        public int                  FrameCornerSize;

        public object               Tag;

        public Texture2D            Icon
        {
            get { return mImage.Texture; }
            set {
                mImage.Texture = value;
                UpdatePaddings();
            }
        }

        public string Text
        {
            get { return mLabel.Text; }
            set {
                mLabel.Text = value;
                UpdatePaddings();
            }
        }

        Label                       mLabel;
        Image                       mImage;

        void UpdatePaddings()
        {
            mImage.Padding = mLabel.Text != "" ? new Box( 0, 0, 0, 10 ) : new Box(0);
            mLabel.Padding = mImage.Texture != null ? new Box( 5, 5, 5, 0 ) : new Box( 5 );

            UpdateContentSize();
        }

        //----------------------------------------------------------------------
        public ListViewCellIndicator( Screen _screen, string _strText, Texture2D _iconTex=null, object _tag=null )
        : base( _screen )
        {
            mLabel = new Label( Screen, _strText );
            mLabel.Font = Screen.Style.SmallFont;
            mLabel.Parent = this;

            mImage = new Image( _screen, _iconTex );

            UpdatePaddings();

            Tag = _tag;

            UpdateContentSize();
        }

        //----------------------------------------------------------------------
        protected internal override void UpdateContentSize()
        {
            ContentWidth = ( mImage.Texture != null ? mImage.ContentWidth : 0 ) + ( mLabel.Text != "" ? mLabel.ContentWidth : 0 );

            // Not calling base.UpdateContentSize();
        }

        public override void DoLayout( Rectangle _rect )
        {
            base.DoLayout( _rect );

            int iLabelX = 0;
            if( mImage.Texture != null )
            {
                mImage.DoLayout( new Rectangle( _rect.X, _rect.Y, mImage.ContentWidth, _rect.Height ) );
                iLabelX += mImage.ContentWidth;
            }

            mLabel.DoLayout( new Rectangle( _rect.X + iLabelX, _rect.Y, _rect.Width - iLabelX, _rect.Height ) );
        }

        //----------------------------------------------------------------------
        public override void Draw()
        {
            if( Frame != null )
            {
                Screen.DrawBox( Frame, LayoutRect, FrameCornerSize, Color.White );
            }

            if( mImage.Texture != null )
            {
                mImage.Draw();
            }

            mLabel.Draw();
        }
    }

    //--------------------------------------------------------------------------
    /*
     * A set of values for a row in a ListView
     */
    public class ListViewRow
    {
        //----------------------------------------------------------------------
        public ListViewCell[]           Cells           { get; private set; }
        public object                   Tag;

        //----------------------------------------------------------------------
        public ListViewRow( ListViewCell[] _aCells, object _tag )
        {
            Cells           = _aCells;
            Tag             = _tag;
        }
    }

    /*
     * A ListView displays data as a grid
     * Rows can be selected
     */
    public class ListView: Widget
    {
        //----------------------------------------------------------------------
        public struct ListViewStyle
        {
            public int              RowHeight;
            public int              RowSpacing;
            public int              CellHorizontalPadding;
            public int              IndicatorHorizontalSpacing;
            public int              IndicatorVerticalPadding;
            public int              ActionButtonsRightPadding;

            public Texture2D        ListViewFrame;
            public int              ListViewFrameCornerSize;

            // Column Header
            public int              ColumnHeaderCornerSize;
            public Texture2D        ColumnHeaderFrame;

            // Cell / Row
            public int              CellCornerSize;
            public Texture2D        CellFrame;
            public Texture2D        CellHoverOverlay;
            public Texture2D        CellFocusOverlay;

            public Texture2D        SelectedCellFrame;
            public Texture2D        SelectedCellHoverOverlay;
            public Texture2D        SelectedCellFocusOverlay;

            public int              NewRowFrameCornerSize;
            public Texture2D        NewRowFrame;
            public Texture2D        NewRowHoveredFrame;

            public int              NewRowHorizontalPadding;
            public Color            NewRowTextColor;
            public Color            NewRowHoveredTextColor;
        }

        //----------------------------------------------------------------------
        public List<ListViewColumn> Columns             { get; private set; }
        public bool                 DisplayColumnHeaders    = true;
        public bool                 MergeColumns            = false;
        public bool                 SelectFocusedRow        = true;

        public ObservableList<ListViewRow>
                                    Rows                { get; private set; }
        public int                  ColSpacing  = 0;

        public ListViewRow          SelectedRow;
        public int                  SelectedRowIndex { get { return ( SelectedRow != null ) ? Rows.IndexOf( SelectedRow ) : -1; } }

        public Action<ListView>     SelectHandler;
        public Action<ListView>     ValidateHandler;

        public ListViewRow          FocusedRow;
        Point                       mHoverPoint;
        public ListViewRow          HoveredRow;

        public ListViewStyle        Style;
        public Color                TextColor;

        public string               NewRowText;

        //----------------------------------------------------------------------
        public readonly ObservableList<Button> ActionButtons;
        public int                  ActionButtonsColumn = -1;
        Button                      mHoveredActionButton;
        bool                        mbIsHoveredActionButtonDown;

        //----------------------------------------------------------------------
        public Scrollbar            Scrollbar           { get; private set; }

        //----------------------------------------------------------------------
        public Action<ListView>             HoverHandler;
        public Action<ListView>             NewRowClickedHandler;

        //----------------------------------------------------------------------
        // Drag & drop
        public Func<ListViewRow,int,bool>   DragNDropHandler;
        bool                                mbIsMouseDown;
        internal bool                       IsDragging { get; private set; }
        Point                               mMouseDownPoint;
        Point                               mMouseDragPoint;
        bool                                mbInsertAfter;

        bool                                mbIsHoveringNewRow;

        //----------------------------------------------------------------------
        public ListView( Screen _screen )
        : base( _screen )
        {
            Columns = new List<ListViewColumn>();
            
            Rows    = new ObservableList<ListViewRow>();

            Rows.ListCleared += delegate {
                SelectedRow         = null;
                HoveredRow          = null;
                FocusedRow          = null;

                mHoveredActionButton = null;
                mbIsHoveredActionButtonDown = false;
            };

            Rows.ListChanged += delegate( object _source, ObservableList<ListViewRow>.ListChangedEventArgs _args )
            {
                if( ! _args.Added )
                {
                    if( _args.Item == SelectedRow )
                    {
                        SelectedRow = null;
                    }

                    if( _args.Item == HoveredRow )
                    {
                        UpdateHoveredRow();
                    }

                    if( _args.Item == FocusedRow )
                    {
                        FocusedRow = null;
                        IsDragging = false;
                    }
                }
            };

            SelectedRow     = null;
            FocusedRow      = null;
            HoveredRow      = null;
            TextColor = Screen.Style.DefaultTextColor;

            Padding = Screen.Style.ListViewPadding;
            Style = Screen.Style.ListViewStyle;

            Scrollbar = new Scrollbar( _screen );
            Scrollbar.Parent = this;
            
            ActionButtons = new ObservableList<Button>();

            ActionButtons.ListCleared += delegate {
                mHoveredActionButton = null;
            };

            ActionButtons.ListChanged += delegate {
                if( mHoveredActionButton != null && ! ActionButtons.Contains( mHoveredActionButton ) )
                {
                    mHoveredActionButton = null;
                }
            };

            UpdateContentSize();
        }

        //----------------------------------------------------------------------
        public void AddColumn( string _strText, int _iWidth, Anchor _anchor = Anchor.Center )
        {
            Columns.Add( new ListViewColumn( this, _strText, _iWidth, _anchor ) );
        }

        public void AddColumn( Texture2D _tex, Color _color, int _iWidth, Anchor _anchor = Anchor.Center )
        {
            Columns.Add( new ListViewColumn( this, _tex, _color, _iWidth, _anchor ) );
        }

        //----------------------------------------------------------------------
        protected internal override void UpdateContentSize()
        {
            ContentWidth    = Padding.Left + Padding.Right;
            ContentHeight   = Padding.Top + Padding.Bottom;

            base.UpdateContentSize();
        }

        //----------------------------------------------------------------------
        public override void Update( float _fElapsedTime )
        {
            foreach( Button actionButton in ActionButtons )
            {
                actionButton.Update( _fElapsedTime );
            }

            Scrollbar.Update( _fElapsedTime );

            base.Update( _fElapsedTime );
        }

        //----------------------------------------------------------------------
        public override void DoLayout( Rectangle _rect )
        {
            base.DoLayout( _rect );
            HitBox = new Rectangle( LayoutRect.X + Padding.Left, LayoutRect.Y + Padding.Top, LayoutRect.Width - Padding.Horizontal, LayoutRect.Height - Padding.Vertical );
            
            int iColX = 0;
            int iColIndex = 0;
            foreach( ListViewColumn col in Columns )
            {
                if( col.Label != null )
                {
                    col.Label.DoLayout( new Rectangle( LayoutRect.X + Padding.Left + iColX, LayoutRect.Y + Padding.Top, col.Width, Style.RowHeight ) );
                }
                else
                {
                    col.Image.DoLayout( new Rectangle( LayoutRect.X + Padding.Left + iColX, LayoutRect.Y + Padding.Top, col.Width, Style.RowHeight ) );
                }

                int iRowIndex = 0;
                foreach( ListViewRow row in Rows )
                {
                    int iRowY = GetRowY( iRowIndex );
                    row.Cells[ iColIndex ].DoLayout( new Rectangle( LayoutRect.X + Padding.Left + iColX, LayoutRect.Y + Padding.Top + iRowY, col.Width, Style.RowHeight + Style.RowSpacing ), iColIndex, col, row );

                    iRowIndex++;
                }

                iColIndex++;

                iColX += col.Width + ColSpacing;
            }

            //------------------------------------------------------------------
            if( HoveredRow != null )
            {
                int iRight = LayoutRect.Right - Padding.Right - Style.CellHorizontalPadding;
                if( ActionButtonsColumn != -1 )
                {
                    iRight = LayoutRect.Left + Padding.Left + Columns.Take(ActionButtonsColumn + 1).Sum( x => x.Width );
                }

                int iButtonX = Style.ActionButtonsRightPadding;
                foreach( Button button in ActionButtons.Reverse<Button>() )
                {
                    button.DoLayout( new Rectangle(
                        iRight - iButtonX - button.ContentWidth,
                        LayoutRect.Y + Padding.Top + GetRowY( Rows.IndexOf( HoveredRow ) ) + Style.RowHeight / 2 - button.ContentHeight / 2,
                        button.ContentWidth, button.ContentHeight )
                    );

                    iButtonX += button.ContentWidth + Style.CellHorizontalPadding;
                }
            }

            //------------------------------------------------------------------
            ContentHeight = Padding.Vertical + Rows.Count * ( Style.RowHeight + Style.RowSpacing ) - Style.RowSpacing;
            if( NewRowText != null )
            {
                ContentHeight += Style.RowHeight;
            }

            Scrollbar.DoLayout( LayoutRect, ContentHeight );
        }

        //----------------------------------------------------------------------
        public override Widget HitTest( Point _point )
        {
            return Scrollbar.HitTest( _point ) ?? base.HitTest( _point );
        }

        //----------------------------------------------------------------------
        public override void OnMouseEnter( Point _hitPoint )
        {
            mHoverPoint = _hitPoint;
            UpdateHoveredRow();
        }

        public override void OnMouseMove( Point _hitPoint )
        {
            if( mbIsMouseDown && FocusedRow != null )
            {
                IsDragging = DragNDropHandler != null && (
                        Math.Abs( _hitPoint.Y - mMouseDownPoint.Y ) > MouseDragTriggerDistance
                    ||  Math.Abs( _hitPoint.X - mMouseDownPoint.X ) > MouseDragTriggerDistance );
                mMouseDragPoint = _hitPoint;
            }

            mHoverPoint = _hitPoint;
            UpdateHoveredRow();
        }

        void UpdateHoveredRow()
        {
            int iRowY = ( mHoverPoint.Y - ( LayoutRect.Y + Padding.Top + ( DisplayColumnHeaders ? Style.RowHeight : 0 ) - (int)Scrollbar.LerpOffset ) );
            int iHoveredRowIndex = Math.Max( -1, iRowY / ( Style.RowHeight + Style.RowSpacing ) );

            var oldHoveredRow = HoveredRow;
            HoveredRow = null;

            bool bWasHoveringNewRow = mbIsHoveringNewRow;
            mbIsHoveringNewRow = false;

            int iOffset = iRowY % ( Style.RowHeight + Style.RowSpacing );
            mbInsertAfter = iOffset >= ( Style.RowHeight + Style.RowSpacing ) / 2;

            if( iHoveredRowIndex >= Rows.Count )
            {
                mbInsertAfter = true;

                if( mbIsHoveredActionButtonDown )
                {
                    mHoveredActionButton.ResetPressState();
                    mbIsHoveredActionButtonDown = false;
                }
                mHoveredActionButton = null;

                if( iHoveredRowIndex == Rows.Count && NewRowText != null )
                {
                    mbIsHoveringNewRow = true;
                }
            }
            else
            if( iHoveredRowIndex >= 0 )
            {
                HoveredRow = Rows[ iHoveredRowIndex ];

                if( HoverHandler != null && oldHoveredRow != HoveredRow )
                {
                    HoverHandler( this );
                }

                if( mHoveredActionButton != null )
                {
                    if( mHoveredActionButton.HitTest( mHoverPoint ) != null )
                    {
                        mHoveredActionButton.OnMouseMove( mHoverPoint );
                    }
                    else
                    {
                        mHoveredActionButton.OnMouseOut( mHoverPoint );

                        if( mbIsHoveredActionButtonDown )
                        {
                            mHoveredActionButton.ResetPressState();
                            mbIsHoveredActionButtonDown = false;
                        }

                        mHoveredActionButton = null;
                    }
                }

                if( mHoveredActionButton == null )
                {
                    mbIsHoveredActionButtonDown = false;

                    foreach( Button button in ActionButtons )
                    {
                        if( button.HitTest( mHoverPoint ) != null )
                        {
                            mHoveredActionButton = button;
                            mHoveredActionButton.OnMouseEnter( mHoverPoint );
                            break;
                        }
                    }
                }
            }

            if( bWasHoveringNewRow && ! mbIsHoveringNewRow )
            {
#if !MONOGAME
                Screen.Game.Form.Cursor = System.Windows.Forms.Cursors.Default;
#endif
            }
            else
            if( ! bWasHoveringNewRow && mbIsHoveringNewRow )
            {
#if !MONOGAME
                Screen.Game.Form.Cursor = System.Windows.Forms.Cursors.Hand;
#endif
            }
        }

        //----------------------------------------------------------------------
        public override void OnMouseOut( Point _hitPoint )
        {
            HoveredRow = null;

            if( mbIsHoveringNewRow )
            {
#if !MONOGAME
                Screen.Game.Form.Cursor = System.Windows.Forms.Cursors.Default;
#endif
                mbIsHoveringNewRow = false;
            }

            if( mHoveredActionButton != null )
            {
                if( mbIsHoveredActionButtonDown )
                {
                    mHoveredActionButton.ResetPressState();
                    mbIsHoveredActionButtonDown = false;
                }
                mHoveredActionButton = null;
            }
        }

        //----------------------------------------------------------------------
        protected internal override void OnMouseWheel( Point _hitPoint, int _iDelta )
        {
            DoScroll( -_iDelta / 120 * 3 * ( Style.RowHeight + Style.RowSpacing ) );
        }

        //----------------------------------------------------------------------
        void DoScroll( int _iDelta )
        {
            int iScrollChange = (int)MathHelper.Clamp( _iDelta, -Scrollbar.Offset, Math.Max( 0, Scrollbar.Max - Scrollbar.Offset ) );
            Scrollbar.Offset += iScrollChange;

            UpdateHoveredRow();
        }

        //----------------------------------------------------------------------
        protected internal override bool OnMouseDown( Point _hitPoint, int _iButton )
        {
            if( _iButton != Screen.Game.InputMgr.PrimaryMouseButton ) return false;

            mHoverPoint = _hitPoint;

            if( mHoveredActionButton != null )
            {
                mHoveredActionButton.OnActivateDown();
                mbIsHoveredActionButtonDown = true;
            }
            else
            {
                mbIsMouseDown = true;
                mMouseDownPoint = new Point( _hitPoint.X, _hitPoint.Y + (int)Scrollbar.LerpOffset );

                Screen.Focus( this );

                int iFocusedRowIndex = Math.Max( 0, ( _hitPoint.Y - ( LayoutRect.Y + Padding.Top + ( DisplayColumnHeaders ? Style.RowHeight : 0 ) ) + (int)Scrollbar.LerpOffset ) / ( Style.RowHeight + Style.RowSpacing ) );

                if( iFocusedRowIndex > Rows.Count - 1 )
                {
                    iFocusedRowIndex = -1;
                }

                FocusedRow = (iFocusedRowIndex != -1) ? Rows[ iFocusedRowIndex ] : null;
            }

            return true;
        }

        protected internal override void OnMouseUp( Point _hitPoint, int _iButton )
        {
            if( _iButton != Screen.Game.InputMgr.PrimaryMouseButton ) return;

            mbIsMouseDown = false;

            mHoverPoint = _hitPoint;

            if( mHoveredActionButton != null )
            {
                if( mbIsHoveredActionButtonDown )
                {
                    mHoveredActionButton.OnMouseUp( _hitPoint, _iButton );
                    mbIsHoveredActionButtonDown = false;
                }
            }
            else
            if( IsDragging )
            {
                Debug.Assert( FocusedRow != null );
                ListViewRow draggedRow = FocusedRow;

                if( HitBox.Contains( _hitPoint ) && HoveredRow != draggedRow &&  DragNDropHandler != null )
                {
                    int iIndex = ( ( HoveredRow != null ) ? Rows.IndexOf( HoveredRow ) : (Rows.Count - 1) ) + ( mbInsertAfter ? 1 : 0 );

                    if( iIndex > Rows.IndexOf( draggedRow ) )
                    {
                        iIndex--;
                    }

                    if( DragNDropHandler( draggedRow, iIndex ) )
                    {
                        Rows.Remove( draggedRow );
                        Rows.Insert( iIndex, draggedRow );
                    }
                }

                IsDragging = false;
            }
            else
            if( mbIsHoveringNewRow )
            {
                if( NewRowClickedHandler != null )
                {
                    NewRowClickedHandler( this );
                }
            }
            else
            {
                SelectRowAt( _hitPoint );
            }
        }

        //----------------------------------------------------------------------
        protected internal override bool OnMouseDoubleClick( Point _hitPoint )
        {
            if( mHoveredActionButton == null && ValidateHandler != null )
            {
                SelectRowAt( _hitPoint );

                if( SelectedRow != null )
                {
                    ValidateHandler( this );
                    return true;
                }
            }

            return false;
        }

        //----------------------------------------------------------------------
        protected internal override void OnFocus()
        {
            base.OnFocus();

            if( FocusedRow == null )
            {
                FocusedRow = SelectedRow;
            }
        }

        //----------------------------------------------------------------------
        void SelectRowAt( Point _hitPoint )
        {
            UpdateHoveredRow();

            if( HoveredRow != SelectedRow )
            {
                if( HoveredRow != null || SelectFocusedRow )
                {
                    FocusedRow = SelectedRow = HoveredRow;
                    if( SelectHandler != null ) SelectHandler( this );
                }
            }
        }

        protected internal override void OnActivateUp()
        {
            if( FocusedRow != SelectedRow && FocusedRow != null )
            {
                SelectedRow = FocusedRow;
                if( SelectHandler != null ) SelectHandler( this );
            }
            else
            {
                if( ValidateHandler != null ) ValidateHandler( this );
            }
        }

        //----------------------------------------------------------------------
        protected internal override void OnPadMove( Direction _direction )
        {
            if( _direction == Direction.Up )
            {
                if( FocusedRow != null && ! IsDragging )
                {
                    int iFocusedRowIndex = Math.Max( 0, Rows.IndexOf( FocusedRow ) - 1 );
                    FocusedRow = Rows[ iFocusedRowIndex ];

                    if( SelectFocusedRow )
                    {
                        SelectedRow = FocusedRow;
                    }
                }

                if( SelectHandler != null ) SelectHandler( this );
            }
            else
            if( _direction == Direction.Down )
            {
                if( FocusedRow != null && ! IsDragging )
                {
                    int iFocusedRowIndex = Math.Min( Rows.Count - 1, Rows.IndexOf( FocusedRow ) + 1 );
                    FocusedRow = Rows[ iFocusedRowIndex ];

                    if( SelectFocusedRow )
                    {
                        SelectedRow = FocusedRow;
                    }
                }

                if( SelectHandler != null ) SelectHandler( this );
            }
            else
            {
                base.OnPadMove( _direction );
            }
        }

        //----------------------------------------------------------------------
        protected internal override void OnOSKeyPress( OSKey _key )
        {
            switch( _key )
            {
                case OSKey.Home:
                    Scrollbar.Offset = 0;
                    break;
                case OSKey.End:
                    Scrollbar.Offset = Scrollbar.Max;
                    break;
                case OSKey.PageUp:
                    Scrollbar.Offset -= LayoutRect.Height;
                    break;
                case OSKey.PageDown:
                    Scrollbar.Offset += LayoutRect.Height;
                    break;
#if !MONOMAC
                case OSKey.Enter:
#else
                case OSKey.Return:
#endif
                    if( SelectedRow != null && ValidateHandler != null ) ValidateHandler( this );
                    break;
            }
        }

        //----------------------------------------------------------------------
        protected internal override void OnBlur()
        {
            FocusedRow = null;
        }

        int GetRowY( int _iRowIndex )
        {
            return ( ( DisplayColumnHeaders ? 1 : 0 ) + _iRowIndex ) * ( Style.RowHeight + Style.RowSpacing ) - (int)Scrollbar.LerpOffset;
        }

        //----------------------------------------------------------------------
        public override void Draw()
        {
            Screen.DrawBox( Style.ListViewFrame, LayoutRect, Style.ListViewFrameCornerSize, Color.White );

            if( DisplayColumnHeaders )
            {
                int iColX = 0;
                foreach( ListViewColumn col in Columns )
                {
                    Screen.DrawBox( Style.ColumnHeaderFrame, new Rectangle( LayoutRect.X + Padding.Left + iColX, LayoutRect.Y + Padding.Top, col.Width, Style.RowHeight ), Style.ColumnHeaderCornerSize, Color.White );

                    if( col.Label != null )
                    {
                        col.Label.Draw();
                    }
                    else
                    {
                        col.Image.Draw();
                    }
                    iColX += col.Width + ColSpacing;
                }
            }

            Screen.PushScissorRectangle( new Rectangle( LayoutRect.X + Padding.Left, LayoutRect.Y + Padding.Top + ( DisplayColumnHeaders ? Style.RowHeight : 0 ), LayoutRect.Width - Padding.Horizontal, LayoutRect.Height - Padding.Vertical - ( DisplayColumnHeaders ? Style.RowHeight : 0 ) ) );

            int iRowIndex = 0;
            foreach( ListViewRow row in Rows )
            {
                int iRowY = GetRowY( iRowIndex );
                if( ( iRowY + Style.RowHeight + Style.RowSpacing < 0 ) || ( iRowY > LayoutRect.Height - Padding.Vertical ) )
                {
                    iRowIndex++;
                    continue;
                }

                if( MergeColumns )
                {
                    Rectangle rowRect = new Rectangle( LayoutRect.X + Padding.Left, LayoutRect.Y + Padding.Top + iRowY, LayoutRect.Width - Padding.Horizontal, Style.RowHeight );

                    Screen.DrawBox( SelectedRow == row ? Style.SelectedCellFrame : Style.CellFrame, rowRect, Style.CellCornerSize, Color.White );

                    if( HasFocus && FocusedRow == row )
                    {
                        if( SelectedRow != row )
                        {
                            Screen.DrawBox( Style.CellFocusOverlay, rowRect, Style.CellCornerSize, Color.White );
                        }
                        else
                        if( Style.SelectedCellFocusOverlay != null )
                        {
                            Screen.DrawBox( Style.SelectedCellFocusOverlay, rowRect, Style.CellCornerSize, Color.White );
                        }
                    }

                    if( HoveredRow == row && ! IsDragging )
                    {
                        if( SelectedRow != row )
                        {
                            Screen.DrawBox( Style.CellHoverOverlay, rowRect, Style.CellCornerSize, Color.White );
                        }
                        else
                        if( Style.SelectedCellHoverOverlay != null )
                        {
                            Screen.DrawBox( Style.SelectedCellHoverOverlay, rowRect, Style.CellCornerSize, Color.White );
                        }
                    }
                }

                int iColX = 0;
                for( int i = 0; i < row.Cells.Length; i++ )
                {
                    ListViewColumn col = Columns[i];
                    Rectangle rowRect = new Rectangle( LayoutRect.X + Padding.Left + iColX, LayoutRect.Y + Padding.Top + iRowY, col.Width, Style.RowHeight );

                    if( ! MergeColumns )
                    {
                        Screen.DrawBox( SelectedRow == row ? Style.SelectedCellFrame : Style.CellFrame, rowRect, Style.CellCornerSize, Color.White );

                        if( HasFocus && FocusedRow == row )
                        {
                            if( SelectedRow != row )
                            {
                                Screen.DrawBox( Style.CellFocusOverlay, rowRect, Style.CellCornerSize, Color.White );
                            }
                            else
                            if( Style.SelectedCellFocusOverlay != null )
                            {
                                Screen.DrawBox( Style.SelectedCellFocusOverlay, rowRect, Style.CellCornerSize, Color.White );
                            }
                        }

                        if( HoveredRow == row && ! IsDragging )
                        {
                            if( SelectedRow != row )
                            {
                                Screen.DrawBox( Style.CellHoverOverlay, rowRect, Style.CellCornerSize, Color.White );
                            }
                            else
                            if( Style.SelectedCellHoverOverlay != null )
                            {
                                Screen.DrawBox( Style.SelectedCellHoverOverlay, rowRect, Style.CellCornerSize, Color.White );
                            }
                        }
                    }

                    row.Cells[i].Draw( new Point( LayoutRect.X + Padding.Left + iColX, LayoutRect.Y + Padding.Top + iRowY ) );

                    iColX += col.Width + ColSpacing;
                }

                iRowIndex++;
            }

            if( NewRowText != null )
            {
                int iRowY = GetRowY( iRowIndex );
                Screen.DrawBox( mbIsHoveringNewRow ? Style.NewRowHoveredFrame : Style.NewRowFrame, new Rectangle( LayoutRect.X + Padding.Left, LayoutRect.Y + Padding.Top + iRowY, LayoutRect.Width - Padding.Horizontal, Style.RowHeight ), Style.NewRowFrameCornerSize, Color.White );

                Vector2 vTextPos = new Vector2( LayoutRect.X + Padding.Left + Style.NewRowHorizontalPadding, LayoutRect.Y + Padding.Top + iRowY + Style.RowHeight / 2f - (int)( Screen.Style.MediumFont.LineSpacing * 0.9f / 2f ) + Screen.Style.MediumFont.YOffset );
                Screen.Game.SpriteBatch.DrawString( Screen.Style.MediumFont, NewRowText, vTextPos, mbIsHoveringNewRow ? Style.NewRowHoveredTextColor : Style.NewRowTextColor );

                iRowIndex++;
            }

            if( HoveredRow != null && ! IsDragging )
            {
                foreach( Button button in ActionButtons )
                {
                    button.Draw();
                }
            }

            if( IsDragging && HitBox.Contains( mHoverPoint ) )
            {
                int iX = LayoutRect.X + Padding.Left;
                int iWidth = LayoutRect.Width - Padding.Horizontal;

                if( HoveredRow != null )
                {
                    int iY = LayoutRect.Y + Padding.Top + GetRowY( Rows.IndexOf( HoveredRow ) + ( mbInsertAfter ? 1 : 0 ) ) - ( Style.RowSpacing + Screen.Style.ListRowInsertMarker.Height ) / 2;

                    Rectangle markerRect = new Rectangle( iX, iY, iWidth, Screen.Style.ListRowInsertMarker.Height );
                    Screen.DrawBox( Screen.Style.ListRowInsertMarker, markerRect, Screen.Style.ListRowInsertMarkerCornerSize, Color.White );
                }
                else
                if( IsHovered )
                {
                    int iY = LayoutRect.Y + Padding.Top + ( mbInsertAfter ? GetRowY( Rows.Count ) : 0 ) - ( Style.RowSpacing + Screen.Style.ListRowInsertMarker.Height ) / 2;

                    Rectangle markerRect = new Rectangle( iX, iY, iWidth, Screen.Style.ListRowInsertMarker.Height );
                    Screen.DrawBox( Screen.Style.ListRowInsertMarker, markerRect, Screen.Style.ListRowInsertMarkerCornerSize, Color.White );
                }
            }

            Screen.PopScissorRectangle();

            Scrollbar.Draw();
        }

        //----------------------------------------------------------------------
        protected internal override void DrawHovered()
        {
            if( mHoveredActionButton != null && ! IsDragging )
            {
                mHoveredActionButton.DrawHovered();
            }
        }

        //----------------------------------------------------------------------
        protected internal override void DrawFocused()
        {
            if( IsDragging )
            {
                Debug.Assert( FocusedRow != null );
                int iRowY = GetRowY( Rows.IndexOf( FocusedRow ) );

                int iColX = 0;
                for( int i = 0; i < FocusedRow.Cells.Length; i++ )
                {
                    ListViewColumn col = Columns[i];

                    FocusedRow.Cells[i].Draw( new Point( LayoutRect.X + 10 + iColX + mMouseDragPoint.X - mMouseDownPoint.X, LayoutRect.Y + mMouseDragPoint.Y - mMouseDownPoint.Y + (int)Scrollbar.LerpOffset + iRowY ) );

                    iColX += col.Width + ColSpacing;
                }
            }
        }
    }
}
