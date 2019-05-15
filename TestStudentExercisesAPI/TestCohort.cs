using Newtonsoft.Json;
using StudentExercises.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Linq;


namespace TestStudentExercisesAPI
{
    public class TestCohort

    {

        public class TestCohorts
        {
            public async Task<Cohort> createCohortTest(HttpClient client)
            {
                Cohort CohortTest = new Cohort
                {
                    Name = "Test"
                };

                string TestAsJSON = JsonConvert.SerializeObject(CohortTest);

                HttpResponseMessage response = await client.PostAsync("api/cohort",
                    new StringContent(TestAsJSON, Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                Cohort newCohort = JsonConvert.DeserializeObject<Cohort>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);

                return newCohort;
            }

            public async Task deleteTest(Cohort test, HttpClient client)
            {
                HttpResponseMessage deleteResponse = await client.DeleteAsync($"api/cohort/{test.Id}");
                deleteResponse.EnsureSuccessStatusCode();
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }


            [Fact]
            public async Task Test_Get_All_Cohorts()
            {
                using (HttpClient client = new APIClientProvider().Client)
                {
                    HttpResponseMessage response = await client.GetAsync("api/cohort");

                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    List<Cohort> cohortList = JsonConvert.DeserializeObject<List<Cohort>>(responseBody);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                    Assert.True(cohortList.Count > 0);
                }
            }


            [Fact]
            public async Task Test_Get_Single_Cohort()
            {
                using (HttpClient client = new APIClientProvider().Client)
                {
                    Cohort newCohort = await createCohortTest(client);

                    HttpResponseMessage response = await client.GetAsync($"api/cohort/{newCohort.Id}");

                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();

                    Cohort cohort = JsonConvert.DeserializeObject<Cohort>(responseBody);

                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.Equal("Test", newCohort.Name);

                    deleteTest(newCohort, client);

                }
            }
        }













    }
}
