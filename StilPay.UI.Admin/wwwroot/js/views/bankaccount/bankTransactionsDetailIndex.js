

$(document).ready(function () {
    $(".datepicker").datepicker({
        dateFormat: "dd.mm.yy",
        altFieldTimeOnly: false,
        altFormat: "yy-mm-dd",
        altTimeFormat: "h:m",
        altField: "#tarih-db",
        monthNames: ["Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"],
        dayNamesMin: ["Pa", "Pt", "Sl", "Ça", "Pe", "Cu", "Ct"],
        firstDay: 1
    });

    $("#btnList").click(function () {
        Table.init();
    });

    Table.init();

})

var isInit = false;
var mainData = [];
var Table = function () {

    var initTable = function () {

        var table = $('#tblIncome');

        var otable = table.dataTable({

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

            "columnDefs": [
                {
                    "searchable": false,
                    "targets": [4]
                },
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
                        $(nTd).attr('class', 'text-end');
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
                }
            ],

            "order": [],

            "lengthMenu": [
                [15, 25, 50, 100, -1],
                [15, 25, 50, 100, "Tümü"]
            ],

            "columns": [
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                }
            ]
        });

        var tableWrapper = $('#Table_wrapper');
        tableWrapper.find('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
        });

        var table2 = $('#tblExpense');

        var otable2 = table2.dataTable({

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

            "columnDefs": [
                {
                    "searchable": false,
                    "targets": [4]
                },
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
                        $(nTd).attr('class', 'text-end');
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
                }
            ],

            "order": [],

            "lengthMenu": [
                [15, 25, 50, 100, -1],
                [15, 25, 50, 100, "Tümü"]
            ],

            "columns": [
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                }
            ]
        });

        var tableWrapper = $('#Table_wrapper');
        tableWrapper.find('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
        });

        var table3 = $('#tblIncomePaymentInstitution');

        var otable3 = table3.dataTable({

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

            "columnDefs": [
                {
                    "searchable": false,
                    "targets": [5]
                },
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
                        $(nTd).attr('class', 'text-end');
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
                    }
                }
            ],

            "order": [],

            "lengthMenu": [
                [15, 25, 50, 100, -1],
                [15, 25, 50, 100, "Tümü"]
            ],

            "columns": [
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                }
            ]
        });

        var tableWrapper = $('#Table_wrapper');
        tableWrapper.find('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
        });

        var table4 = $('#tblIncomeBank');

        var otable4 = table4.dataTable({

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

            "columnDefs": [
                {
                    "searchable": false,
                    "targets": [5]
                },
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
                        $(nTd).attr('class', 'text-end');
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
                    }
                }
            ],

            "order": [],

            "lengthMenu": [
                [15, 25, 50, 100, -1],
                [15, 25, 50, 100, "Tümü"]
            ],

            "columns": [
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                }
            ]
        });

        var tableWrapper = $('#Table_wrapper');
        tableWrapper.find('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
        });


        var table5 = $('#tblExpenseBank');

        var otable5 = table5.dataTable({

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

            "columnDefs": [
                {
                    "searchable": false,
                    "targets": [5]
                },
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
                        $(nTd).attr('class', 'text-end');
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
                    }
                }
            ],

            "order": [],

            "lengthMenu": [
                [15, 25, 50, 100, -1],
                [15, 25, 50, 100, "Tümü"]
            ],

            "columns": [
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                }
            ]
        });

        var tableWrapper = $('#Table_wrapper');
        tableWrapper.find('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
        });
    }

    var initData = function () {
        $.ajax({
            type: "POST",
            url: 'BankTransactionsDetailIncomeList?id=0/',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                StartDate: $("#dtStartDate").val(),
                EndDate: $("#dtEndDate").val()
            }),
            beforeSend: function () {
                showLoading();

                var otbl = $('#tblIncome').DataTable();
                otbl.clear().draw();
                mainData = [];
            },
            success: function (list) {
                var grid = [];
                if (list)
                    $.each(list, function (i, item) {
                        grid.push([item.transactionDate, item.amount.toFixed(2), item.incomeType , item.description, item.cUserName]);
                        mainData.push({
                            İşlemTarihi: item.transactionDate,
                            Tutar: item.amount.toFixed(2),
                            GelirTipi: item.incomeType,
                            Açıklama: item.description,
                            Yetkili: item.cUserName
                        });
                    });

                var otbl = $('#tblIncome').DataTable();
                otbl.rows.add(grid);
                otbl.draw();
            },
            error: function () {
                onFailure();
            },
            complete: function () {
                hideLoading();
            }
        });
    }

    var initData2 = function () {
        $.ajax({
            type: "POST",
            url: 'BankTransactionsDetailExpenseList?id=0/',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                StartDate: $("#dtStartDate").val(),
                EndDate: $("#dtEndDate").val()
            }),
            beforeSend: function () {
                showLoading();

                var otbl = $('#tblExpense').DataTable();
                otbl.clear().draw();
                mainData = [];
            },
            success: function (list) {
                var grid = [];
                if (list)
                    $.each(list, function (i, item) {
                        grid.push([item.transactionDate, item.amount.toFixed(2), item.expenseType, item.description, item.cUserName]);
                        mainData.push({
                            İşlemTarihi: item.transactionDate,
                            Tutar: item.amount.toFixed(2),
                            GelirTipi: item.expenseType,
                            Açıklama: item.description,
                            Yetkili: item.cUserName
                        });
                    });

                var otbl = $('#tblExpense').DataTable();
                otbl.rows.add(grid);
                otbl.draw();
            },
            error: function () {
                onFailure();
            },
            complete: function () {
                hideLoading();
            }
        });
    }

    var initData3 = function () {
        $.ajax({
            type: "POST",
            url: 'BankTransactionsDetailPaymentInstitutionsList?id=0/',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                StartDate: $("#dtStartDate").val(),
                EndDate: $("#dtEndDate").val()
            }),
            beforeSend: function () {
                showLoading();

                var otbl = $('#tblIncomePaymentInstitution').DataTable();
                otbl.clear().draw();
                mainData = [];
            },
            success: function (list) {
                var grid = [];
                if (list)
                    $.each(list, function (i, item) {
                        grid.push([item.transactionDate, item.amount.toFixed(2), item.incomeType, item.description, item.paymentMethodName, item.cUserName]);
                        mainData.push({
                            İşlemTarihi: item.transactionDate,
                            Tutar: item.amount.toFixed(2),
                            GelirTipi: item.incomeType,
                            Açıklama: item.description,
                            ÖdemeKuruluşu: item.paymentMethodName,
                            Yetkili: item.cUserName
                        });
                    });

                var otbl = $('#tblIncomePaymentInstitution').DataTable();
                otbl.rows.add(grid);
                otbl.draw();
            },
            error: function () {
                onFailure();
            },
            complete: function () {
                hideLoading();
            }
        });
    }

    var initData4 = function () {
        $.ajax({
            type: "POST",
            url: 'BankTransactionsDetailIncomeBankList?id=0/',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                StartDate: $("#dtStartDate").val(),
                EndDate: $("#dtEndDate").val()
            }),
            beforeSend: function () {
                showLoading();

                var otbl = $('#tblIncomeBank').DataTable();
                otbl.clear().draw();
                mainData = [];
            },
            success: function (list) {
                var grid = [];
                if (list)
                    $.each(list, function (i, item) {
                        grid.push([item.transactionDate, item.amount.toFixed(2), item.incomeType, item.description, item.idBankName2,  item.cUserName]);
                        mainData.push({
                            İşlemTarihi: item.transactionDate,
                            Tutar: item.amount.toFixed(2),
                            GelirTipi: item.incomeType,
                            Açıklama: item.description,
                            ÖdemeKuruluşu: item.idBankName2,
                            Yetkili: item.cUserName
                        });
                    });

                var otbl = $('#tblIncomeBank').DataTable();
                otbl.rows.add(grid);
                otbl.draw();
            },
            error: function () {
                onFailure();
            },
            complete: function () {
                hideLoading();
            }
        });
    }

    var initData5 = function () {
        $.ajax({
            type: "POST",
            url: 'BankTransactionsDetailExpenseBankList?id=0/',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                StartDate: $("#dtStartDate").val(),
                EndDate: $("#dtEndDate").val()
            }),
            beforeSend: function () {
                showLoading();

                var otbl = $('#tblExpenseBank').DataTable();
                otbl.clear().draw();
                mainData = [];
            },
            success: function (list) {
                var grid = [];
                if (list)
                    $.each(list, function (i, item) {
                        grid.push([item.transactionDate, item.amount.toFixed(2), item.incomeType, item.description, item.idBankName2, item.cUserName]);
                        mainData.push({
                            İşlemTarihi: item.transactionDate,
                            Tutar: item.amount.toFixed(2),
                            GelirTipi: item.incomeType,
                            Açıklama: item.description,
                            ÖdemeKuruluşu: item.idBankName2,
                            Yetkili: item.cUserName
                        });
                    });

                var otbl = $('#tblExpenseBank').DataTable();
                otbl.rows.add(grid);
                otbl.draw();
            },
            error: function () {
                onFailure();
            },
            complete: function () {
                hideLoading();
            }
        });
    }


    return {
        init: function () {
            if (!jQuery().dataTable) {
                return;
            }
            if (!isInit) {
                initTable();
                isInit = true;
            }
            initData();
            initData2();
            initData3();
            initData4();
            initData5();
        }
    };

}();

function idBankChange(idBankId) {

    $("#CompanyIntegrationType").val(idBankId);
    $("#CompanyIntegrationType").val(idBankId);
};