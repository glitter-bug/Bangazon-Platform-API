using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

using System.Threading.Tasks;
using Xunit;

using BangazonAPI.Models;
using System.Net.Http;
using System.Text;

namespace TestBangazonAPI
{
    public class TestPayementTypes
    {
        [Fact]
        public async Task Test_Get_All_PaymentTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/paymenttypes");


                string responseBody = await response.Content.ReadAsStringAsync();
                var paymentTypes = JsonConvert.DeserializeObject<List<PaymentType>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(paymentTypes.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_PaymentType()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var response = await client.GetAsync("/api/paymenttypes/1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var paymenttype = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
               

            }
        }

        [Fact]
        public async Task Test_Get_NonExitant_PaymentType_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var response = await client.GetAsync("/api/paymenttypes/999999999");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Create_And_Delete_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                PaymentType Credit = new PaymentType
                {
                    AcctNumber= 3432,
                    Name = "Visa",
                    CustomerId = 2
                };
                var CreditAsJSON = JsonConvert.SerializeObject(Credit);

                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/paymenttypes",
                    new StringContent(CreditAsJSON, Encoding.UTF8, "application/json")
                );


                string responseBody = await response.Content.ReadAsStringAsync();
                var NewCredit = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(Credit.AcctNumber, NewCredit.AcctNumber);
                Assert.Equal(Credit.Name, NewCredit.Name);
                Assert.Equal(Credit.CustomerId, NewCredit.CustomerId);
                /*
                    ACT
                */
                var deleteResponse = await client.DeleteAsync($"/api/paymenttypes/{NewCredit.Id}");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_PaymentType_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var deleteResponse = await client.DeleteAsync("/api/paymenttypes/600000");

                /*
                    ASSERT
                */
                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_PaymentType()
        {
            // New eating habit value to change to and test
            

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                PaymentType ModifiedCard = new PaymentType
                {
                    AcctNumber = 2,
                    Name = "Visa",
                    CustomerId = 3
                };
                var ModifiedCardAsJSON = JsonConvert.SerializeObject(ModifiedCard);

                var response = await client.PutAsync(
                    "/api/paymenttypes/1",
                    new StringContent(ModifiedCardAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var GetCard = await client.GetAsync("/api/paymenttypes/1");
                GetCard.EnsureSuccessStatusCode();

                string GetCardBody = await GetCard.Content.ReadAsStringAsync();
                PaymentType NewCard= JsonConvert.DeserializeObject<PaymentType>(GetCardBody);

                Assert.Equal(HttpStatusCode.OK, GetCard.StatusCode);
               
            }
        }
    }
}


  



