using GL_EditorFramework;
using Spotlight.Level;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spotlight.GUI
{
    class LayerListControl : FlexibleUIControl
    {
        private SM3DWorldZone zone;


        private int scenario = 0;
        private Brush shadowBrush = new SolidBrush(Color.FromArgb(20,0,0,0));

        public event EventHandler ScenarioConfigChanged;

        public void SetZone(SM3DWorldZone zone)
        {
            this.zone = zone;

            BackColor = SystemColors.ControlLightLight;
        }

        public void SetScenario(int scenario)
        {
            this.scenario = scenario;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!TryInitDrawing(e))
                return;

            if (zone == null)
                return;

            int checkX = usableWidth-textBoxHeight*15;

#if !ODYSSEY
            checkX = usableWidth; //get it out of the way
#endif

            int y = 10 + textBoxHeight + AutoScrollPosition.Y;

            bool changed = false;

            foreach (var layer in zone.availibleLayers)
            {
                var newLayerName = EditableText(10, y+1, checkX - 10, layer.Name, false, (SolidBrush)SystemBrushes.ControlLightLight);

                if(layer.Name != newLayerName && !zone.availibleLayers.Any(x=>x.Name==newLayerName))
                {
                    Console.WriteLine($"Change layer {layer} to {newLayerName}");

                    zone.RenameLayer(layer, newLayerName);
                }

                g.FillRectangle(shadowBrush, 10, y + textBoxHeight, Width, 1);

#if ODYSSEY
                for (int i = 0; i < 15; i++)
                {
                    Rectangle rect = new Rectangle(checkX + i * textBoxHeight, y, textBoxHeight, textBoxHeight);

                    bool hovered = rect.Contains(mousePos);

                    Brush outline = hovered ? SystemBrushes.Highlight : SystemBrushes.ActiveBorder;

                    bool isChecked = zone.activeLayersPerScenario[i].Contains(layer);

                    DrawField(checkX + i* textBoxHeight, y, textBoxHeight, 
                        isChecked ? "x" : "",
                        outline, i==scenario ? Brushes.AliceBlue : SystemBrushes.ControlLightLight);

                    if(hovered && eventType == EventType.CLICK)
                    {
                        if(isChecked)
                            zone.activeLayersPerScenario[i].Remove(layer);
                        else
                            zone.activeLayersPerScenario[i].Add(layer);

                        changed = true;
                    }
                }
#endif

                y += textBoxHeight;
            }

            


            g.FillRectangle(shadowBrush, 0, 0, Width, 10 + textBoxHeight + 1);
            g.FillRectangle(shadowBrush, 0, 0, Width, 10 + textBoxHeight + 2);
            g.FillRectangle(Brushes.White, 0, 0, Width, 10 + textBoxHeight);

#if ODYSSEY
            for (int i = 0; i < 15; i++)
            {
                int x = checkX + i * textBoxHeight;

                if (i == scenario)
                {
                    g.FillRectangle(Brushes.RoyalBlue, x, 0, textBoxHeight, 10 + textBoxHeight);
                }

                string scenarioName = (i + 1).ToString();

                var textWidth = g.MeasureString(scenarioName, Font).Width;

                g.DrawString(scenarioName, Font, i == scenario ? SystemBrushes.HighlightText : SystemBrushes.ControlText, 
                    x + textBoxHeight/2-textWidth/2, 10);
            }
#endif


            AutoScrollMinSize = new Size(0, y - AutoScrollPosition.Y + 10);

            if (changed)
                ScenarioConfigChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);

            Refresh();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            Refresh();
        }
    }
}
