using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Afina
{
    internal class ConfigParse
    {
        public enum BookType
        {
            OtherBook,
            SaveBook,
            Catalog
        }

        public static List<string> lines = new List<string>();
        static string PathCfg = "afina.conf";

        public static List<string> OtherBooksList = new List<string>();
        public static List<string> SaveBooks = new List<string>();

        public static string PathTocatalog = "";


        public static string Theam = "";
        public static string Reader = "";
        public static List<string> Parse(BookType bt, bool StartProgram)
        {
            if (!File.Exists(PathCfg))
            {
                File.Create(PathCfg);

                string text = "<OtherBooks>\r\n</OtherBooks>\r\n\r\n<SaveBooks>\r\n</SaveBooks>\r\n\r\n<CatalogPath>\r\n</CatalogPath>\r\n<Settings>\r\n</Settings>";
                File.WriteAllText(PathCfg, text);
            }

            lines = File.ReadLines(PathCfg).ToList();


            bool otherbooks = false;
            bool savebooks = false;
            bool catalog = false;
            bool settings = false;

            for(int i = 0;i < lines.Count; i++ )
            {
                switch (lines[i].Trim('\r'))
                {
                    case "</SaveBooks>": savebooks = false; continue;
                    case "</OtherBooks>": otherbooks = false; continue;
                    case "</CatalogPath>": catalog = false; continue;
                    case "</Settings>": settings = false; continue;


                    case "<OtherBooks>": otherbooks = true; continue;
                    case "<SaveBooks>": savebooks = true; continue;
                    case "<CatalogPath>": catalog = true; continue;
                    case "<Settings>": settings = true; continue;



                }
                try
                {
                    if (!StartProgram)
                        if (lines[i] == OtherBooksList[i] || lines[i] == SaveBooks[i]) continue; // с этой строякой надо аккуратней

                    //она спасает от дубликатов но ломает загрузку проги

                    if (!catalog)
                    {
                        string PathToBook = lines[i].Split(',')[0].Trim('"'); // также тема 
                        string ReadedPages = lines[i].Split(',')[1]; // так же читалка

                        if (otherbooks) OtherBooksList.Add($"{PathToBook},{ReadedPages}");
                        if (savebooks) SaveBooks.Add($"{PathToBook},{ReadedPages}");


                        if (settings)
                        {
                            Theam = PathToBook; // тема
                            Reader = ReadedPages; // читалка
                        }
                    }
                    else PathTocatalog = lines[i];

                }
                catch (Exception e) { } // а вдруг аказия 

            }
            if (bt == BookType.OtherBook) return OtherBooksList;
            else return SaveBooks;
        }
        public static bool Add(string AddStringOrPath, string ReadedPages, BookType bt)
        {
            foreach (string line in OtherBooksList)
            {
                //MessageBox.Show($"Split - {line.Split(',')[0].Trim('"')}\n Add- {AddString}");
                if (line.Split(',')[0].Trim('"') == AddStringOrPath  && bt == BookType.OtherBook)
                {
                    MessageBox.Show("Внимание! Книга уже есть", "Дублирование книги");
                    return false;
                }
            }
            if(bt == BookType.SaveBook && SaveBooks.Count == 0) SaveBooks.Add($"{AddStringOrPath},{ReadedPages}");
            for (int i = 0; i < SaveBooks.Count; i++)
            {
                if (SaveBooks[i].Split(',')[0] == AddStringOrPath && bt == BookType.SaveBook) return false;
            }

            if (AddStringOrPath == PathTocatalog && bt == BookType.Catalog)
            {
                MessageBox.Show("Внимание! Каталог уже существует", "Дублирование каталога");
                return false;
            }
            else if (AddStringOrPath != PathTocatalog && bt == BookType.Catalog) Remove(AddStringOrPath, BookType.Catalog);

            if (bt == BookType.OtherBook)
                OtherBooksList.Add($"{AddStringOrPath},{ReadedPages}");
            if (bt == BookType.SaveBook)
                SaveBooks.Add($"{AddStringOrPath},{ReadedPages}");

            if (bt == BookType.Catalog)
                PathTocatalog = AddStringOrPath;
            Save();
            return true;
        }
        public static void Remove(string findetextOrSetCatolog, BookType bt) // удалить только там где надо. В ДРУГИХ ИЛИ ИЗ КАТЕГОРИИ
        {
            if (bt == BookType.OtherBook)
            {
                for (int i = 0; i < OtherBooksList.Count; i++)
                {
                    if (OtherBooksList[i].Split(',')[0] == findetextOrSetCatolog) OtherBooksList.RemoveAt(i);
                }
            }
            else
            {
                for (int i = 0; i < SaveBooks.Count; i++)
                {
                    if (SaveBooks[i].Split(',')[0] == findetextOrSetCatolog) SaveBooks.RemoveAt(i);
                }
            }

            if (bt == BookType.Catalog && findetextOrSetCatolog != "FullRemove") PathTocatalog = findetextOrSetCatolog;
            else if (bt == BookType.Catalog && findetextOrSetCatolog == "FullRemove") PathTocatalog = "";
            Save();
        }

        public static void Save()
        {
            string strconfig = "<OtherBooks>\n";
            foreach (string value in OtherBooksList) strconfig += value + "\n";
            strconfig += "</OtherBooks>\n\n";


            strconfig += "<SaveBooks>\n";
            foreach (string value in SaveBooks) strconfig += value + "\n";
            strconfig += "</SaveBooks>\n\n";

            strconfig += $"<CatalogPath>\n{PathTocatalog}\n</CatalogPath>\n\n";

            strconfig += $"<Settings>\n{Theam},{Reader}\n</Settings>";
            File.WriteAllText(PathCfg, strconfig);
        }
    }
}
