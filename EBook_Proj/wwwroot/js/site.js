﻿// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function showBookDetails(id, title, author, description, buyPrice, borrowPrice, genre, publicationDate, coverImage) {
    // Update modal content
    document.getElementById('modalBookTitle').textContent = title;
    document.getElementById('modalBookAuthor').textContent = author;
    document.getElementById('modalBookDescription').textContent = description || 'No description available';
    document.getElementById('modalBookBuyPrice').textContent = `$${buyPrice}`;
    document.getElementById('modalBookBorrowPrice').textContent = `$${borrowPrice}`;
    document.getElementById('modalBookGenre').textContent = genre || 'Not specified';
    document.getElementById('modalBookDate').textContent = publicationDate || 'Not specified';

    // Update cover image
    const modalBookCover = document.getElementById('modalBookCover');
    modalBookCover.src = coverImage || 'https://via.placeholder.com/300x400';
    modalBookCover.alt = title;

    // Show the modal
    const modal = new bootstrap.Modal(document.getElementById('bookDetailsModal'));
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
                this.dataset.cover
            );
        });
    });
});
function isUserLoggedIn() {
    return '@Context.Session.GetString("Email")' !== '';
}