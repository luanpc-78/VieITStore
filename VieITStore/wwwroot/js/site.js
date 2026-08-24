function updateCartCount() {
    if (!window.storeUrls?.cartCount) return;
    $.get(window.storeUrls.cartCount).done(function (data) {
        $('#cartCount').text(data.count ?? 0);
    });
}

function showStoreToast(message, type = 'success') {
    const region = document.getElementById('storeToastRegion');
    if (!region) return;
    const toast = document.createElement('div');
    toast.className = `toast show text-bg-${type}`;
    toast.setAttribute('role', type === 'danger' ? 'alert' : 'status');
    toast.innerHTML = `<div class="d-flex"><div class="toast-body">${escapeStoreHtml(message)}</div><button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Đóng"></button></div>`;
    region.appendChild(toast);
    window.setTimeout(() => toast.remove(), 3500);
}

function escapeStoreHtml(value) {
    const element = document.createElement('div');
    element.textContent = value ?? '';
    return element.innerHTML;
}

function setCartButtonLoading(button, loading) {
    if (!button) return;
    button.disabled = loading || button.dataset.unavailable === 'true';
    button.classList.toggle('is-loading', loading);
    const label = button.querySelector('[data-button-label]');
    if (label) label.textContent = loading ? 'Đang thêm...' : (button.dataset.defaultLabel || 'Thêm vào giỏ');
}

function formatVND(amount) {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
}

function initializeHeroShowcase() {
    const root = document.querySelector('[data-hero-showcase]');
    if (!root) return;
    const slides = [...root.querySelectorAll('[data-slide-index]')];
    const dots = [...root.querySelectorAll('[data-carousel-dot]')];
    const previous = root.querySelector('[data-carousel-prev]');
    const next = root.querySelector('[data-carousel-next]');
    const message = root.querySelector('.hero-message');
    const messageText = root.querySelector('[data-message-text]');
    const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    const messages = [
        ['fa-shield-alt', 'Bảo hành rõ ràng'], ['fa-certificate', 'Sản phẩm chính hãng'],
        ['fa-tag', 'Giá bán minh bạch'], ['fa-truck', 'Giao hàng nhanh chóng'],
        ['fa-rotate-left', 'Đổi trả thuận tiện'], ['fa-headset', 'Hỗ trợ tận tâm']
    ];
    let current = 0, messageIndex = 0, slideTimer = null, messageTimer = null, locked = false, paused = false;
    const clearTimers = () => { window.clearTimeout(slideTimer); window.clearTimeout(messageTimer); slideTimer = messageTimer = null; };
    const schedule = () => {
        clearTimers();
        if (paused || document.hidden || reduceMotion) return;
        if (slides.length > 1) slideTimer = window.setTimeout(() => changeSlide((current + 1) % slides.length, 1), 5000);
        messageTimer = window.setTimeout(changeMessage, 3000);
    };
    const changeMessage = () => {
        if (!message || !messageText || paused || document.hidden) return;
        message.classList.add('is-leaving');
        window.setTimeout(() => {
            messageIndex = (messageIndex + 1) % messages.length;
            const icon = message.querySelector('i');
            if (icon) icon.className = `fas ${messages[messageIndex][0]}`;
            messageText.textContent = messages[messageIndex][1];
            message.classList.remove('is-leaving'); message.classList.add('is-entering');
            window.requestAnimationFrame(() => window.requestAnimationFrame(() => message.classList.remove('is-entering')));
            messageTimer = window.setTimeout(changeMessage, 3000);
        }, 300);
    };
    const changeSlide = (target, direction) => {
        if (locked || target === current || !slides[target]) { schedule(); return; }
        locked = true; clearTimers();
        const outgoing = slides[current], incoming = slides[target];
        const exitClass = direction > 0 ? 'is-exiting-left' : 'is-exiting-right';
        const enterClass = direction > 0 ? 'is-entering-right' : 'is-entering-left';
        incoming.className = `product-slide ${enterClass}`;
        incoming.setAttribute('aria-hidden', 'false');
        incoming.querySelector('a')?.setAttribute('tabindex', '0');
        window.requestAnimationFrame(() => window.requestAnimationFrame(() => {
            outgoing.classList.remove('is-active'); outgoing.classList.add(exitClass);
            incoming.classList.remove(enterClass); incoming.classList.add('is-active');
        }));
        window.setTimeout(() => {
            outgoing.className = 'product-slide'; outgoing.setAttribute('aria-hidden', 'true');
            outgoing.querySelector('a')?.setAttribute('tabindex', '-1');
            current = target; dots.forEach((dot, index) => { dot.classList.toggle('is-active', index === current); dot.setAttribute('aria-current', index === current ? 'true' : 'false'); });
            locked = false; previous && (previous.disabled = false); next && (next.disabled = false); schedule();
        }, reduceMotion ? 0 : 600);
        if (previous) previous.disabled = true; if (next) next.disabled = true;
    };
    previous?.addEventListener('click', () => changeSlide((current - 1 + slides.length) % slides.length, -1));
    next?.addEventListener('click', () => changeSlide((current + 1) % slides.length, 1));
    dots.forEach((dot, index) => dot.addEventListener('click', () => changeSlide(index, index > current ? 1 : -1)));
    const pause = () => { paused = true; clearTimers(); };
    const resume = () => { paused = false; schedule(); };
    root.addEventListener('mouseenter', pause); root.addEventListener('mouseleave', resume);
    root.addEventListener('focusin', pause); root.addEventListener('focusout', event => { if (!root.contains(event.relatedTarget)) resume(); });
    document.addEventListener('visibilitychange', () => document.hidden ? pause() : resume());
    schedule();
}

document.addEventListener('DOMContentLoaded', initializeHeroShowcase);
