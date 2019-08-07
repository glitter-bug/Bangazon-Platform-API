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
    public class TestEmployees
    {
        [Fact]
        public async Task Test_Get_All_Employees()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/Employees");


                string responseBody = await response.Content.ReadAsStringAsync();
                var employees = JsonConvert.DeserializeObject<List<Employee>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(employees.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Employees()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/Employees/2");


                string responseBody = await response.Content.ReadAsStringAsync();
                var employees = JsonConvert.DeserializeObject<Employee>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Garry", employees.FirstName);
                Assert.Equal("Levington", employees.LastName);
                Assert.Equal(2, employees.DepartmentId);
                //Assert.Equal(false, employees.IsSuperVisor);
                Assert.NotNull(employees);
            }
        }

        [Fact]
        public async Task Test_Post_Employees()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                Employee Deep = new Employee
                {
                    FirstName = "Deep",
                    LastName = "Patel",
                    DepartmentId = 1,
                    IsSuperVisor = true
                };

                var DeepAsJSON = JsonConvert.SerializeObject(Deep);
             


                /*
                    ACT
                */
                var response = await client.PostAsync("/api/Employees",
                     new StringContent(DeepAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();
                var employees = JsonConvert.DeserializeObject<Employee>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Deep", employees.FirstName);
                Assert.NotNull(employees);

      
            }
        }

        [Fact]
        public async Task Test_Put_Employees()
        {
            string NewFirstName = "Khusboo";


            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                Employee ModifiedKhusboo = new Employee
                {
                    FirstName = NewFirstName,
                    LastName = "Patel",
                    DepartmentId = 1,
                    IsSuperVisor = true
                };

                var ModifiedKhusbooAsJSON = JsonConvert.SerializeObject(ModifiedKhusboo);


                /*
                    ACT
                */
                var response = await client.PutAsync("/api/Employees/6",
                     new StringContent(ModifiedKhusbooAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var GetKhusboo = await client.GetAsync("/api/Employees/6");
                GetKhusboo.EnsureSuccessStatusCode();

                string GetKhusbooBody = await GetKhusboo.Content.ReadAsStringAsync();
                Employee NewKhusboo = JsonConvert.DeserializeObject<Employee>(GetKhusbooBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, GetKhusboo.StatusCode);
                Assert.Equal(NewFirstName, NewKhusboo.FirstName);
            }
        }


    }
}
