using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using RestAPISampleApplication.Model;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RestAPISampleApplication.View
{
    /// <summary>
    /// LunchOrderDetail.xaml の相互作用ロジック
    /// </summary>
    public partial class LunchOrderDetailNew : Window
    {

        List<LunchMenu> listLunchMenu = new List<LunchMenu>();

        LunchOrder lunchOrder = new LunchOrder();

        LunchOrderManagementNew wndOrderManagement = new LunchOrderManagementNew();

        bool isNewOrder = true;
        public LunchOrderDetailNew()
        {
            InitializeComponent();

            listLunchMenu = GetLunchMenu();
            ddlLunchMenu.ItemsSource = listLunchMenu;
            ddlLunchMenu.DisplayMemberPath = "MenuName";
            ddlLunchMenu.SelectedValuePath = "Id";

        }

        public LunchOrderDetailNew(LunchOrderManagementNew wndOrderMng, int passId)
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

        private LunchOrder GetLunchOrderById(int intId)
        {
            LunchOrder lunchOrder = new LunchOrder();
            try
            {
                HttpClient client = new HttpClient();

                client.BaseAddress = new Uri("https://saibugasinformations-dev.outsystemsenterprise.com/LunchOrder_Core/rest/LunchOrder/");

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("GetLunchOrder?LunchOrderId=" + intId.ToString()).Result;

                if (response.IsSuccessStatusCode)
                {
                    lunchOrder = response.Content.ReadAsAsync<LunchOrder>().Result;
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
            return lunchOrder;
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

                bool result = PostCreateOrderData(lunchOrderRequest);

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

                bool result = PostUpdateOrderData(lunchOrderRequest);

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

        public static bool PostCreateOrderData(LunchOrderCreateRequest request)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://saibugasinformations-dev.outsystemsenterprise.com/LunchOrder_Core/rest/LunchOrder/");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    client.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(1000000));

                    HttpResponseMessage response = new HttpResponseMessage();

                    response = client.PostAsJsonAsync("CreateLunchOrder", request).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else{
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool PostUpdateOrderData(LunchOrderUpdateRequest request)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://saibugasinformations-dev.outsystemsenterprise.com/LunchOrder_Core/rest/LunchOrder/");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    client.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(1000000));

                    HttpResponseMessage response = new HttpResponseMessage();

                    response = client.PostAsJsonAsync("UpdateLunchOrder", request).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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
