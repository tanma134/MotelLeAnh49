// booking-form-validation.js
document.addEventListener("DOMContentLoaded", () => {
    const fullname = document.getElementById("fullname");
    const phone = document.getElementById("phone");
    const email = document.getElementById("email");
    const checkin = document.getElementById("checkin");
    const checkout = document.getElementById("checkout");

    const nameError = document.getElementById("nameError");
    const phoneError = document.getElementById("phoneError");
    const emailError = document.getElementById("emailError");
    const checkinError = document.getElementById("checkinError");
    const checkoutError = document.getElementById("checkoutError");

    const submitBtn = document.getElementById("submitBtn");
    const form = document.getElementById("bookingForm");

    const today = new Date().toISOString().split("T")[0];
    checkin.min = today;
    checkout.min = today;

    function validateForm() {
        let isValid = true;

        // Họ và tên
        if (fullname.value.trim().length < 3) {
            nameError.textContent = "Tên phải có ít nhất 3 ký tự";
            isValid = false;
        } else {
            nameError.textContent = "";
        }

        // Số điện thoại
        const phoneRegex = /^0[1-9][0-9]{8}$/;
        if (!phoneRegex.test(phone.value.trim())) {
            phoneError.textContent = "Số điện thoại phải 10 số và bắt đầu bằng 0";
            isValid = false;
        } else {
            phoneError.textContent = "";
        }

        // Email – sửa đúng ở đây
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email.value.trim())) {
            emailError.textContent = "Email không hợp lệ";
            isValid = false;
        } else {
            emailError.textContent = "";
        }

        // Check-in & Check-out
        checkinError.textContent = "";
        checkoutError.textContent = "";

        if (!checkin.value) {
            checkinError.textContent = "Vui lòng chọn ngày Check-in";
            isValid = false;
        } else if (!checkout.value) {
            checkoutError.textContent = "Vui lòng chọn ngày Check-out";
            isValid = false;
        } else if (checkout.value <= checkin.value) {
            checkoutError.textContent = "Ngày Check-out phải sau ngày Check-in";
            isValid = false;
        }

        submitBtn.disabled = !isValid;
    }

    phone.addEventListener("input", validateForm);
    email.addEventListener("input", validateForm);
    fullname.addEventListener("input", validateForm);

    checkin.addEventListener("change", () => {
        if (checkin.value) {
            checkout.min = checkin.value;
            if (checkout.value && checkout.value < checkin.value) {
                checkout.value = "";
            }
        } else {
            checkout.min = today;
        }
        validateForm();
    });

    checkout.addEventListener("change", validateForm);

    form.addEventListener("submit", (e) => {
        validateForm();
        if (submitBtn.disabled) {
            e.preventDefault();
            alert("Vui lòng kiểm tra và điền đầy đủ thông tin hợp lệ!");
        }
    });

    validateForm();
});