window.SmartNavBarShrink = {
    init: function () {
        const navbar = document.querySelector(".mud-appbar");
        if (!navbar) return;

        const fullHeight = "70px"; // original navbar height
        const miniHeight = "45px"; // shrunk height

        window.addEventListener("scroll", function () {
            if (window.pageYOffset > 50) {
                navbar.style.height = miniHeight;
            } else {
                navbar.style.height = fullHeight;
            }
        });
    }
};
