function initMemberSelection() {
    $('#slcMembers').select2({
        placeholder: 'Bireysel Üye Seçiniz',
        allowClear: true,
        ajax: {
            url: '/Member/GetActives',
            type: 'POST',
            dataType: 'json',
            delay: 250, // Delay to avoid making too many requests
            data: function (params) {
                return {
                    length: 20, // Number of results to return
                    start: (params.page || 0) * 20, // Offset calculation
                    search: {
                        value: params.term // Search term entered by the user
                    }
                };
            },
            processResults: function (data, params) {
                // Parse the results into the format expected by Select2
                return {
                    results: $.map(data.data, function (item) {
                        return {
                            id: item.id,
                            text: item.name // Assuming 'name' contains the "Name - Phone" format
                        };
                    }),
                    pagination: {
                        more: data.recordsFiltered > ((params.page || 0) + 1) * 20
                    }
                };
            },
            cache: true
        },
        minimumInputLength: 1,
        language: {
            inputTooShort: function () {
                return 'Lütfen arama yapmak için en az 1 karakter girin';
            },
            searching: function () {
                return "Aranıyor..."
            }
        }
    });
}

function initCompanySelection() {
    $.ajax({
        async: false,
        type: "GET",
        url: '/Dealer/GetActives',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({}),
        success: function (list) {
            $('#slcCompanies').append(new Option("Üye İşyeri Seçiniz", "", true, true))
            $('#slcCompanies').append(new Option("Tümü", "all", false, false))
            if (list)
                $.each(list, function (i, item) {
                    $('#slcCompanies').append(new Option(item.name, item.id, false, false))
                })

            $('#slcCompanies').select2()
        }
    })
}

function initCompanySelectionWithoutAll(isSelect2 = true) {
    $.ajax({
        async: false,
        type: "GET",
        url: '/Dealer/GetActives',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({}),
        success: function (list) {
            $('#slcCompanies').append(new Option("Üye İşyeri Seçiniz", "", true, true))
            if (list)
                $.each(list, function (i, item) {
                    $('#slcCompanies').append(new Option(item.name, item.id, false, false))
                })

            if (isSelect2) {
                $('#slcCompanies').select2()
            }
        }
    })
}

function initMultipleCompanySelectionWithoutAll() {
    $.ajax({
        async: false,
        type: "GET",
        url: '/Dealer/GetActives',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({}),
        success: function (list) {
    
            if (list) {
                $.each(list, function (i, item) {
                    $('#slcCompanies').append(new Option(item.name, item.id, false, false));
                });
            }

            $('#slcCompanies').select2({    
                allowClear: true,
                placeholder: 'Üye işyeri seçiniz',
                multiple: true,
                closeOnSelect: false,
                search:false,
                theme: 'bootstrap-5',
                minimumResultsForSearch: -1
            }).on('select2:opening', function () {

                $('.select2-search--inline').attr('hidden', true);
            });;



            $('#slcCompanies').on('select2:unselecting', function (e) {
                if (e.params.args.originalEvent && !$(e.params.args.originalEvent.currentTarget).hasClass('select2-selection__choice__remove')) {
                    e.preventDefault(); 
                }
            });
        }
    });
}


function initMonthAndYearSelection(autoSelect, month,year) {
    var months = [
        "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran",
        "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"
    ];

    var currentMonth = new Date().getMonth() + 1;
    for (var i = 1; i <= 12; i++) {
        if (autoSelect) {
            $('#slcMonth').append(new Option(months[i - 1], i, i === currentMonth, i === currentMonth));
        }else {
            $('#slcMonth').append(new Option(months[i - 1], i, month, month));
        }
    }

    var currentYear = new Date().getFullYear();
    for (var i = currentYear; i >= 2023; i--) {
        if (autoSelect) {
            $('#slcYear').append(new Option(i, i, i === currentYear, i === currentYear));
        }else {
            $('#slcYear').append(new Option(i, i, year, year));
        }

    }
}

