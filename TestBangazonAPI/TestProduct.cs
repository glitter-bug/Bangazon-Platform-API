using System;
using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TestBangazonAPI
{
    public class TestProducts
    {
        [Fact]
        public async Task Test_Get_All_Products()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/products");


                string responseBody = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<Product>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(products.Count > 0);
                Assert.NotNull(products[0].Customer);
                Assert.NotNull(products[0].ProductType);

            }
        }
        [Fact]
        public async Task Test_Get_Single_Product()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var response = await client.GetAsync("/api/Products/1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<Product>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Honey - Liquid", product.Title);
                Assert.Equal(0.6100, product.Price);
                Assert.Equal("ligula suspendisse ornare consequat lectus in est", product.Description);
                Assert.Equal(25, product.Quantity);
                Assert.Equal(2, product.CustomerId);
                Assert.Equal(1, product.ProductTypeId);
                Assert.NotNull(product);
            }
        }
    }
}
