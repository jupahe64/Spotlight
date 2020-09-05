using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpotLight
{
    public class SuggestingTextBox : TextBox
    {
        protected SuggestionDropDown suggestionsDropDown = new SuggestionDropDown();
        readonly Control focusControl = new Label() { Size = new Size() };

        public event CancelEventHandler ValueEntered;

        public bool SuggestClear { get; set; } = false;

        public string[] PossibleSuggestions { get; set; } = Array.Empty<string>();

        public bool FilterSuggestions { get; set; } = true;

        public SuggestingTextBox()
        {
            suggestionsDropDown.ItemSelected += SuggestionsDropDown_ItemSelected;
        }

        private void SuggestionsDropDown_ItemSelected(object sender, EventArgs e)
        {
            Text = suggestionsDropDown.SelectedSuggestion;

            var args = new CancelEventArgs();
            ValueEntered?.Invoke(this, args);
            if (args.Cancel)
                ForeColor = Color.Red; //mark the value red to indicate it's invalid
            else
                ForeColor = SystemColors.ControlText;

            ignoreFocusChange = true;
            focusControl.Focus(); //because Microsoft forgot to put in Unfocus() smh
            suggestionsDropDown.Hide(); //because OnLostFocus won't get called
            ignoreFocusChange = false;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            ignoreFocusChange = true;

            if (e.KeyCode == Keys.Return && Focused)
            {
                var args = new CancelEventArgs();
                ValueEntered?.Invoke(this, args);
                if (!args.Cancel)
                {
                    ForeColor = SystemColors.ControlText;
                    focusControl.Focus(); //because Microsoft forgot to put in Unfocus() smh
                    suggestionsDropDown.Hide(); //because OnLostFocus won't get called
                    e.SuppressKeyPress = true;
                }
                else
                    ForeColor = Color.Red;
            }
            else
                base.OnKeyDown(e);

            ignoreFocusChange = false;
        }

        private bool ignoreFocusChange = false;

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (ignoreFocusChange)
                return;

            ignoreFocusChange = true;
            suggestionsDropDown.Show(PointToScreen(new Point(-2, Height-2)), Width, Text, PossibleSuggestions, SuggestClear, FilterSuggestions);
            ignoreFocusChange = false;
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            if (ignoreFocusChange)
                return;

            Form parentForm = this.FindForm();

            if (parentForm != null && parentForm.ContainsFocus) //another Control inside the parent form got focused, we can be very sure that this was intentional
            {
                var args = new CancelEventArgs();
                ValueEntered?.Invoke(this, args);
                if (args.Cancel)
                    ForeColor = Color.Red; //mark the value red to indicate it's invalid
                else
                    ForeColor = SystemColors.ControlText;
            }

            suggestionsDropDown.Hide();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            suggestionsDropDown.UpdateCurrentText(Text);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            Form parentForm = this.FindForm();

            if (parentForm != null)
            {
                parentForm.Controls.Add(focusControl);

                parentForm.LocationChanged += ParentForm_LocationChanged;
                parentForm.SizeChanged += ParentForm_SizeChanged;

                parentForm.FormClosed += ParentForm_FormClosed;
            }
        }

        private void ParentForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            suggestionsDropDown.Close();
        }

        private void ParentForm_SizeChanged(object sender, EventArgs e)
        {
            suggestionsDropDown.Width = Width;
        }

        private void ParentForm_LocationChanged(object sender, EventArgs e)
        {
            suggestionsDropDown.Location = PointToScreen(new Point(-2, Height - 2));
        }

        protected class SuggestionDropDown : Form
        {
            public SuggestionDropDown()
            {
                SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer,
            true);

                FormBorderStyle = FormBorderStyle.None;
                TopMost = true;
                BackColor = SystemColors.ControlLightLight;
                AutoScrollMinSize = new Size(0, 1000);
            }

            private string[] possibleSuggestions = Array.Empty<string>();

            private string[] suggestions = Array.Empty<string>();

            private bool suggestClear = false;

            private bool filterSuggestions = false;

            public void UpdateCurrentText(string currentText, bool allowKeepCurrentSuggestions = true)
            {
                if (filterSuggestions)
                {
                    List<string> suggestionList = new List<string>();

                    for (int i = 0; i < possibleSuggestions.Length; i++)
                    {
                        if (possibleSuggestions[i].StartsWith(currentText))
                            suggestionList.Add(possibleSuggestions[i]);
                    }
                    if (suggestionList.Count > 0)
                        suggestions = suggestionList.ToArray();
                    else if (!allowKeepCurrentSuggestions)
                        suggestions = possibleSuggestions;
                }
                else
                {
                    suggestions = possibleSuggestions;
                }

                int maxHeight = 8 * Font.Height;

                int desiredHeight = (suggestions.Length + (suggestClear ? 1 : 0))
                    * Font.Height;


                if (desiredHeight > maxHeight)
                {
                    AutoScrollMinSize = new Size(0, desiredHeight);
                    Height = maxHeight;
                }
                else
                {
                    AutoScrollMinSize = new Size(0, 0);
                    Height = desiredHeight;
                }

                Refresh();
            }

            #region make absolutely sure that this form can't be focused on
            private const int SW_SHOWNOACTIVATE = 4;
            private const int HWND_TOPMOST = -1;
            private const uint SWP_NOACTIVATE = 0x0010;

            [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
            static extern bool SetWindowPos(
                 int hWnd,             // Window handle
                 int hWndInsertAfter,  // Placement-order handle
                 int X,                // Horizontal position
                 int Y,                // Vertical position
                 int cx,               // Width
                 int cy,               // Height
                 uint uFlags);         // Window positioning flags

            [DllImport("user32.dll")]
            static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            static void ShowInactiveTopmost(Form frm)
            {
                ShowWindow(frm.Handle, SW_SHOWNOACTIVATE);
                SetWindowPos(frm.Handle.ToInt32(), HWND_TOPMOST,
                frm.Left, frm.Top, frm.Width, frm.Height,
                SWP_NOACTIVATE);
            }
            #endregion

            public void Show(Point location, int width, string currentText, string[] possibleSuggestions, bool suggestClear, bool filterSuggestions)
            {
                mouseY = -1;
                hoveredIndex = -2;

                this.filterSuggestions = filterSuggestions;
                this.possibleSuggestions = possibleSuggestions;
                this.suggestClear = suggestClear;
                Bounds = new Rectangle(location, new Size(width, 20));
                UpdateCurrentText(currentText, false);

                ShowInactiveTopmost(this);
            }

            int mouseY = -1;
            int hoveredIndex = -2;

            public string SelectedSuggestion { get; private set; }

            public event EventHandler ItemSelected;

            bool mouseDown = false;
            private const string CLEAR_STRING = "<Clear>";

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                e.Graphics.DrawRectangle(SystemPens.Highlight, new Rectangle(0, 0, Width - 1, Height - 1));

                int fontHeight = Font.Height;

                int y = AutoScrollPosition.Y;

                if (suggestClear)
                {
                    if (mouseY >= y && mouseY < y + fontHeight)
                    {
                        e.Graphics.FillRectangle(SystemBrushes.Highlight, new Rectangle(0, y, Width, fontHeight));
                        e.Graphics.DrawString(CLEAR_STRING, Font, SystemBrushes.HighlightText, new Point(0, y));
                        hoveredIndex = -1;
                    }
                    else
                        e.Graphics.DrawString(CLEAR_STRING, Font, SystemBrushes.ControlText, new Point(0, y));

                    y += fontHeight;
                }

                for (int i = 0; i < suggestions.Length; i++)
                {
                    if(mouseY >= y && mouseY < y+fontHeight)
                    {
                        e.Graphics.FillRectangle(SystemBrushes.Highlight, new Rectangle(0, y, Width, fontHeight));
                        e.Graphics.DrawString(suggestions[i], Font, SystemBrushes.HighlightText, new Point(0,y));
                        hoveredIndex = i;
                    }
                    else
                        e.Graphics.DrawString(suggestions[i], Font, SystemBrushes.ControlText, new Point(0, y));

                    y += fontHeight;
                }
            }

            protected override bool ShowWithoutActivation => true;

            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams baseParams = base.CreateParams;

                    const int WS_EX_NOACTIVATE = 0x08000000;
                    const int WS_EX_TOOLWINDOW = 0x00000080;
                    baseParams.ExStyle |= (int)(WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW);

                    if (Environment.OSVersion.Version.Major >= 6)
                        baseParams.ExStyle |= 0x02000000;

                    return baseParams;
                }
            }

            private const int WM_MOUSEACTIVATE = 0x0021, MA_NOACTIVATE = 0x0003;

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_MOUSEACTIVATE)
                {
                    m.Result = (IntPtr)MA_NOACTIVATE;
                    return;
                }
                base.WndProc(ref m);
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                mouseY = e.Y;
                Refresh();
            }

            protected override void OnScroll(ScrollEventArgs se)
            {
                base.OnScroll(se);

                mouseY -= se.NewValue - se.OldValue; //makes no sense but it makes sure that hoveredIndex doesn't change when scrolling
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);
                mouseDown = true;
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);
                if (mouseDown)
                {
                    if (hoveredIndex == -1)
                        SelectedSuggestion = string.Empty;
                    else
                        SelectedSuggestion = suggestions[hoveredIndex];

                    mouseDown = false;

                    Hide();
                    ItemSelected?.Invoke(this, null);
                }
            }
        }
    }
}
