window.checkIfPopoverWillFlip = (element) => {
    if (!element) return false;
    const rect = element.getBoundingClientRect();
    const popoverWidth = 320; // 280px content + margins/padding
    const availableSpaceOnRight = window.innerWidth - rect.right;

    // Flip ONLY if the space on the right is less than the popover width
    return availableSpaceOnRight < popoverWidth;
};