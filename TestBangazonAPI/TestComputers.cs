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
        public async Task Test_Create_And_Delete_TrainingProgram()
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
                var response = await client.PostAsync("/api/TrainingPrograms",
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

        //[Fact]
        //public async Task Test_Put_TrainingProgram()
        //{
        //    string newName = "Feather Factory Learning Center For Soft People";



        //    using (var client = new APIClientProvider().Client)
        //    {
        //        /*
        //            ARRANGE
        //        */
        //        TrainingProgram ModifiedFeatherFactory = new TrainingProgram
        //        {
        //            Name = newName,
        //            StartDate = DateTime.Now,
        //            EndDate = DateTime.Now.AddDays(30),
        //            MaxAttendees = 5
        //        };

        //        var ModifiedFeatherFactoryAsJSON = JsonConvert.SerializeObject(ModifiedFeatherFactory);


        //        /*
        //            ACT
        //        */
        //        var response = await client.PutAsync("/api/TrainingPrograms/2",
        //             new StringContent(ModifiedFeatherFactoryAsJSON, Encoding.UTF8, "application/json"));


        //        string responseBody = await response.Content.ReadAsStringAsync();

        //        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        //        var GetFeatherFactory = await client.GetAsync("/api/TrainingPrograms/2");
        //        GetFeatherFactory.EnsureSuccessStatusCode();

        //        string GetFeatherFactoryBody = await GetFeatherFactory.Content.ReadAsStringAsync();
        //        TrainingProgram NewFeatherFactory = JsonConvert.DeserializeObject<TrainingProgram>(GetFeatherFactoryBody);

        //        /*
        //            ASSERT
        //        */
        //        Assert.Equal(HttpStatusCode.OK, GetFeatherFactory.StatusCode);
        //        Assert.Equal(newName, NewFeatherFactory.Name);
        //    }
        //}
    }
}


