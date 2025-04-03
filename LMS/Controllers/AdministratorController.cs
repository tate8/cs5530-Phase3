using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS.Controllers
{
    public class AdministratorController : Controller
    {
        private readonly LMSContext db;

        public AdministratorController(LMSContext _db)
        {
            db = _db;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Department(string subject)
        {
            ViewData["subject"] = subject;
            return View();
        }

        public IActionResult Course(string subject, string num)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Create a department which is uniquely identified by it's subject code
        /// </summary>
        /// <param name="subject">the subject code</param>
        /// <param name="name">the full name of the department</param>
        /// <returns>A JSON object containing {success = true/false}.
        /// false if the department already exists, true otherwise.</returns>
        public IActionResult CreateDepartment(string subject, string name)
        {
            var query = from d in db.Departments select d.DeptAbrv;
            bool alreadyExists = query.Contains(subject);

            if (alreadyExists) {
                return Json(new { success = false});
            }

            Department newDepartment = new();
            newDepartment.DeptAbrv = subject;
            newDepartment.Name = name;

            db.Departments.Add(newDepartment);
            db.SaveChanges();
            
            return Json(new { success = true});
        }


        /// <summary>
        /// Returns a JSON array of all the courses in the given department.
        /// Each object in the array should have the following fields:
        /// "number" - The course number (as in 5530)
        /// "name" - The course name (as in "Database Systems")
        /// </summary>
        /// <param name="subjCode">The department subject abbreviation (as in "CS")</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetCourses(string subject)
        {
            var query = from c in db.Courses where c.DeptAbrv == subject
            select new {
                number = c.CNum,
                name = c.CName
            };
            
            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the professors working in a given department.
        /// Each object in the array should have the following fields:
        /// "lname" - The professor's last name
        /// "fname" - The professor's first name
        /// "uid" - The professor's uid
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetProfessors(string subject)
        {
            var query = from p in db.Professors where p.DeptAbrv == subject
            select new {
                lname = p.LastName,
                fname = p.FirstName,
                uid = p.UId
            };
            
            return Json(query.ToArray());
        }



        /// <summary>
        /// Creates a course.
        /// A course is uniquely identified by its number + the subject to which it belongs
        /// </summary>
        /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
        /// <param name="number">The course number</param>
        /// <param name="name">The course name</param>
        /// <returns>A JSON object containing {success = true/false}.
        /// false if the course already exists, true otherwise.</returns>
        public IActionResult CreateCourse(string subject, int number, string name)
        {           
            var query = from c in db.Courses where c.CNum == number && c.DeptAbrv == subject select c;

            bool isAvailable = query.Count() == 0;

            if (!isAvailable) {
                return Json(new { success = false});
            }

            Course newCourse = new();
            newCourse.DeptAbrv = subject;
            newCourse.CName = name;
            newCourse.CNum = (uint)number;

            db.Courses.Add(newCourse);
            db.SaveChanges();
            
            return Json(new { success = true});
        }



        /// <summary>
        /// Creates a class offering of a given course.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="number">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="start">The start time</param>
        /// <param name="end">The end time</param>
        /// <param name="location">The location</param>
        /// <param name="instructor">The uid of the professor</param>
        /// <returns>A JSON object containing {success = true/false}. 
        /// false if another class occupies the same location during any time 
        /// within the start-end range in the same semester, or if there is already
        /// a Class offering of the same Course in the same Semester,
        /// true otherwise.</returns>
        public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
        {
            var getCourse = from c in db.Courses where c.DeptAbrv == subject && c.CNum == number select c;
            // course doesn't exist
            if (getCourse.Count() == 0) {
                return Json(new { success = false});
            }
            Course course = getCourse.Single();

            TimeOnly startTime = TimeOnly.FromDateTime(start);
            TimeOnly endTime = TimeOnly.FromDateTime(end);

            var query = from c in db.Classes where
                        // if there is already a Class offering of the same Course in the same Semester
                        (c.CourseId == course.CourseId && c.Year == year && c.Season == season)
                        // if another class occupies the same location during any time within the start-end range
                        || (c.Location == location && ((c.StartTime > startTime && c.StartTime < endTime) || (c.EndTime > startTime && c.EndTime < endTime)))
                        select c;

            bool isAvailable = query.Count() == 0;

            if (!isAvailable) {
                return Json(new { success = false});
            }

            Class newClass = new();
            newClass.CourseId = course.CourseId;
            newClass.Season = season;
            newClass.Year = (uint)year;
            newClass.Location = location;
            newClass.StartTime = startTime;
            newClass.EndTime = endTime;
            newClass.UId = instructor;

            db.Classes.Add(newClass);
            db.SaveChanges();
            
            return Json(new { success = true});
        }


        /*******End code to modify********/

    }
}

