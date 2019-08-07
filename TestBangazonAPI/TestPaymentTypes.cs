using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

using System.Threading.Tasks;
using Xunit;

using BangazonAPI.Models;

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
    }
}



