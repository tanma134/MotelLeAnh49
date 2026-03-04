using BusinessLogic.Interfaces;
using DataAccess.Repositories;
using MotelLeAnh49.Models;

namespace BusinessLogic.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public List<Room> GetAllRooms()
        {
            return _roomRepository.GetAll();
        }

        public Room? GetRoomById(int id)
        {
            return _roomRepository.GetById(id);
        }

        public void CreateRoom(Room room, int adminId, List<string> imagePaths)
        {
            room.AdminId = adminId;

            if (imagePaths != null && imagePaths.Any())
            {
                room.RoomImages = imagePaths.Select(path => new RoomImage
                {
                    ImagePath = path
                }).ToList();
            }

            _roomRepository.Add(room);
        }

        public bool UpdateRoom(Room room, List<string> newImagePaths, List<int> deletedImageIds)
        {
            var existing = _roomRepository.GetById(room.Id);
            if (existing == null) return false;

            // Cập nhật thông tin phòng
            existing.RoomNumber = room.RoomNumber;
            existing.RoomType = room.RoomType;
            existing.OvernightPrice = room.OvernightPrice;
            existing.DayPrice = room.DayPrice;
            existing.FirstHourPrice = room.FirstHourPrice;
            existing.NextHourPrice = room.NextHourPrice;
            existing.MaxGuests = room.MaxGuests;
            existing.ExtraGuestFee = room.ExtraGuestFee;

            // XÓA ảnh cũ
            if (deletedImageIds != null && deletedImageIds.Any())
            {
                existing.RoomImages = existing.RoomImages
                    .Where(img => !deletedImageIds.Contains(img.Id))
                    .ToList();
            }

            // THÊM ảnh mới
            if (newImagePaths != null && newImagePaths.Any())
            {
                foreach (var path in newImagePaths)
                {
                    existing.RoomImages.Add(new RoomImage
                    {
                        ImagePath = path
                    });
                }
            }

            _roomRepository.Update(existing);
            return true;
        }

        public void DeleteRoom(int id)
        {
            _roomRepository.Delete(id);
        }

        public List<Room> SearchAvailableRooms(DateTime checkIn, DateTime checkOut, int adults, int children)
        {
            int totalGuests = adults + children;
            return _roomRepository.SearchAvailable(checkIn, checkOut, totalGuests);
        }

        public bool BookRoom(int roomId, DateTime checkIn, DateTime checkOut)
        {
            if (!_roomRepository.IsAvailable(roomId, checkIn, checkOut))
                return false;

            // Logic tạo booking có thể tách riêng BookingRepository nếu muốn
            return true;
        }
        public bool IsRoomAvailable(int roomId, DateTime checkIn, DateTime checkOut)
        {
            return _roomRepository.IsAvailable(roomId, checkIn, checkOut);
        }

    }
}