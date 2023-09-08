using Employee_Management_System.Models;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Management_System.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index(string? searchName, string? searchDepartment, DateTime? searchJoiningDateFrom, DateTime? searchJoiningDateTo)
        {
            EmployeeModel model = new EmployeeModel();
            List<EmployeeModel> employees = model.GetEmployeeList();
            foreach( var employee in employees)
            {
                employee.SelectedSkills = model.GetSelectedSkills(employee.EmployeeID);
            }
            if (!string.IsNullOrEmpty(searchName))
            {
                employees = employees.Where(e => (e.FirstName + " " + e.LastName).Contains(searchName)).ToList();
            }
            if (!string.IsNullOrEmpty(searchDepartment) )
            {
                employees = employees.Where(e => e.DepartmentName == searchDepartment).ToList();
            }
            if (searchJoiningDateFrom != null)
            {
                employees = employees.Where(e => e.JoiningDate >= searchJoiningDateFrom).ToList();
            }

            if (searchJoiningDateTo != null)
            {
                employees = employees.Where(e => e.JoiningDate <= searchJoiningDateTo).ToList();
            }
            //if (searchKnowledge != null && searchKnowledge.Any())
            //{
            //    employees = employees.Where(e => e.SelectedSkills != null && e.SelectedSkills.Intersect(searchKnowledge).Any()).ToList();
            //}

            TempData["SearchName"] = searchName;
            TempData["SearchDepartment"] = searchDepartment;
            TempData["SearchJoiningDateFrom"] = searchJoiningDateFrom;
            TempData["SearchJoiningDateTo"] = searchJoiningDateTo;
            //TempData["SearchKnowledge"] = searchKnowledge;

            return View(employees);

        }

        [HttpGet]
        public IActionResult AddEmployee(int? id)
        {
            EmployeeModel model = new EmployeeModel();
            model.Skills = model.GetAllSkills();
            List<Employee> reportingPersons = model.GetReportingEmployeeList();
            model.JoiningDate = DateTime.Now;
            model.ReportingPerson = reportingPersons;

            if (id.HasValue)
            {
                // Editing an existing employee
                EmployeeModel data = model.GetEmployeeById(id.Value);
                List<int> selectedSkills = model.GetSelectedSkills(id.Value);
                data.ReportingPerson = reportingPersons;
                data.Skills = model.GetAllSkills();
                data.SelectedSkills = selectedSkills;
                if (data!= null)
                {
                    return View(data);
                }
                else
                {
                    return View("AddEmployee", new {id = id.Value});
                }
            }
            else
            {
                return View(model);
            }
        }
        [HttpPost]
        // Add / Update Employee
        public IActionResult AddEmployee(EmployeeModel emp)
        {
            // Retrieve the filter values TempData
            string? searchName = TempData["SearchName"] as string;
            string? searchDepartment = TempData["SearchDepartment"] as string;
            DateTime? searchJoiningDateFrom = TempData["SearchJoiningDateFrom"] as DateTime?;
            DateTime? searchJoiningDateTo = TempData["SearchJoiningDateTo"] as DateTime?;
            //List<int>? searchKnowledge = TempData["SearchKnowledge"] as List<int>;


            if (emp.EmployeeID > 0)
            {
                EmployeeModel employee = new EmployeeModel();
                if (employee.Update(emp))
                {
                    TempData["msg"] = "Employee Updated successfully!";
                    return RedirectToAction("Index", new
                    {
                        searchName,
                        searchDepartment,
                        searchJoiningDateFrom,
                        searchJoiningDateTo,
                        //searchKnowledge
                    });
                }
                else
                {
                    TempData["msg"] = "Failed to Update employee.";
                    return View(emp);
                }
            }
            else
            {
                EmployeeModel employee = new EmployeeModel();
                if (employee.Insert(emp))
                {
                    TempData["msg"] = "Employee added successfully!";
                    return RedirectToAction("Index", new
                    {
                        searchName,
                        searchDepartment,
                        searchJoiningDateFrom,
                        searchJoiningDateTo,
                        //searchKnowledge
                    });
                }
                else
                {
                    TempData["msg"] = "Failed to add employee.";
                    return View(emp);
                }
            }

        }


        // Delete Employee
    public IActionResult DeleteEmp(int id)
    {
        EmployeeModel model = new EmployeeModel();
        model.DeleteEmployee(id);
        return RedirectToAction("Index");
    }
    }

}
