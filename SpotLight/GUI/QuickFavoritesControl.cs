using GL_EditorFramework;
using GL_EditorFramework.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using static Spotlight.EditorDrawables.SM3DWorldScene;

namespace Spotlight.GUI
{
    public class QuickFavoriteClosingEventArgs : EventArgs
    {
        public QuickFavoriteControl.QuickFavorite Favorite { get; set; }

        public QuickFavoriteClosingEventArgs(QuickFavoriteControl.QuickFavorite favorite)
        {
            Favorite = favorite;
        }
    }

    public delegate void QuickFavoriteClosingEventHandler(object sender, QuickFavoriteClosingEventArgs e);


    public class QuickFavoriteControl : UserControl
    {
        List<QuickFavorite> favorites = new List<QuickFavorite>();

        int selectedIndex = -1;

        int hoveredIndex = -1;

        int hoveringOverClose = -1;
        
        Graphics g;

        public override string Text { get => base.Text; set => base.Text = value; }

        public event EventHandler SelectedFavoriteChanged;

        public event QuickFavoriteClosingEventHandler FavoriteClosing;

        public class QuickFavorite
        {
            public string Name;
            public ObjectPlacementHandler PlacementHandler;

            public QuickFavorite(string name, ObjectPlacementHandler placementHandler)
            {
                Name = name;
                PlacementHandler = placementHandler;
            }
        }

        public QuickFavoriteControl()
        {
            SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer,
            true);

            BorderStyle = BorderStyle.None;

            if (!DesignMode)
                return;

            favorites.Add(new QuickFavorite(
@"Kuribo
Obj:Kuribo
Mod:
"
                , null));

            favorites.Add(new QuickFavorite(
@"RailWithMoveParameters
RoundedRect
Closed
"
    , null));
        }

        public QuickFavorite SelectedFavorite
        {
            get
            {
                if (selectedIndex != -1)
                    return favorites[selectedIndex];
                else
                    return null;
            }
        }

        public void Deselect()
        {
            if (selectedIndex != -1)
            {
                selectedIndex = -1;
                SelectedFavoriteChanged?.Invoke(this, new EventArgs());
            }

            Invalidate();
        }

        public void Select(QuickFavorite favorite)
        {
            int index = favorites.IndexOf(favorite);
            if (index != -1)
                Select(index);
        }

        public void Select(int index)
        {
            if (index < 0 || index > favorites.Count)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be within the bounds of " + nameof(Favorites));

            if(selectedIndex!=index)
            {
                selectedIndex = index;
                SelectedFavoriteChanged?.Invoke(this, new EventArgs());
            }

            Invalidate();
        }

        public void AddFavorite(QuickFavorite favorite)
        {
            favorites.Add(favorite);

            Invalidate();
        }

        public IReadOnlyList<QuickFavorite> Favorites => favorites;

        public Font HeaderFont { get; private set; } = new Font(SystemFonts.DefaultFont, FontStyle.Bold);

        public void InsertFavorite(int index, QuickFavorite favorite, bool select)
        {
            if (index < 0 || index > favorites.Count)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be within the bounds of "+nameof(Favorites));

            favorites.Insert(index, favorite);

            if (select)
            {
                selectedIndex = index;
                SelectedFavoriteChanged?.Invoke(this, new EventArgs());
            }

            Invalidate();
        }

        public void RemoveFavorite(QuickFavorite favorite)
        {
            int index = favorites.IndexOf(favorite);
            if (index != -1)
                RemoveFavorite(index);
        }

        public void RemoveFavorite(int index)
        {
            favorites.RemoveAt(index);

            if (selectedIndex >= index || selectedIndex > favorites.Count - 1)
            {
                selectedIndex--;
                SelectedFavoriteChanged?.Invoke(this, new EventArgs());
            }

            Invalidate();
        }

        public void ClearFavorites()
        {
            favorites.Clear();
            selectedIndex = -1;
            SelectedFavoriteChanged?.Invoke(this, new EventArgs());

            Invalidate();
        }

        protected static Point[] arrowLeft = new Point[]
        {
            new Point(arrowWidth/2+4,  2),
            new Point(arrowWidth/2+4, 18),
            new Point(arrowWidth/2-4, 10)
        };

        protected static Point[] arrowRight = new Point[]
        {
            new Point(arrowWidth/2-4,  2),
            new Point(arrowWidth/2-4, 18),
            new Point(arrowWidth/2+4, 10)
        };

        static readonly char[] newLineSeperator = new char[] { '\n' };

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            g = e.Graphics;
            g.Clear(SystemColors.ControlLightLight);

            int x = arrowWidth + 5;

