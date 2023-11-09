const params = new URLSearchParams(document.location.search);
const token = params.get("confirmation-token");

fetch(
    `${document.location.origin}/api/Auth/confirm-email?confirmationGUID=${token}`,
    {
        method: 'post',
        body: '',
        headers: {
            'accept': "*/*"
        }
    })
    .then(async response => {
        let spinner = document.getElementById("confirmation-spinner");
        spinner.style.display = 'none';

        if (response.status == 200) {
            let apiSuccessView = document.getElementById("apiSuccessView");
            apiSuccessView.style.display = 'flex';
        } else {
            let apiFailView = document.getElementById("apiFailView");
            apiFailView.style.display = 'flex';
        }

    })
    .catch(err => console.error(err))

let calls = 0;
const timer = (time) => {
    let timerDisplay = document
        .getElementsByClassName('timer-display')
        .item(0);
    timerDisplay.style.visibility = 'visible';
    let timer = setInterval(() => {
        let display = document.getElementById('timer');
        let minutes = Math.floor(time / 60);
        let seconds = time % 60;
        if (seconds < 10) {
            display.innerHTML = `${minutes}:0${seconds}`
        } else {
            display.innerHTML = `${minutes}:${seconds}`
        }

        time--;
        if (time < 0) {
            timerDisplay.style.visibility = 'hidden';
            calls = 0;
            clearInterval(timer);
        }
    }, 1000);
}


const resendConfirmation = (x) => {
    let spinner = document.getElementById('resend-spinner');
    spinner.style.display = "inline-block";
    let email = document.getElementById('email');
    fetch(
        `${document.location.origin}/api/Auth/confirmation-token?emailToVerify=${email.value}`,
        {
            method: 'post',
            body: '',
            headers: {
                'accept': "*/*"
            }
        })
        .then(async response => {
            calls++;
            let text = await response.text();
            let message = document.getElementById('resend-status');
            try {
                //Tries to parse response as json and gets the title
                text = JSON.parse(text).Message;
            } catch (error) { }

            spinner.style.display = 'none';
            if (response.status == 200) {
                message.classList = ["success-message"];
                message.style.display = "block";
                message.innerHTML = "Verification link sent to your email";
            } else {
                message.classList = ["error-message"];
                message.style.display = "block";
                message.innerHTML = text;
            }

            if (calls >= 3) timer(90);
        })
    return false;
};