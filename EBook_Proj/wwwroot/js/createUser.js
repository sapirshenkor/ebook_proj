document.addEventListener("DOMContentLoaded", function() {
    const form = document.querySelector('form');
    const password = document.getElementById('password');
    const confirmPassword = document.getElementById('confirm_password');
    const email = document.querySelector('input[name="Email"]');

    // Create modal instance
    const validationModal = new bootstrap.Modal(document.getElementById('validationModal'));

    function checkPassword(password) {
        const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$/;
        return regex.test(password);
    }

    function checkEmail(email) {
        const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return regex.test(email);
    }

    function showErrors(errors) {
        const errorList = document.getElementById('errorList');
        errorList.innerHTML = '';

        errors.forEach(error => {
            const li = document.createElement('li');
            li.className = 'list-group-item text-danger';
            li.innerHTML = `<i class="bi bi-x-circle me-2"></i>${error}`;
            errorList.appendChild(li);
        });

        // Show modal and add close handler
        const modalElement = document.getElementById('validationModal');
        modalElement.addEventListener('hidden.bs.modal', function () {
            // Enable the submit button after modal is closed
            document.querySelector('button[type="submit"]').disabled = false;
        }, { once: true }); // Use once: true to prevent multiple listeners

        validationModal.show();
    }

    // Use a single submit handler
    form.onsubmit = function(e) {
        let isValid = true;
        const errorMsg = [];

        if (!form.FirstName.value.trim()) {
            errorMsg.push('First name is required');
            isValid = false;
        }

        if (!form.LastName.value.trim()) {
            errorMsg.push('Last name is required');
            isValid = false;
        }

        if (!form.Email.value.trim()) {
            errorMsg.push('Email is required');
            isValid = false;
        }
        else if (!checkEmail(email.value.trim())) {
            errorMsg.push('Please enter a valid email address');
            isValid = false;
        }

        if (!password.value.trim()) {
            errorMsg.push('Password is required');
            isValid = false;
        } else if (!checkPassword(password.value)) {
            errorMsg.push('Password must contain at least 6 characters, including uppercase, lowercase, number and special character');
            isValid = false;
        }

        if (!confirmPassword.value.trim()){
            errorMsg.push('Confirm Password is required');
            isValid = false;
        } else if(password.value !== confirmPassword.value){
            errorMsg.push('Passwords do not match');
            isValid = false;
        }

        if(!isValid){
            e.preventDefault();
            // Disable submit button to prevent multiple submissions
            document.querySelector('button[type="submit"]').disabled = true;
            showErrors(errorMsg);
            return false;
        }
        return true;
    };

    // Password match validation
    confirmPassword.oninput = function() {
        if (password.value !== confirmPassword.value) {
            confirmPassword.setCustomValidity('Passwords do not match');
        } else {
            confirmPassword.setCustomValidity('');
        }
    };
});