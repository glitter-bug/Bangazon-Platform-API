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
    public class TestTrainingPrograms
    {
        [Fact]
        public async Task Test_Get_All_Training_Programs()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/TrainingPrograms");


                string responseBody = await response.Content.ReadAsStringAsync();
                var trainingPrograms = JsonConvert.DeserializeObject<List<TrainingProgram>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(trainingPrograms.Count > 0);
            }
        }
        [Fact]
        public async Task Test_Get_Single_TrainingProgram()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var response = await client.GetAsync("/api/TrainingPrograms/1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var trainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("York University", trainingProgram.Name);
                //Assert.Equal(2018-09-12 00:00:00.000, trainingProgram.StartDate);
                //Assert.Equal(2020-04-29 00:00:00.000, trainingProgram.EndDate);
                //Assert.Equal(82, trainingProgram.MaxAttendees);
                //Assert.NotNull(trainingProgram);
            }
        }
        //[Fact]
        //public async Task Test_Create_And_Delete_TrainingProgram()
        //{
        //    using (var client = new APIClientProvider().Client)
        //    {
        //        /*
        //            ARRANGE
        //        */
        //        TrainingProgram GameHenU = new TrainingProgram
        //        {
        //            Name = "Cornish Game Hen University for Champions",
        //            StartDate = DateTime.Now,
        //            EndDate = DateTime.Now.AddDays(30),
        //            MaxAttendees = 5
        //        };
        //        var GameHenUAsJSON = JsonConvert.SerializeObject(GameHenU);
        //        /*
        //            ACT
        //        */
        //        var response = await client.PostAsync("/api/TrainingPrograms",
        //             new StringContent(GameHenUAsJSON, Encoding.UTF8, "application/json"));


        //        string responseBody = await response.Content.ReadAsStringAsync();
        //        var newTP = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);
        //        /*
        //            ASSERT
        //        */
        //        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        //        //Assert.Equal(BBJam.Price, newProduct.Price);
        //        //Assert.Equal(BBJam.Title, newProduct.Title);
        //        //Assert.Equal(BBJam.Description, newProduct.Description);
        //        //Assert.Equal(BBJam.Quantity, newProduct.Quantity);
        //        //Assert.Equal(BBJam.CustomerId, newProduct.CustomerId);
        //        //Assert.Equal(BBJam.ProductTypeId, newProduct.ProductTypeId);
        //        //Assert.NotNull(newProduct); deletetrainingprogramfuturestardate

        //        /*
        //            ACT
        //        */
        //        var deleteResponse = await client.DeleteAsync($"/api/TrainingPrograms/{newTP.Id}");

        //        /*
        //            ASSERT
        //        */
        //        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        //    }
        //}

        [Fact]
        public async Task Test_Put_TrainingProgram()
        {
            string newName = "Feather Factory Learning Center For Soft People";



            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                TrainingProgram ModifiedFeatherFactory = new TrainingProgram
                {
                    Name = newName,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(30),
                    MaxAttendees = 5
                };

                var ModifiedFeatherFactoryAsJSON = JsonConvert.SerializeObject(ModifiedFeatherFactory);


                /*
                    ACT
                */
                var response = await client.PutAsync("/api/TrainingPrograms/2",
                     new StringContent(ModifiedFeatherFactoryAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var GetFeatherFactory = await client.GetAsync("/api/TrainingPrograms/2");
                GetFeatherFactory.EnsureSuccessStatusCode();

                string GetFeatherFactoryBody = await GetFeatherFactory.Content.ReadAsStringAsync();
                TrainingProgram NewFeatherFactory = JsonConvert.DeserializeObject<TrainingProgram>(GetFeatherFactoryBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, GetFeatherFactory.StatusCode);
                Assert.Equal(newName, NewFeatherFactory.Name);
            }
        }
        [Fact]
        public async Task Test_Post_TrainingProgram()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                TrainingProgram trainingProgram = new TrainingProgram()
                {
                    Name = "Christmas Scented Basket Weaving Class ",
                    StartDate = new DateTime(2045, 12, 25),
                    EndDate = new DateTime(2046, 05, 11),
                    MaxAttendees = 300
                };

                var trainingProgramAsJSON = JsonConvert.SerializeObject(trainingProgram);

                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/TrainingPrograms",
                    new StringContent(trainingProgramAsJSON, Encoding.UTF8, "application/json")
                    );


                string responseBody = await response.Content.ReadAsStringAsync();
                var newTP = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.True(newTP.Id != 0);
                Assert.Equal(newTP.Name, trainingProgram.Name);
                Assert.Equal(newTP.MaxAttendees, trainingProgram.MaxAttendees);
            }
        }
        [Fact]
        public async Task Test_Delete_TrainingPrograms_InTheFuture()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                TrainingProgram trainingProgram = new TrainingProgram()
                {
                    Name = "Super Deluxe Leadership Retreat For Men",
                    StartDate = new DateTime(2055, 04, 21),
                    EndDate = new DateTime(2055, 06, 22),
                    MaxAttendees = 8
                };

                var trainingProgramAsJSON = JsonConvert.SerializeObject(trainingProgram);


                var response = await client.PostAsync(
                    "/api/TrainingPrograms",
                    new StringContent(trainingProgramAsJSON, Encoding.UTF8, "application/json")
                    );


                string responseBody = await response.Content.ReadAsStringAsync();
                var newTP = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);


                /*
                    ACT
                */
                var deleteResponse = await client.DeleteAsync($"/api/TrainingPrograms/{newTP.Id}");


                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
                // If the future training program is deleted it wont exist, therefore, the user will be given a NoContent status code.
            }
        }

        [Fact]
        public async Task Test_Delete_TrainingPrograms_ThatDontExist()
        {


            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var deleteResponse = await client.DeleteAsync($"/api/traingingPrograms/99999999");


                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
                // This tests when a user tries to delete a Training Program that doesnt exist in the first place. They are given  NotFound status code.
            }
        }

        [Fact]
        public async Task Test_Delete_NotFuture_TrainingPrograms()
        {


            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var deleteResponse = await client.DeleteAsync($"/api/TrainingPrograms/2");


                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Forbidden, deleteResponse.StatusCode);
                // user may only delete training programs that have a start date in the future. If the program is already in progress, the user is given a forbidden status code.
            }
        }


    }
}

