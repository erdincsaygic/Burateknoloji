$(document).ready(function () {

    initDatePicker();
    $("#btnList").click(function () {
        getData();
    });

    getData();
});

function getData() {
    $('#Table').DataTable({
        destroy: true,
        serverSide: true,
        processing: true,
        "language": {
            "emptyTable": "Gösterilecek Veri Yok",
            "info": "Toplam _TOTAL_ veriden _START_ ile _END_ arasındaki veriler gösteriliyor",
            "infoEmpty": "",
            "infoFiltered": "",
            "lengthMenu": "_MENU_ Veri Göster",
            "search": "Ara:",
            "zeroRecords": "Eşleşen Veri Yok",
            "paginate": {
                "previous": "Geri",
                "next": "İleri"
            }
        },
        "ajax": {
            "url": "/AccountingReport/GetIncomeBankTransactionList",
            "type": "POST",
            "data": function (d) {
                d.StartDate = $("#dtStartDate").val();
                d.EndDate = $("#dtEndDate").val();
                d.ID = $("#getID").val();
            },
            "dataSrc": function (json) {

                if (json.data.length == 0 || json.data === []) {
                    total = 0;
                }
                else {

                    total = json.data[0].totalBalance;

                    for (var j = 0; j <= json.data.length - 1; j++) {
                        if (j == 0) {
                            json.data[j].totalBalance = total;
                        }
                        else {
                            if (json.data[j - 1].isPositiveBalance)
                                json.data[j].totalBalance = parseFloat(json.data[j - 1].totalBalance) - parseFloat(json.data[j - 1].amount);
                            else
                                json.data[j].totalBalance = parseFloat(json.data[j - 1].totalBalance) + parseFloat(json.data[j - 1].amount);
                        }
                    }
                }
                return json.data;
            }
        },
        "columns": [
            { "data": "transactionDate", "orderable": false },
            { "data": "bankName", "orderable": false },
            { "data": "senderName", "orderable": false },
            { "data": "description", "orderable": false },
            { "data": "amount", "orderable": false },
            { "data": "totalBalance", "orderable": false }
        ],

        "columnDefs": [
            {
                "aTargets": [0],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = formatDateTime(sData));
                }
            },
            {
                "aTargets": [1],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                }
            },
            {
                "aTargets": [2],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                }
            },
            {
                "aTargets": [3],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                }
            },
            {
                "aTargets": [4],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-end');
                    if (oData.isPositiveBalance)
                        $(nTd).attr('class', 'text-end fw-bold text-success');
                    else
                        $(nTd).attr('class', 'text-end fw-bold text-danger');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [5],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    if (oData.isPositiveBalance)
                        $(nTd).attr('class', 'text-center fw-bold text-success');
                    else
                        $(nTd).attr('class', 'text-center fw-bold text-danger');

                    $(nTd).html(sData.toFixed(2));
                }
            }   
        ],

        "order": [],

        "lengthMenu": [
            [15, 25, 50, 100, 2000],
            [15, 25, 50, 100, 2000]
        ],
    });
}
