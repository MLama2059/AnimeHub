// --- SIMPLIFIED HIDING NAV BAR SCRIPT (Function Only) ---
window.setupSmartNavbar = () => {
    const appBar = document.querySelector('.mud-appbar');
    const hideThreshold = 5;

    if (!appBar) {
        console.warn("MudAppBar element not found for smart header setup.");
        return;
    }

    window.onscroll = function () {
        const currentScrollPos = window.pageYOffset;

        // 1. Hide the bar if we scroll past the threshold (scrolling down)
        if (currentScrollPos > hideThreshold) {
            appBar.classList.add("hidden-nav");
        }

        // 2. Show the bar ONLY if we reach the very top of the page
        if (currentScrollPos < 50) {
            appBar.classList.remove("hidden-nav");
        }
    }
};