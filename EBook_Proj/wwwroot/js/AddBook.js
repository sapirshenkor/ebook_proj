document.addEventListener("DOMContentLoaded", function() {
    // Get form once at the start
    const form = document.getElementById("addBook");
    if (!form) return;

    form.onsubmit = function(e) {  // Note: onsubmit not onSubmit
        e.preventDefault();

        // Get form elements
        const title = document.getElementById("Title");
        const author = document.getElementById("Author");
        const publisher = document.getElementById("Publisher");
        const publicationDate = document.getElementById("PublicationDate");
        const genre = document.getElementById("Genre");
        const ageLimit = document.getElementById("AgeLimit");
        const description = document.getElementById("Description");
        const buyingPrice = document.getElementById("BuyingPrice");
        const borrowPrice = document.getElementById("BorrowPrice");
        const pageCount = document.getElementById("PageCount");
        const bookCover = document.getElementById("BookCover");

        let isValid = true;
        let errorMsg = [];

        //clear prev fields
        form.querySelectorAll('.is-invalid').forEach(el=> {
            el.classList.remove('is-invalid');
        });

        //Title validations
        if(!title.value.trim()){
            errorMsg.push("Book title is required");
            title.classList.add('is-invalid');
            isValid = false;
        }

        if(!author.value.trim()){
            errorMsg.push("Author is required");
            author.classList.add('is-invalid');
            isValid = false;
        }

        if(!publisher.value.trim()){
            errorMsg.push("Publisher is required");
            publisher.classList.add('is-invalid');
            isValid = false;
        }

        // Publication Date validation - fixed logic
        if(!publicationDate.value){
            errorMsg.push("Publication date is required");
            publicationDate.classList.add('is-invalid');
            isValid = false;
        } else {
            const selectedDate = new Date(publicationDate.value);
            const currentDate = new Date();

            if (isNaN(selectedDate.getTime())){
                errorMsg.push("Please enter a valid date");
                publicationDate.classList.add('is-invalid');
                isValid = false;
            }
            else if(selectedDate > currentDate){
                errorMsg.push("Publication date cannot be in the future");
                publicationDate.classList.add('is-invalid');
                isValid = false;
            }
        }

        if(!genre.value.trim()){
            errorMsg.push("Genre is required");
            genre.classList.add('is-invalid');
            isValid = false;
        }

        if (!ageLimit.value){
            errorMsg.push("Age limit is required");
            ageLimit.classList.add('is-invalid');
            isValid = false;
        } else {
            const ageLimitValue = parseInt(ageLimit.value);
            if (isNaN(ageLimitValue) || ageLimitValue < 0 || ageLimitValue > 100) {
                errorMsg.push('Age limit must be between 0 and 100');
                ageLimit.classList.add('is-invalid');
                isValid = false;
            }
        }

        if (!description.value.trim()){
            errorMsg.push("Description is required");
            description.classList.add('is-invalid');
            isValid = false;
        }

        // Fixed price validations
        if (!buyingPrice.value){
            errorMsg.push("Buying price is required");
            buyingPrice.classList.add('is-invalid');
            isValid = false;
        } else {
            const buyingPriceValue = parseFloat(buyingPrice.value);
            if (isNaN(buyingPriceValue) || buyingPriceValue <= 0) {
                errorMsg.push("Buying price must be greater than 0");
                buyingPrice.classList.add('is-invalid');
                isValid = false;
            }
        }

        if (!borrowPrice.value){
            errorMsg.push("Borrow price is required");
            borrowPrice.classList.add('is-invalid');
            isValid = false;
        } else {
            const borrowPriceValue = parseFloat(borrowPrice.value);
            const buyingPriceValue = parseFloat(buyingPrice.value);
            if (isNaN(borrowPriceValue) || borrowPriceValue <= 0 || borrowPriceValue >= buyingPriceValue) {
                errorMsg.push("Borrow price must be greater than 0 and less than the buying price");
                borrowPrice.classList.add('is-invalid');
                isValid = false;
            }
        }

        if(!pageCount.value){
            errorMsg.push("Page count is required");
            pageCount.classList.add('is-invalid');
            isValid = false;
        } else {
            const pageCountValue = parseInt(pageCount.value);
            if (isNaN(pageCountValue) || pageCountValue <= 0) {
                errorMsg.push("Page count must be greater than 0");
                pageCount.classList.add('is-invalid');
                isValid = false;
            }
        }

        if(!bookCover.value.trim()){
            errorMsg.push("Book cover URL is required");
            bookCover.classList.add('is-invalid');
            isValid = false;
        }

        if(!isValid){
            showErrors(errorMsg);
            return false;
        }
        
        fetch(form.action, {
            method: 'POST',
            body: new FormData(form)
        })
            .then(response => {
                if (response.ok) {
                    // Close modal
                    const modal = bootstrap.Modal.getInstance(document.getElementById('addBookModal'));
                    modal.hide();

                    // Reload page to show new book
                    window.location.reload();
                } else {
                    throw new Error('Something went wrong');
                }
            })
            .catch(error => {
                showErrors(['Failed to add book. Please try again.']);
            });

        return false;
    };

    function showErrors(errors) {
        // Remove any existing error messages
        const existingAlert = form.querySelector('.alert');
        if (existingAlert) {
            existingAlert.remove();
        }

        // Create error message container
        const errorDiv = document.createElement('div');
        errorDiv.className = 'alert alert-danger';

        // Add each error message
        const errorList = document.createElement('ul');
        errorList.style.marginBottom = '0';
        errors.forEach(error => {
            const li = document.createElement('li');
            li.textContent = error;
            errorList.appendChild(li);
        });

        errorDiv.appendChild(errorList);
        form.insertBefore(errorDiv, form.firstChild);

        // Remove error message after 5 seconds
        setTimeout(() => {
            errorDiv.remove();
        }, 5000);
    }
});