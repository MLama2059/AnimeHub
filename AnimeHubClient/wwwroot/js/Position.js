window.checkIfPopoverWillFlip = (element) => {
    if (!element) return false;
    const rect = element.getBoundingClientRect();
    const popoverWidth = 310; // 280px + 30px gap/buffer

    // Check if space on the right is less than the popover width
    const spaceOnRight = window.innerWidth - rect.right;
    return spaceOnRight < popoverWidth;
};