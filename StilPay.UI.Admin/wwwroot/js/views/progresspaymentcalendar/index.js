$(document).ready(function () {
    var calendarEl = document.getElementById('calendar');

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        events: '/ProgressPaymentCalendar/GetEvents',
        locale: 'tr',
        headerToolbar: {
            right: 'today prev,next',
            center: 'title',
            left: ''
        },
        buttonText: {
            today: 'Şu anki Ay',
            prev: 'Önceki Ay',
            next: 'Sonraki Ay'
        },
        eventDidMount: function (info) {
            // Add custom content to event element
            var progressPayment = info.event.extendedProps.progressPayment;
            var paymentInstitutionName = info.event.extendedProps.paymentInstitutionName;

            var content = document.createElement('div');
            content.innerHTML = `
                        <p class="pyName">Ödeme Kuruluşu: <span class="pyValue">${paymentInstitutionName}</span></p>
                        <p class="pyTotal">Hakediş: <span class="pyValue">${progressPayment}</span></p>
                    `;
            info.el.appendChild(content);
        }
    });

    calendar.render();
});