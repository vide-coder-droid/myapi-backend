using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _db;

    public HealthController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("db")]
    public async Task<IActionResult> CheckDatabase()
    {
        try
        {
            // Thử query đơn giản để kiểm tra kết nối
            await _db.Database.ExecuteSqlRawAsync("SELECT 1");
            return Ok(new { success = true, message = "Database connection is healthy" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Database connection failed", error = ex.Message });
        }
    }
}