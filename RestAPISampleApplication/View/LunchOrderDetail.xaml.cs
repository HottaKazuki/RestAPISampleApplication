using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using RestAPISampleApplication.Model;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace RestAPISampleApplication.View
{
    /// <summary>
    /// LunchOrderDetail.xaml の相互作用ロジック
    /// </summary>
    public partial class LunchOrderDetail : Window
    {

        List<LunchMenu> listLunchMenu = new List<LunchMenu>();

        LunchOrder lunchOrder = new LunchOrder();

        LunchOrderManagement wndOrderManagement = new LunchOrderManagement();

        bool isNewOrder = true;
        public LunchOrderDetail()
        {
            InitializeComponent();

            listLunchMenu = GetLunchMenu();
            ddlLunchMenu.ItemsSource = listLunchMenu;
            ddlLunchMenu.DisplayMemberPath = "MenuName";
            ddlLunchMenu.SelectedValuePath = "Id";

        }

        public LunchOrderDetail(LunchOrderManagement wndOrderMng, int passId)
        {
            InitializeComponent();

            this.wndOrderManagement = wndOrderMng;

            listLunchMenu = GetLunchMenu();
            ddlLunchMenu.ItemsSource = listLunchMenu;
            ddlLunchMenu.DisplayMemberPath = "MenuName";
            ddlLunchMenu.SelectedValuePath = "Id";

            if (passId > 0)
            {
                Console.WriteLine(passId);

                lunchOrder = GetLunchOrderById(passId);

                if (lunchOrder != null)
                {
                    txtOrderId.Text = lunchOrder.OrderId;
                    txtOrderPerson.Text = lunchOrder.OrderPerson;
                    dtOrderDate.SelectedDate = lunchOrder.OrderDate;
                    ddlLunchMenu.SelectedValue = lunchOrder.LunchMenuId;
                    txtQuantity.Text = lunchOrder.Quantity.ToString();

                    LunchMenu menu = listLunchMenu.Find(x => x.Id == lunchOrder.LunchMenuId);
                    lblAmount.Content = "￥" + Math.Round(lunchOrder.Quantity * menu.Price, 2);

                    isNewOrder = false;
                }
                else
                {
                    this.Close();

                    MessageBox.Show("Lunch Order is not existed. Please check again.", "Read Data Failed");
                }
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

        private LunchOrder GetLunchOrderById(int intId)
        {
            LunchOrder lunchOrder = new LunchOrder();
            try
            {
                string url = "https://saibugasinformations-dev.outsystemsenterprise.com/LunchOrder_Core/rest/LunchOrder/GetLunchOrder";
                string parameter = "?LunchOrderId=" + intId.ToString();
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format(url + parameter));

                WebReq.Method = "GET";

                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

                Console.WriteLine(WebResp.StatusCode);

                string jsonString;
                using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
                {
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                    jsonString = reader.ReadToEnd();
                }

                lunchOrder = JsonConvert.DeserializeObject<LunchOrder>(jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return lunchOrder;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(ddlLunchMenu.SelectedValue);
        }

        private void CalculateAmount()
        {
            int lunchMenuId = 0;
            int quantity;

            if (ddlLunchMenu.SelectedValue != null && int.TryParse(ddlLunchMenu.SelectedValue.ToString(),out lunchMenuId))
            {
                 lunchMenuId = int.Parse(ddlLunchMenu.SelectedValue.ToString());
            }

            if (ddlLunchMenu.SelectedValue != null && txtQuantity.Text != "" && int.TryParse(txtQuantity.Text,out quantity))
            {
                LunchMenu menu = listLunchMenu.Find(x => x.Id == lunchMenuId);
                quantity = int.Parse(txtQuantity.Text);
                lblAmount.Content = "￥" + Math.Round(quantity * menu.Price,2);
            }
            else
            {
                lblAmount.Content = "";
            }
        }

        private void TxtQuantity_LostFocus(object sender, RoutedEventArgs e)
        {
            CalculateAmount();

        }

        private void DdlLunchMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateAmount();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (isNewOrder)
            {
                LunchOrderCreateRequest lunchOrderRequest = new LunchOrderCreateRequest();
                DateTime? selectedDate = dtOrderDate.SelectedDate;
                int quantity;

                lunchOrderRequest.LunchMenuId = int.Parse(ddlLunchMenu.SelectedValue.ToString());
                if (selectedDate.HasValue)
                {
                    lunchOrderRequest.OrderDate = selectedDate.Value.ToString("yyyy-MM-dd");
                }
                lunchOrderRequest.OrderId = txtOrderId.Text;
                lunchOrderRequest.OrderPerson = txtOrderPerson.Text;
                if (int.TryParse(txtQuantity.Text, out quantity))
                {
                    lunchOrderRequest.Quantity = int.Parse(txtQuantity.Text);
                }

                String request = JsonConvert.SerializeObject(lunchOrderRequest);
                Console.WriteLine(request);

                bool result = PostCreateOrderData(request);

                if (result)
                {
                    this.Close();
                    MessageBox.Show("Lunch Order " + lunchOrder.OrderId + " create successful!", "Create Order Success");
                }
                else
                {
                    MessageBox.Show("Lunch Order " + lunchOrder.OrderId + " create failed!", "Create Order Failed");
                }
            }
            else
            {
                LunchOrderUpdateRequest lunchOrderRequest = new LunchOrderUpdateRequest();
                DateTime? selectedDate = dtOrderDate.SelectedDate;
                int quantity;

                lunchOrderRequest.Id = lunchOrder.Id;
                lunchOrderRequest.LunchMenuId = int.Parse(ddlLunchMenu.SelectedValue.ToString());
                if (selectedDate.HasValue)
                {
                    lunchOrderRequest.OrderDate = selectedDate.Value.ToString("yyyy-MM-dd");
                }
                lunchOrderRequest.OrderId = txtOrderId.Text;
                lunchOrderRequest.OrderPerson = txtOrderPerson.Text;
                if (int.TryParse(txtQuantity.Text, out quantity))
                {
                    lunchOrderRequest.Quantity = int.Parse(txtQuantity.Text);
                }

                String request = JsonConvert.SerializeObject(lunchOrderRequest);
                Console.WriteLine(request);

                bool result = PostUpdateOrderData(request);

                if (result)
                {
                    this.Close();
                    MessageBox.Show("Lunch Order " + lunchOrder.OrderId + " update successful!", "Update Order Success");
                }
                else
                {
                    MessageBox.Show("Lunch Order " + lunchOrder.OrderId + " update failed!", "Update Order Failed");
                }
            }

            this.wndOrderManagement.ReadAllData();

        }

        public static bool PostCreateOrderData(string request)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://saibugasinformations-dev.outsystemsenterprise.com/LunchOrder_Core/rest/LunchOrder/CreateLunchOrder");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(request);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if(result != null && result!= "0")
                    {
                        return true;
                    } else return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool PostUpdateOrderData(string request)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://saibugasinformations-dev.outsystemsenterprise.com/LunchOrder_Core/rest/LunchOrder/UpdateLunchOrder");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(request);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result != null && result != "0")
                    {
                        return true;
                    }
                    else return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
