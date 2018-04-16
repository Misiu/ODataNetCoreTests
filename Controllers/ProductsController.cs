using System.Collections.Generic;
using System.Linq;
using CoreTest1.Models;
using Microsoft.AspNet.OData;

namespace CoreTest1.Controllers
{
	public class ProductsController : ODataController
	{
		private readonly List<Product> products = new List<Product>()
		{
			new Product()
			{
				ID = 1,
				Name = "Bread",
				Category = Category.Food
			},
			new Product()
			{
				ID = 2,
				Name = "Handbreak",
				Category = Category.Cars
			},
			new Product()
			{
				ID = 3,
				Name = "Tire",
				Category = Category.Cars
			},
			new Product()
			{
				ID = 4,
				Name = "Toy Car",
				Category = Category.Toys
			}
		};

		[EnableQuery]
		public IQueryable<Product> Get()
		{
			return products.AsQueryable();
		}
	}
}