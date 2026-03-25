// booking-update.js - PHIÊN BẢN MỚI NHẤT (Fix vượt số người)
document.addEventListener("DOMContentLoaded", function () {

    const fullNameInput = document.getElementById("fullName");
    const phoneInput = document.getElementById("phone");
    const emailInput = document.getElementById("email");
    const roomSelect = document.getElementById("roomId");
    const checkInInput = document.getElementById("checkIn");
    const checkOutInput = document.getElementById("checkOut");
    const adultsInput = document.getElementById("adults");
    const childrenInput = document.getElementById("children");
    const btnSubmit = document.getElementById("btnSubmit");
    const extraFeeBox = document.getElementById("extraFeeBox");

    const roomMaxGuests = { 1: 2, 2: 3, 3: 4 };
    const extraFeePerPerson = 50000;

    function getToday() { return new Date().toISOString().split('T')[0]; }

    function showError(id, message) {
        const el = document.getElementById("err-" + id);
        if (el) el.textContent = message;
    }
    function clearError(id) {
        const el = document.getElementById("err-" + id);
        if (el) el.textContent = "";
    }

    // ====================== VALIDATE SỐ NGƯỜI ======================
    function validateGuests() {
        if (!adultsInput || !childrenInput) return true;

        const adults = parseInt(adultsInput.value) || 0;
        const children = parseInt(childrenInput.value) || 0;
        const total = adults + children;
        const roomId = roomSelect ? roomSelect.value : "";
        const max = roomMaxGuests[roomId] || 2;

        extraFeeBox.innerHTML = "";
        extraFeeBox.className = "extra-fee";   // reset class

        if (adults < 1) {
            extraFeeBox.innerHTML = "❌ Adults phải ít nhất là 1 người";
            extraFeeBox.style.background = "#ffe0e0";
            extraFeeBox.style.color = "#c82333";
            return false;
        }

        if (total > max + 1) {
            extraFeeBox.innerHTML = `❌ Vượt quá giới hạn cho phép (tối đa ${max} người)`;
            extraFeeBox.style.background = "#ffe0e0";
            extraFeeBox.style.color = "#c82333";
            return false;
        }

        if (total > max) {
            const extra = total - max;
            const fee = (extra * extraFeePerPerson).toLocaleString('vi-VN');
            extraFeeBox.innerHTML = `💰 Phụ thu ${extra} người: +${fee} VNĐ`;
            extraFeeBox.style.background = "#fff3cd";
            extraFeeBox.style.color = "#856404";
            return true;
        }

        // Trường hợp bình thường
        extraFeeBox.innerHTML = `✅ Đúng quy định (${total}/${max} người)`;
        extraFeeBox.style.background = "#d4edda";
        extraFeeBox.style.color = "#155724";
        return true;
    }

    // Các validate khác giữ nguyên...
    function validateFullName() {
        clearError("fullName");
        if (fullNameInput && fullNameInput.value.trim().length < 3) {
            showError("fullName", "Tên phải có ít nhất 3 ký tự");
            return false;
        }
        return true;
    }

    function validatePhone() {
        clearError("phone");
        const regex = /^0[0-9]{9}$/;
        if (phoneInput && !regex.test(phoneInput.value.trim())) {
            showError("phone", "Số điện thoại phải 10 số, bắt đầu bằng 0");
            return false;
        }
        return true;
    }

    function validateEmail() {
        clearError("email");
        if (!emailInput || emailInput.value.trim() === "") return true;
        const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!regex.test(emailInput.value.trim())) {
            showError("email", "Email không đúng định dạng");
            return false;
        }
        return true;
    }

    function validateDates() {
        clearError("checkIn");
        clearError("checkOut");
        const today = getToday();
        const cin = checkInInput ? checkInInput.value : "";
        const cout = checkOutInput ? checkOutInput.value : "";

        if (cin && cin < today) { showError("checkIn", "Không được chọn ngày trong quá khứ"); return false; }
        if (cout && cout < today) { showError("checkOut", "Không được chọn ngày trong quá khứ"); return false; }
        if (cin && cout && cout <= cin) { showError("checkOut", "Check Out phải sau Check In"); return false; }
        return true;
    }

    function validateAll() {
        const ok = validateFullName() && validatePhone() && validateEmail() && validateDates() && validateGuests();

        if (btnSubmit) {
            btnSubmit.disabled = !ok;
        }
        return ok;
    }

    // Events
    [fullNameInput, phoneInput, emailInput, roomSelect, adultsInput, childrenInput].forEach(el => {
        if (el) el.addEventListener("input", validateAll);
    });
    if (roomSelect) {
        const roomIdHidden = document.getElementById("roomIdHidden");
        if (roomIdHidden) {
            roomIdHidden.value = roomSelect.value; // set giá trị ban đầu
            roomSelect.addEventListener("change", function () {
                roomIdHidden.value = this.value;
            });
        }
    }
    if (checkInInput) {
        checkInInput.addEventListener("change", () => {
            if (checkOutInput) checkOutInput.min = checkInInput.value;
            validateAll();
        });
    }
    if (checkOutInput) checkOutInput.addEventListener("change", validateAll);

    // Init
    const today = getToday();
    if (checkInInput) checkInInput.min = today;
    if (checkInInput && checkInInput.value && checkOutInput) {
        checkOutInput.min = checkInInput.value;
    }

    validateAll();
});