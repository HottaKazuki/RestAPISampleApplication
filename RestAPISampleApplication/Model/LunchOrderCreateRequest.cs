using System;

namespace RestAPISampleApplication.Model
{
    public class LunchOrderCreateRequest
    {
        public int LunchMenuId { get; set; }
        public string OrderId { get; set; }
        public string OrderPerson { get; set; }
        public string OrderDate { get; set; }
        public int Quantity { get; set; }

    }

}