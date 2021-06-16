using GL_EditorFramework;
using GL_EditorFramework.EditorDrawables;
using Spotlight.EditorDrawables;
using Spotlight.Level;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GL_EditorFramework.Framework;

namespace Spotlight.GUI
{
    public partial class SceneListView3dWorld : UserControl
    {
        public SM3DWorldScene Scene
        {
            get => TreeView.Scene;
            set
            {
                TreeView.Scene = value;

                if(value==null)
                {
                    UnselectCurrentList();
                    return;
                }

                SelectedItems = Scene.SelectedObjects;

                FilterTextBox.Text = string.Empty;
            }
        }

        private readonly Stack<IList> listStack = new Stack<IList>();

        public event EventHandler SelectionChanged;

        public event ListEventHandler ListExited;
        public event ItemClickedEventHandler ItemClicked;

        public SceneListView3dWorld()
        {
            InitializeComponent();

            ItemsListView.SelectionChanged += ListView_SelectionChanged;
            ItemsListView.ItemClicked += (x, y) => ItemClicked?.Invoke(x, y);

            TreeView.SelectionChanged += (x, y) => SelectionChanged?.Invoke(x, y);
            TreeView.ItemClicked += (x, y) => ItemClicked?.Invoke(x, y);

            DoubleBuffered = true;

            SetStyle(
        ControlStyles.AllPaintingInWmPaint |
        ControlStyles.UserPaint |
        ControlStyles.OptimizedDoubleBuffer,
        true);


        }

        /// <summary>
        /// The set used to determine which objects are selected
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ISet<object> SelectedItems
        {
            get => ItemsListView.SelectedItems;
            set
            {
                ItemsListView.SelectedItems = value;
            }
        }

        /// <summary>
        /// The current list in the list view
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList CurrentList
        {
            get => TreeView.Visible ? null : ItemsListView.CurrentList;
        }

        /// <summary>
        /// Views a new list and adds the current one to the Stack
        /// </summary>
        /// <param name="list">the list to be entered</param>
        public void EnterList(IList list)
        {
            if (listStack == null || list == CurrentList)
                return;

            if (ItemsListView.CurrentList != null)
                listStack.Push(ItemsListView.CurrentList);
            ItemsListView.CurrentList = list;

            TreeView.Visible = false;
            FilterTextBox.Visible = false;
            BackButton.Visible = true;
        }

        public void UnselectCurrentList()
        {
            ItemsListView.CurrentList = null;

            listStack.Clear();

            TreeView.Visible = true;
            FilterTextBox.Visible = true;
            BackButton.Visible = false;

        }

        /// <summary>
        /// Tries to go back to the last list in the Stack
        /// </summary>
        public void ExitList()
        {
            if (listStack.Count != 0)
                ItemsListView.CurrentList = listStack.Pop();

            if (listStack.Count == 0)
            {
                TreeView.Visible = true;
                FilterTextBox.Visible = true;
                BackButton.Visible = false;
            }
        }

        public void InvalidateCurrentList()
        {
            ItemsListView.CurrentList = null;
        }

        public bool TryEnsureVisible(object item)
        {
            if (listStack.Count == 0)
            {
                TreeView.TryEnsureVisible(item);
            }
            else
            {
                int index = CurrentList.IndexOf(item);

                if (index != -1)
                {
                    ItemsListView.EnsureVisisble(index);
                }
            }

            return false;
        }

        public void ExpandNode(object node)
        {
            TreeView.ExpandNode(node);
        }

        public void CollapseNode(object node)
        {
            TreeView.CollapseNode(node);
        }

        public void CollapseAllNodes()
        {
            TreeView.CollapseAllNodes();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            ExitList();
            ListEventArgs args = new ListEventArgs(ItemsListView.CurrentList);
            ListExited?.Invoke(this, args);
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Scene == null)
                return;

            //apply selection changes to scene
            if (e.SelectionChangeMode == SelectionChangeMode.SET)
            {
                Scene.SelectedObjects.Clear();

                foreach (ISelectable obj in e.Items)
                    obj.SelectDefault(Scene.GL_Control);
            }
            else if (e.SelectionChangeMode == SelectionChangeMode.ADD)
            {
                foreach (ISelectable obj in e.Items)
                    obj.SelectDefault(Scene.GL_Control);
            }
            else //SelectionChangeMode.SUBTRACT
            {
                foreach (ISelectable obj in e.Items)
                    obj.DeselectAll(Scene.GL_Control);
            }

