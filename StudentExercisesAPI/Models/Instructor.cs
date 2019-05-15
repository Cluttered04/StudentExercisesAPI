namespace StudentExercises.Models
{


    // A method to assign an exercise to a student
    // Exercise
    public class Instructor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SlackHandle { get; set; }
        public int CohortId { get; set; }
        public Cohort InstructorCohort { get; set; }
 
        public void assignExercise(Student student, Exercise exercise)
        {
            student.CurrentExercises.Add(exercise);
        }
    }


}