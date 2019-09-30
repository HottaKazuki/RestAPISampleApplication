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
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using RestAPISampleApplication.Model;

namespace RestAPISampleApplication.View
{
    /// <summary>
    /// LunchOrderManagement.xaml の相互作用ロジック
    /// </summary>
    public partial class LunchOrderManagement : Window
    {
        public LunchOrderManagement()
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
                List<LunchOrder> items = new List<LunchOrder>();

                string url = string.Format(@"https://saibugasinformations-dev.outsystemsenterprise.com/LunchOrder_Core/rest/LunchOrder/GetLunchOrders");
                string param = "?PersonName=" + strSearchValue;

                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(url + param);

                WebReq.Method = "GET";

                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

                Console.WriteLine(WebResp.StatusCode);
                Console.WriteLine(WebResp.Server);

                string jsonString;
                using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
                {
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                    jsonString = reader.ReadToEnd();
                }

                items = JsonConvert.DeserializeObject<List<LunchOrder>>(jsonString);

                List<LunchMenu> lunchMenu = GetLunchMenu();

                foreach (LunchOrder item in items)
                {
                    LunchMenu menu = lunchMenu.Find(x => x.Id == item.LunchMenuId);
                    item.LunchMenuName = menu.MenuName;
                }

                DataCount.Text = "Data Count : " + items.Count;

                OrderDataGrid.ItemsSource = items;
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
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format(@"https://saibugasinformations-dev.outsystemsenterprise.com/LunchOrder_Core/rest/LunchOrder/GetLunchMenu"));

                WebReq.Method = "GET";

                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

                Console.WriteLine(WebResp.StatusCode);
                Console.WriteLine(WebResp.Server);

                string jsonString;
                using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
                {
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                    jsonString = reader.ReadToEnd();
                }

                lunchMenu = JsonConvert.DeserializeObject<List<LunchMenu>>(jsonString);
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

            var window = new LunchOrderDetail(this, rowData.Id);

            window.Show();
        }

        private void BtnNewOrder_Click(object sender, RoutedEventArgs e)
        {
            LunchOrderDetail wndOrderDetai = new LunchOrderDetail(this, 0);

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
            string url = string.Format(@"https://saibugasinformations-dev.outsystemsenterprise.com/LunchOrder_Core/rest/LunchOrder/DeleteLunchOrder");
            string param = "?LunchOrderId=" + intId;

            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(url + param);

            WebReq.Method = "DELETE";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            Console.WriteLine(WebResp.StatusCode);

            if(WebResp.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
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
