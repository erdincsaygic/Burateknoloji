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