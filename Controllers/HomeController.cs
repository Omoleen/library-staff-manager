using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers;

/// <summary>
/// Controller responsible for handling basic navigation and system-wide functionality.
/// Most actions require Admin role authorization, except for Index and Error pages.
/// </summary>
[Authorize(Roles = "Admin")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Initializes a new instance of the HomeController.
    /// </summary>
    /// <param name="logger">The logger for the HomeController</param>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Displays the home page of the application.
    /// This action is accessible to all users, including those who are not authenticated.
    /// </summary>
    /// <returns>The home page view</returns>
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Displays the privacy policy page.
    /// This action requires Admin role authorization.
    /// </summary>
    /// <returns>The privacy policy view</returns>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Handles and displays error information when an error occurs in the application.
    /// This action is accessible to all users and includes diagnostic information.
    /// </summary>
    /// <returns>The error view with error details</returns>
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}