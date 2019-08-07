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
    public class TestComputers
    {
        [Fact]
        public async Task Test_Get_All_Computers()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/Computers");


                string responseBody = await response.Content.ReadAsStringAsync();
                var computers = JsonConvert.DeserializeObject<List<Computer>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(computers.Count > 0);
            }
        }
        [Fact]
        public async Task Test_Get_Single_Computer()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var response = await client.GetAsync("/api/Computers/1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var computer = JsonConvert.DeserializeObject<Computer>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("GFE-744", computer.Make);

            }
        }
        [Fact]
        public async Task Test_Create_And_Delete_Computer()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                Computer CompBoxNow = new Computer
                {
                    PurchaseDate = DateTime.Now,
                    DecomissonDate = DateTime.Now.AddYears(10),
                    Make = "GLP-500",
                    Manufacturer = "Advanced Solutions"
                };
                var CompBoxNowAsJSON = JsonConvert.SerializeObject(CompBoxNow);
                /*
                    ACT
                */
                var response = await client.PostAsync("/api/Computers",
                     new StringContent(CompBoxNowAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();
                var newComputer = JsonConvert.DeserializeObject<Computer>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);


                /*
                    ACT
                */
                var deleteResponse = await client.DeleteAsync($"/api/Computers/{newComputer.Id}");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Put_Computers()
        {
            string newMake = "XSG-540";



            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                Computer ModifiedXSG540 = new Computer
                {
                    //Make = newMake,
                    //PurchaseDate = DateTime.Now,
                    //DecomissonDate = DateTime.Now.AddYears(8),
                    //Manufacturer = 
                };

                var ModifiedXSG540AsJSON = JsonConvert.SerializeObject(ModifiedXSG540);


                /*
                    ACT
                */
                var response = await client.PutAsync("/api/Computers/2",
                     new StringContent(ModifiedXSG540AsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var GetXSG = await client.GetAsync("/api/Computers/2");
                GetXSG.EnsureSuccessStatusCode();

                string GetXSGBody = await GetXSG.Content.ReadAsStringAsync();
                Computer NewXSG540 = JsonConvert.DeserializeObject<Computer>(GetXSGBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, GetXSG.StatusCode);
                Assert.Equal(newMake, NewXSG540.Make);
            }
        }
    }
}


