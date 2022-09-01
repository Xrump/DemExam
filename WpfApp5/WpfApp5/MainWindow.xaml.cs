using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp5
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static List<Products> products = null;
        static List<List<Products>> pages = new List<List<Products>>();
        static int pageCount = 0;
        static int currentPage = 0;
        public MainWindow()
        {
            InitializeComponent();
            products = GetProducts();
            dbInitialize(products);  
        }
        void dbInitialize(List<Products> products)
        {
            GetPages(products);
            SetPage();
            PageSwitchButtons();

        }
        List<Products> GetProducts()
        {
            List<Products> products = null;
            using (var DbContext = new demExEntities())
            {
                products = DbContext.Products.ToList();
            }
            return products;
        }
        void GetPages(List<Products> product)
        {
            if (product != null)
                MainWindow.pages.Clear();
            else
                MainWindow.pages=new List<List<Products>>();
            int elements = 0;
            int pages = product.Count / 4;
            int lastElements = product.Count % 4;
            for (int i = 0; i < pages; i++)
            {
                List<Products> page = new List<Products>();
                for (int j = 0; j < 4; j++)
                {
                    page.Add(product[elements++]);
                }
                MainWindow.pages.Add(page);
            }
            if (lastElements>0)
            {
                List<Products> lastPage = new List<Products>();
                for (int i = 0; i < lastElements; i++)
                {
                    lastPage.Add(product[elements++]);
                }
                MainWindow.pages.Add(lastPage);
            }
            pageCount = MainWindow.pages.Count;
        }

        void SetPage()
        {
            if (pageCount <= 0)
                return;
            else
                ListView1.ItemsSource = pages[currentPage];
        }

        void PageSwitchButtons()
        {
            PagesPanel.Children.Clear();

            Button left_page = new Button();
            left_page.Content = "<";
            left_page.Margin = new Thickness(0, 0, 2, 0);
            left_page.BorderBrush = new SolidColorBrush(Colors.White);
            left_page.BorderThickness = new Thickness(0, 0, 0, 0);
            left_page.Background = new SolidColorBrush(Colors.White);
            left_page.Click += PageLeft_Click;

            PagesPanel.Children.Add(left_page);

            for (int i = 0; i < pages.Count; i++)
            {
                Button number_page = new Button();
                number_page.Content = $"{i + 1}";
                number_page.Margin = new Thickness(2, 0, 2, 0);
                number_page.BorderBrush = new SolidColorBrush(Colors.White);
                number_page.BorderThickness = new Thickness(0, 0, 0, 0);
                number_page.Background = new SolidColorBrush(Colors.White);
                number_page.Click += Page_Click;

                PagesPanel.Children.Add(number_page);
            }

            Button right_page = new Button();
            right_page.Content = ">";
            right_page.Margin = new Thickness(2, 0, 0, 0);
            right_page.BorderBrush = new SolidColorBrush(Colors.White);
            right_page.BorderThickness = new Thickness(0, 0, 0, 0);
            right_page.Background = new SolidColorBrush(Colors.White);
            right_page.Click += PageRight_Click;

            PagesPanel.Children.Add(right_page);

        }
        private void PageLeft_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage == 0)
                return;
            currentPage--;
            SetPage();
        }
        private void PageRight_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage == pages.Count-1)
                return;
            currentPage++;
            SetPage();
        }
        private void Page_Click(object sender, RoutedEventArgs e)
        {
            Button button=sender as Button;
            int page = Convert.ToInt32(button.Content);
            currentPage = page - 1;
            SetPage();
        }
        void Search(string text)
        {
            List<Products> searchedList = new List<Products>();
            searchedList = products.Where(p=>p.ProductName.ToLower().Contains(text.ToLower())).ToList();
            currentPage = 0;
            GetPages(searchedList);
        }
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox text=sender as TextBox;
            if (text.Text == "")
                dbInitialize(products);
            else
            {
                Search(text.Text);
                SetPage();
                PageSwitchButtons();
            }
        }
        private void Sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox=sender as ComboBox;
            switch (comboBox.SelectedIndex)
            {
                case 0:
                    dbInitialize(products);
                    break;
                case 1:
                    for (int i = 0; i < products.Count-1; i++)
                    {
                        for (int j = 0; j < products.Count-1-i; j++)
                        {
                            if (products[j].ProductCost>products[j+1].ProductCost)
                            {
                                Products temp = products[j];
                                products[j] = products[j + 1];
                                products[j+1] = temp;
                            }
                        }
                    }
                    dbInitialize(products);
                        break;
                case 2:
                    for (int i = 0; i < products.Count - 1; i++)
                    {
                        for (int j = 0; j < products.Count - 1 - i; j++)
                        {
                            if (products[j].ProductCost < products[j + 1].ProductCost)
                            {
                                Products temp = products[j];
                                products[j] = products[j + 1];
                                products[j + 1] = temp;
                            }
                        }
                    }
                    dbInitialize(products);
                    break;
            }
        }
        private void Fister_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            List<Products> searchedList = new List<Products>();
            string text = "";
            switch (comboBox.SelectedIndex)
            {
                case 0:
                    dbInitialize(products);
                    break;
                case 1:
                    text = "None Materials";
                    searchedList = products.Where(p => p.Materials.ToLower().Contains(text.ToLower())).ToList();
                    currentPage = 0;
                    GetPages(searchedList);
                    break;
                case 2:
                    text = "Material1";
                    searchedList = products.Where(p => p.Materials.ToLower().Contains(text.ToLower())).ToList();
                    currentPage = 0;
                    GetPages(searchedList);
                    break;
                case 3:
                    text = "Material2";
                    searchedList = products.Where(p => p.Materials.ToLower().Contains(text.ToLower())).ToList();
                    currentPage = 0;
                    GetPages(searchedList);
                    break;
                case 4:
                    text = "Material3";
                    searchedList = products.Where(p => p.Materials.ToLower().Contains(text.ToLower())).ToList();
                    currentPage = 0;
                    GetPages(searchedList);
                    break;
                case 5:
                    text = "Material4";
                    searchedList = products.Where(p => p.Materials.ToLower().Contains(text.ToLower())).ToList();
                    currentPage = 0;
                    GetPages(searchedList);
                    break;
                case 6:
                    text = "Material5";
                    searchedList = products.Where(p => p.Materials.ToLower().Contains(text.ToLower())).ToList();
                    currentPage = 0;
                    GetPages(searchedList);
                    break;
                case 7:
                    text = "Material6";
                    searchedList = products.Where(p => p.Materials.ToLower().Contains(text.ToLower())).ToList();
                    currentPage = 0;
                    GetPages(searchedList);
                    break;
                case 8:
                    text = "Material7";
                    searchedList = products.Where(p => p.Materials.ToLower().Contains(text.ToLower())).ToList();
                    currentPage = 0;
                    GetPages(searchedList);
                    break;
            }
        }

    }
}
