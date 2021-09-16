namespace PowerIBroker.Models
{
    public class ManagePageSize
    {
        public int ManageContactUsEnquiriesPageSize { get; set; }
        public int ManageComapanyPageSize { get; set; }
        public int ManageACAPageSize { get; set; }
        public int CommonPageSize { get; set; }
        public ManagePageSize()
        {
            this.ManageContactUsEnquiriesPageSize = 50;
            this.ManageComapanyPageSize = 50;
            this.ManageACAPageSize = 50;
            this.CommonPageSize = 20;
        }

    }
}
