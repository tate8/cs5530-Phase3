using LMS.Controllers;
using LMS.Models.LMSModels;
using LMS_CustomIdentity.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Cryptography.X509Certificates;

namespace LMSControllerTests
{
    public class UnitTest1
    {
        // Uncomment the methods below after scaffolding
        // (they won't compile until then)

        // [Fact]
        // public void Test1()
        // {
        //    // An example of a simple unit test on the CommonController
        //    CommonController ctrl = new CommonController(MakeTinyDB());

        //    var allDepts = ctrl.GetDepartments() as JsonResult;

        //    dynamic x = allDepts.Value;

        //    Assert.Equal( 1, x.Length );
        //    Assert.Equal( "CS", x[0].subject );
        // }
        [Fact]
        public void CreateDepartmentTest()
        {
           var database = MakeEmptyDB();
           AdministratorController ctrl = new(database);
           
           var success = ctrl.CreateDepartment("CS", "Comp Sci") as JsonResult;
           dynamic x = success.Value;
           Assert.Equal( true, x.success );

           var success2 = ctrl.CreateDepartment("CS", "Comp Sci") as JsonResult;
           dynamic x2 = success2.Value;
           Assert.Equal( false, x2.success );

            var dept = from depart in database.Departments select depart;
            Assert.Equal(1, dept.Count() );
            Assert.Equal("CS", dept.Single().DeptAbrv);

            Assert.Equal("Comp Sci", dept.Single().Name);
        }

        [Fact]
        public void GetCatalogTest()
        {
            var database = MakeTinyDB();
            CommonController ctrl = new(database);

            var output = ctrl.GetCatalog() as JsonResult;
            dynamic x = output.Value;
            Assert.Equal(2, x.Length);
        }

        /// <summary>
        /// Make a very tiny in-memory database, containing just one department
        /// and nothing else.
        /// </summary>
        /// <returns></returns>
        LMSContext MakeTinyDB()
        {
           var contextOptions = new DbContextOptionsBuilder<LMSContext>()
           .UseInMemoryDatabase( "LMSControllerTest" )
           .ConfigureWarnings( b => b.Ignore( InMemoryEventId.TransactionIgnoredWarning ) )
           .UseApplicationServiceProvider( NewServiceProvider() )
           .Options;

           var db = new LMSContext(contextOptions);

           db.Database.EnsureDeleted();
           db.Database.EnsureCreated();

           db.Departments.Add( new Department { Name = "KSoC", DeptAbrv = "CS" } );
           db.Courses.Add(new Course { DeptAbrv = "CS", CNum = 3500, CName = "Software Practice 1" });
           db.Courses.Add(new Course { DeptAbrv = "CS", CNum = 3505, CName = "Software Practice 2" });
           db.Departments.Add(new Department { Name = "Math", DeptAbrv = "MATH" });
           db.Courses.Add(new Course { DeptAbrv = "MATH", CNum = 1000, CName = "Calculus 1" });

            // TODO: add more objects to the test database

            db.SaveChanges();

           return db;
        }
        LMSContext MakeEmptyDB()
        {
           var contextOptions = new DbContextOptionsBuilder<LMSContext>()
           .UseInMemoryDatabase( "LMSControllerTest" )
           .ConfigureWarnings( b => b.Ignore( InMemoryEventId.TransactionIgnoredWarning ) )
           .UseApplicationServiceProvider( NewServiceProvider() )
           .Options;

           var db = new LMSContext(contextOptions);

           db.Database.EnsureDeleted();
           db.Database.EnsureCreated();

           db.SaveChanges();

           return db;
        }

        private static ServiceProvider NewServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
          .AddEntityFrameworkInMemoryDatabase()
          .BuildServiceProvider();

            return serviceProvider;
        }

    }
}