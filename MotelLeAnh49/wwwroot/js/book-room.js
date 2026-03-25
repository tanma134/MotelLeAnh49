document.addEventListener("DOMContentLoaded", function () {

    const form = document.getElementById("bookingForm");
    const checkIn = document.getElementById("checkin");
    const checkOut = document.getElementById("checkout");
    const phone = document.getElementById("phone");
    const email = document.getElementById("email");
    const btn = document.getElementById("submitBtn");

    if (!form || !checkIn || !checkOut) return;

    function getToday() {
        return new Date().toISOString().split("T")[0];
    }

    function showError(id, msg) {
        const el = document.getElementById(id);
        if (el) el.innerText = msg;
    }

    function clearError(id) {
        const el = document.getElementById(id);
        if (el) el.innerText = "";
    }

    function validateDates() {
        const today = getToday();
        const cin = checkIn.value;
        const cout = checkOut.value;

        clearError("checkinError");
        clearError("checkoutError");

        let ok = true;

        if (cin && cin < today) {
            showError("checkinError", "Không chọn ngày quá khứ");
            ok = false;
        }

        if (cout && cout <= cin) {
            showError("checkoutError", "CheckOut phải sau CheckIn");
            ok = false;
        }

        return ok;
    }

    function validatePhone() {
        const regex = /^0[0-9]{9}$/;
        clearError("phoneError");

        if (!regex.test(phone.value.trim())) {
            showError("phoneError", "Sai SĐT");
            return false;
        }
        return true;
    }

    function validateEmail() {
        const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        clearError("emailError");

        if (!regex.test(email.value.trim())) {
            showError("emailError", "Sai email");
            return false;
        }
        return true;
    }

    function validateAll() {
        let ok = true;

        if (!validateDates()) ok = false;
        if (!validatePhone()) ok = false;
        if (!validateEmail()) ok = false;

        btn.disabled = !ok;
        return ok;
    }

    // FIX DATE
    checkIn.addEventListener("change", () => {
        const today = getToday();

        if (checkIn.value < today) {
            checkIn.value = today;
        }

        checkOut.min = checkIn.value;

        if (checkOut.value && checkOut.value <= checkIn.value) {
            checkOut.value = "";
        }

        validateAll();
    });

    checkOut.addEventListener("change", () => {
        if (checkOut.value && checkOut.value <= checkIn.value) {
            checkOut.value = "";
        }

        validateAll();
    });

    phone.addEventListener("input", validateAll);
    email.addEventListener("input", validateAll);
    checkIn.addEventListener("input", validateAll);
    checkOut.addEventListener("input", validateAll);

    checkIn.min = getToday();
    checkOut.min = getToday();

    form.addEventListener("submit", function (e) {
        if (!validateAll()) {
            e.preventDefault();
        }
    });

});