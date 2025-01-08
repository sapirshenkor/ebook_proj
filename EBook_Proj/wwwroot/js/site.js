// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function showBookDetails(id, title, author, description, buyPrice, borrowPrice, genre, publicationDate, coverImage, isPopular, originalPrice, discount) {
    // Store book data in modal's dataset
    const modalElement = document.getElementById('bookDetailsModal');
    modalElement.dataset.id = id;
    modalElement.dataset.title = title;
    modalElement.dataset.buyPrice = buyPrice;
    modalElement.dataset.borrowPrice = borrowPrice;
    modalElement.dataset.cover = coverImage;

    // Update modal content
    document.getElementById('modalBookTitle').textContent = title;
    document.getElementById('modalBookAuthor').textContent = author;
    document.getElementById('modalBookDescription').textContent = description || 'No description available';
    document.getElementById('modalBookBuyPrice').textContent = `$${buyPrice}`;
    document.getElementById('modalBookBorrowPrice').textContent = `$${borrowPrice}`;
    document.getElementById('modalBookGenre').textContent = genre || 'Not specified';
    document.getElementById('modalBookDate').textContent = publicationDate || 'Not specified';

    const modalBookCover = document.getElementById('modalBookCover');
    modalBookCover.src = coverImage || 'https://via.placeholder.com/300x400';
    modalBookCover.alt = title;

    // Handle popular book display
    const borrowSection = document.getElementById('borrowSection');
    const popularBadge = document.getElementById('popularBookBadge');

    if (isPopular) {
        borrowSection.style.display = 'none';
        popularBadge.classList.remove('d-none');
    } else {
        borrowSection.style.display = 'block';
        popularBadge.classList.add('d-none');
    }
    const priceElement=document.querySelector('.cart-item-price');
    // If there's a discount
    if (priceElement) {
        if (discount && discount !== '0') {
            priceElement.innerHTML = `
        <span class="badge bg-primary me-2 text-decoration-line-through">Buy: $${parseFloat(originalPrice).toFixed(2)}</span>
        <span class="badge bg-danger">Sale: $${parseFloat(buyPrice).toFixed(2)}</span>
    `;
        }
        // If no discount
        else {
            priceElement.innerHTML = `
        <span class="badge bg-primary">$${parseFloat(buyPrice).toFixed(2)}</span>
    `;
        }
    }

    const modal = new bootstrap.Modal(modalElement);
    modal.show();
}

document.addEventListener('DOMContentLoaded', function() {
    document.querySelectorAll('[data-book-details]').forEach(card => {
        card.addEventListener('click', function() {
            showBookDetails(
                this.dataset.id,
                this.dataset.title,
                this.dataset.author,
                this.dataset.description,
                this.dataset.buyPrice,
                this.dataset.borrowPrice,
                this.dataset.genre,
                this.dataset.pubdate,
                this.dataset.cover,
                this.dataset.isPopular === 'true',
                this.dataset.originalPrice,
                this.dataset.discount
            );
        });
    });
});
