// booking-create.js
document.addEventListener("DOMContentLoaded", function () {

    // ==================== ELEMENTS ====================
    const fullName = document.getElementById("fullName");
    const phone = document.getElementById("phone");
    const email = document.getElementById("email");
    const roomSelect = document.getElementById("roomId");
    const checkIn = document.getElementById("checkIn");
    const checkOut = document.getElementById("checkOut");
    const adults = document.getElementById("adults");
    const children = document.getElementById("children");
    const submitBtn = document.getElementById("submitBtn");
    const infoBox = document.getElementById("infoBox");

    const nameError = document.getElementById("nameError");
    const phoneError = document.getElementById("phoneError");
    const emailError = document.getElementById("emailError");

    // Config
    const roomMaxGuests = {
        1: 2,
        2: 3,
        3: 4
        // Thêm phòng khác nếu cần: 4: 5, ...
    };
    const extraFee = 50000;

    // ==================== VALIDATION FUNCTIONS ====================
    function validateFullName() {
        if (fullName.value.trim().length < 3) {
            nameError.textContent = "Tên khách hàng phải có ít nhất 3 ký tự";
            return false;
        }
        nameError.textContent = "";
        return true;
    }

    function validatePhone() {
        const phoneRegex = /^0\d{9}$/; // Bắt đầu bằng 0, tổng 10 số
        if (!phoneRegex.test(phone.value)) {
            phoneError.textContent = "Số điện thoại phải là 10 số và bắt đầu bằng 0";
            return false;
        }
        phoneError.textContent = "";
        return true;
    }

    function validateEmail() {
        if (email.value.trim() === "") return true; // Không bắt buộc

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email.value)) {
            emailError.textContent = "Email không đúng định dạng";
            return false;
        }
        emailError.textContent = "";
        return true;
    }

    function checkGuests() {
        const roomId = roomSelect.value;
        if (!roomId) {
            infoBox.innerHTML = "";
            submitBtn.disabled = true;
            return false;
        }

        const max = roomMaxGuests[roomId] || 2;
        const total = (parseInt(adults.value) || 0) + (parseInt(children.value) || 0);

        let msg = `👥 Tối đa ${max} người`;

        if (total > max + 1) {
            msg += " ❌ Vượt quá giới hạn cho phép";
            infoBox.style.color = "#dc3545";
            submitBtn.disabled = true;
            return false;
        } else {
            submitBtn.disabled = false;
            if (total > max) {
                const fee = (total - max) * extraFee;
                msg += ` | 💰 Phụ thu ${fee.toLocaleString('vi-VN')} VNĐ`;
                infoBox.style.color = "#fd7e14";
            } else {
                infoBox.style.color = "#28a745";
            }
        }
        infoBox.textContent = msg;
        return true;
    }

    // ==================== LOAD ROOMS BY DATE ====================
    async function loadRooms() {
        if (!checkIn.value || !checkOut.value) return;

        try {
            const res = await fetch(`/Bookings/GetRoomsByDate?checkIn=${checkIn.value}&checkOut=${checkOut.value}`);
            const data = await res.json();

            roomSelect.innerHTML = '<option value="">-- Chọn phòng --</option>';
            let firstAvailable = null;

            data.forEach(r => {
                const opt = document.createElement("option");
                opt.value = r.value;
                opt.textContent = r.text + (r.isBooked ? " ❌ (Đã đặt)" : " ✅ (Trống)");

                if (r.isBooked) {
                    opt.disabled = true;
                    opt.style.color = "red";
                } else {
                    opt.style.color = "green";
                    if (!firstAvailable) firstAvailable = r.value;
                }
                roomSelect.appendChild(opt);
            });

            if (firstAvailable) {
                roomSelect.value = firstAvailable;
            }

            checkGuests();
        } catch (err) {
            console.error("Lỗi khi tải danh sách phòng:", err);
        }
    }

    // ==================== EVENT LISTENERS ====================
    fullName.addEventListener("input", validateFullName);
    phone.addEventListener("input", validatePhone);
    email.addEventListener("input", validateEmail);

    roomSelect.addEventListener("change", checkGuests);
    adults.addEventListener("input", checkGuests);
    children.addEventListener("input", checkGuests);

    checkIn.addEventListener("change", loadRooms);
    checkOut.addEventListener("change", loadRooms);

    // Date min validation
    const today = new Date().toISOString().split("T")[0];
    checkIn.min = today;

    checkIn.addEventListener("change", () => {
        checkOut.min = checkIn.value;
        if (checkOut.value < checkIn.value) checkOut.value = "";
    });

    // Submit validation
    document.getElementById("bookingForm").addEventListener("submit", function (e) {
        const isNameValid = validateFullName();
        const isPhoneValid = validatePhone();
        const isEmailValid = validateEmail();
        const isGuestsValid = checkGuests();

        if (!roomSelect.value) {
            alert("Vui lòng chọn phòng!");
            e.preventDefault();
            return;
        }

        if (!isNameValid || !isPhoneValid || !isEmailValid || !isGuestsValid) {
            alert("Vui lòng sửa các lỗi highlighted trước khi tạo booking!");
            e.preventDefault();
        }
    });

    // ==================== INITIAL LOAD ====================
    if (checkIn.value && checkOut.value) {
        loadRooms();
    } else {
        checkGuests();
    }
});