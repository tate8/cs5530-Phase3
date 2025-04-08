using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS_CustomIdentity.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {

        private readonly LMSContext db;

        public ProfessorController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
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

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
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

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var students = from co in db.Courses
                           where co.CName == subject && co.CNum == num
                           from c in co.Classes
                           where c.Season == season && c.Year == year
                           from e in db.Enrolleds
                           where e.ClassId == c.ClassId
                           from s in db.Students
                           where s.UId == c.UId
                           select new
                           {
                               fname = s.FirstName,
                               lname = s.LastName,
                               uid = s.UId,
                               dob = s.Dob,
                               grade = e.Grade
                           };
            return Json(students.ToArray());
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            if (category == null)
            {
                var categories = from co in db.Courses
                                 where co.DeptAbrv == subject && co.CNum == num
                                 from c in co.Classes
                                 where c.Season == season && c.Year == year
                                 from cat in c.AssignmentCategories
                                 from a in cat.Assignments
                                 select new
                                 {
                                     aname = a.Name,
                                     cname = cat.Name,
                                     due = a.Due,
                                     submissions = a.Submissions.Count()
                                 };
                return Json(categories.ToArray());
            }
            else
            {
                var categories = from co in db.Courses
                                 where co.DeptAbrv == subject && co.CNum == num
                                 from c in co.Classes
                                 where c.Season == season && c.Year == year
                                 from cat in c.AssignmentCategories
                                 where cat.Name == category
                                 from a in cat.Assignments
                                 select new
                                 {
                                     aname = a.Name,
                                     cname = cat.Name,
                                     due = a.Due,
                                     submissions = a.Submissions.Count()
                                 };
                return Json(categories.ToArray());
            }
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var categories = from co in db.Courses
                             where co.DeptAbrv == subject && co.CNum == num
                             from c in co.Classes
                             where c.Season == season && c.Year == year
                             from cat in c.AssignmentCategories
                             select new
                             {
                                 name = cat.Name,
                                 weight = cat.Weight
                             };
            return Json(categories.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            var classQuery = from co in db.Courses
                             where co.DeptAbrv == subject && co.CNum == num
                             from c in co.Classes
                             where c.Season == season && c.Year == year
                             select c;
            var singleClass = classQuery.Single();
            if (singleClass == null)
            {
                return Json(new { success = false });
            }
            AssignmentCategory newCat = new AssignmentCategory();
            newCat.Name = category;
            newCat.Weight = (byte)catweight;
            newCat.ClassId = singleClass.ClassId;
            singleClass.AssignmentCategories.Add(newCat);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            // TODO grades
            var catID = from co in db.Courses
                             where co.DeptAbrv == subject && co.CNum == num
                             from c in co.Classes
                             where c.Season == season && c.Year == year
                             from cat in c.AssignmentCategories
                             where cat.Name == category
                             select cat.CatId;
            if (catID == null)
            {
                return Json(new { success = false });
            }
            Assignment newAssign = new Assignment();
            newAssign.CatId = (uint)catID.Single();
            newAssign.Name = asgname;
            newAssign.MaxPoints = (uint)asgpoints;
            newAssign.Due = asgdue;
            newAssign.Contents= asgcontents;
            db.Assignments.Add(newAssign);
            try
            {
                db.SaveChanges();
            }
            catch 
            {
                return Json(new { success = false });
            }
            bool success = AutoGrade(subject, num, season, year);

            return Json(new { success = true });
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            var submissions = from co in db.Courses
                              where co.DeptAbrv == subject && co.CNum == num
                              from c in co.Classes
                              where c.Season == season && c.Year == year
                              from cat in c.AssignmentCategories
                              where cat.Name == category
                              from a in cat.Assignments
                              where a.Name == asgname
                              from s in a.Submissions
                              select new
                              {
                                  fname = s.UIdNavigation.FirstName,
                                  lname = s.UIdNavigation.LastName,
                                  uid = s.UIdNavigation.UId,
                                  time = s.SubmitTime,
                                  score = s.Score
                              };
            return Json(submissions.ToArray());
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            // TODO grades
            var submission = from co in db.Courses
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
            Submission updateSub = submission.Single();
            updateSub.Score = (uint)score;
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return Json(new { success = false });
            }

            bool success = AutoGrade(subject, num, season, year);


            return Json(new { success = true });
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var classes = from p in db.Professors
                          where p.UId == uid
                          from c in p.Classes
                          select new
                          {
                              subject = c.Course.DeptAbrv,
                              number = c.Course.CNum,
                              name = c.Course.CName,
                              season = c.Season,
                              year = c.Year
                          };
            return Json(classes.ToArray());
        }


        private string GradePercentageToString(double gradePercentage) {
            if (gradePercentage >= 93) {
                return "A";
            } else if (gradePercentage >= 90) {
                return "A-";
            } else if (gradePercentage >= 87) {
                return "B+";
            } else if (gradePercentage >= 83) {
                return "B";
            } else if (gradePercentage >= 80) {
                return "B-";
            } else if (gradePercentage >= 77) {
                return "C+";
            } else if (gradePercentage >= 73) {
                return "C";
            } else if (gradePercentage >= 70) {
                return "C-";
            } else if (gradePercentage >= 67) {
                return "D+";
            } else if (gradePercentage >= 63) {
                return "D";
            } else if (gradePercentage >= 60) {
                return "D-";
            } else {
                return "E";
            }
        }

        public bool AutoGrade(string subject, int num, string season, int year)
        {
            var classToGrade = from c in db.Classes where
                        c.Season == season && c.Year == year
                        && c.Course.DeptAbrv == subject && c.Course.CNum == num
                        select c;
            var classId = classToGrade.Single().ClassId;

            var enrolled_in_class = from e in db.Enrolleds where e.ClassId == classId
                        select e;

            // for each student
            foreach (var enrollment in enrolled_in_class) {
                double scaledCategoryPoints = 0;
                double numCategories = 0;
                // calculate points they got on all categories
                foreach(var cat in classToGrade.Single().AssignmentCategories) {
                    uint studentCategoryPts = 0;
                    uint totalCategoryPts = 0;

                    // calculate total points student earned for this category
                    foreach (var assignment in cat.Assignments) {
                        totalCategoryPts += assignment.MaxPoints;

                        uint studentScore = 0;

                        foreach (var submission in assignment.Submissions) {
                            if (submission.UId == enrollment.UId) {
                                studentScore = submission.Score ?? 0;
                            }
                        }
                        
                        studentCategoryPts += studentScore;
                    }

                    double percentage = studentCategoryPts / totalCategoryPts;
                    double weightedPoints = percentage * cat.Weight;
                    numCategories += 1;
                    scaledCategoryPoints += weightedPoints;
                }

                // re-scale points to be out of 100
                double studentPercentageInClass;
                if (scaledCategoryPoints == 0)
                {
                    studentPercentageInClass = 0;
                }
                else
                {
                    double scalingFactor = 100 / scaledCategoryPoints; 
                    studentPercentageInClass = scaledCategoryPoints * scalingFactor;
                }

                string studentGrade = GradePercentageToString(studentPercentageInClass);

                enrollment.Grade = studentGrade;

                // return false if it fails
                try {
                    db.SaveChanges();
                } catch {
                    return false;
                }
            }


            return true;

            

            /*

            class_to_grade = ...
            for each student enrolled in class_to_grade:
                category_totals = []
                for each category in class_to_grade.assignmentCategories:
                    student_category_pts = 0
                    for each assignment in category:
                        student_category_pts += student grade on assignment or 0 if not found
                    
                    percentage = student_category_pts / total_pts
                    weighted = percentage * weight
                    category_totals.append(weighted)

                # need to rescale
                total = sum(category_totals)
                scaling_factor = 100 / total
                
                total *= scaling_factor

                grade = convert_to_letter_grade(total)


                class = ...

                from s in db.Students
                    join e in db.Enrolled on s.UId equals e.UId
                    join ac in db.AssignmentCategories on e.ClassId equals ac.ClassId
                    join a in db.Assignments on ac.CatId equals a.CatId
                    select new {
                    
                    }


                    join sub in db.Submissions on a.AssignId equals sub.AssignId
                    into rightSide
                    from r in rightSide.DefaultIfEmpty()

                    group by ac

                    select new
                    {

                    }

                    where ClassId == class
            */

        }
        
        /*******End code to modify********/
    }
}

