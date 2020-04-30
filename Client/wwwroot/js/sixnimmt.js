window.sixnimmt = {
    getElementCoordinates: function (element) {
        let bounds = element.getBoundingClientRect();
        let parentStyles = window.getComputedStyle(element.parentElement);
        return { X: bounds.left - parseFloat(parentStyles.paddingLeft), Y: bounds.top - parseFloat(parentStyles.paddingTop) }
    },
    getScreenSize: function () {
        return { Width: window.innerWidth, Height: window.innerHeight }
    },
    getCardSize: function () {
        let cards = document.getElementsByClassName('card');
        if (cards.length > 0) {
            let styles = window.getComputedStyle(cards[0]);
            return { Width: parseFloat(styles.width), Height: parseFloat(styles.height) };
        }
        return null;
    },
    initialiseGamesDataTable: function () {
        $('#GamesTable').DataTable({
            retrieve: true,
            paging: true,
            pageLength: 10,
            lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
            ordering: false,
            columnDefs: [
                {
                    targets: 0,
                    searchable: true
                },
                {
                    targets: '_all',
                    searchable: false
                }
            ],
            language: {
                info: "Showing _START_ to _END_ of _TOTAL_ games."
            }
        });
    },
}