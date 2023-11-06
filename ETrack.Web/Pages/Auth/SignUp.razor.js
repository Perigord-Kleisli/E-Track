window.validatePasswords = function () {
    document
        .getElementById("passwordconfirm")
        .setCustomValidity("Passwords do not match");
}

window.validateGuid = function () {
    document
        .getElementById("register-token")
        .setCustomValidity("Invalid Guid");
}