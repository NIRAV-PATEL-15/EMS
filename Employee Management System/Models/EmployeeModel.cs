using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;

namespace Employee_Management_System.Models
    {
    public class EmployeeModel
        {
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DesignationID { get; set; }
        public int DepartmentID { get; set; }
        public List<int> Knowledge { get; set; }
        public decimal Salary { get; set; }
        public DateTime JoiningDate { get; set; }
        public int ReportingPersonID { get; set; }
        public List<Employee> ReportingPerson { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string ReportingPersonName { get; set; }
        public List<int> SelectedSkills { get; set; }
        public List<Skill> Skills { get; set; }


        public bool Insert(EmployeeModel emp)
            {

            using ( SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\NIRAV\Desktop\EMS-main\EMS-main\Employee Management System\App_Data\EmployeeDB.mdf"";Integrated Security=True") )
                {
                con.Open();

                try
                    {


                    SqlCommand cmd = new SqlCommand("INSERT INTO Employee (FirstName, LastName, DesignationID, DepartmentID, Salary, JoiningDate, ReportingPersonID) VALUES (@FirstName, @LastName, @DesignationID, @DepartmentID, @Salary, @JoiningDate, @ReportingPersonID);SELECT SCOPE_IDENTITY();", con);



                    cmd.Parameters.AddWithValue("@FirstName", emp.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", emp.LastName);
                    cmd.Parameters.AddWithValue("@DesignationID", emp.DesignationID);
                    cmd.Parameters.AddWithValue("@DepartmentID", emp.DepartmentID);
                    cmd.Parameters.AddWithValue("@Salary", emp.Salary);
                    cmd.Parameters.AddWithValue("@JoiningDate", emp.JoiningDate.Date);
                    cmd.Parameters.AddWithValue("@ReportingPersonID", emp.ReportingPersonID);

                    int employeeId = Convert.ToInt32(cmd.ExecuteScalar());

                    foreach ( int skillId in emp.Knowledge )
                        {
                        string insertSkillQuery = "INSERT INTO EmployeeSkill (EmployeeID, SkillID) VALUES (@EmployeeID, @SkillID)";
                        cmd = new SqlCommand(insertSkillQuery, con);
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                        cmd.Parameters.AddWithValue("@SkillID", skillId);
                        cmd.ExecuteNonQuery();
                        }
                    return true;
                    }
                catch ( Exception ex )
                    {
                    Console.WriteLine("An error occurred while executing the SQL command:");
                    Console.WriteLine(ex.Message);
                    return false;
                    }
                }
            }
        public List<Employee> GetReportingEmployeeList()
            {
            List<Employee> employeeList = new List<Employee>();

            using ( SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\NIRAV\Desktop\EMS-main\EMS-main\Employee Management System\App_Data\EmployeeDB.mdf';Integrated Security=True") )
                {
                con.Open();

                using ( SqlCommand cmd = new SqlCommand("SELECT EmployeeID, FirstName, LastName FROM Employee", con) )
                    {
                    using ( SqlDataReader reader = cmd.ExecuteReader() )
                        {
                        while ( reader.Read() )
                            {
                            Employee employee = new Employee
                                {
                                EmployeeID = (int)reader["EmployeeID"],
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString()
                                };

                            employeeList.Add(employee);
                            }
                        }
                    }
                }

            return employeeList;
            }
        public List<EmployeeModel> GetEmployeeList()
            {
            List<EmployeeModel> employees = new List<EmployeeModel>();

            string query = @"
        SELECT e.*, d.DepartmentName, des.DesignationName,CONCAT(rp.FirstName, ' ', rp.LastName) AS ReportingPersonName
        FROM Employee e
        JOIN Department d ON e.DepartmentID = d.DepartmentID
        JOIN Designation des ON e.DesignationID = des.DesignationID
        LEFT JOIN Employee rp ON e.ReportingPersonID = rp.EmployeeID WHERE e.IsDeleted = 0 ";

            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\NIRAV\Desktop\EMS-main\EMS-main\Employee Management System\App_Data\EmployeeDB.mdf';Integrated Security=True");
            SqlDataAdapter apt = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            apt.Fill(ds);

            foreach ( DataRow dr in ds.Tables[0].Rows )
                {
                employees.Add(new EmployeeModel
                    {
                    EmployeeID = Convert.ToInt32(dr["EmployeeID"].ToString()),
                    FirstName = dr["FirstName"].ToString(),
                    LastName = dr["LastName"].ToString(),
                    JoiningDate = DateTime.Parse(dr["JoiningDate"].ToString()),
                    DepartmentName = dr["DepartmentName"].ToString(),
                    DesignationName = dr["DesignationName"].ToString(),
                    ReportingPersonName = dr["ReportingPersonName"].ToString()
                    });

                }

            return employees;
            }
        public void DeleteEmployee(int employeeId)
            {
            using ( SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\NIRAV\Desktop\EMS-main\EMS-main\Employee Management System\App_Data\EmployeeDB.mdf';Integrated Security=True") )
                {
                con.Open();
                using ( SqlCommand cmd = new SqlCommand("UPDATE Employee SET IsDeleted = 1 WHERE EmployeeID = @EmployeeID", con) )
                    {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                    cmd.ExecuteNonQuery();
                    }
                }
            }
        public EmployeeModel GetEmployeeById(int employeeId)
            {
            EmployeeModel employee = null;

            string query = @"
                SELECT e.*, d.DepartmentName, des.DesignationName, CONCAT(rp.FirstName, ' ', rp.LastName) AS ReportingPersonName
                FROM Employee e
                JOIN Department d ON e.DepartmentID = d.DepartmentID
                JOIN Designation des ON e.DesignationID = des.DesignationID
                LEFT JOIN Employee rp ON e.ReportingPersonID = rp.EmployeeID
                WHERE e.EmployeeID = @EmployeeID";

            using ( SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\NIRAV\Desktop\EMS-main\EMS-main\Employee Management System\App_Data\EmployeeDB.mdf';Integrated Security=True") )

                {
                con.Open();
                using ( SqlCommand cmd = new SqlCommand(query, con) )
                    {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

                    using ( SqlDataReader reader = cmd.ExecuteReader() )
                        {
                        if ( reader.Read() )
                            {
                            employee = new EmployeeModel
                                {
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                DesignationID = Convert.ToInt32(reader["DesignationID"]),
                                DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                                Salary = Convert.ToDecimal(reader["Salary"]),
                                JoiningDate = Convert.ToDateTime(reader["JoiningDate"]),
                                ReportingPersonID = Convert.ToInt32(reader["ReportingPersonID"]),
                                DepartmentName = reader["DepartmentName"].ToString(),
                                DesignationName = reader["DesignationName"].ToString(),
                                ReportingPersonName = reader["ReportingPersonName"].ToString()
                                };
                            }
                        }
                    }
                }

            return employee;
            }
        public List<Skill> GetAllSkills()
            {
            List<Skill> skills = new List<Skill>();

            using ( SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\NIRAV\Desktop\EMS-main\EMS-main\Employee Management System\App_Data\EmployeeDB.mdf';Integrated Security=True") )
                {
                con.Open();

                using ( SqlCommand cmd = new SqlCommand("SELECT SkillID, SkillName FROM Skill", con) )
                    {
                    using ( SqlDataReader reader = cmd.ExecuteReader() )
                        {
                        while ( reader.Read() )
                            {
                            Skill skill = new Skill
                                {
                                SkillID = (int)reader["SkillID"],
                                SkillName = reader["SkillName"].ToString()
                                };

                            skills.Add(skill);
                            }
                        }
                    }
                }

            return skills;
            }
        public List<int> GetSelectedSkills(int employeeId)
            {
            List<int> selectedSkills = new List<int>();

            using ( SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\NIRAV\Desktop\EMS-main\EMS-main\Employee Management System\App_Data\EmployeeDB.mdf';Integrated Security=True") )

                {
                con.Open();

                using ( SqlCommand cmd = new SqlCommand("SELECT SkillID FROM EmployeeSkill WHERE EmployeeID = @EmployeeID", con) )
                    {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

                    using ( SqlDataReader reader = cmd.ExecuteReader() )
                        {
                        while ( reader.Read() )
                            {
                            int skillId = (int)reader["SkillID"];
                            selectedSkills.Add(skillId);
                            }
                        }
                    }
                }

            return selectedSkills;
            }
        public bool Update(EmployeeModel emp)
            {
            using ( SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\NIRAV\Desktop\EMS-main\EMS-main\Employee Management System\App_Data\EmployeeDB.mdf"";Integrated Security=True") )
                {
                con.Open();

                try
                    {

                    // Update the Employee table with new data.
                    SqlCommand cmd = new SqlCommand(
                        "UPDATE Employee " +
                        "SET FirstName = @FirstName, LastName = @LastName, DesignationID = @DesignationID, " +
                        "DepartmentID = @DepartmentID, Salary = @Salary, JoiningDate = @JoiningDate, " +
                        "ReportingPersonID = @ReportingPersonID " +
                        "WHERE EmployeeID = @EmployeeID", con);

                    cmd.Parameters.AddWithValue("@EmployeeID", emp.EmployeeID);
                    cmd.Parameters.AddWithValue("@FirstName", emp.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", emp.LastName);
                    cmd.Parameters.AddWithValue("@DesignationID", emp.DesignationID);
                    cmd.Parameters.AddWithValue("@DepartmentID", emp.DepartmentID);
                    cmd.Parameters.AddWithValue("@Salary", emp.Salary);
                    cmd.Parameters.AddWithValue("@JoiningDate", emp.JoiningDate.Date);
                    cmd.Parameters.AddWithValue("@ReportingPersonID", emp.ReportingPersonID);

                    cmd.ExecuteNonQuery();

                    // Remove existing skill associations.
                    cmd = new SqlCommand("DELETE FROM EmployeeSkill WHERE EmployeeID = @EmployeeID", con);
                    cmd.Parameters.AddWithValue("@EmployeeID", emp.EmployeeID);
                    cmd.ExecuteNonQuery();

                    // Insert new skill associations.
                    foreach ( int skillId in emp.Knowledge )
                        {
                        string insertSkillQuery = "INSERT INTO EmployeeSkill (EmployeeID, SkillID) VALUES (@EmployeeID, @SkillID)";
                        cmd = new SqlCommand(insertSkillQuery, con);
                        cmd.Parameters.AddWithValue("@EmployeeID", emp.EmployeeID);
                        cmd.Parameters.AddWithValue("@SkillID", skillId);
                        cmd.ExecuteNonQuery();
                        }

                    return true;
                    }
                catch ( Exception ex )
                    {
                    Console.WriteLine("An error occurred while executing the SQL command:");
                    Console.WriteLine(ex.Message);

                    return false;
                    }
                }
            }
        }
    public class Employee
        {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        }

    public class Skill
        {
        public int SkillID { get; set; }
        public string SkillName { get; set; }
        }

    }
