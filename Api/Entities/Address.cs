namespace Api.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string AddressAddition { get; set; }
        public int Zip { get; set; }
        public string City { get; set; }
    }
}