function initActionTypesSelection() {
    $('#slcActionTypes').append(new Option("İşlem Tipi Seçiniz", "", true, true))
    $('#slcActionTypes').append(new Option("Ödeme (Otomatik)", "10", false, false))
    $('#slcActionTypes').append(new Option("Ödeme (IFrame Bildirim)", "20", false, false))
    $('#slcActionTypes').append(new Option("Üye İşyeri Nakit Çekim", "30", false, false))
    $('#slcActionTypes').append(new Option("Bireysel Üye Nakit Çekim", "40", false, false))
    $('#slcActionTypes').append(new Option("Para Transferi (Giden)", "50", false, false))
    $('#slcActionTypes').append(new Option("Para Transferi (Gelen)", "60", false, false))
    $('#slcActionTypes').append(new Option("Üye İşyeri Bakiye Yükeme", "70", false, false))
    $('#slcActionTypes').append(new Option("Bireysel Üye Bakiye Yükeme", "80", false, false))
    $('#slcActionTypes').append(new Option("İade", "90", false, false))
    $('#slcActionTypes').append(new Option("Kredi Kartı Ödeme (Otomatik)", "100", false, false))
    $('#slcActionTypes').append(new Option("Kredi Kartı Ödeme", "110", false, false))
}

function initDatePicker() {
    $(".datepicker").datepicker({
        //changeMonth: true,
        //changeYear: true,
        dateFormat: "dd.mm.yy",
        altFormat: "dd.mm.yy",
        monthNames: ["Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"],
        dayNamesMin: ["Pa", "Pt", "Sl", "Ça", "Pe", "Cu", "Ct"],
        firstDay: 1
    }).datepicker('setDate', 'today');
}

function initDatePickerLastMonth() {
    const today = new Date();
    const oneMonthAgo = new Date();
    oneMonthAgo.setMonth(today.getMonth() - 1);

    $(".datepicker-start").datepicker({
        dateFormat: "dd.mm.yy",
        altFormat: "dd.mm.yy",
        monthNames: ["Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"],
        dayNamesMin: ["Pa", "Pt", "Sl", "Ça", "Pe", "Cu", "Ct"],
        firstDay: 1,
    }).datepicker('setDate', oneMonthAgo); 

    $(".datepicker-end").datepicker({
        dateFormat: "dd.mm.yy",
        altFormat: "dd.mm.yy",
        monthNames: ["Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"],
        dayNamesMin: ["Pa", "Pt", "Sl", "Ça", "Pe", "Cu", "Ct"],
        firstDay: 1,
    }).datepicker('setDate', today); 
}

function downloadExcel(excelName, excelData) {
    var createXLSLFormatObj1 = [];
    if (excelData && excelData.length > 0) {
        var xlsHeader1 = Object.keys(excelData[0]);
        createXLSLFormatObj1.push(xlsHeader1);
        $.each(excelData, function (index, value) {
            var innerRowData = [];
            $.each(value, function (i, item) {
                innerRowData.push(item);
            });
            createXLSLFormatObj1.push(innerRowData);
        })
    }

    var wb = XLSX.utils.book_new();
    var ws1 = XLSX.utils.aoa_to_sheet(createXLSLFormatObj1);
    var d = new Date();
    XLSX.utils.book_append_sheet(wb, ws1, d.getDate().toString().padStart(2, '0') + '.' + (d.getMonth() + 1).toString().padStart(2, '0') + '.' + d.getFullYear());
    XLSX.writeFile(wb, excelName + '.xlsx');
}


function formatCurrency(input, blur) {
    // appends $ to value, validates decimal side
    // and puts cursor back in right position.

    // get input value
    var input_val = input.val();

    // don't validate empty input
    if (input_val === "") { return; }

    // original length
    var original_len = input_val.length;

    // initial caret position 
    var caret_pos = input.prop("selectionStart");

    // check for decimal
    if (input_val.indexOf(",") >= 0) {

        // get position of first decimal
        // this prevents multiple decimals from
        // being entered
        var decimal_pos = input_val.indexOf(",");

        // split number by decimal point
        var left_side = input_val.substring(0, decimal_pos);
        var right_side = input_val.substring(decimal_pos);

        // add commas to left side of number
        left_side = formatNumber(left_side);

        // validate right side
        right_side = formatNumber(right_side);

        // On blur make sure 2 numbers after decimal
        if (blur === "blur") {
            right_side += "00";
        }

        // Limit decimal to only 2 digits
        right_side = right_side.substring(0, 2);

        // join number by .
        input_val = left_side + "," + right_side;

    } else {
        // no decimal entered
        // add commas to number
        // remove all non-digits
        input_val = formatNumber(input_val);

        // final formatting
        if (blur === "blur") {
            input_val += ",00";
        }
    }

    // send updated string to input
    input.val(input_val);

    // put caret back in the right position
    var updated_len = input_val.length;
    caret_pos = updated_len - original_len + caret_pos;
    input[0].setSelectionRange(caret_pos, caret_pos);
};

function formatNumber(n) {
    return n.replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ".")
};