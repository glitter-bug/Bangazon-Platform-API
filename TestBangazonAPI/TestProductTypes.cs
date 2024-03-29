﻿using System;
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
    public class TestProductTypes
    {
        [Fact]
        public async Task Test_Get_All_ProductTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/ProductTypes");


                string responseBody = await response.Content.ReadAsStringAsync();
                var productTypes = JsonConvert.DeserializeObject<List<ProductType>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productTypes.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_ProductTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/ProductTypes/2");


                string responseBody = await response.Content.ReadAsStringAsync();
                var productTypes = JsonConvert.DeserializeObject<ProductType>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Electronics", productTypes.Name);
                Assert.NotNull(productTypes);
            }
        }

        [Fact]
        public async Task Test_Post_And_Delete_ProductTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                ProductType Clothes = new ProductType
                {
                    Name = "Clothes"
                };

                var ClothesAsJSON = JsonConvert.SerializeObject(Clothes);


                /*
                    ACT
                */
                var response = await client.PostAsync("/api/ProductTypes",
                     new StringContent(ClothesAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();
                var productTypes = JsonConvert.DeserializeObject<ProductType>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Clothes", productTypes.Name);
                Assert.NotNull(productTypes);

                var deleteResponse = await client.DeleteAsync($"/api/ProductTypes/{productTypes.Id}");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Put_ProductTypes()
        {
            string NewName = "Toys";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                ProductType ModifiedClothes = new ProductType
                {
                    Name = NewName,
                };

                var ModifiedClothesAsJSON = JsonConvert.SerializeObject(ModifiedClothes);


                /*
                    ACT
                */
                var response = await client.PutAsync("/api/ProductTypes/3",
                     new StringContent(ModifiedClothesAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var GetClothes = await client.GetAsync("/api/ProductTypes/3");
                GetClothes.EnsureSuccessStatusCode();

                string GetClothesBody = await GetClothes.Content.ReadAsStringAsync();
                ProductType NewClothes = JsonConvert.DeserializeObject<ProductType>(GetClothesBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, GetClothes.StatusCode);
                Assert.Equal(NewName, NewClothes.Name);
            }
        }

       
    }
}
