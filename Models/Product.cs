namespace CoreTest1.Models
{
	public class Product
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public Category Category { get; set; }
	}

	public enum Category
	{
		Toys = 1,
		Food = 2,
		Cars = 3
	}
}
