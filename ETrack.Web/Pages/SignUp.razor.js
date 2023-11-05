window.validatePasswords = function () {
    document
        .getElementById("passwordconfirm")
        .setCustomValidity("Passwords do not match");
}