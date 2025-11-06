window.navbarScroll = {
    init: function () {
        const navbar = document.querySelector(".navbar");
        if (!navbar) return;

        window.addEventListener("scroll", () => {
            if (window.scrollY > 33) {
                navbar.classList.add("scrolled");
            } else {
                navbar.classList.remove("scrolled");
            }
        });
    }
};