using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Spotlight
{
    public class Language
    {
        /// <summary>
        /// Name of the language
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// Indexer = Name of the control that this entry belongs to<para/>
        /// Value = Translated text
        /// </summary>
        public Dictionary<string, string> Translations { get; internal set; } = new Dictionary<string, string>();

        public Language()
        {
            Name = "English";
        }
        public Language(string Name)
        {
            this.Name = Name;
        }
        public Language(string Name, string filename)
        {
            this.Name = Name;
            Load(filename);
        }

        public void Load(string Filename)
        {
            string[] Lines = File.ReadAllLines(Filename);
            for (int i = 0; i < Lines.Length; i++)
            {
                if (Lines[i].Equals("") || Lines[i].StartsWith("#"))
                    continue;
                string[] Current = Lines[i].Split('|');
                if (!Translations.ContainsKey(Current[0]))
                    Translations.Add(Current[0], Current[1].Replace("<N>", Environment.NewLine).Replace("</>","|"));
            }
        }

        public void Save(string Filename)
        {
            string Final = "";
            foreach (KeyValuePair<string, string> Translation in Translations)
                Final += Translation.Key + "|" + Translation.Value + Environment.NewLine;
            File.WriteAllText(Filename, Final);
        }

        public string GetTranslation(string Key)
        {
            Translations.TryGetValue(Key, out string value);
            return value;
        }
    }
}