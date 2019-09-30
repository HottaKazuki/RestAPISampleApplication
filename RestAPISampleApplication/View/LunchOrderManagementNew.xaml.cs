using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Net.Http;
using System.Net.Http.Headers;
using RestAPISampleApplication.Model;

namespace RestAPISampleApplication.View
{
    /// <summary>
    /// LunchOrderManagementNew.xaml の相互作用ロジック
    /// </summary>
    public partial class LunchOrderManagementNew : Window
    {
        public LunchOrderManagementNew()
        {
            InitializeComponent();
            ReadAllData();
        }

        public void ReadAllData()
        {
            ReadAllData("");
        }

        public void ReadAllData(string strSearchValue)
        {
            try
            {
                HttpClient client = new HttpClient();

                client.BaseAddress = new Uri("https://saibugasinformations-dev.outsystemsenterprise.com/LunchOrder_Core/rest/LunchOrder/");

                client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("GetLunchOrders?PersonName=" + strSearchValue).Result;

                if (response.IsSuccessStatusCode)
                {
                    var orders = response.Content.ReadAsAsync<IEnumerable<LunchOrder>>().Result;
                    OrderDataGrid.ItemsSource = orders;

                    List<LunchMenu> lunchMenu = GetLunchMenu();

                    foreach (LunchOrder order in orders)
                    {
                        LunchMenu menu = lunchMenu.Find(x => x.Id == order.LunchMenuId);
                        order.LunchMenuName = menu.MenuName;
                    }

                    DataCount.Text = "Data Count : " + orders.Count<LunchOrder>();
                }
                else
                {
                    MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private List<LunchMenu> GetLunchMenu()
        {
            List<LunchMenu> lunchMenu = new List<LunchMenu>();
            try
            {
                HttpClient client = new HttpClient();

                client.BaseAddress = new Uri("https://saibugasinformations-dev.outsystemsenterprise.com/LunchOrder_Core/rest/LunchOrder/");

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("GetLunchMenu").Result;

                if (response.IsSuccessStatusCode)
                {
                    lunchMenu = response.Content.ReadAsAsync<List<LunchMenu>>().Result;
                }
                else
                {
                    MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return lunchMenu;
        }

        private void DG_Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var rowData = ((Hyperlink)e.OriginalSource).DataContext as LunchOrder;
            Console.WriteLine("Update " + rowData.Id);

            var window = new LunchOrderDetailNew(this, rowData.Id);

            window.Show();
        }

        private void BtnNewOrder_Click(object sender, RoutedEventArgs e)
        {
            LunchOrderDetailNew wndOrderDetai = new LunchOrderDetailNew(this, 0);

            wndOrderDetai.Show();

        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var rowData = ((Button)e.OriginalSource).DataContext as LunchOrder;
                Console.WriteLine("Delete " + rowData.Id);

                MessageBoxResult msgBoxResult = MessageBox.Show("Are you sure to delete this lunch order?", "Delete Confirmation", MessageBoxButton.YesNo);
                if (msgBoxResult == MessageBoxResult.Yes){
                    bool result = DeleteOrder(rowData.Id);

                    if (result)
                    {
                        MessageBox.Show("Delete " + rowData.OrderId + " successful", "Delete Lunch Order");

                        ReadAllData();
                    }
                    else
                    {
                        MessageBox.Show("Delete " + rowData.OrderId + " falied", "Delete Lunch Order");
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private bool DeleteOrder(int intId)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://saibugasinformations-dev.outsystemsenterprise.com/LunchOrder_Core/rest/LunchOrder/");

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.DeleteAsync("DeleteLunchOrder?LunchOrderId=" + intId).Result;

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                return false;
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            ReadAllData(txtSearchValue.Text);
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            txtSearchValue.Text = "";
            ReadAllData();
        }
    }
}
