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

    public class TestExercise
    {

        // Since we need to clean up after ourselves, we'll create and delete a student when we test POST and PUT
        // Otherwise, eveyr time we ran our test suite it would create a new David entry and we'd end up with a tooon of Davids

        // Create a new student in the db and make sure we get a 200 OK status code back
        public async Task<Exercise> createChickenMonkey(HttpClient client)
        {
            Exercise ChickenMonkey = new Exercise
            {
                Name = "Chicken Monkey",
                Language = "Javascript"
            };
            string chickenMonkeyAsJSON = JsonConvert.SerializeObject(ChickenMonkey);


            HttpResponseMessage response = await client.PostAsync(
                "api/exercise",
                new StringContent(chickenMonkeyAsJSON, Encoding.UTF8, "application/json")
            );

            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            Exercise newChickenMonkey = JsonConvert.DeserializeObject<Exercise>(responseBody);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            return newChickenMonkey;

        }

        // Delete a student in the database and make sure we get a no content status code back
        public async Task deleteChickenMonkey(Exercise ChickenMonkey, HttpClient client)
        {
            HttpResponseMessage deleteResponse = await client.DeleteAsync($"api/exercise/{ChickenMonkey.Id}");
            deleteResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        }


        [Fact]
        public async Task Test_Get_All_Exercises()
        {
            // Use the http client
            using (HttpClient client = new APIClientProvider().Client)
            {

                // Call the route to get all our students; wait for a response object
                HttpResponseMessage response = await client.GetAsync("api/exercise");

                // Make sure that a response comes back at all
                response.EnsureSuccessStatusCode();

                // Read the response body as JSON
                string responseBody = await response.Content.ReadAsStringAsync();

                // Convert the JSON to a list of student instances
                List<Exercise> exerciseList = JsonConvert.DeserializeObject<List<Exercise>>(responseBody);

                // Did we get back a 200 OK status code?
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                // Are there any students in the list?
                Assert.True(exerciseList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Exercise()
        {

            using (HttpClient client = new APIClientProvider().Client)
            {

                // Create a new student
                Exercise newChickenMonkey = await createChickenMonkey(client);

                // Try to get that student from the database
                HttpResponseMessage response = await client.GetAsync($"api/exercise/{newChickenMonkey.Id}");

                response.EnsureSuccessStatusCode();

                // Turn the response into JSON
                string responseBody = await response.Content.ReadAsStringAsync();

                // Turn the JSON into C#
                Exercise exercise = JsonConvert.DeserializeObject<Exercise>(responseBody);

                // Did we get back what we expected to get back? 
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Chicken Monkey", newChickenMonkey.Name);
                Assert.Equal("Javascript", newChickenMonkey.Language);

                // Clean up after ourselves- delete david!
                deleteChickenMonkey(newChickenMonkey, client);
            }
        }

        [Fact]
        public async Task Test_Get_NonExitant_Exercise_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                // Try to get a student with an enormously huge Id
                HttpResponseMessage response = await client.GetAsync("api/exercise/999999999");

                // It should bring back a 204 no content error
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }


        [Fact]
        public async Task Test_Create_And_Delete_Exercise()
        {
            using (var client = new APIClientProvider().Client)
            {

                // Create a new David
                Exercise newChickenMonkey = await createChickenMonkey(client);

                // Make sure his info checks out
                Assert.Equal("Chicken Monkey", newChickenMonkey.Name);
                Assert.Equal("Javascript", newChickenMonkey.Language);
                

                // Clean up after ourselves - delete David!
                deleteChickenMonkey(newChickenMonkey, client);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_Exercise_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Try to delete an Id that shouldn't exist in the DB
                HttpResponseMessage deleteResponse = await client.DeleteAsync("/api/exercise/600000");
                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_Exercise()
        {

            // We're going to change a student's name! This is their new name.
            string newName = "Chicken Monkey Monkey";

            using (HttpClient client = new APIClientProvider().Client)
            {

                // Create a new student
                Exercise newChickenMonkey = await createChickenMonkey(client);

                // Change their first name
                newChickenMonkey.Name = newName;

                // Convert them to JSON
                string modifiedChickenMonkeyAsJSON = JsonConvert.SerializeObject(newChickenMonkey);

                // Make a PUT request with the new info
                HttpResponseMessage response = await client.PutAsync(
                    $"api/exercise/{newChickenMonkey.Id}",
                    new StringContent(modifiedChickenMonkeyAsJSON, Encoding.UTF8, "application/json")
                );


                response.EnsureSuccessStatusCode();

                // Convert the response to JSON
                string responseBody = await response.Content.ReadAsStringAsync();

                // We should have gotten a no content status code
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                // Try to GET the student we just edited
                HttpResponseMessage getChickenMonkey = await client.GetAsync($"api/exercise/{newChickenMonkey.Id}");
                getChickenMonkey.EnsureSuccessStatusCode();

                string getChickenMonkeyBody = await getChickenMonkey.Content.ReadAsStringAsync();
                Exercise modifiedChickenMonkey = JsonConvert.DeserializeObject<Exercise>(getChickenMonkeyBody);

                Assert.Equal(HttpStatusCode.OK, getChickenMonkey.StatusCode);

                // Make sure his name was in fact updated
                Assert.Equal(newName, modifiedChickenMonkey.Name);

                // Clean up after ourselves- delete him
                deleteChickenMonkey(modifiedChickenMonkey, client);
            }
        }
    }
}

