document.addEventListener('DOMContentLoaded', function () {
    const dateInput = document.getElementById('reservation-date');
    const timeInput = document.getElementById('reservation-time');
    const peopleSelect = document.getElementById('reservation-people');
    const tableGrid = document.getElementById('table-grid');
    const tableLoading = document.getElementById('table-loading');
    const tableInstructions = document.getElementById('table-instructions');
    const tableLegend = document.getElementById('table-legend');
    const closedWarning = document.getElementById('closed-warning');
    const selectedTableId = document.getElementById('selected-table-id');
    const selectedTableDisplay = document.getElementById('selected-table-display');
    const submitBtn = document.getElementById('submit-btn');

    let openingHours = [];
    let currentSelectedTableId = selectedTableId.value || null;

    // Set min date to today
    const today = new Date();
    const todayStr = today.getFullYear() + '-' +
        String(today.getMonth() + 1).padStart(2, '0') + '-' +
        String(today.getDate()).padStart(2, '0');
    dateInput.setAttribute('min', todayStr);

    // Load opening hours
    fetch('/Reservation?handler=OpeningHours')
        .then(r => r.json())
        .then(data => { openingHours = data; })
        .catch(() => { });

    function getDayOfWeekCz(dateStr) {
        const date = new Date(dateStr);
        const jsDay = date.getDay(); // 0=Sunday
        return jsDay === 0 ? 7 : jsDay; // 1=Monday..7=Sunday
    }

    function getOpeningHoursForDay(dayOfWeekCz) {
        return openingHours.find(h => h.dayOfWeek === dayOfWeekCz) || null;
    }

    function validateDateTime() {
        const dateVal = dateInput.value;
        const timeVal = timeInput.value;

        if (!dateVal) return;

        const dayCz = getDayOfWeekCz(dateVal);
        const hours = getOpeningHoursForDay(dayCz);

        if (!hours || hours.isClosed) {
            closedWarning.classList.remove('d-none');
            tableGrid.innerHTML = '';
            tableLegend.classList.add('d-none');
            tableInstructions.classList.add('d-none');
            clearSelection();
            return;
        }

        closedWarning.classList.add('d-none');

        if (timeVal && hours.openTime && hours.closeTime) {
            if (timeVal < hours.openTime || timeVal > hours.closeTime) {
                closedWarning.classList.remove('d-none');
                closedWarning.innerHTML =
                    '<i class="bi bi-exclamation-triangle me-2"></i>' +
                    '<strong>Mimo otevírací dobu</strong> — V tento den je otevřeno ' +
                    hours.openTime + ' – ' + hours.closeTime + '.';
                tableGrid.innerHTML = '';
                tableLegend.classList.add('d-none');
                clearSelection();
                return;
            }
        }

        loadTables();
    }

    function loadTables() {
        const dateVal = dateInput.value;
        const timeVal = timeInput.value;
        const peopleVal = peopleSelect.value;

        if (!dateVal || !timeVal || !peopleVal) {
            tableInstructions.classList.remove('d-none');
            return;
        }

        tableInstructions.classList.add('d-none');
        tableLoading.classList.remove('d-none');
        tableGrid.innerHTML = '';
        tableLegend.classList.add('d-none');

        const url = '/Reservation?handler=Tables&date=' +
            encodeURIComponent(dateVal) +
            '&time=' + encodeURIComponent(timeVal) +
            '&numberOfPeople=' + encodeURIComponent(peopleVal);

        fetch(url)
            .then(r => r.json())
            .then(tables => {
                tableLoading.classList.add('d-none');
                renderTables(tables);
                tableLegend.classList.remove('d-none');
            })
            .catch(() => {
                tableLoading.classList.add('d-none');
                tableGrid.innerHTML =
                    '<div class="col-12 text-center text-danger py-3">' +
                    '<i class="bi bi-exclamation-circle me-1"></i> Nepodařilo se načíst stoly.</div>';
            });
    }

    function renderTables(tables) {
        tableGrid.innerHTML = '';

        if (tables.length === 0) {
            tableGrid.innerHTML =
                '<div class="col-12 text-center text-muted py-3">' +
                '<i class="bi bi-info-circle me-1"></i> Žádné stoly nejsou k dispozici.</div>';
            return;
        }

        // Check if previously selected table is still available
        let selectionStillValid = false;

        tables.forEach(function (table) {
            const col = document.createElement('div');
            col.className = 'col-6 col-md-4';

            const isSelected = currentSelectedTableId === table.id;
            if (isSelected && table.isAvailable) selectionStillValid = true;

            const card = document.createElement('div');
            card.className = 'table-card' +
                (table.isAvailable ? ' available' : ' unavailable') +
                (isSelected && table.isAvailable ? ' selected' : '');

            card.innerHTML =
                '<div class="table-card-icon">' +
                '<i class="bi bi-' + (table.isAvailable ? 'check-circle' : 'x-circle') + '"></i>' +
                '</div>' +
                '<div class="table-card-name">' + escapeHtml(table.name) + '</div>' +
                '<div class="table-card-capacity">' +
                '<i class="bi bi-people me-1"></i>' + table.capacity + ' osob' +
                '</div>';

            if (table.isAvailable) {
                card.addEventListener('click', function () {
                    selectTable(table.id, table.name);
                });
            }

            col.appendChild(card);
            tableGrid.appendChild(col);
        });

        if (!selectionStillValid) {
            clearSelection();
        }
    }

    function selectTable(tableId, tableName) {
        currentSelectedTableId = tableId;
        selectedTableId.value = tableId;
        selectedTableDisplay.value = tableName;

        document.querySelectorAll('.table-card.available').forEach(function (card) {
            card.classList.remove('selected');
        });

        document.querySelectorAll('.table-card.available').forEach(function (card) {
            if (card.querySelector('.table-card-name').textContent === tableName) {
                card.classList.add('selected');
            }
        });

        updateSubmitButton();
    }

    function clearSelection() {
        currentSelectedTableId = null;
        selectedTableId.value = '';
        selectedTableDisplay.value = 'Zatím nevybrán';
        document.querySelectorAll('.table-card').forEach(function (card) {
            card.classList.remove('selected');
        });
        updateSubmitButton();
    }

    function updateSubmitButton() {
        submitBtn.disabled = !selectedTableId.value;
    }

    function escapeHtml(str) {
        var div = document.createElement('div');
        div.appendChild(document.createTextNode(str));
        return div.innerHTML;
    }

    dateInput.addEventListener('change', validateDateTime);
    timeInput.addEventListener('change', validateDateTime);
    peopleSelect.addEventListener('change', function () {
        clearSelection();
        validateDateTime();
    });

    // If form was resubmitted with errors and values are pre-filled
    if (dateInput.value && timeInput.value && peopleSelect.value) {
        validateDateTime();
    }

    updateSubmitButton();
});
