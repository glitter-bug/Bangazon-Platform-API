using System;
using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

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
        [Fact]
        public async Task Test_Create_And_Delete_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                Product BBJam = new Product
                {
                    Title = "Blueberry Jam",
                    Price = 3.50,
                    Description = "This jam is mmmm mmm so good in your tummy",
                    Quantity = 20,
                    CustomerId = 4,
                    ProductTypeId = 1

                };
                var BBJamAsJSON = JsonConvert.SerializeObject(BBJam);
                /*
                    ACT
                */
                var response = await client.PostAsync("/api/Products",
                     new StringContent(BBJamAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();
                var newProduct = JsonConvert.DeserializeObject<Product>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(BBJam.Price, newProduct.Price);
                Assert.Equal(BBJam.Title, newProduct.Title);
                Assert.Equal(BBJam.Description, newProduct.Description);
                Assert.Equal(BBJam.Quantity, newProduct.Quantity);
                Assert.Equal(BBJam.CustomerId, newProduct.CustomerId);
                Assert.Equal(BBJam.ProductTypeId, newProduct.ProductTypeId);
                Assert.NotNull(newProduct);

                /*
                    ACT
                */
                var deleteResponse = await client.DeleteAsync($"/api/Products/{newProduct.Id}");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Put_Product()
        {
            string newTitle = "Blackberry Jam";



            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                Product ModifiedRaspJam = new Product
                {
                    Title = newTitle,
                    Price = 3.50,
                    Description = "This jam is mmmm mmm so good in your tummy",
                    Quantity = 20,
                    CustomerId = 4,
                    ProductTypeId = 1
                };

                var ModifiedRaspJamAsJSON = JsonConvert.SerializeObject(ModifiedRaspJam);


                /*
                    ACT
                */
                var response = await client.PutAsync("/api/Products/3",
                     new StringContent(ModifiedRaspJamAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var GetRaspJam = await client.GetAsync("/api/Products/3");
                GetRaspJam.EnsureSuccessStatusCode();

                string GetRaspJamBody = await GetRaspJam.Content.ReadAsStringAsync();
                Product NewRaspJam = JsonConvert.DeserializeObject<Product>(GetRaspJamBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, GetRaspJam.StatusCode);
                Assert.Equal(newTitle, NewRaspJam.Title);
            }
        }
    }
}


