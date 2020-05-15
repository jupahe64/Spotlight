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
using static SpotLight.EditorDrawables.SM3DWorldScene;

namespace SpotLight
{
    public class QuickFavoriteClosingEventArgs : EventArgs
    {
        public QuickFavoriteControl.QuickFavorite Favorite { get; set; }

        public QuickFavoriteClosingEventArgs(QuickFavoriteControl.QuickFavorite tab)
        {
            Favorite = tab;
        }
    }

    public delegate void QuickFavoriteClosingEventHandler(object sender, QuickFavoriteClosingEventArgs e);


    public class QuickFavoriteControl : UserControl
    {
        List<QuickFavorite> favorites = new List<QuickFavorite>();

        int selectedIndex = -1;

        int hoveredIndex = -1;

        bool hoveringOverClose = false;

        Graphics g;

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

        public void Select(QuickFavorite tab)
        {
            int index = favorites.IndexOf(tab);
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

        public void AddFavorite(QuickFavorite tab)
        {
            favorites.Add(tab);

            Invalidate();
        }

        public IReadOnlyList<QuickFavorite> Favorites => favorites;

        public Font HeaderFont { get; private set; } = new Font(SystemFonts.DefaultFont, FontStyle.Bold);

        public void InsertFavorite(int index, QuickFavorite tab, bool select)
        {
            if (index < 0 || index > favorites.Count)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be within the bounds of "+nameof(Favorites));

            favorites.Insert(index, tab);

            if (select)
            {
                selectedIndex = index;
                SelectedFavoriteChanged?.Invoke(this, new EventArgs());
            }

            Invalidate();
        }

        public void RemoveFavorite(QuickFavorite tab)
        {
            int index = favorites.IndexOf(tab);
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

        static char[] newLineSeperator = new char[] { '\n' };

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            g = e.Graphics;

            g.Clear(SystemColors.ControlLightLight);

            hoveredIndex = -1;

            hoveringOverClose = false;

            int x = arrowWidth + 5;

            Rectangle favoritesArea = new Rectangle(arrowWidth + 5, 0, Width - 10 - arrowWidth * 2, Height);

            g.SetClip(favoritesArea);

            for (int i = scrollIndexOffset; i < favorites.Count; i++)
            {
                int width = (int)Math.Ceiling(g.MeasureString(favorites[i].Name, Font).Width);

                if (i == selectedIndex)
                    g.FillRectangle(SystemBrushes.Highlight, x, 0, width + 25, Height);
                else
                    g.FillRectangle(SystemBrushes.ControlDark, x, 0, width + 25, Height);

                g.FillRectangle(SystemBrushes.ControlLightLight, x + 1, 1, width + 23, Height - 2);

                string[] parts = favorites[i].Name.Split(newLineSeperator, 2);

                if (i == selectedIndex)
                    g.DrawString(parts[0], Font, SystemBrushes.Highlight, x + 5, 2);
                else
                    g.DrawString(parts[0], Font, SystemBrushes.ControlText, x + 5, 2);

                if (parts.Length>1)
                    g.DrawString(parts[1], Font, SystemBrushes.ControlDarkDark, x + 5, HeaderFont.Height + 4);

                if (favoritesArea.Contains(mousePos))
                {
                    if (mousePos.X > x && mousePos.X < x + width + 25)
                    {
                        hoveredIndex = i;

                        if (mousePos.X > x + width + 13)
                        {
                            hoveringOverClose = true;

                            g.DrawImage(Resources.CloseTabIconHover, x + width + 13, 4);
                            goto ICON_HOVERED;
                        }
                    }
                }

                g.DrawImage(Resources.CloseTabIcon, x + width + 13, 4);

                ICON_HOVERED:
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
                g.FillRectangle(SystemBrushes.ControlDark, Width - arrowWidth - 5 - 2, 1, 2, Height - 2);

                if (new Rectangle(Width - arrowWidth - 5, 1, arrowWidth, Height - 2).Contains(mousePos))
                    hoveredArrow = HoveredArrow.RIGHT;

                g.TranslateTransform(Width - arrowWidth - 5, 10);
                g.FillPolygon(hoveredArrow == HoveredArrow.RIGHT ? SystemBrushes.ControlDark : Framework.backBrush, arrowRight);
                g.ResetTransform();
            }

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
                if (hoveringOverClose)
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
