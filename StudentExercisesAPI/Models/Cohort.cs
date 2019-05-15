using System.Collections.Generic;

namespace StudentExercises.Models
{
    public class Cohort
    {
        public string Name { get; set; }
        public int Id { get; set; }

        public List<Student> Students = new List<Student>();

        public List<Instructor> Instructors = new List<Instructor>();
    }



}