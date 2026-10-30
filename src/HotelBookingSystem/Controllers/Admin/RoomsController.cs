using HotelBookingSystem.Services;
using HotelBookingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    [Route("Admin/[controller]")]
    public class RoomsController : Controller
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        // GET: /Admin/Rooms
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var rooms = await _roomService.GetAllRoomsForAdminAsync();
            return View("~/Views/Admin/Rooms/Index.cshtml", rooms);
        }

        // GET: /Admin/Rooms/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            var model = new RoomAdminViewModel
            {
                PricePerNight = 250.00m,
                Capacity = 2,
                IsActive = true
            };
            return View("~/Views/Admin/Rooms/Create.cshtml", model);
        }

        // POST: /Admin/Rooms/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomAdminViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Rooms/Create.cshtml", model);
            }

            var (success, message, createdRoom) = await _roomService.CreateRoomAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View("~/Views/Admin/Rooms/Create.cshtml", model);
            }

            TempData["SuccessMessage"] = $"Room {model.RoomNumber} created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Rooms/Edit/5
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null)
            {
                TempData["ErrorMessage"] = "Room not found.";
                return RedirectToAction(nameof(Index));
            }

            var model = new RoomAdminViewModel
            {
                RoomId = room.RoomId,
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                Description = room.Description,
                PricePerNight = room.PricePerNight,
                Capacity = room.Capacity,
                ImageUrl = room.ImageUrl,
                Amenities = room.Amenities,
                IsActive = room.IsActive
            };

            return View("~/Views/Admin/Rooms/Edit.cshtml", model);
        }

        // POST: /Admin/Rooms/Edit/5
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoomAdminViewModel model)
        {
            if (id != model.RoomId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Rooms/Edit.cshtml", model);
            }

            var (success, message) = await _roomService.UpdateRoomAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View("~/Views/Admin/Rooms/Edit.cshtml", model);
            }

            TempData["SuccessMessage"] = $"Room {model.RoomNumber} updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Rooms/ToggleActive/5
        [HttpPost("ToggleActive/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var (success, message) = await _roomService.ToggleRoomActiveStatusAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Rooms/Delete/5
        [HttpPost("Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, message) = await _roomService.DeleteRoomAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
