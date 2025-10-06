namespace Assignment.DTO
{
    public class DeliveryInfoDTO
    {
        public int total_price {  get; set; }
        public int small_order_surcharge {  get; set; }
        public int cart_value {  get; set; }
        public DeliveryDTO? delivery {  get; set; }
    }
    public class DeliveryDTO
    {
        public int fee {  get; set; }
        public int distance {  get; set; }
    }
}
