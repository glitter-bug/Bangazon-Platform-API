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
    public class TestDepartments
    {
        [Fact]
        public async Task Test_Get_All_Departments()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/Departments");


                string responseBody = await response.Content.ReadAsStringAsync();
                var departments = JsonConvert.DeserializeObject<List<Department>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(departments.Count > 0);
            }
        }
        [Fact]
        public async Task Test_Get_Single_Department()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var response = await client.GetAsync("/api/Departments/1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var department = JsonConvert.DeserializeObject<Department>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Marketing", department.Name);
            }
        }
        [Fact]
        public async Task Test_Post_Department()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                Department Accounting = new Department
                {
                    Name = "Accounting",
                    Budget = 40000
                };
                var AccountingAsJSON = JsonConvert.SerializeObject(Accounting);
                /*
                    ACT
                */
                var response = await client.PostAsync("/api/Departments",
                     new StringContent(AccountingAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();
                var newDP = JsonConvert.DeserializeObject<Department>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(Accounting.Name, newDP.Name);
            }
        }

        [Fact]
        public async Task Test_Put_Department()
        {
            string newName = "HR";



            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                Department ModifiedHR = new Department
                {
                    Name = newName,
                    Budget = 31221
                };

                var ModifiedHRAsJSON = JsonConvert.SerializeObject(ModifiedHR);


                /*
                    ACT
                */
                var response = await client.PutAsync("/api/Departments/2",
                     new StringContent(ModifiedHRAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var GetHR = await client.GetAsync("/api/Departments/2");
                GetHR.EnsureSuccessStatusCode();

                string GetHRBody = await GetHR.Content.ReadAsStringAsync();
                Department NewHR = JsonConvert.DeserializeObject<Department>(GetHRBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, GetHR.StatusCode);
                Assert.Equal(newName, NewHR.Name);
            }
        }
    }
}