            Rectangle favoritesArea = new Rectangle(arrowWidth + 5, 0, Width - 10 - arrowWidth * 2, Height);
            g.SetClip(favoritesArea);

            if (favorites.Count == 0)
                g.DrawString(Program.CurrentLanguage?.GetTranslation("NoQuickFavourites") ?? "No Quick Favourites", Font, SystemBrushes.ControlDarkDark, Width /2, Height/2, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            hoveredIndex = -1;
            hoveringOverClose = -1;
            for (int i = scrollIndexOffset; i < favorites.Count; i++)
            {
                int width = (int)Math.Ceiling(g.MeasureString(favorites[i].Name, Font).Width);
                if (favoritesArea.Contains(mousePos) && (mousePos.X > x && mousePos.X < x + width + 25) && (mousePos.Y > 5 && mousePos.Y < Height - 5))
                    hoveredIndex = i;
                if ((favoritesArea.Contains(mousePos) && (mousePos.X > x + width && mousePos.X < x + width + 25) && (mousePos.Y > 5 && mousePos.Y < 25)))
                    hoveringOverClose = i;

                g.FillRectangle(i == selectedIndex ? SystemBrushes.Highlight : SystemBrushes.ControlDark, x, 5, width + 25, Height - 10);

                g.FillRectangle(hoveredIndex == i ? SystemBrushes.ControlLight : SystemBrushes.ControlLightLight, x + 1, 6, width + 23, Height - 12);

                string[] parts = favorites[i].Name.Split(newLineSeperator, 2);

                g.DrawString(parts[0], Font, i == selectedIndex ? SystemBrushes.Highlight : SystemBrushes.ControlText, x + 5, 7);

                if (parts.Length>1)
                    g.DrawString(parts[1], Font, SystemBrushes.ControlDarkDark, x + 5, HeaderFont.Height + 9);

                g.DrawImage(hoveringOverClose == i ? Resources.CloseTabIconHover : Resources.CloseTabIcon, x + width + 13, 9);

                x += width + 27;
            }

            canScrollRight = x > Width - 5 - arrowWidth;

            g.ResetClip();

            hoveredArrow = HoveredArrow.NONE;

            if (scrollIndexOffset>0)
            {
                if (new Rectangle(5, 1, arrowWidth, Height - 2).Contains(mousePos))
                    hoveredArrow = HoveredArrow.LEFT;

                g.TranslateTransform(5, 10);
                g.FillPolygon(hoveredArrow == HoveredArrow.LEFT ? SystemBrushes.ControlDark : Framework.backBrush, arrowLeft);
                g.ResetTransform();
            }

            if (canScrollRight)
            {
                g.FillRectangle(SystemBrushes.ControlDark, Width - arrowWidth - 5 - 2, 6, 2, Height - 12);

                if (new Rectangle(Width - arrowWidth - 5, 6, arrowWidth, Height - 12).Contains(mousePos))
                    hoveredArrow = HoveredArrow.RIGHT;

                g.TranslateTransform(Width - arrowWidth - 5, 10);
                g.FillPolygon(hoveredArrow == HoveredArrow.RIGHT ? SystemBrushes.ControlDark : Framework.backBrush, arrowRight);
                g.ResetTransform();
            }

            g.DrawRectangle(new Pen(SystemBrushes.ControlDarkDark, 2), 0, 0, Width, Height);
        }

        enum HoveredArrow
        {
            NONE,
            LEFT,
            RIGHT,
        }

        HoveredArrow hoveredArrow = HoveredArrow.NONE;

        const int arrowWidth = 20;

        int scrollIndexOffset = 0;

        bool canScrollRight = false;

        Point mousePos = new Point(-1,-1);

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            mousePos = e.Location;

            Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (hoveredIndex != -1)
            {
                if (hoveringOverClose > -1)
                {
                    QuickFavoriteClosingEventArgs args = new QuickFavoriteClosingEventArgs(favorites[hoveredIndex]);

                    FavoriteClosing?.Invoke(this, args);
                    RemoveFavorite(hoveredIndex);
                }
                else
                {
                    Select(hoveredIndex);
                }

                Invalidate();
            }

            if(hoveredArrow==HoveredArrow.LEFT)
            {
                scrollIndexOffset--;
                Refresh();
            }
            else if (hoveredArrow == HoveredArrow.RIGHT)
            {
                scrollIndexOffset++;
                Refresh();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (e.Delta > 0)
            {
                if (scrollIndexOffset>0)
                {
                    scrollIndexOffset--;
                    Refresh();
                }
            }
            else if (e.Delta<0)
            {
                if (canScrollRight)
                {
                    scrollIndexOffset++;
                    Refresh();
                }
            }
        }
    }
}
