﻿using System.Collections.Generic;

namespace StudentExercises.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SlackHandle { get; set; }
        public Cohort Cohort { get; set; }
        public int CohortId { get; set; }
        public List<Exercise> CurrentExercises = new List<Exercise>();



    }




}