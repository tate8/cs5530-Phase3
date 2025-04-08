﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private LMSContext db;
        public StudentController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {           
            var query = from e in db.Enrolleds where e.UId == uid
                        select new {
                            subject = e.Class.Course.DeptAbrv,
                            number = e.Class.Course.CNum,
                            name = e.Class.Course.CName,
                            season = e.Class.Season,
                            year = e.Class.Year,
                            grade = e.Grade,
                        };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {   
            var assignments = from co in db.Courses
                                where co.DeptAbrv == subject && co.CNum == num
                                from c in co.Classes
                                where c.Season == season && c.Year == year
                                from cat in c.AssignmentCategories
                                from a in cat.Assignments
                                select a;

            var res = from a in assignments
                            join s in db.Submissions
                            on new { A = a.AssignId, B = uid } equals new { A = s.AssignId, B = s.UId }
                            into joined
                            from j in joined.DefaultIfEmpty()
                            select new {
                                aname = a.Name,
                                cname = a.Cat.Name,
                                due = a.Due,
                                score = j.Score,
                            };

            return Json(res.ToArray());
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
        {
            var existingSubmission = from co in db.Courses
                        where co.DeptAbrv == subject && co.CNum == num
                        from c in co.Classes
                        where c.Season == season && c.Year == year
                        from cat in c.AssignmentCategories
                        where cat.Name == category
                        from a in cat.Assignments
                        where a.Name == asgname
                        from s in a.Submissions
                        where s.UId == uid
                        select s;
            
            // if submission exists, update
            if (existingSubmission.Any()) {
                existingSubmission.Single().Answer = contents;
                existingSubmission.Single().SubmitTime = DateTime.Now;
            // else create new
            } else {
                var asgnId = from co in db.Courses
                            where co.DeptAbrv == subject && co.CNum == num
                            from c in co.Classes
                            where c.Season == season && c.Year == year
                            from cat in c.AssignmentCategories
                            where cat.Name == category
                            from a in cat.Assignments
                            where a.Name == asgname
                            select a.AssignId;

                Submission newSubmission = new();
                newSubmission.Answer = contents;
                newSubmission.SubmitTime = DateTime.Now;
                newSubmission.UId = uid;
                newSubmission.Score = 0;
                newSubmission.AssignId = asgnId.Single();

                db.Submissions.Add(newSubmission);
            }

            try {
                db.SaveChanges();
            } catch {
                return Json(new { success = false });    
            }

            return Json(new { success = true });
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false}. 
        /// false if the student is already enrolled in the class, true otherwise.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {          
            var classId = from co in db.Courses
                            where co.DeptAbrv == subject && co.CNum == num
                            from c in co.Classes
                            where c.Season == season && c.Year == year
                            select c.ClassId;

            Enrolled newEnrollment = new();
            newEnrollment.ClassId = classId.Single();
            newEnrollment.UId = uid;
            newEnrollment.Grade = "--";

            db.Enrolleds.Add(newEnrollment);

            try {
                db.SaveChanges();
            } catch {
                return Json(new { success = false });    
            }

            return Json(new { success = true});
        }



        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student is not enrolled in any classes, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {
            var classesWithGrades = from e in db.Enrolleds where e.UId == uid && e.Grade != "--"
                                    select e;
            // if no enrolled classes
            if (!classesWithGrades.Any()) {
                return Json(new { gpa = 0.0 });
            }

            List<double> GPAs = new();

            foreach (var c in classesWithGrades) {
                GPAs.Add(GradeToGPA(c.Grade));
            }

            double averageGPA = GPAs.Sum() / GPAs.Count;

            return Json(new { gpa = averageGPA });
        }


        private double GradeToGPA(string grade) {
            if (grade == "A") return 4.0;
            else if (grade == "A-") return 3.7;
            else if (grade == "B+") return 3.3;
            else if (grade == "B") return 3.0;
            else if (grade == "B-") return 2.7;
            else if (grade == "C+") return 2.3;
            else if (grade == "C") return 2.0;
            else if (grade == "C-") return 1.7;
            else if (grade == "D+") return 1.3;
            else if (grade == "D") return 1.0;
            else if (grade == "D-") return 0.7;
            else return 0.0;
        }
                
        /*******End code to modify********/

    }
}

