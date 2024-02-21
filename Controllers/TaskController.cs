using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.SqlClient;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using TaskManagerApi.Models;

namespace TaskManagerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        public static readonly IEnumerable<TaskModel> TaskList = new[]
        {
            new TaskModel()
        };

        
        /*public TaskModel Get(TaskModel[] taskList,int taskId)
        {
            TaskModel taskItem = taskList.FirstOrDefault(i => i.TaskId == taskId);

            return taskItem;
        }*/

        /* public bool ValidTask(TaskModel item)
         {
             if (item.TaskId == null)
                 return false;
             else
                 return true;
         }*/
        /*
                #region XML Interation
                public void XmlSave(List<TaskModel> taskItem)
                {
                    // List of Task Objects
                    List<TaskModel> tasks = new List<TaskModel>();
                    foreach(var item in taskItem)
                    {
                        tasks.Add(item);
                    }

                    // Create an instance of the XmlSerializer class
                    XmlSerializer xs = new XmlSerializer(typeof(List<TaskModel>));

                    // Create an instance of the StreamWriter class
                    StreamWriter sw = new StreamWriter("tasks.xml");

                    // Call the Serialize method
                    xs.Serialize(sw, tasks);

                    // Close the StreamWriter instance
                    sw.Close();
                }

                public List<TaskModel> XmlLoad()
                {
                    // Create an instance of the XmlSerializer class, passing the type of the list as a parameter
                    XmlSerializer xs = new XmlSerializer(typeof(List<TaskModel>));

                    // Create an instance of the StreamReader class, passing the file name or path as a parameter
                    StreamReader sr = new StreamReader("tasks.xml");


                    // Call the Deserialize method of the XmlSerializer class, passing the StreamReader instance as a parameter
                    List<TaskModel> tasks = (List<TaskModel>)xs.Deserialize(sr);

                    // Close the StreamWriter instance
                    sr.Close();

                    return tasks;
                }

                #endregion*/


        #region SQL Database Interaction

        private readonly string connectionString = "Data Source=localhost;Initial Catalog=TaskDatabase;Persist Security Info=True;User ID=sa;Password=Password@SQL;TrustServerCertificate=True";
        
        [HttpPost("PostTask")]
        public IActionResult AddRecord([FromBody] TaskModel taskItem)
        {
            try
            {
                if (taskItem != null)
                {

                    taskItem.Created = DateTime.Now;
                    taskItem.Updated = DateTime.Now;

                    //Takes Serialized JSON File and Pushes it Into the Database
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "INSERT INTO Tasks (TaskName, TaskDesc, Status, Created, Updated, ProjectId, ProjectDesc) "
                            + "VALUES (@Name, @Desc, @Status, @Created, @Updated, @ProjectId, @ProjectDesc)";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Name", taskItem.Name);
                            command.Parameters.AddWithValue("@Desc", taskItem.Description);
                            command.Parameters.AddWithValue("@Status", taskItem.Status);
                            command.Parameters.AddWithValue("@Created", taskItem.Created);
                            command.Parameters.AddWithValue("@Updated", taskItem.Updated);
                            command.Parameters.AddWithValue("@ProjectId", taskItem.ProjectId);
                            command.Parameters.AddWithValue("@ProjectDesc", taskItem.ProjectName);
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                    }

                    return Ok(); 
                }
                else
                {
                    return BadRequest("No file uploaded.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        
        [HttpDelete("DeleteTask")]
        public IActionResult DeleteRecord([FromQuery] int taskID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE Tasks WHERE TaskId = @Id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", taskID);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPut("UpdateTask")]
        public IActionResult UpdateRecord([FromBody] TaskModel taskItem)
        {
            try
            {
                if (taskItem == null)
                    BadRequest("No updates to the Task Item were found.");

                taskItem.Updated = DateTime.Now;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Tasks SET TaskName = @Name, TaskDesc = @Desc, Status = @Status, Created = @Created, Updated = @Updated, ProjectId = @ProjectId, ProjectDesc = @ProjectDesc WHERE TaskId = @Id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", taskItem.TaskId);
                        command.Parameters.AddWithValue("@Name", taskItem.Name);
                        command.Parameters.AddWithValue("@Desc", taskItem.Description);
                        command.Parameters.AddWithValue("@Status", taskItem.Status);
                        command.Parameters.AddWithValue("@Created", taskItem.Created);
                        command.Parameters.AddWithValue("@Updated", taskItem.Updated);
                        command.Parameters.AddWithValue("@ProjectId", taskItem.ProjectId);
                        command.Parameters.AddWithValue("@ProjectDesc", taskItem.ProjectName);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }            
        }

        [HttpGet("GetTask")]
        public TaskModel PullRecord([FromQuery] int taskID)
        {
            TaskModel matchingItem = new TaskModel();

            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                string oString = "SELECT * FROM Tasks WHERE TaskId=@taskID";
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                oCmd.Parameters.AddWithValue("@taskID", taskID);
                myConnection.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())                        {
                        matchingItem.TaskId = int.Parse(oReader["TaskId"].ToString());
                        if (oReader["ProjectId"] != null)
                            matchingItem.Name = oReader["TaskName"].ToString();
                        if (oReader["ProjectId"] != null)
                            matchingItem.Description = oReader["TaskDesc"].ToString();
                        if (oReader["ProjectId"] != null)
                            matchingItem.Status = int.Parse(oReader["Status"].ToString());
                        matchingItem.Created = DateTime.Parse(oReader["Created"].ToString());
                        matchingItem.Updated = DateTime.Parse(oReader["Updated"].ToString());
                        if (oReader["ProjectId"] != null)
                            matchingItem.ProjectId = int.Parse(oReader["ProjectId"].ToString());
                        if (oReader["ProjectId"] != null)
                            matchingItem.ProjectName = oReader["ProjectDesc"].ToString();
                    }

                    myConnection.Close();
                }
            }
            return matchingItem;
        }

        #endregion

    }
}