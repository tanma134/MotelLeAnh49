using BusinessLogic.Interfaces;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Http;
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

        public void CreateRoom(Room room, int adminId, List<IFormFile> images, string webRootPath)
        {
            if (_roomRepository.IsRoomNumberExist(room.RoomNumber))
            {
                throw new Exception("Số phòng đã tồn tại");
            }

            room.AdminId = adminId;

            var imagePaths = new List<string>();

            if (images != null && images.Any())
            {
                string uploadFolder = Path.Combine(webRootPath, "images/rooms");
                Directory.CreateDirectory(uploadFolder);

                foreach (var file in images)
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(uploadFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    imagePaths.Add("/images/rooms/" + fileName);
                }
            }

            room.RoomImages = imagePaths.Select(p => new RoomImage
            {
                ImagePath = p
            }).ToList();

            _roomRepository.Add(room);
        }

        public bool UpdateRoom(Room room, List<IFormFile> images, List<int> deletedImageIds, string webRootPath)
        {
            var existing = _roomRepository.GetById(room.Id);
            if (existing == null) return false;

            if (_roomRepository.IsRoomNumberExist(room.RoomNumber, room.Id))
            {
                throw new Exception("Số phòng đã tồn tại");
            }
            // update info
            existing.RoomNumber = room.RoomNumber;
            existing.RoomType = room.RoomType;
            existing.OvernightPrice = room.OvernightPrice;
            existing.DayPrice = room.DayPrice;
            existing.FirstHourPrice = room.FirstHourPrice;
            existing.NextHourPrice = room.NextHourPrice;
            existing.MaxGuests = room.MaxGuests;
            existing.ExtraGuestFee = room.ExtraGuestFee;

            // delete images
            if (deletedImageIds != null && deletedImageIds.Any())
            {
                existing.RoomImages = existing.RoomImages
                    .Where(img => !deletedImageIds.Contains(img.Id))
                    .ToList();
            }

            // upload new images
            if (images != null && images.Any())
            {
                string uploadFolder = Path.Combine(webRootPath, "images/rooms");
                Directory.CreateDirectory(uploadFolder);

                foreach (var file in images)
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(uploadFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    existing.RoomImages.Add(new RoomImage
                    {
                        ImagePath = "/images/rooms/" + fileName
                    });
                }
            }

            _roomRepository.Update(existing);
            return true;
        }

        public void DeleteRoom(int id)
        {
            if (_roomRepository.HasActiveBooking(id))
            {
                throw new Exception("Phòng đang có booking hoặc chờ duyệt, không thể xóa");
            }

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