﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS.Controllers
{
    public class CommonController : Controller
    {
        private readonly LMSContext db;

        public CommonController(LMSContext _db)
        {
            db = _db;
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Retreive a JSON array of all departments from the database.
        /// Each object in the array should have a field called "name" and "subject",
        /// where "name" is the department name and "subject" is the subject abbreviation.
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetDepartments()
        {            
            var departments = from dep in db.Departments
                               select new 
                               {
                                   subject = dep.DeptAbrv,
                                   name = dep.Name 
                               };
            return Json(departments.ToArray());
        }



        /// <summary>
        /// Returns a JSON array representing the course catalog.
        /// Each object in the array should have the following fields:
        /// "subject": The subject abbreviation, (e.g. "CS")
        /// "dname": The department name, as in "Computer Science"
        /// "courses": An array of JSON objects representing the courses in the department.
        ///            Each field in this inner-array should have the following fields:
        ///            "number": The course number (e.g. 5530)
        ///            "cname": The course name (e.g. "Database Systems")
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetCatalog()
        {
            var courses = from dep in db.Departments
                              select new
                              {
                                  subject = dep.DeptAbrv,
                                  dname = dep.Name,
                                  courses = from c in db.Courses where c.DeptAbrv == dep.DeptAbrv
                                            select new
                                            {
                                                number = c.CNum,
                                                cname = c.CName
                                            }
                              };
            return Json(courses.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all class offerings of a specific course.
        /// Each object in the array should have the following fields:
        /// "season": the season part of the semester, such as "Fall"
        /// "year": the year part of the semester
        /// "location": the location of the class
        /// "start": the start time in format "hh:mm:ss"
        /// "end": the end time in format "hh:mm:ss"
        /// "fname": the first name of the professor
        /// "lname": the last name of the professor
        /// </summary>
        /// <param name="subject">The subject abbreviation, as in "CS"</param>
        /// <param name="number">The course number, as in 5530</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetClassOfferings(string subject, int number)
        {      
            var classes = from course in db.Courses where course.CNum == number && course.DeptAbrv == subject
                          from c in course.Classes 
                          select new
                          {
                              season = c.Season,
                              year = c.Year,
                              location = c.Location,
                              start = c.StartTime,
                              end = c.EndTime,
                              fname = c.UIdNavigation.FirstName,
                              lname = c.UIdNavigation.LastName
                          };
            
            return Json(classes.ToArray());
        }

        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <returns>The assignment contents</returns>
        public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
        {
            var asignContents = from course in db.Courses
                        where course.CNum == num && course.DeptAbrv == subject
                        from c in course.Classes
                        where c.Season == season && c.Year == year
                        from ac in c.AssignmentCategories
                        where ac.Name == category
                        from a in ac.Assignments
                        where a.Name == asgname
                        select a.Contents;
            string result = asignContents.FirstOrDefault() ?? "";
            return Content(result);
        }


        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment submission.
        /// Returns the empty string ("") if there is no submission.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <param name="uid">The uid of the student who submitted it</param>
        /// <returns>The submission text</returns>
        public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
        {
            var asignContents = from course in db.Courses
                                where course.CNum == num && course.DeptAbrv == subject
                                from c in course.Classes
                                where c.Season == season && c.Year == year
                                from ac in c.AssignmentCategories
                                where ac.Name == category
                                from a in ac.Assignments
                                where a.Name == asgname
                                from s in a.Submissions where s.UId == uid
                                select s.Answer;
            string result = asignContents.FirstOrDefault() ?? "";
            return Content(result);
        }


        /// <summary>
        /// Gets information about a user as a single JSON object.
        /// The object should have the following fields:
        /// "fname": the user's first name
        /// "lname": the user's last name
        /// "uid": the user's uid
        /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
        ///               If the user is a Professor, this is the department they work in.
        ///               If the user is a Student, this is the department they major in.    
        ///               If the user is an Administrator, this field is not present in the returned JSON
        /// </summary>
        /// <param name="uid">The ID of the user</param>
        /// <returns>
        /// The user JSON object 
        /// or an object containing {success: false} if the user doesn't exist
        /// </returns>
        public IActionResult GetUser(string uid)
        {           
            var userAdmin = from a in db.Administrators where a.UId == uid 
                            select new
                            {
                                fname = a.FirstName,
                                lname = a.LastName,
                                uid = a.UId
                            };
            if (userAdmin.Any()) {
                return Json(userAdmin.Single());
            }
            var otherUser = (from p in db.Professors where p.UId == uid
                             select new 
                             {
                                 fname = p.FirstName,
                                 lname = p.LastName,
                                 uid = p.UId,
                                 department = p.DeptAbrv
                             }).Union(from s in db.Students where s.UId == uid
                                      select new
                                      {
                                          fname = s.FirstName,
                                          lname = s.LastName,
                                          uid = s.UId,
                                          department = s.DeptAbrv
                                       });
            if (otherUser.Any())
            {
                return Json(otherUser.Single());
            }
            return Json(new { success = false });
        }


        /*******End code to modify********/
    }
}

