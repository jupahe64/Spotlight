﻿using GL_EditorFramework;
using GL_EditorFramework.EditorDrawables;
using OpenTK;
using SpotLight.EditorDrawables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GL_EditorFramework.Framework;

namespace SpotLight
{
    public partial class LevelEditorForm : Form
    {
        SM3DWorldLevel currenLevel;
        public LevelEditorForm()
        {
            InitializeComponent();
            
            gL_ControlModern1.CameraDistance = 20;
            gL_ControlModern1.KeyDown += GL_ControlModern1_KeyDown;
            
            sceneListView1.SelectionChanged += SceneListView1_SelectionChanged;
            sceneListView1.ItemsMoved += SceneListView1_ItemsMoved;

            
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if(SM3DWorldLevel.TryOpen(ofd.FileName, gL_ControlModern1, sceneListView1, out SM3DWorldLevel level))
                {
                    currenLevel = level;

                    level.scene.SelectionChanged += Scene_SelectionChanged;
                    level.scene.ListChanged += Scene_ListChanged;

                    sceneListView1.Enabled = true;
                    sceneListView1.SetRootList("ObjectList");
                    sceneListView1.ListExited += SceneListView1_ListExited;
                }


            }
        }

        private void SceneListView1_ListExited(object sender, ListEventArgs e)
        {
            currenLevel.scene.CurrentList = e.List;
            //fetch availible properties for list
            objectUIControl1.CurrentObjectUIProvider = currenLevel.scene.GetObjectUIProvider();
        }

        private void SceneListView1_ItemsMoved(object sender, ItemsMovedEventArgs e)
        {
            currenLevel.scene.ReorderObjects(sceneListView1.CurrentList, e.OriginalIndex, e.Count, e.Offset);
            e.Handled = true;
            gL_ControlModern1.Refresh();
        }

        private void Scene_ListChanged(object sender, GL_EditorFramework.EditorDrawables.ListChangedEventArgs e)
        {
            if (e.Lists.Contains(sceneListView1.CurrentList))
            {
                sceneListView1.UpdateAutoScrollHeight();
                sceneListView1.Refresh();
            }
        }

        private void SceneListView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currenLevel == null)
                return;

            foreach (object obj in e.ItemsToDeselect)
                currenLevel.scene.ToogleSelected((EditableObject)obj, false);

            foreach (object obj in e.ItemsToSelect)
                currenLevel.scene.ToogleSelected((EditableObject)obj, true);

            e.Handled = true;
            gL_ControlModern1.Refresh();

            Scene_SelectionChanged(this, null);
        }

        private void GL_ControlModern1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && currenLevel!=null)
            {
                currenLevel.scene.DeleteSelected();
                gL_ControlModern1.Refresh();
                sceneListView1.UpdateAutoScrollHeight();
                Scene_SelectionChanged(this, null);
            }
        }

        private void Scene_SelectionChanged(object sender, EventArgs e)
        {
            if (currenLevel.scene.SelectedObjects.Count > 1)
                lblCurrentObject.Text = "Multiple objects selected";
            else if (currenLevel.scene.SelectedObjects.Count == 0)
                lblCurrentObject.Text = "Nothing selected";
            else
                lblCurrentObject.Text = currenLevel.scene.SelectedObjects.First().ToString() + " selected";
            sceneListView1.Refresh();

            objectUIControl1.CurrentObjectUIProvider = currenLevel.scene.GetObjectUIProvider();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currenLevel == null)
                return;

            currenLevel.SaveAs();
        }

        private void SplitContainer2_Panel2_Click(object sender, EventArgs e)
        {
            Debugger.Break();
        }
    }
}