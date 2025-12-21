window.setupInfiniteScroll = (dotNetHelper) => {
    const observer = new IntersectionObserver((entries) => {
        if (entries[0].isIntersecting) {
            dotNetHelper.invokeMethodAsync('LoadMoreData');
        }
    }, { threshold: 0.5 });

    const el = document.getElementById('scroll-trigger');
    if (el) observer.observe(el);
};