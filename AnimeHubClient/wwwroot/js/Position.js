window.isNearRightEdge = (element) => {
    const rect = element.getBoundingClientRect();
    return rect.right + 280 > window.innerWidth;
};
