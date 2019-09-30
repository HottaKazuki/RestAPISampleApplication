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
using Newtonsoft.Json;
using System.Net;
using System.IO;
using RestAPISampleApplication.Model;
using RestAPISampleApplication.View;

namespace RestAPISampleApplication
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //OrderDataGrid.ItemsSource = ReadAllData();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window window = new LunchOrderManagement();
            window.Show();
        }

        private void CreateOrder_Click(object sender, RoutedEventArgs e)
        {
            Window window = new LunchOrderDetail();
            window.Show();
        }
        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = new LunchOrderManagementNew();
            window.Show();
        }
        private void CreateNewOrder_Click(object sender, RoutedEventArgs e)
        {
            Window window = new LunchOrderDetailNew();
            window.Show();
        }

        //public List<LunchOrder> ReadAllData()
        //{
        //    List<LunchOrder> items = new List<LunchOrder>();
        //    try
        //    {
        //        HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format(@"https://saibugasinformations-dev.outsystemsenterprise.com/LunchOrder_Core/rest/LunchOrder/GetLunchOrders?PersonName="));

        //        WebReq.Method = "GET";

        //        HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

        //        Console.WriteLine(WebResp.StatusCode);
        //        Console.WriteLine(WebResp.Server);

        //        string jsonString;
        //        using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
        //        {
        //            StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
        //            jsonString = reader.ReadToEnd();
        //        }

        //        items = JsonConvert.DeserializeObject<List<LunchOrder>>(jsonString);

        //        List<LunchMenu> lunchMenu = GetLunchMenu();

        //        foreach(LunchOrder item in items)
        //        {
        //            LunchMenu menu = lunchMenu.Find(x => x.Id == item.LunchMenuId);
        //            item.LunchMenuName = menu.MenuName;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }

        //    DataCount.Text = "Data Count : " + items.Count;

        //    return items;

        //}

        //private List<LunchMenu> GetLunchMenu()
        //{
        //    List<LunchMenu> lunchMenu = new List<LunchMenu>();
        //    try
        //    {
        //        HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format(@"https://saibugasinformations-dev.outsystemsenterprise.com/LunchOrder_Core/rest/LunchOrder/GetLunchMenu"));

        //        WebReq.Method = "GET";

        //        HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

        //        Console.WriteLine(WebResp.StatusCode);
        //        Console.WriteLine(WebResp.Server);

        //        string jsonString;
        //        using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
        //        {
        //            StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
        //            jsonString = reader.ReadToEnd();
        //        }

        //        lunchMenu = JsonConvert.DeserializeObject<List<LunchMenu>>(jsonString);
        //    }
        //    catch(Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }

        //    return lunchMenu;
        //}
    }
}
