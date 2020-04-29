window.sixnimmt = {
    getElementCoordinates: function (element) {
        let bounds = element.getBoundingClientRect();
        let parentStyles = window.getComputedStyle(element.parentElement);
        return { X: bounds.left - parseFloat(parentStyles.paddingLeft), Y: bounds.top - parseFloat(parentStyles.paddingTop) }
    }
}