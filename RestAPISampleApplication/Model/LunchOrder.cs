using System;

namespace RestAPISampleApplication.Model
{
    public class LunchOrder
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public int LunchMenuId { get; set; }
        public string OrderPerson { get; set; }
        public DateTime OrderDate { get; set; }
        public int Quantity { get; set; }
        public string LunchMenuName { get; set; }
    }

}