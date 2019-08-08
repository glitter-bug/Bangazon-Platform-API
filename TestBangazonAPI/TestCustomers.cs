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
    public class TestCustomers
    {
        [Fact]
        public async Task Test_Get_All_Customers()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/customers");


                string responseBody = await response.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<List<Customer>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(customers.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_All_Customers_Q()
        {
            using (var client = new APIClientProvider(). Client)
            {
                //ACT
                var responseQ = await client.GetAsync("/api/customers?q=a");

                string responseBodyQ = await responseQ.Content.ReadAsStringAsync();
                var customersQ = JsonConvert.DeserializeObject<List<Customer>>(responseBodyQ);
                
                //ASSERT
                Assert.Equal(HttpStatusCode.OK, responseQ.StatusCode);
                Assert.Contains("a", customersQ[1].FirstName);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Customer()
        {

        using (var client = new APIClientProvider().Client)
        {
            /*
                ARRANGE
            */

            /*
                ACT
            */
            var response = await client.GetAsync("/api/customers/2");

            string responseBody = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(responseBody);

            /*
                ASSERT
            */
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Liè", customer.FirstName);
                Assert.Equal("Dible", customer.LastName);
                Assert.NotNull(customer);
            }
        }

        [Fact]
        public async Task Test_Get_Single_By_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/customers/2?_include=products");

                string responseBody = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<Customer>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.NotEmpty(customer.Products);
            }
        }

        [Fact]
        public async Task Test_Get_Single_By_Payments()
        {
           using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/customers/1?_include=payments");

                string responseBody = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<Customer>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                //Assert.NotEmpty(customer.PaymentTypes);
            }
        }

        [Fact]
        public async Task Test_Create_Customer()
        {
            using(var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                Customer John = new Customer
                {
                    FirstName = "John",
                    LastName = "Doe"
                };

                var JohnAsJSON = JsonConvert.SerializeObject(John);

                /*
                    ACT
                */

                var response = await client.PostAsync(
                    "/api/customers",
                    new StringContent(JohnAsJSON, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();

                Customer newJohn = JsonConvert.DeserializeObject<Customer>(responseBody);

                /*
                     ASSERT
                */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("John", newJohn.FirstName);
                Assert.Equal("Doe", newJohn.LastName);
            }

        }

        [Fact]
        public async Task Test_Modify_Customer()
        {
            string newFirstName = "Rob";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                Customer ModifiedJohn= new Customer
                {
                    FirstName = newFirstName,
                    LastName = "Zombie"
                };
                var ModifiedJohnAsJSON = JsonConvert.SerializeObject(ModifiedJohn);

                var response = await client.PutAsync(
                    "/api/customers/3",
                    new StringContent(ModifiedJohnAsJSON, Encoding.UTF8, "application/json")
                    );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                */
                var GetJohn = await client.GetAsync("/api/customers/3");
                GetJohn.EnsureSuccessStatusCode();

                string GetJohnBody = await GetJohn.Content.ReadAsStringAsync();
                Customer NewJohn = JsonConvert.DeserializeObject<Customer>(GetJohnBody);

                Assert.Equal(HttpStatusCode.OK, GetJohn.StatusCode);
                Assert.Equal(newFirstName, NewJohn.FirstName);
            }
        }
    }
}
