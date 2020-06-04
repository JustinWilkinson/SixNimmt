window.sixnimmt = {
    getElementCoordinates: function (element) {
        let bounds = element.getBoundingClientRect();
        return { X: bounds.left, Y: bounds.top };
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
            ajax: {
                url: '/api/Game/List',
                dataSrc: function (res) {
                    return res;
                }
            },
            retrieve: true,
            paging: true,
            pageLength: 10,
            lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
            order: [[1, 'desc']],
            columns: [
                {
                    data: 'Name',
                    searchable: true
                },
                {
                    data: 'CreatedAtUtc',
                    render: (data, type) => type === 'display' ? new Date(data).toLocaleString('en-GB') : data
                },
                {
                    data: 'StartedAtUtc',
                    render: data => data ? new Date(data).toLocaleString('en-GB') : 'Not yet started'
                },
                {
                    data: 'CompletedAtUtc',
                    render: (data, type, row) => data ? new Date(data).toLocaleString('en-GB') : (row.StartedAtUtc ? 'In Progress' : 'Not yet started')
                },
                {
                    data: 'Players',
                    render: data => data ? data.length : 0
                },
                {
                    data: 'Players',
                    render: (data, type, row) => row.CompletedAtUtc ? data.sort((a, b) => (a.Points > b.Points) ? 1 : ((b.Points > a.Points) ? -1 : 0))[0].Name : 'Undecided'
                },
                {
                    data: 'Id',
                    render: (data, type, row) => row.CompletedAtUtc ? 'Game Completed' : `<a href="WaitingRoom/${data}"><span class="oi oi-list-rich" aria-hidden="true"></span> Go to game</a>`,
                    orderable: false
                }
            ],
            columnDefs: [
                {
                    targets: '_all',
                    className: 'align-middle',
                    searchable: false,
                    orderable: true
                }
            ],
            language: {
                info: "Showing _START_ to _END_ of _TOTAL_ games.",
                searchPlaceholder: 'Search by name...'
            }
        });
    },
    reloadGamesDataTable: () => $('#GamesTable').DataTable().ajax.reload(),
    runningOnFirefox: function () {
        let isFirefox = navigator.userAgent.indexOf('Firefox') !== -1;
        if (isFirefox) {
            $(document).on('dragover', function (event) {
                $(document).data('dragX', event.originalEvent.clientX);
                $(document).data('dragY', event.originalEvent.clientY);
            });
        }
        return isFirefox;
    },
    getDocumentDragCoordinates: function () {
        return { X: $(document).data('dragX'), Y: $(document).data('dragY') }
    }
}