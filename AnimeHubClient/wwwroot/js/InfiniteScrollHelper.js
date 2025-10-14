function setupInfiniteScroll(dotNetHelper) {
    const scrollHandler = () => {
        const scrollTop = document.documentElement.scrollTop;
        const scrollHeight = document.documentElement.scrollHeight;
        const clientHeight = document.documentElement.clientHeight;

        // Trigger loading when within 500px of the bottom
        const threshold = 500;

        if (scrollTop + clientHeight >= scrollHeight - threshold) {
            // Call the Blazor C# method decorated with [JSInvokable]
            dotNetHelper.invokeMethodAsync('LoadMoreData');
        }
    };

    // Attach the event listener
    window.addEventListener('scroll', scrollHandler);
}