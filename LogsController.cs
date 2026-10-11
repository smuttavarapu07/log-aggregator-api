using LoggingAPI.Data;
using LoggingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace LoggingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LogsController(AppDbContext context)
        {
            _context = context;
        }

        // POST api/logs
        [HttpPost]
        public async Task<IActionResult> SaveLog(SystemLog log)
        {
            try
            {
                log.Timestamp = DateTime.Now;

                _context.SystemLogs.Add(log);

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Log Saved Successfully"
                });
            }
            catch (TimeoutException)
            {
                return StatusCode(408, new
                {
                    Message = "Database timeout occurred."
                });
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, new
                {
                    Message = "Database connection failed.",
                    Error = ex.Message
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new
                {
                    Message = "Database update failed.",
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message
                });
            }
        }

        // GET api/logs/filter?level=Error
        [HttpGet("filter")]
        public async Task<IActionResult> FilterLogs(string level)
        {
            try
            {
                var logs = await _context.SystemLogs
                    .Where(x => x.LogLevel == level)
                    .ToListAsync();

                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message
                });
            }
        }

        // Test Null Reference Exception
        [HttpGet("test-null")]
        public IActionResult TestNull()
        {
            string text = null;

            return Ok(text.Length);
        }

        // Test Divide By Zero Exception
        [HttpGet("test-divide")]
        public IActionResult TestDivide()
        {
            int x = 10;
            int y = 0;

            return Ok(x / y);
        }

        // Test File Not Found Exception
        [HttpGet("test-file")]
        public IActionResult TestFile()
        {
            string content =
                System.IO.File.ReadAllText("abc.txt");

            return Ok(content);
        }

        // Test Invalid Cast Exception
        [HttpGet("test-cast")]
        public IActionResult TestCast()
        {
            object value = "Hello";

            int number = (int)value;

            return Ok(number);
        }
    }
}
