
using MotelLeAnh49.Models;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Interfaces
{
    public interface IRoomService
    {
        List<Room> GetAllRooms();
        Room? GetRoomById(int id);
        void CreateRoom(Room room, int adminId, List<IFormFile> images, string webRootPath);
        bool UpdateRoom(Room room, List<IFormFile> images, List<int> deletedImageIds, string webRootPath);
        void DeleteRoom(int id);
        List<Room> SearchAvailableRooms(DateTime checkIn, DateTime checkOut, int adults, int children);
        bool BookRoom(int roomId, DateTime checkIn, DateTime checkOut);
        bool IsRoomAvailable(int roomId, DateTime checkIn, DateTime checkOut);
    }
}