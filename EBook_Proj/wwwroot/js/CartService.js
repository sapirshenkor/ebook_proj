class CartService {
    constructor() {
        this.userId = this.getUserId();
        this.cartKey = `bookCart_${this.userId}`;
        this.isUserLoggedIn = this.getUserId() !== 'guest';
        this.initialize();
    }
    
    initialize() {
        document.addEventListener('DOMContentLoaded', () => {
            this.updateCartCount();
            if (document.querySelector('#cartItems')) {
                this.displayCart();
            }
        });
    }
    getUserId() {
        // Get user ID from session
        return document.querySelector('[data-user-id]')?.dataset.userId || 'guest';
    }
    getCartKey(){
        return this.cartKey;
    }
    getCart() {
        return JSON.parse(localStorage.getItem(this.cartKey)) || [];
    }

    addToCart(book, type) { // type can be 'buy' or 'borrow'
        if (!this.isUserLoggedIn) {
            window.location.href = '/User/Login';
            return;
        }
        const cart = this.getCart();
        
        // Check if book already exists in cart with same id
        const existingItem = cart.find(item =>
            item.bookId === book.id 
        );
        if (existingItem) {
            alert('This book is already in your cart!');
            return;
        }
        var countBorrowedBooks = cart.filter(item => item.type === 'borrow').length;
        if (countBorrowedBooks >= 3 && type === "borrow") {
            alert('user cannot borrow more than 3 books!');
            return;
        }
        const price = type === 'buy' ? parseFloat(book.buyPrice) : parseFloat(book.borrowPrice);
        cart.push({
            bookId: book.id,
            title: book.title,
            price: price.toFixed(2),
            cover: book.cover,
            quantity: 1,
            type: type
        });
        localStorage.setItem(this.cartKey, JSON.stringify(cart));
        this.updateCartCount();
        alert(`Book added to cart for ${type}ing!`);
    }

    removeFromCart(bookId) {
        let cart = this.getCart();
        cart = cart.filter(item => item.bookId !== bookId);
        localStorage.setItem(this.cartKey, JSON.stringify(cart));
        this.updateCartCount();
        this.displayCart();
    }

    updateCartCount() {
        if (this.isUserLoggedIn) {
            const count = this.getCart().length;
            const badge = document.querySelector('.cart-count');
            if (badge) badge.textContent = count;
        }
    }

    clearCart() {
        localStorage.removeItem(this.cartKey);
        this.displayCart();
        this.updateCartCount();
        alert('Cart has been cleared!');
    }
    displayCart() {
        const cart = this.getCart();
        const cartContainer = document.querySelector('#cartItems .card-body');
        const template = document.getElementById('cartItemTemplate');
        let buySubtotal = 0;
        let borrowSubtotal = 0;

        if (!cartContainer) return;
        cartContainer.innerHTML = '';

        if (cart.length === 0) {
            cartContainer.innerHTML = '<p class="text-center my-5">Your cart is empty</p>';
            this.updateOrderSummary(0, 0);
            return;
        }

        cart.forEach(item => {
            const clone = template.content.cloneNode(true);
            const itemTotal = item.price * item.quantity;

            if (item.type === 'buy') {
                buySubtotal += itemTotal;
            } else {
                borrowSubtotal += itemTotal;
            }

            clone.querySelector('.cart-item-image').src = item.cover;
            clone.querySelector('.cart-item-title').textContent = item.title;
            clone.querySelector('.cart-item-price').textContent = item.price;
            clone.querySelector('.cart-item-type').textContent = item.type === 'buy' ? 'Purchase' : 'Borrow';
            clone.querySelector('.quantity-input').value = item.quantity;
            clone.querySelector('.remove-item').onclick = () => this.removeFromCart(item.bookId);

            cartContainer.appendChild(clone);
        });
        this.updateOrderSummary(buySubtotal, borrowSubtotal);
    }
    updateOrderSummary(buySubtotal, borrowSubtotal) {
        document.getElementById('buySubtotal').textContent = `$${buySubtotal.toFixed(2)}`;
        document.getElementById('borrowSubtotal').textContent = `$${borrowSubtotal.toFixed(2)}`;
        document.getElementById('cartTotal').textContent = `$${(buySubtotal + borrowSubtotal).toFixed(2)}`;
    }
    //checkout process for the cart
    checkout(){
        const cart = this.getCart();

        fetch('/Cart/Checkout', {
            method: 'POST',
            headers:{
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(cart)
        })
            .then(response => response.json())
            .then(data => {
                if(data.success) {
                    // Redirect to payment selection page
                    window.location.href = '/Payment/Select';
                } else {
                    
                    alert(data.message);
                    
                }
            })
            .catch(error => {
                console.error('Error:', error);
                alert('Error processing checkout. Please try again.');
            });
    }
}

if (!window.cartService) {
    window.cartService = new CartService();
}
