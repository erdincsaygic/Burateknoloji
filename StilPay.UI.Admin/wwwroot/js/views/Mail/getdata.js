$(document).ready(function () {
    initCompanySelection();

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
            "url": "/Mail/GetDataList",
            "type": "POST",
            "data": function (d) {
                d.IDCompany = $("#slcCompanies").val();
                d.IsSuccess = $("#slcStatus").val();
            },
        },
        "columnDefs": [{
            "targets": [9],
            "searchable": false
        }],
        "columns": [
            { "data": "cDate", "orderable": false },
            { "data": "company", "orderable": false },
            { "data": "email", "orderable": false },
            { "data": "title", "orderable": false },
            { "data": "body", "orderable": false },
            { "data": "isSuccess", "orderable": false },
            { "data": "creator", "orderable": false },
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
                    $(nTd).attr('class', 'text-center');
                }
            },
            {
                "aTargets": [5],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    if (sData)
                        $(nTd).html(sData = '<span class="badge bg-success px-4 py-2">Başarılı</span>');
                    else
                        $(nTd).html(sData = '<span class="badge bg-danger px-4 py-2">Başarısız</span>');
                }
            },        
            {
                "aTargets": [6],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                }
            },
        ],

        "order": [],

        "lengthMenu": [
            [15, 25, 50, 100, 500, 1000],
            [15, 25, 50, 100, 500, 1000]
        ],
    });
}