            e.Handled = true;

            SelectionChanged?.Invoke(this, e);
        }

        public override void Refresh()
        {
            if (TreeView.Visible)
                TreeView.Refresh();
            else
                ItemsListView.Refresh();


            base.Refresh();
        }

        private void FilterTextBox_TextChanged(object sender, EventArgs e)
        {
            string filterBefore = TreeView.FilterString;

            object lastSelected = TreeView.LastSelectedItem;
            TreeView.FilterString = FilterTextBox.Text;

            if (string.IsNullOrEmpty(filterBefore))
                TreeView.AutoScrollPosition = new Point(TreeView.AutoScrollPosition.X,0);
            else
                TreeView.TryEnsureVisible(lastSelected);

            TreeView.Refresh();
        }
    }


    public class SceneTreeView3dWorld : FastListViewBase
    {
        class SuperSet : ISet<object>
        {
            #region not implemented
            public int Count => throw new NotImplementedException();
            public bool IsReadOnly => throw new NotImplementedException();
            public void CopyTo(object[] array, int arrayIndex){throw new NotImplementedException();}
            public void ExceptWith(IEnumerable<object> other){throw new NotImplementedException();}
            public IEnumerator<object> GetEnumerator(){throw new NotImplementedException();}
            public void IntersectWith(IEnumerable<object> other){throw new NotImplementedException();}
            public bool IsProperSubsetOf(IEnumerable<object> other){throw new NotImplementedException();}
            public bool IsProperSupersetOf(IEnumerable<object> other){throw new NotImplementedException();}
            public bool IsSubsetOf(IEnumerable<object> other){throw new NotImplementedException();}
            public bool IsSupersetOf(IEnumerable<object> other){throw new NotImplementedException();}
            public bool Overlaps(IEnumerable<object> other){throw new NotImplementedException();}
            public bool SetEquals(IEnumerable<object> other){throw new NotImplementedException();}
            public void SymmetricExceptWith(IEnumerable<object> other){throw new NotImplementedException();}
            public void UnionWith(IEnumerable<object> other){throw new NotImplementedException();}
            void ICollection<object>.Add(object item){throw new NotImplementedException();}
            IEnumerator IEnumerable.GetEnumerator(){throw new NotImplementedException();}
            #endregion

            public bool Add(object item)
            {
                return true;
            }
            public bool Remove(object item)
            { 
                return false; 
            }

            public void Clear()
            {
                
            }

            public bool Contains(object item)
            {
                return true;
            }
        }

        Regex filter { get; set; } = new Regex(string.Empty);

        string filterString = string.Empty;
        public string FilterString
        {
            get => filterString;
            set
            {
                filterString = value;
                try
                {
                    filter = new Regex(value, RegexOptions.IgnoreCase);
                }
                catch(Exception) { }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SM3DWorldScene Scene { get; set; }

        ISet<object> expandedNodes = new HashSet<object>();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        public void ExpandNode(object node)
        {
            expandedNodes.Add(node);
        }

        public void CollapseNode(object node)
        {
            expandedNodes.Remove(node);
        }

        public void CollapseAllNodes()
        {
            expandedNodes.Clear();
        }

        protected sealed class TreeHandler
        {
            void Dummy_Action() { }
            void Dummy_TreeNodeHandler(bool isExpandable, Image image, string text, object item, bool isSelected, TreeNodeSelectionHandler selectionHandler) { }

            public readonly Action EnterBranch;
            public readonly Action ExitBranch;

            public delegate void TreeNodeHandler(bool isExpandable, Image image, string text, object item, bool isSelected, TreeNodeSelectionHandler selectionHandler);

            public delegate void TreeNodeSelectionHandler(SelectionChangeMode selectionChangeMode, int rangeStartOffset, int rangeEndOffset);

            public readonly TreeNodeHandler TreeNode;

            public TreeHandler(TreeNodeHandler delegate_TreeNode, Action delegate_EnterBranch = null, Action delegate_ExitBranch = null)
            {
                EnterBranch = delegate_EnterBranch ?? Dummy_Action;
                ExitBranch = delegate_ExitBranch ?? Dummy_Action;
                TreeNode = delegate_TreeNode ?? Dummy_TreeNodeHandler;
            }
        }



        protected static (Brush font, Brush back)[] highlightBrushes = new (Brush font, Brush back)[]
        {
            (SystemBrushes.ControlText, SystemBrushes.ControlLightLight), //NONE
            (SystemBrushes.HighlightText, SystemBrushes.Highlight), //SELECTED
            (SystemBrushes.ControlText, SystemBrushes.ControlLightLight), //HOVERED
            (SystemBrushes.HighlightText, SystemBrushes.Highlight), //HOVERED_SELECTED
        };

        (string name, string prefix)[] categories = new (string name, string prefix)[]
        {
            ("Map", SM3DWorldZone.MAP_PREFIX),
            ("Design", SM3DWorldZone.DESIGN_PREFIX),
            ("Sound", SM3DWorldZone.SOUND_PREFIX),
            ("Common", "Common_")
        };

        public SceneTreeView3dWorld()
        {

        }

        static readonly SuperSet superSet = new SuperSet();

        protected virtual void OnTree(TreeHandler tree)
        {
            GL_EditorFramework.EditorDrawables.IEditableObject _obj;
            Rail _rail;

            void HandleSelectionObject(SelectionChangeMode selectionChangeMode, int rangeStartOffset, int rangeEndOffset)
            {
                if (rangeStartOffset > 0 || rangeEndOffset < 0)
                    return;

                if (selectionChangeMode == SelectionChangeMode.SUBTRACT)
                    _obj.DeselectAll(Scene.GL_Control);
                else
                    _obj.SelectDefault(Scene.GL_Control);
            }

            void HandleSelectionRail(SelectionChangeMode selectionChangeMode, int rangeStartOffset, int rangeEndOffset)
            {
                bool expanded = expandedNodes.Contains(_rail);

                if (rangeEndOffset < 0 || rangeStartOffset > (expanded ? _rail.PathPoints.Count : 0))
                    return;

                if (rangeEndOffset == 0 || !expanded)
                {
                    _rail.SelectAll(Scene.GL_Control);
                    return;
                }

                int pointRangeMin = Math.Max(1, rangeStartOffset) - 1;
                int pointRangeMax = Math.Min(_rail.PathPoints.Count, rangeEndOffset) - 1;


                int i = 0;
                foreach (var point in _rail.PathPoints)
                {
                    if (i < pointRangeMin || i > pointRangeMax)
                    {
                        i++;
                        continue;
                    }

                    if (selectionChangeMode == SelectionChangeMode.SUBTRACT)
                        point.DeselectAll(Scene.GL_Control);
                    else
                        point.SelectDefault(Scene.GL_Control);

                    i++;
                }
            }

            void HandleSelectionDummy(SelectionChangeMode selectionChangeMode, int rangeStartOffset, int rangeEndOffset) { }


            if (Scene == null) return;

            var bak = expandedNodes;

            if(!string.IsNullOrEmpty(filterString))
                expandedNodes = superSet;

            foreach (var (categoryName, prefix) in categories)
            {
                tree.TreeNode(true, null, categoryName, prefix, false, null);

                if (!expandedNodes.Contains(prefix))
                    continue;

                tree.EnterBranch();

                foreach (var (name, list) in Scene.EditZone.ObjLists.Append(new KeyValuePair<string, ObjectList>("Common_Linked", Scene.EditZone.LinkedObjects)))
                {
                    if (!name.StartsWith(prefix))
                        continue;

                    string _name = name.Substring(prefix.Length);

                    if (list.Count == 0)
                        _name += " (Empty)"; //TODO localize

                    tree.TreeNode(true, null, _name, list, false, null);

                    if (!expandedNodes.Contains(list))
                        continue;

                    tree.EnterBranch();


                    foreach (I3dWorldObject obj in list)
                    {
                        if (!filter.IsMatch(obj.ToString()))
                            continue;

                        _obj = obj;

                        if (obj is Rail rail)
                        {
                            _rail = rail;
                            tree.TreeNode(true, null, obj.ToString(), obj, obj.IsSelected(), HandleSelectionRail);

                            if (!expandedNodes.Contains(obj))
                                continue;

                            tree.EnterBranch();

                            foreach (var point in rail.PathPoints)
                            {
                                tree.TreeNode(false, null, point.ToString(), point, point.IsSelected(), HandleSelectionDummy);
                            }

                            tree.ExitBranch();
                        }
                        else
                        {
                            tree.TreeNode(false, null, obj.ToString(), obj, obj.IsSelected(), HandleSelectionObject);
                        }

                    }

                    tree.ExitBranch();
                }

                tree.ExitBranch();
            }


            var zonePlacements = Scene.EditZone.ZonePlacements;

            tree.TreeNode(true, null, "Zones", zonePlacements, false, null);

            if (!expandedNodes.Contains(zonePlacements))
                return;

            tree.EnterBranch();

            foreach (var placement in zonePlacements)
            {
                _obj = placement;
                tree.TreeNode(false, null, placement.ToString(), placement, placement.IsSelected(), HandleSelectionObject);
            }

            tree.ExitBranch();

            expandedNodes = bak;
        }

        protected override void DrawItems(DrawItemHandler handler)
        {
            var g = handler.graphics;

            int _indentation = 16 + 1;

            int y = AutoScrollPosition.Y;

            Stack<int> lineStartY_PerLevel = new Stack<int>();

            lineStartY_PerLevel.Push(-1);

            bool isFirst = true;

            void EnterBranch()
            {
                lineStartY_PerLevel.Push(y);

                isFirst = true;
            }

            void ExitBranch()
            {
                lineStartY_PerLevel.Pop();

                isFirst = false;
            }

            void TreeNode(bool isExpandable, Image image, string text, object item, bool isSelected, TreeHandler.TreeNodeSelectionHandler selectionHandler)
            {
                int indentation = _indentation * lineStartY_PerLevel.Count;

                int lineBranchInd = indentation - _indentation / 2;

                int indMinusOne = indentation - _indentation;

                if (isFirst)
                {
                    g.DrawLine(SystemPens.ControlDark, lineBranchInd, y, lineBranchInd, y + FontHeight / 2);

                    lineStartY_PerLevel.Pop();
                }
                else
                {
                    int lineStartY = lineStartY_PerLevel.Pop();

                    g.DrawLine(SystemPens.ControlDark, lineBranchInd, lineStartY, lineBranchInd, y + FontHeight / 2);
                }

                lineStartY_PerLevel.Push(y + FontHeight / 2 + (isExpandable ? 4 : 0));

                g.DrawLine(SystemPens.ControlDark, lineBranchInd, y + FontHeight / 2, indentation, y + FontHeight / 2);

                if (isExpandable)
                    DrawExpansionGlyphManual(g, new Rectangle(indMinusOne, y, _indentation, FontHeight), expandedNodes.Contains(item));

                Brush font, back;

                if (selectionHandler != null)
                    (font, back) = highlightBrushes[(int)handler.HandleItem(item, isSelected, y, FontHeight, indentation)];
                else
                {
                    handler.HandleItem(item, isSelected, y, FontHeight, int.MaxValue);
                    (font, back) = highlightBrushes[0];
                }



                g.FillRectangle(back, indentation, y, Width, FontHeight);
                g.DrawString(text, Font, font, indentation, y);

                y += FontHeight;



                isFirst = false;
            }

            OnTree(new TreeHandler(TreeNode, EnterBranch, ExitBranch));
        }

        //from https://github.com/geomatics-io/ObjectListView/blob/master/ObjectListView/Rendering/TreeRenderer.cs
        protected static void DrawExpansionGlyphManual(Graphics g, Rectangle r, bool isExpanded)
        {
            int h = 8;
            int w = 8;
            int x = r.X + 4;
            int y = r.Y + (r.Height / 2) - 4;

            g.DrawRectangle(new Pen(SystemBrushes.ControlDark), x, y, w, h);
            g.FillRectangle(Brushes.White, x + 1, y + 1, w - 1, h - 1);
            g.DrawLine(Pens.Black, x + 2, y + 4, x + w - 2, y + 4);

            if (!isExpanded)
                g.DrawLine(Pens.Black, x + 4, y + 2, x + 4, y + h - 2);
        }

        protected override object Select(int rangeMin, int rangeMax, SelectionChangeMode selectionChangeMode)
        {
            int _rangeMin = rangeMin;
            int _rangeMax = rangeMax;

            object lastSelected = null;

            if (selectionChangeMode == SelectionChangeMode.SET)
                Scene.SelectedObjects.Clear();

            void TreeNode(bool isExpandable, Image image, string text, object item, bool isSelected, TreeHandler.TreeNodeSelectionHandler selectionHandler)
            {
                selectionHandler?.Invoke(selectionChangeMode, _rangeMin, _rangeMax);

                if (_rangeMax == 0)
                    lastSelected = item;

                _rangeMin--;
                _rangeMax--;
            }

            OnTree(new TreeHandler(TreeNode));

            //TODO return last selected
            return lastSelected;
        }

        //TODO
        protected override object SelectNext(string searchString, int startIndex)
        {
            return null;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            int _indentation = 16 + 1;

            int y = AutoScrollPosition.Y;

            int indentationLevel = 1;

            void EnterBranch()
            {
                indentationLevel++;
            }

            void ExitBranch()
            {
                indentationLevel--;
            }

            void TreeNode(bool isExpandable, Image image, string text, object item, bool isSelected, TreeHandler.TreeNodeSelectionHandler selectionHandler)
            {
                if (!isExpandable)
                {
                    y += FontHeight;
                    return;
                }

                int indentation = _indentation * indentationLevel;

                int indMinusOne = indentation - _indentation;


                if (selectionHandler == null)
                {
                    if (new Rectangle(indMinusOne, y, Width, FontHeight).Contains(e.Location))
                    {
                        if (expandedNodes.Contains(item))
                            expandedNodes.Remove(item);
                        else
                            expandedNodes.Add(item);

                        Invalidate();
                    }
                }
                else
                {
                    if (new Rectangle(indMinusOne, y, _indentation, FontHeight).Contains(e.Location))
                    {
                        if (expandedNodes.Contains(item))
                            expandedNodes.Remove(item);
                        else
                            expandedNodes.Add(item);

                        Invalidate();
                    }
                }

                y += FontHeight;
            }

            OnTree(new TreeHandler(TreeNode, EnterBranch, ExitBranch));
        }


        class ItemFoundException : Exception
        {

        }

        public bool TryEnsureVisible(object item)
        {
            int index = 0;

            Stack<int> lastCorrectIndices = new Stack<int>();

            object lastExpandedNode = null;

            Stack<object> nodesToCollapse = new Stack<object>();

            void EnterBranch()
            {
                if (lastExpandedNode != null)
                {
                    nodesToCollapse.Push(lastExpandedNode);
                    lastCorrectIndices.Push(index);
                    lastExpandedNode = null;
                }
            }

            void ExitBranch()
            {
                if (nodesToCollapse.Count != 0)
                {
                    index = lastCorrectIndices.Pop();
                    expandedNodes.Remove(nodesToCollapse.Pop());
                }
            }

            void TreeNode(bool isExpandable, Image image, string text, object _item, bool isSelected, TreeHandler.TreeNodeSelectionHandler selectionHandler)
            {
                if (item == _item)
                    throw (new ItemFoundException());

                if (isExpandable && !expandedNodes.Contains(_item))
                {
                    expandedNodes.Add(_item);
                    lastExpandedNode = _item;
                }
                index++;
            }

            var bak = expandedNodes;

            try
            {
                OnTree(new TreeHandler(TreeNode, EnterBranch, ExitBranch));

                return false;
            }
            catch (ItemFoundException)
            {
                int y = AutoScrollPosition.Y + index * FontHeight;

                Refresh();

                if (y < 0)
                    AutoScrollPosition = new Point(0, index * FontHeight);
                else if (y > Height - FontHeight)
                    AutoScrollPosition = new Point(0, index * FontHeight - Height + FontHeight);

                return true;
            }
            finally
            {
                expandedNodes = bak;
            }
        }
    }
}
