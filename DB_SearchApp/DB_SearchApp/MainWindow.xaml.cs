using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Data;
using LiteDB;
using Microsoft.Win32;

namespace DB_SearchApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // DataBase
        string dbConnectionString = "CustomerDB.db";
        public void LiteDB_CreateAndImport()
        {
            using (var dataBase = new LiteDatabase(dbConnectionString))
            {
                var collection = dataBase.GetCollection<Customer>("customer");

                OpenFileDialog dialog = new OpenFileDialog();
                {
                    dialog.Filter = "Text Files(*.txt)|*.txt|All(*.*)|*";

                    if (dialog.ShowDialog() == true)
                    {
                        // Read all text in .txt file
                        string line = File.ReadAllText(dialog.FileName);
                        // Change Encoding to Central European
                        byte[] bytes = Encoding.Default.GetBytes(line);
                        line = Encoding.GetEncoding(1250).GetString(bytes);

                        // 1. step: Split strings into 4 groups for every line
                        List<string> listStrLineElements = line.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();// Need using System.Linq at the top.
                                                                                                                                              // 2. step: Split string data by TAB(Tabulator)                                                                                         
                        List<string> rowList = listStrLineElements.SelectMany(s => s.Split('\t')).ToList();

                        dataBase.DropCollection("customer");
                        var customer = new Customer();

                        for (int i = 0; i <= rowList.Count - 2; i += 4)
                        {
                            customer = new Customer
                            {
                                Id = Convert.ToInt16(rowList[i]),
                                CustomerID = Convert.ToInt16(rowList[i]),
                                FirstName_LastName = rowList[i + 1].ToString(),
                                TypeOfBattery = rowList[i + 2].ToString(),
                                DateOfPurchase = rowList[i + 3].ToString()
                            };
                            collection.Insert(customer);
                        }

                        // Use LINQ to query documents
                        var count = collection.Count(Query.All());

                        for (int i = 1; i <= count; i++)
                        {
                            lvCustomers.Items.Add(collection.FindById(i));
                        }
                    }
                }
            }
        }
        public void LiteDB_CreateAndImport1()
        {
            using (var dataBase = new LiteDatabase(dbConnectionString))
            {
                var collection = dataBase.GetCollection<Customer>("customer");
                // Read all text in .txt file
                string line = File.ReadAllText(@"Enter File Path");
                // Change Encoding to Central European
                byte[] bytes = Encoding.Default.GetBytes(line);
                line = Encoding.GetEncoding(1250).GetString(bytes);

                // 1. step: Split strings into 4 groups for every line
                List<string> listStrLineElements = line.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();// Need using System.Linq at the top.
                // 2. step: Split string data by TAB(Tabulator)                                                                                         
                List<string> rowList = listStrLineElements.SelectMany(s => s.Split('\t')).ToList();

                dataBase.DropCollection("customer");
                var customer = new Customer();

                for (int i = 0; i <= rowList.Count - 2; i += 4)
                {
                    customer = new Customer
                    {
                        Id = Convert.ToInt16(rowList[i]),
                        CustomerID = Convert.ToInt16(rowList[i]),
                        FirstName_LastName = rowList[i + 1].ToString(),
                        TypeOfBattery = rowList[i + 2].ToString(),
                        DateOfPurchase = rowList[i + 3].ToString()
                    };
                    collection.Insert(customer);
                }

                // Use LINQ to query documents
                var count = collection.Count(Query.All());

                for (int i = 1; i <= count; i++)
                {
                    lvCustomers.Items.Add(collection.FindById(i));
                }
            }
        }
        public void LiteDB_ShowAll()
        {
            using (var dataBase = new LiteDatabase(dbConnectionString))
            {
                var collection = dataBase.GetCollection<Customer>("customer");
                lvCustomers.Items.Clear();

                var count = collection.Count(Query.All());

                foreach (var customer in collection.FindAll().OrderByDescending(x => x.Id))
                {
                    lvCustomers.Items.Add(customer);
                }
            }
        }
        public void LiteDB_Delete(int index)
        {
            using (var dataBase = new LiteDatabase(dbConnectionString))
            {
                var collection = dataBase.GetCollection<Customer>("customer");

                // LiteDB delete query
                collection.Delete(index);

                //LiteDB re-numbering query
                foreach (var customer in collection.FindAll())
                {
                    if (customer.Id > index)
                    {
                        customer.CustomerID = customer.CustomerID - 1;
                        collection.Update(customer);
                    }
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            LiteDB_ShowAll();
        }

        private void import_btn_Click(object sender, RoutedEventArgs e)
        {
            // LiteDB poizvedba za ustvaritev baze, dodajanje in prikaz vseh podatkov
            LiteDB_CreateAndImport();
        }

        private void search_btn_Click(object sender, RoutedEventArgs e)
        {
            using (var dataBase = new LiteDatabase(dbConnectionString))
            {
                var collection = dataBase.GetCollection<Customer>("customer");
                collection.EnsureIndex("FirstName_LastName");

                var query = collection
                        .Find(Query.All());

                // LiteDB query: search by name 
                if (searchName_tb.Text != "" && searchDates_tb.Text == "")
                {
                    query = collection
                        .Find(Query.StartsWith("FirstName_LastName", searchName_tb.Text.ToUpper()))
                        .OrderByDescending(x => x.DateOfPurchase);
                }
                // LiteDB query: search by date
                else if (searchName_tb.Text == "" && searchDates_tb.Text != "")
                {
                    query = collection
                        .Find(Query.Contains("DateOfPurchase", searchDates_tb.Text))
                        .OrderByDescending(x => x.DateOfPurchase);
                }
                // LiteDB query: search by name and date
                else if (searchName_tb.Text != "" && searchDates_tb.Text != "")
                {
                    query = collection
                        .Find(Query.StartsWith("FirstName_LastName", searchName_tb.Text.ToUpper()))
                        .Where(x => x.DateOfPurchase == searchDates_tb.Text)
                        .OrderByDescending(x => x.DateOfPurchase);
                }
                else if (searchName_tb.Text == "" && searchDates_tb.Text == "")
                {
                    MessageBox.Show("Missing search value");
                }
                // Clear list
                lvCustomers.Items.Clear();

                // Fill list with searched customer data
                foreach (var customer in query)
                {
                    lvCustomers.Items.Add(customer);
                }
            }
        }

        private void load_btn_Click(object sender, RoutedEventArgs e)
        {
            LiteDB_ShowAll();
        }

        private void insert_btn_Click(object sender, RoutedEventArgs e)
        {
            using (var dataBase = new LiteDatabase(dbConnectionString))
            {
                var collection = dataBase.GetCollection<Customer>("customer");
                var customer = new Customer
                {
                    CustomerID = collection.Count() + 1,
                    FirstName_LastName = searchName_tb.Text,
                    TypeOfBattery = searchType_tb.Text,
                    DateOfPurchase = searchDates_tb.Text
                };
                // LiteDB query: add customer do DataBase
                collection.Insert(customer);
                // LiteDB query: update DataBase
                collection.Update(customer);
            }
            LiteDB_ShowAll();
        }

        private void remove_btn_Click(object sender, RoutedEventArgs e)
        {
            int index = Convert.ToInt16(lblNumber.Content);
            LiteDB_Delete(index);
            LiteDB_ShowAll();
        }

        private void search_tb_GotFocus(object sender, RoutedEventArgs e)
        {
            searchName_tb.Clear();
        }

        private void searchType_tb_GotFocus(object sender, RoutedEventArgs e)
        {
            searchType_tb.Clear();
        }

        private void searchDates_tb_GotFocus(object sender, RoutedEventArgs e)
        {
            searchDates_tb.Clear();
        }

        private void lvCustomers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Get selected customer INDEX
            var selectedStockObject = lvCustomers.SelectedItems[0] as Customer;
            if (selectedStockObject == null)
            {
                return;
            }

            int index = selectedStockObject.Id;
            lblNumber.Content = index.ToString();
        }

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            string text = "";
            foreach (Customer item in lvCustomers.Items)
            {
                text += item.Id + "\t" + item.FirstName_LastName + "\t" + item.TypeOfBattery + "\t" + item.DateOfPurchase + "\r\n";
            }
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Text Files(*.txt)|*.txt|All(*.*)|*"
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, text);
            }

        }
    }
}