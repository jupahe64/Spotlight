using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpotLight
{
    public partial class AddObjectForm
    {
        //I have an unhealthy obsession with anonymous function and delegates...
        public static void SetupSearchLogic(ListView listView, TextBox searchBox, ListViewItem[] items, Action<ListViewItem> selectHandler)
        {
            ListViewItem[] filtered = items;

            listView.VirtualMode = true;
            
            listView.VirtualListSize = items.Length;


            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = filtered[e.ItemIndex];
            };

            listView.SelectedIndexChanged += (s, e) =>
            {
                if (listView.SelectedIndices.Count == 1)
                    selectHandler(filtered[listView.SelectedIndices[0]]);
            };

            searchBox.KeyUp += (s, e) =>
            {
                string searchString = searchBox.Text;
                if(string.IsNullOrEmpty(searchString))
                    filtered = items;
                else
                {
                    List<ListViewItem> filteredItems = new List<ListViewItem>();
                    for (int i = 0; i < items.Length; i++)
                    {
                        int index = items[i].Text.IndexOf(searchString, StringComparison.OrdinalIgnoreCase);

                        if (index!=-1)
                            filteredItems.Add(items[i]);
                    }
                    filtered = filteredItems.ToArray();
                }
                listView.VirtualListSize = filtered.Length;
            };
        }

        public static void SetupSearchLogicWithEnglishNames(ListView listView, TextBox searchBox, ListViewItem[] items, TextBox englishNameInput, Action<ListViewItem> selectHandler)
        {
            ListViewItem selectedItem = null;
            ListViewItem[] filtered = items;

            listView.VirtualMode = true;

            listView.VirtualListSize = items.Length;


            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = filtered[e.ItemIndex];
            };

            listView.SelectedIndexChanged += (s, e) =>
            {
                if (listView.SelectedIndices.Count == 1)
                {
                    selectedItem = filtered[listView.SelectedIndices[0]];
                    selectHandler(selectedItem);
                }
            };

            searchBox.KeyUp += (s, e) =>
            {
                string searchString = searchBox.Text;
                if (string.IsNullOrEmpty(searchString))
                    filtered = items;
                else
                {
                    List<ListViewItem> filteredItems = new List<ListViewItem>();
                    for (int i = 0; i < items.Length; i++)
                    {
                        int indexA = items[i].Text.IndexOf(searchString, StringComparison.OrdinalIgnoreCase);
                        int indexB = items[i].SubItems[1].Text.IndexOf(searchString, StringComparison.OrdinalIgnoreCase);

                        if (indexA != -1 || indexB != -1)
                            filteredItems.Add(items[i]);
                    }
                    filtered = filteredItems.ToArray();
                }
                listView.VirtualListSize = filtered.Length;
            };

            if (englishNameInput == null)
                return;

            englishNameInput.TextChanged += (s, e) =>
            {
                selectedItem.SubItems[1].Text = englishNameInput.Text;
            };
        }
    }
}
