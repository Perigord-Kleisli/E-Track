const params = new URLSearchParams(document.location.search);
const token = params.get("password-reset-token");

const togglePass = () => {
    let eye = document.getElementById("password-toggle");
    let input = document.getElementById("newPassword");
    switch (eye.innerHTML) {
        case 'visibility':
            eye.innerHTML = 'visibility_off'
            input.type = 'password';
            break;
        default:
            eye.innerHTML = 'visibility'
            input.type = 'text';
            break;
    }
};

const toggleConfirmPass = () => {
    let eye = document.getElementById("confirm-password-toggle");
    let input = document.getElementById("confirmPassword");
    switch (eye.innerHTML) {
        case 'visibility':
            eye.innerHTML = 'visibility_off'
            input.type = 'password';
            break;
        default:
            eye.innerHTML = 'visibility'
            input.type = 'text';
            break;
    }
};

const ratePass = (pass) => {
    let ratePath = document.getElementById('rate-path');
    let ratePathG = document.getElementById('rate-path-g');

    for (let i = 1; i <= 4; i++) {
        document.getElementById(`rating-${i}`).style.color = 'red';
        document.getElementById(`rating-icon-${i}`).innerHTML = 'close';
    }

    let passwordRating = Math.min(pass.length / 8, 1) / 4;
    if (pass.length >= 8) {
        document.getElementById('rating-1').style.color = 'green';
        document.getElementById('rating-icon-1').innerHTML = 'done';
    }
    if (pass.search(/[A-Z]/) > -1) {
        passwordRating += 0.25;
        document.getElementById('rating-2').style.color = 'green';
        document.getElementById('rating-icon-2').innerHTML = 'done';
    }
    if (pass.search(/[0-9.]/) > -1) {
        passwordRating += 0.25;
        document.getElementById('rating-3').style.color = 'green';
        document.getElementById('rating-icon-3').innerHTML = 'done';
    }
    if (pass.search(/[^a-zA-Z0-9]/) > -1) {
        passwordRating += 0.25;
        document.getElementById('rating-4').style.color = 'green';
        document.getElementById('rating-icon-4').innerHTML = 'done';
    }
    ratePath.setAttribute('d', `M 2 2 l ${92 * passwordRating} 0`);
    ratePathG.setAttribute('stroke', `rgb(${255 - (255 * passwordRating)},${255 * passwordRating},50)`);
}

const resetPassword = () => {
    let newPass = document.getElementById("newPassword");
    let confirmPass = document.getElementById("confirmPassword");
    let passMismatch = document.getElementById("passMismatch");

    if (newPass.value !== confirmPass.value) {
        passMismatch.style.display = 'block';
        return false;
    } else {
        passMismatch.style.display = 'none';
    }

    fetch(
        `${document.location.origin}/api/Auth/reset-password`,
        {
            method: 'post',
            headers: {
                'accept': "*/*",
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                guid: token,
                password: newPass.value
            }),
        })
        .then(async response => {
            let message = document.getElementById("message");

            let responseText = await response.text();

            if (response.status == 200) {
                message.innerHTML = "Succesfully changed password, you may now return to the login page";
                message.classList.add('success-message');
            } else {
                console.log(response);
                console.log(responseText);
                message.innerHTML = responseText;
                message.classList.add('error-message');
            }

        })

    return false;
};