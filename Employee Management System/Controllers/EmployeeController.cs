using Employee_Management_System.Models;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Management_System.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            EmployeeModel model = new EmployeeModel();
            List<EmployeeModel> employees = model.GetEmployeeList();
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
        //Add Employee
        public IActionResult AddEmployee(EmployeeModel emp)
        {
            if (emp.EmployeeID > 0)
            {
                EmployeeModel employee = new EmployeeModel();
                if (employee.Update(emp))
                {
                    TempData["msg"] = "Employee Updated successfully!";
                    return RedirectToAction("Index");
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
                    return RedirectToAction("Index");
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
