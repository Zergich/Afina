using System;
using System.IO;
using System.Linq;
using System.Windows;
using wps = System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Aspose.Pdf;
using Aspose.Pdf.Devices;
using Aspose.Pdf.Facades;
using wpf = Microsoft.Win32;
using Image = System.Windows.Controls.Image;
using Winforms = System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using System.Windows.Controls;
using Patagames.Pdf.Net.Controls.Wpf;
using Syncfusion.PdfToImageConverter;
using System.Drawing;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;

namespace Afina
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StartProgram();
            //pdffile.LoadDocument(@"D:\Либерия\C++\RUS - Sutter - CPP Coding Standards..pdf");
            //pdffile.CloseDocument();
            //pdffile.CurrentPage;
        }

        void StartProgram() // кого ебет чужое горе? извините за говнокод
        {
            foreach (string line in ConfigParse.Parse(ConfigParse.BookType.OtherBook, true)) //otherbooks
            {
                string path = line.Split(',')[0];
                //string readedpages = line.Split(',')[1];
                ShowBookCover(path);
            }
            try
            {
                foreach (string pathfolder in Directory.GetDirectories(ConfigParse.PathTocatalog))// и записывать в файл конфига полный путь а потом так жу парсить 
                {
                    string name = pathfolder.Split('\\').Last();
                    Katalog.Items.Add(name);
                }
            }
            catch { }
            if (ConfigParse.Theam.Trim('\r') == "Ligth") Theames.SelectedIndex = 2;
            else Theames.SelectedIndex = 1;

            if (ConfigParse.Reader.Trim('\r') == "NeMoy") Readers.SelectedIndex = 2;
            else Readers.SelectedIndex = 1;

        }
        private void AnotherBook_Click(object sender, RoutedEventArgs e)
        {
            Books.Children.Clear();
            foreach (string line in ConfigParse.OtherBooksList) //otherbooks
            {
                string path = line.Split(',')[0];
                //string readedpages = line.Split(',')[1];
                ShowBookCover(path);
            }
        }
        private void AddAnother_Click(object sender, RoutedEventArgs e)
        {
            wpf.OpenFileDialog open = new wpf.OpenFileDialog();
            open.Filter = "pdf files (*.*)|*.pdf";

            bool? result = open.ShowDialog();
            if (result == true)
            {
                if (ConfigParse.Add(open.FileName, "0", ConfigParse.BookType.OtherBook)); //otherbuuk
                AnotherBook_Click(null, null);
                //сначала получить картинку pdf
                //ShowBookCover(@"C:\Users\pytho\Pictures\Обои\IstlBQmxmNA.jpg", "0");
            }
        }

        private void ShowMenu_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation btnshow = new DoubleAnimation();
            btnshow.From = 25;
            btnshow.To = 120;
            btnshow.Duration = TimeSpan.FromMilliseconds(200);
            Menu.BeginAnimation(wps.Button.WidthProperty, btnshow);

            ReturnMenu.Visibility = Visibility.Hidden;

            ThicknessAnimation Grid = new ThicknessAnimation();
            Grid.From = new Thickness(25, 0, 0, 0);
            Grid.To = new Thickness(120, 0, 0, 0);
            Grid.Duration = TimeSpan.FromMilliseconds(30);
            Moy.BeginAnimation(wps.Grid.MarginProperty, Grid);


            a.Visibility = Visibility.Visible;
            Katalog.Visibility = Visibility.Visible;
            b.Visibility = Visibility.Visible;
            c.Visibility = Visibility.Visible;
            O.Visibility = Visibility.Visible;
        }

        private void HideMenu_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation btnhide = new DoubleAnimation();
            btnhide.From = 120;
            btnhide.To = 25;
            btnhide.Duration = TimeSpan.FromMilliseconds(200);
            Menu.BeginAnimation(wps.Button.WidthProperty, btnhide);

            ThicknessAnimation Grid = new ThicknessAnimation();
            Grid.From = new Thickness(120, 0,0,0);
            Grid.To = new Thickness(25, 0, 0, 0);
            Grid.Duration = TimeSpan.FromMilliseconds(30);
            Moy.BeginAnimation(wps.Grid.MarginProperty, Grid);
            //Lib.Margin = new Thickness(25, 0, 0, 0);

            a.Visibility = Visibility.Hidden;
            Katalog.Visibility = Visibility.Hidden;
            b.Visibility = Visibility.Hidden;
            c.Visibility = Visibility.Hidden;
            O.Visibility = Visibility.Hidden;

            ReturnMenu.Visibility = Visibility.Visible;
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            Winforms.FolderBrowserDialog dialog = new Winforms.FolderBrowserDialog();
            //bool? result = fileDialog.ShowDialog();
            Winforms.DialogResult result = dialog.ShowDialog();

            string folderName = "";
            if (result == Winforms.DialogResult.OK)
            {
                folderName = dialog.SelectedPath;
                ConfigParse.Add(folderName, null, ConfigParse.BookType.Catalog);
                Katalog.Items.Clear();

                int i = 0;
                foreach (string pathfolder in Directory.GetDirectories(folderName))// и записывать в файл конфига полный путь а потом так жу парсить 
                {
                    string name = pathfolder.Split('\\').Last();
                    Katalog.Items.Add(name);
                    i++;
                }
                if (i == 0) System.Windows.MessageBox.Show("Внимание! Папка которую вы указали, пуста!","Пустой католог");

            }

        }
        string readingbook = ""; //книгу которую сейчас читабт
        void ShowBookCover(string PathToBook)
        {

            string getnamebook = PathToBook.Split('\\').Last().Replace(".pdf", "");

            if (!File.Exists($"Miniaturs\\{getnamebook}.jpg"))
            {
                // Открыть документ
                Document pdfDocument = new Document(PathToBook);

                //Aspose.Pdf.Page pdfPage = pdfDocument.Background; //для темной темы


                int pageIndex = 1;

                // Получить страницу нужного индекса из коллекции
                var page = pdfDocument.Pages[pageIndex];

                // Создать поток для файла изображения
                using (FileStream imageStream = new FileStream($"Miniaturs\\{getnamebook}.jpg", FileMode.Create))
                {
                    // Создать объект разрешения
                    Resolution resolution = new Resolution(1444);

                    // Создайте экземпляр JpegDevice и установите высоту, ширину, разрешение и качество изображения.
                    JpegDevice jpegDevice = new JpegDevice(250, 200, resolution, 100);

                    // Преобразование определенной страницы и сохранение изображения в поток
                    jpegDevice.Process(page, imageStream);

                    // Закрыть поток
                    imageStream.Close();
                }
            }


            wps.Grid grid = new wps.Grid();
            wps.Button StartreadingBook = new wps.Button();
            wps.TextBlock txt = new wps.TextBlock();
            wps.Border radius = new wps.Border(); // для маленького скругления
            wps.Image img = new wps.Image();
            wps.ContextMenu menu = new wps.ContextMenu();
            wps.MenuItem imenu = new wps.MenuItem();
            wps.Label GetpathBook = new wps.Label();

            imenu.Header = "Удалить";
            imenu.Click += DeletBook;
            menu.Items.Add(imenu);


            img.Width = 150;
            img.Height = 200;
            //img.Source = new BitmapImage(new Uri($"\\Miniaturs\\{getnamebook}.jpg", UriKind.RelativeOrAbsolute));
            img.Source = new BitmapImage(new Uri($@"D:\Progects\Afina\Afina\bin\Debug\Miniaturs\{getnamebook}.jpg", UriKind.Absolute));
            img.Stretch = Stretch.Fill;
            img.Margin = new Thickness(-1, -20, 0, 0);

            radius.CornerRadius = new CornerRadius(2);

            grid.Height = 220; //img = 200
            grid.Width = 150;
            grid.Margin = new Thickness(5);

            SolidColorBrush covercolor = (SolidColorBrush)new BrushConverter().ConvertFrom("Transparent");

            StartreadingBook.Background = covercolor;
            StartreadingBook.BorderThickness = new Thickness(0);
            StartreadingBook.Content = img;
            StartreadingBook.ContextMenu = menu;
            StartreadingBook.Click += StartReadBook_Click;
            //StartreadingBook.Name = PathToBook.Split('\\').Last().Replace(" ", "~");//sdfsdfsdfsddsfdsdfsdfsdfsdfs

            GetpathBook.Content = PathToBook;
            GetpathBook.Visibility = Visibility.Hidden; // я чертов гений

            //txt.Text = readedBook + "%";

            for (int i = 0; i < ConfigParse.OtherBooksList.Count; i++)
            {
                if (ConfigParse.OtherBooksList[i].Split(',')[0] == PathToBook) txt.Text = ConfigParse.OtherBooksList[i].Split(',')[1];

            }
            for (int i = 0; i < ConfigParse.SaveBooks.Count; i++)
            {
                if (ConfigParse.SaveBooks[i].Split(',')[0] == PathToBook) txt.Text = ConfigParse.SaveBooks[i].Split(',')[1];

            }


            txt.Margin = new Thickness(60, 203, 0, 0);

            grid.Children.Add(StartreadingBook);
            grid.Children.Add(GetpathBook);
            grid.Children.Add(txt);
            radius.Child = grid;

            Books.Children.Add(radius);
            
            void DeletBook(object sender, RoutedEventArgs e)
            {
                Books.Children.Remove(radius);
                ConfigParse.Remove((string)GetpathBook.Content, ConfigParse.BookType.OtherBook);
            }
            void StartReadBook_Click(object sender, RoutedEventArgs e)
            {
                ScrollBooks.Visibility = Visibility.Hidden;
                Moy.Visibility = Visibility.Visible;
                HideMenu.Visibility = Visibility.Visible;
                readingbook = (string)GetpathBook.Content;
                //MessageBox.Show(readingbook);

                //else
                //{
                //    ScrollBooks.Visibility = Visibility.Hidden;
                //    Reading.Visibility = Visibility.Visible;
                //    HideMenu.Visibility = Visibility.Visible;

                //    pdffile.SizeMode = SizeModes.FitToSize;
                //    pdffile.Zoom = 100;
                //    pdffile.ViewMode = ViewModes.TilesVertical;

                //    int page = int.Parse(txt.Text);
                //    //for (int i = 0; i < ConfigParse.OtherBooksList.Count; i++)
                //    //{
                //    //    if (ConfigParse.OtherBooksList[i].Split(',')[0] == readingbook)
                //    //    {
                //    //        page = int.Parse(ConfigParse.OtherBooksList[i].Split(',')[1]);
                //    //    }
                //    //    //else
                //    //    //{
                //    //    //    page = int.Parse(ConfigParse.SaveBooks[i].Split(',')[1]);

                //    //    //}

                //    //}

                //    readingbook = (string)GetpathBook.Content;
                //    pdffile.LoadDocument((string)GetpathBook.Content);
                //    pdffile.ScrollToPage(page);
                //}
            }
        }

        private void DeletCatalogItems_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить запись о каталоге?","Удаление записей о катологе", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                Katalog.Items.Clear();
                ConfigParse.Remove("FullRemove", ConfigParse.BookType.Catalog);
            }
        }

        private void ShowBooksInCatalog_Click(object sender, RoutedEventArgs e)
        {
            Books.Children.Clear();
            string PathToCatalog = $"{ConfigParse.PathTocatalog}\\{Katalog.SelectedValue}";

            foreach (string file in Directory.GetFiles(PathToCatalog))
            {
                if (file.Split('.').Last().ToLower() == "pdf")
                {
                    ConfigParse.Add(file, "0", ConfigParse.BookType.SaveBook);
                    ShowBookCover(file);
                }
            }
        }

        private void GoToLiberAndClosePdf_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < ConfigParse.OtherBooksList.Count; i++)
            {
                if (ConfigParse.OtherBooksList[i].Split(',')[0] == readingbook)
                {
                    ConfigParse.Remove(readingbook, ConfigParse.BookType.OtherBook);
                    ConfigParse.Add(readingbook, $"{page}", ConfigParse.BookType.OtherBook);
                }

            }
            for (int i = 0; i < ConfigParse.SaveBooks.Count; i++) // я хуй знает че оно канаебится но работает только так елсе не помогает 
            {
                if (ConfigParse.SaveBooks[i].Split(',')[0] == readingbook)
                {
                    ConfigParse.Remove(readingbook, ConfigParse.BookType.SaveBook);
                    ConfigParse.Add(readingbook, $"{page}", ConfigParse.BookType.SaveBook);
                }

            }

            Moy.Visibility = Visibility.Hidden;
            AnotherBook_Click(null, null);
            ScrollBooks.Visibility = Visibility.Visible;
            HideMenu.Visibility = Visibility.Hidden;
            //int page = pdffile.CurrentIndex;
            //for(int i = 0; i < ConfigParse.OtherBooksList.Count; i++)
            //{
            //    if (ConfigParse.OtherBooksList[i].Split(',')[0] == readingbook)
            //    {
            //        ConfigParse.Remove(readingbook, ConfigParse.BookType.OtherBook);
            //        ConfigParse.Add(readingbook, $"{page}", ConfigParse.BookType.OtherBook);
            //    }

            //}
            //for (int i = 0; i < ConfigParse.SaveBooks.Count; i++) // я хуй знает че оно канаебится но работает только так елсе не помогает 
            //{
            //    if (ConfigParse.SaveBooks[i].Split(',')[0] == readingbook)
            //    {
            //        ConfigParse.Remove(readingbook, ConfigParse.BookType.SaveBook);
            //        ConfigParse.Add(readingbook, $"{page}", ConfigParse.BookType.SaveBook);
            //    }

            //}

            //Reading.Visibility = Visibility.Hidden;
            //AnotherBook_Click(null, null);
            //pdffile.CloseDocument();
            //ScrollBooks.Visibility = Visibility.Visible;
            //HideMenu.Visibility = Visibility.Hidden;
            ////pdffile.ScrollToPage(); pdffile.CurrentIndex
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //if(ConfigParse.Reader == "Moy")
            //SaveMotalka();
            //else
              GoToLiberAndClosePdf_Click(null,null);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(ConfigParse.Theam);
            if (ConfigParse.Theam.Trim('\r') == "Ligth") Theames.SelectedIndex = 2;
            else Theames.SelectedIndex = 1;

            if (ConfigParse.Reader.Trim('\r') == "NeMoy") Readers.SelectedIndex = 2;
            else Readers.SelectedIndex = 1;

            //Theames.SelectedIndex = 1;   // по умолчанию будет выбран второй элемент
            //Readers.SelectedIndex = 1;   // по умолчанию будет выбран второй элемент

            SettingsMenu.Visibility = Visibility.Visible;
            Lib.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SettingsMenu.Visibility = Visibility.Collapsed;
            Lib.Visibility = Visibility.Visible;
        }
        private void TheamComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Theames.Text == "Светлая") // так надо по другоиу почемуто не созраняет
                ConfigParse.Theam = "Dark";
            else ConfigParse.Theam = "Ligth";

            ConfigParse.Save();

        }
        private void ReaderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Readers.Text == "Моталка") // так надо по другоиу почемуто не созраняет
                ConfigParse.Reader = "Moy";
            else ConfigParse.Reader = "NeMoy";

            ConfigParse.Save();

        }

        int page = 0;
        //Initialize the PDF to Image converter.
        PdfToImageConverter imageConverter = new PdfToImageConverter();

        // в будующем могут быть проблемой после выборы другой книги
        // не сохраняется путь?
        byte[] bytesFromFile(string path)
        {
            return File.ReadAllBytes(path);
        }
        //byte[] bytesFromFile = GetBytes();

        //Load the PDF document as a stream.
        private void Image_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            //FileStream inputStream = new FileStream(@"C:\Users\pytho\Downloads\AByteofPythonRussian-2.01.pdf", FileMode.Open, FileAccess.ReadWrite); // как то надо вынести за скобки
            MemoryStream bytesStream = new MemoryStream(bytesFromFile(readingbook));



            if (e.Delta < 0)
            {
                if (imageConverter.PageCount != page + 1)
                {

                    page++;

                    imageConverter.Load(bytesStream);

                    //Convert PDF to image.
                    Stream outputStream = imageConverter.Convert(page, false, false);
                    MemoryStream stream = outputStream as MemoryStream;
                    byte[] bytes = stream.ToArray();
                    //var s = System.Drawing.Image.FromStream(inputStream);

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = new MemoryStream(bytes);
                    bitmapImage.EndInit();

                    Left.Source = bitmapImage;
                }
            }
            else if (e.Delta > 0)
            {
                if (page != 0)
                {
                    page--;

                    imageConverter.Load(bytesStream);

                    //Convert PDF to image.
                    Stream outputStream = imageConverter.Convert(page, false, false);
                    MemoryStream stream = outputStream as MemoryStream;
                    byte[] bytes = stream.ToArray();
                    //var s = System.Drawing.Image.FromStream(inputStream);

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = new MemoryStream(bytes);
                    bitmapImage.EndInit();

                    Left.Source = bitmapImage;
                }
            }
            CurrentPage.Text = $" из {imageConverter.PageCount}";
            SetPage.Text = $"{page+1}";
        }

        private void SetPage_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                int savevalue = page; // от ошибок
                try
                {
                    page = int.Parse(SetPage.Text);

                    MemoryStream bytesStream = new MemoryStream(bytesFromFile(readingbook));

                    imageConverter.Load(bytesStream);

                    //Convert PDF to image.
                    Stream outputStream = imageConverter.Convert(page, false, false);
                    MemoryStream stream = outputStream as MemoryStream;
                    byte[] bytes = stream.ToArray();
                    //var s = System.Drawing.Image.FromStream(inputStream);

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = new MemoryStream(bytes);
                    bitmapImage.EndInit();

                    Left.Source = bitmapImage;
                }
                catch { }
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SetPage.Text = $"{page+1}";
            Moy.Focus();
        }

        private void ZoomP_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
