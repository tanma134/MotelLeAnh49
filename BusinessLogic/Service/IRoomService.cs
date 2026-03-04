
using MotelLeAnh49.Models;

namespace BusinessLogic.Interfaces
{
    public interface IRoomService
    {
        List<Room> GetAllRooms();
        Room? GetRoomById(int id);
        void CreateRoom(Room room, int adminId, List<string> imagePaths);
        bool UpdateRoom(Room room, List<string> newImagePaths, List<int> deletedImageIds);
        void DeleteRoom(int id);
        List<Room> SearchAvailableRooms(DateTime checkIn, DateTime checkOut, int adults, int children);
        bool BookRoom(int roomId, DateTime checkIn, DateTime checkOut);
        bool IsRoomAvailable(int roomId, DateTime checkIn, DateTime checkOut);
    }
}