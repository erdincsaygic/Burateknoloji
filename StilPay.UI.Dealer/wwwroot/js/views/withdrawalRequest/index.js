
$(document).ready(function () {
    initDatePicker();

    $("#btnExcel").click(function () {
        downloadExcel("Çekim-Talepleri", mainData);
    });

    $("#btnList").click(function () {
        getData();
    });

    getData();
});

var list = [];
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
            "url": "/WithdrawalRequest/GetData",
            "type": "POST",
            "data": function (d) {
                d.StartDate = $("#dtStartDate").val();
                d.EndDate = $("#dtEndDate").val();
            },
            "dataSrc": function (json) {
                list = [];
                json.data.forEach(function (item) {
                    list.push(item);
                });

                return list;
            }
        },
        "columns": [
            { "data": "cDate", "orderable": false },
            { "data": "transactionNr", "orderable": false },
            { "data": "title", "orderable": false },
            { "data": "amount", "orderable": false },
            { "data": "costTotal", "orderable": false },
            { "data": "currencyCode", "orderable": false },
            { "data": "status", "orderable": false },
            { "data": "mDate", "orderable": false },
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
                    $(nTd).html(sData = '<a href="javascript:ShowDetail(\'' + oData.id + '\')">' + sData + '</a>');
                }
            },
            {
                "aTargets": [3],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [4],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [5],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                }
            },
            {
                "aTargets": [6],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    if (sData === 1 || sData === 8) {
                        $(nTd).attr('class', 'text-center fw-bold text-info');
                        $(nTd).html(sData = StatusToText(sData));
                    }
                    else if (sData === 2) {
                        $(nTd).attr('class', 'text-center fw-bold text-success');
                        $(nTd).html(sData = StatusToText(sData));
                    }
                    else {
                        $(nTd).attr('class', 'text-center fw-bold text-danger');
                        $(nTd).html(sData = StatusToText(sData) + (oData.description != null && oData.description != '' ? '<br/>' + oData.description : ''));
                    }

                }
            },
            {
                "aTargets": [7],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = formatDateTime(sData));
                }
            },
        ],

        "order": [],

        "lengthMenu": [
            [15, 25, 50, 100, -1],
            [15, 25, 50, 100, "Tümü"]
        ],
    });
}


function StatusToText(status) {
    if (status === 1)
        return 'BEKLİYOR';
    else if (status === 2)
        return 'ONAYLANDI';
    else if (status === 8)
        return 'İŞLEME ALINDI';
    else
        return 'İPTAL EDİLDİ';
}


function HideShow() {
    $("#idFilter").slideToggle(500);
}

function ShowDetail(id) {
    for (var i = 0; i < list.length; i++) {
        var item = list[i];
        if (item.id === id) {
            $("#txtDetailBank").val(item.bank);
            $("#txtDetailIBAN").val(item.iban);
            $("#txtDetailTitle").val(item.title);
            $("#txtDetailType").val(item.CurrencyCode != "TRY" ?  'Swift' : item.isEFT ? 'Havale/EFT' : 'FAST');
            $("#mdlDetail").modal('show');
            break;
        }
    }
}