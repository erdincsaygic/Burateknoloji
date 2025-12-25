$(document).ready(function () {
    initCompanySelectionWithoutAll();

    $("#btnList").click(function () {
        initCalendar();
    });
});


function initCalendar() {

    var calendarEl = document.getElementById('calendar');

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        events: {
            url: '/CompanyProgressPaymentCalendar/GetEvents',
            type: 'GET',
            extraParams: function () {
                return {
                    idCompany: $('#slcCompanies').val()
                };
            }
        },
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
            //var companyName = info.event.extendedProps.companyName;
            var rebateAmount = info.event.extendedProps.rebateAmount;

            var content = document.createElement('div');
            content.innerHTML = `
                        <p class="pyName">İade Tutarı: <span class="rbValue">${rebateAmount}</span></p>
                        <p class="pyTotal">Hakediş: <span class="pyValue">${progressPayment}</span></p>
                    `;
            info.el.appendChild(content);
        }
    });

    calendar.render();
}