using GL_EditorFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spotlight.GUI
{
    public partial class PathShapeSelector : UserControl
    {
        List<PathShape> shapes = new List<PathShape>();

        int selectedIndex = -1;

        int hoveredIndex = -1;

        Graphics g;

        public override string Text { get => base.Text; set => base.Text = value; }

        public event EventHandler SelectedShapeChanged;

        public class PathShape
        {
            public readonly string Name;
            public readonly GraphicsPath GraphicsPath;
            public readonly PathPoint[] PathPoints;
            public readonly bool Closed;

            public PathShape(string name, PathPoint[] pathPoints, bool closed)
            {
                Name = name;
                PathPoints = pathPoints;
                Closed = closed;

                GraphicsPath = new GraphicsPath();

                void Bezier(int i1, int i2)
                {
                    var p1 = pathPoints[i1].Position;
                    var p2 = pathPoints[i1].Position + pathPoints[i1].ControlPoint2;

                    var p4 = pathPoints[i2].Position;
                    var p3 = pathPoints[i2].Position + pathPoints[i2].ControlPoint1;

                    GraphicsPath.AddBezier(
                         p1.X, p1.Z,
                         p2.X, p2.Z,
                         p3.X, p3.Z,
                         p4.X, p4.Z
                        );
                }

                for (int i = 0; i < pathPoints.Length-1; i++)
                {
                    Bezier(i, i + 1);
                }

                if (Closed)
                    Bezier(pathPoints.Length - 1, 0);
            }
        }

        public PathShapeSelector()
        {
            SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer,
            true);

            BorderStyle = BorderStyle.None;
        }

        public PathShape SelectedShape
        {
            get
            {
                if (selectedIndex != -1)
                    return shapes[selectedIndex];
                else
                    return null;
            }
        }

        public void Deselect()
        {
            if (selectedIndex != -1)
            {
                selectedIndex = -1;
                SelectedShapeChanged?.Invoke(this, new EventArgs());
            }

            Invalidate();
        }

        public void Select(PathShape shape)
        {
            int index = shapes.IndexOf(shape);
            if (index != -1)
                Select(index);
        }

        public void Select(int index)
        {
            if (index < 0 || index > shapes.Count)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be within the bounds of " + nameof(Shapes));

            if (selectedIndex != index)
            {
                selectedIndex = index;
                SelectedShapeChanged?.Invoke(this, new EventArgs());
            }

            Invalidate();
        }

        public void AddShape(PathShape shape)
        {
            shapes.Add(shape);

            Invalidate();
        }

        public IReadOnlyList<PathShape> Shapes => shapes;

        public Font HeaderFont { get; private set; } = new Font(SystemFonts.DefaultFont, FontStyle.Bold);

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

            Rectangle shapesArea = new Rectangle(arrowWidth + 5, 0, Width - 10 - arrowWidth * 2, Height);
            g.SetClip(shapesArea);

            if (shapes.Count == 0)
                g.DrawString("No Shapes availible", Font, SystemBrushes.ControlDarkDark, Width / 2, Height / 2, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            hoveredIndex = -1;
            for (int i = scrollIndexOffset; i < shapes.Count; i++)
            {
                var p = shapes[i].GraphicsPath;

                var bounds = p.GetBounds();

                if(bounds.Height==0)
                    bounds.Inflate(0, bounds.Width/2);


                bounds.Inflate(0.5f, 0.5f);

                var cx = (bounds.Left + bounds.Right) / 2;
                var cy = (bounds.Top + bounds.Bottom) / 2;

                var s = (Height - 4) / bounds.Height;

                int width = (int)Math.Ceiling(bounds.Width*s);

                

                if (shapesArea.Contains(mousePos) && (mousePos.X > x && mousePos.X < x + width + 4) && (mousePos.Y > 5 && mousePos.Y < Height - 5))
                    hoveredIndex = i;


                Brush shapeFill = i == selectedIndex ? SystemBrushes.Highlight : (hoveredIndex == i ? Brushes.Gray : Brushes.DarkGray);

                if (!Enabled)
                    shapeFill = SystemBrushes.ControlLight;

                //g.FillRectangle(i == selectedIndex ? SystemBrushes.Highlight : SystemBrushes.ControlDark, x, 1, width + 6, Height - 2);

                //g.FillRectangle(hoveredIndex == i ? SystemBrushes.ControlLight : SystemBrushes.ControlLightLight, x + 1, 2, width + 4, Height - 4);

                //g.DrawString(parts[0], Font, i == selectedIndex ? SystemBrushes.Highlight : SystemBrushes.ControlText, x + 5, 7);

                //place drawing
                g.TranslateTransform(x + (width + 6) / 2, Height / 2);

                //fit drawing
                g.ScaleTransform(s, s);

                //center drawing
                g.TranslateTransform(-cx, -cy);

                //draw drawing
                g.DrawPath(new Pen(shapeFill, 0.1f), p);


                g.FillRectangle(shapeFill, p.PathPoints[0].X - 0.5f, p.PathPoints[0].Y - 0.5f, 1f, 1f);

                for (int j = 1; j < p.PathPoints.Length; j+=3)
                {
                    g.FillRectangle(shapeFill, p.PathPoints [j+0].X - 0.25f, p.PathPoints[j+0].Y - 0.25f, 0.5f, 0.5f);
                    g.FillRectangle(shapeFill, p.PathPoints [j+1].X - 0.25f, p.PathPoints[j+1].Y - 0.25f, 0.5f, 0.5f);

                    g.FillRectangle(shapeFill, p.PathPoints [j+2].X - 0.5f,  p.PathPoints[j+2].Y - 0.5f, 1f, 1f);
                }

                g.DrawLine(new Pen(Brushes.Gray, 0.1f), 0, -0.5f, 0, 0.5f);
                g.DrawLine(new Pen(Brushes.Gray, 0.1f), -0.5f, 0, 0.5f, 0);

                g.ResetTransform();

                x += width + 10;
            }

            canScrollRight = x > Width - 5 - arrowWidth;

            g.ResetClip();

            hoveredArrow = HoveredArrow.NONE;

            if (scrollIndexOffset > 0)
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

        Point mousePos = new Point(-1, -1);

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
                Select(hoveredIndex);

                Invalidate();
            }

            if (hoveredArrow == HoveredArrow.LEFT)
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
                if (scrollIndexOffset > 0)
                {
                    scrollIndexOffset--;
                    Refresh();
                }
            }
            else if (e.Delta < 0)
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
