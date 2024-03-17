using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.SqlClient;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using TaskManagerApi.Models;
using TaskManagerApi.Data; // Assuming TaskDbContext is defined in this namespace
using TaskItem = TaskManagerApi.Models.TaskItem; // Assuming Task class is defined in this namespace

namespace TaskManagerApi.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
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

        [HttpPost("PostTask")]
        public IActionResult AddRecord([FromBody] TaskItem taskItem, [FromServices] TaskDbContext dbContext)
        {
            try
            {
                if (taskItem != null)
                {
                    // Set Created and Updated timestamps
                    taskItem.Created = DateTime.Now;
                    taskItem.Updated = DateTime.Now;

                    // Add the taskItem to the context
                    dbContext.Tasks.Add(taskItem);

                    // Save changes to the database
                    dbContext.SaveChanges();

                    return Ok("Task Added");
                }
                else
                {
                    return BadRequest("No task item provided.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpDelete("DeleteTask")]
        public IActionResult DeleteRecord([FromQuery] int taskId, [FromServices] TaskDbContext dbContext)
        {
            try
            {
                // Retrieve the task to delete from the database
                var taskToDelete = dbContext.Tasks.Find(taskId);

                if (taskToDelete == null)
                {
                    return NotFound($"There is no Task with the ID: {taskId}"); // Return 404 Not Found if task is not found
                }

                // Remove the task from the context
                dbContext.Tasks.Remove(taskToDelete);

                // Save changes to the database
                dbContext.SaveChanges();

                return Ok("Task Removed");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateTask")]
        public IActionResult UpdateRecord([FromBody] TaskItem taskItem, [FromServices] TaskDbContext dbContext)
        {
            try
            {
                if (taskItem == null)
                {
                    return BadRequest("No updates to the Task Item were found.");
                }

                // Retrieve the task to update from the database
                var existingTask = dbContext.Tasks.Find(taskItem.TaskId);

                if (existingTask == null)
                {
                    return NotFound(); // Return 404 Not Found if task is not found
                }

                // Update task properties
                existingTask.TaskName = taskItem.TaskName;
                existingTask.TaskDesc = taskItem.TaskDesc;
                existingTask.Status = taskItem.Status;
                existingTask.Updated = DateTime.Now; // Update Updated timestamp to current time
                existingTask.ProjectId = taskItem.ProjectId;
                existingTask.ProjectDesc = taskItem.ProjectDesc;

                // Save changes to the database
                dbContext.SaveChanges();

                return Ok("Task Updated");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetTasks")]
        public ActionResult<IEnumerable<TaskItem>> PullRecords([FromQuery] int projectID, [FromServices] TaskDbContext dbContext)
        {
            try
            {
                // Retrieve the tasks using Entity Framework Core
                var matchingItems = dbContext.Tasks
                    .Where(x => x.ProjectId == projectID) // Filter tasks based on the provided taskIDs
                    .ToList();

                if (matchingItems.Count == 0)
                {
                    return NotFound(); // Return 404 Not Found if no tasks are found
                }

                return matchingItems;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, $"An error occurred while retrieving tasks: {ex.Message}");
            }
        }

        [HttpGet("GetTask")]
        public ActionResult<TaskItem> PullRecord([FromQuery] int taskID, [FromServices] TaskDbContext dbContext)
        {
            try
            {
                var taskItem = dbContext.Tasks.FirstOrDefault(x => x.TaskId == taskID);
                
                if (taskItem == null)
                {
                    return NotFound(); // Return 404 Not Found if no tasks are found
                }
                
                return taskItem;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, $"An error occurred while retrieving task: {ex.Message}");
            }
        }

        #endregion

    }
}