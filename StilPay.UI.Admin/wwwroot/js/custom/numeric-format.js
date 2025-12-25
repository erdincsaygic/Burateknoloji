(function ($) {
    if (screen.width >= 992) {
        //eklentimizi tanıtan fonksiyon options adında değişken yolluyoruz.
        $.fn.numeric_format = function (options) {
            var defaults = {
                defaultvalue: "",
                centsLimit: 0,
                thousandslimit: 7,
                centsSeparator: ',',
                thousandsSeparator: '.',
                clearOnEmpty: true
            };

            options = $.extend(defaults, options);

            var elementValue = $(this).val();
            if (elementValue == "0" && elementValue != options.defaultvalue && !options.clearOnEmpty) {
                $(this).val(options.defaultvalue);
            }

            $(this).bind("focusin", function () {
                if ($(this).prop("readonly") == false) {
                    if ($(this).val() == options.defaultvalue)
                        $(this).val("");
                }
            });

            $(this).bind("focusout", function () {
                if ($(this).prop("readonly") == false) {
                    if ($(this).val() == null || $(this).val().trim() == "") {
                        if (!options.clearOnEmpty)
                            $(this).val(options.defaultvalue);
                    }
                }
            });

            $(this).bind("keypress", function (e) {
                if ($(this).prop("readonly") == false) {
                    if (window.e) // IE
                        e.returnValue = false;
                    else
                        e.preventDefault();

                    if (String.fromCharCode(e.keyCode) != "," && String.fromCharCode(e.keyCode) != "." && (e.keyCode < 48 || e.keyCode > 57))//virgül ve nokta ve Sayı değilse dönüş yapıyorum...
                        return;
                    else {
                        if (getSelectionStart(this) == 0 && getSelectionEnd(this) == $(this).val().length) {
                            var newvalue = String.fromCharCode(e.keyCode);
                            if (newvalue == "," || newvalue == ".") // Eğer girilen değer virgül veya noktaysa 0 değer ataması yapılıyor..
                                newvalue = "0";

                            if (options.centsLimit > 0) { //tam sayı değerine varsa küsürat sayısı kadar sıfır atanıyor..
                                newvalue += options.centsSeparator;
                                for (var i = 0; i < options.centsLimit; i++) {
                                    newvalue += "0";
                                }
                            }

                            $(this).val(newvalue)
                            if (newvalue.length > 0) { //Eğer yeni değer uzunluğu 0 dan büyükse
                                if ((String.fromCharCode(e.keyCode) == "," || String.fromCharCode(e.keyCode) == ".") && options.centsLimit > 0) { //Küsürat var ise imlec virgül veya nokta sonrasına öteleniyor..
                                    var pos = newvalue.indexOf(options.centsSeparator) + 1;
                                    setSelection(this, pos, pos);
                                }
                                else //İmlec 1 sağa öteleniyor..
                                    setSelection(this, 1, 1);
                            }
                        }
                        else {
                            if ($(this).val().length == 0) { //text uzunluğu 0 ise ve virgül yada noktaya basarsa..    
                                var newvalue = String.fromCharCode(e.keyCode);
                                if (newvalue == "." || newvalue == ",") // Eğer girilen değer virgül veya noktaysa değer temizleniyor..
                                    newvalue = "0";

                                if (options.centsLimit > 0) { //tam sayı değerine varsa küsürat sayısı kadar sıfır atanıyor..
                                    newvalue += options.centsSeparator;
                                    for (var i = 0; i < options.centsLimit; i++) {
                                        newvalue += "0";
                                    }
                                }

                                $(this).val(newvalue)
                                if (newvalue.length > 0) { //Eğer yeni değer uzunluğu 0 dan büyükse
                                    if ((String.fromCharCode(e.keyCode) == "," || String.fromCharCode(e.keyCode) == ".") && options.centsLimit > 0) { //Küsürat var ise imlec virgül veya nokta sonrasına öteleniyor..
                                        var pos = newvalue.indexOf(options.centsSeparator) + 1;
                                        setSelection(this, pos, pos);
                                    }
                                    else //İmlec 1 sağa öteleniyor..
                                        setSelection(this, 1, 1);
                                }
                            }
                            else if ($(this).val().length > 0 && (String.fromCharCode(e.keyCode) == "," || String.fromCharCode(e.keyCode) == ".")) {  //text uzunluğu 0 da büyük ise ve virgül yada noktaya basarsa..
                                if (options.centsLimit > 0) { //küsüratlı ise if e giriyor değilse bişey yapmıyor..
                                    var pos = $(this).val().indexOf(options.centsSeparator) + 1;
                                    setSelection(this, pos, pos); //imlec 1 sağa öteleniyor..
                                }
                            }
                            else if (options.centsLimit == 0) {  //küsürat uzunluğu 0 ise ve rakama basılmışsa..
                                var oldvalue = $(this).val();
                                if (oldvalue.length < options.thousandslimit) { //Eğer tam kısmı istenen uzunluğa ulaşmamışsa..
                                    var indexNewValue = getSelectionStart(this);
                                    var newvalue = oldvalue.substring(0, indexNewValue) + String.fromCharCode(e.keyCode) + oldvalue.substring(indexNewValue);
                                    $(this).val(newvalue);
                                    setSelection(this, indexNewValue + 1, indexNewValue + 1);
                                }
                            }
                            else if ($(this).val().indexOf(options.centsSeparator) >= getSelectionStart(this)) { //küsürat uzunluğu 0 dan büyük ise ve virgül vaya noktadan önce rakama basılmışsa..
                                var oldvalue = $(this).val();
                                var indexNewValue = getSelectionStart(this);
                                var indexcentsSeparator = oldvalue.indexOf(options.centsSeparator);
                                var lengthcentsSeparator = oldvalue.substring(0, indexcentsSeparator).length;

                                var newvalue = "";
                                if (oldvalue.substring(0, indexNewValue) == "0") { //tam kısımdan sadece sıfır varsa rakam tam sayı kısmına atılıyor ve sonrasına virgül veya noktayla küsürat getiriliyor..
                                    var newvalue = String.fromCharCode(e.keyCode) + oldvalue.substring((indexNewValue));
                                    $(this).val(newvalue);
                                    setSelection(this, 1, 1);
                                }
                                else if (lengthcentsSeparator < options.thousandslimit) {
                                    var newvalue = oldvalue.substring(0, indexNewValue) + String.fromCharCode(e.keyCode) + oldvalue.substring((indexNewValue));
                                    $(this).val(newvalue);
                                    setSelection(this, indexNewValue + 1, indexNewValue + 1);
                                }
                            }
                            else if ($(this).val().indexOf(options.centsSeparator) < getSelectionStart(this)) { //küsürat uzunluğu 0 dan büyük ise ve virgül vaya noktadan sonra rakama basılmışsa..
                                var oldvalue = $(this).val();
                                var indexNewValue = getSelectionStart(this);
                                var newvalue = oldvalue.substring(0, indexNewValue) + String.fromCharCode(e.keyCode) + oldvalue.substring((indexNewValue + 1));

                                var indexcentsSeparator = oldvalue.indexOf(options.centsSeparator);
                                if ((indexNewValue - indexcentsSeparator) <= options.centsLimit) {
                                    $(this).val(newvalue);
                                    setSelection(this, indexNewValue + 1, indexNewValue + 1);
                                }
                            }

                        }
                    }
                }
            });

            $(this).bind("keydown", function (e) {
                if ($(this).prop("readonly") == false) {
                    if (e.keyCode == 8) { //backspace ile silme işlemi
                        if (window.e) // IE
                            e.returnValue = false;
                        else
                            e.preventDefault();
                        if ($(this).val() == null || $(this).val().trim() == "")
                            return;
                        else {
                            if (getSelectionStart(this) == 0 && getSelectionEnd(this) == $(this).val().length) {
                                $(this).val(options.defaultvalue);
                                setSelection(this, 1, 1);
                            }
                            else {
                                var oldvalue = $(this).val();
                                var indexNewValue = getSelectionStart(this);
                                var indexcentsSeparator = oldvalue.indexOf(options.centsSeparator);
                                if (options.centsLimit == 0) { //eğere küsürat 0 ise ve silme işlemi yapılmışsa..
                                    var newvalue = oldvalue.substring(0, indexNewValue - 1) + oldvalue.substring(indexNewValue);
                                    $(this).val(newvalue);
                                    setSelection(this, indexNewValue - 1, indexNewValue - 1);
                                }
                                else if (indexNewValue == (indexcentsSeparator + 1)) { //eğere küsürat 0 ise ve virgül yada noktadan hemen sonra silme işlemi yapılmışsa..
                                    setSelection(this, indexNewValue - 1, indexNewValue - 1);
                                }
                                else if (indexNewValue > (indexcentsSeparator + 1)) {  //eğere küsürat 0 ise ve virgül yada noktadan en az bir rakam sonra silme işlemi yapılmışsa..
                                    var newvalue = oldvalue.substring(0, indexNewValue - 1) + "0" + oldvalue.substring(indexNewValue);
                                    $(this).val(newvalue);
                                    setSelection(this, indexNewValue - 1, indexNewValue - 1);
                                }
                                else if (indexNewValue < (indexcentsSeparator + 1)) { //eğere küsürat 0 ise ve virgül yada noktadan önce silme işlemi yapılmışsa..
                                    var strbeforecentsSeparator = oldvalue.substring(0, indexcentsSeparator);

                                    if (strbeforecentsSeparator.length == 1) {
                                        var newvalue = "0" + oldvalue.substring(indexcentsSeparator);
                                        $(this).val(newvalue);
                                        setSelection(this, 1, 1);
                                    }
                                    else {
                                        var newvalue = oldvalue.substring(0, indexNewValue - 1) + oldvalue.substring(indexNewValue);
                                        strbeforecentsSeparator = newvalue.substring(0, indexcentsSeparator);
                                        if (parseInt(strbeforecentsSeparator) == 0) {
                                            newvalue = "0" + oldvalue.substring(indexcentsSeparator);
                                        }
                                        $(this).val(newvalue);
                                        setSelection(this, indexNewValue - 1, indexNewValue - 1);
                                    }

                                }
                            }
                        }
                    }
                    else if (e.keyCode == 46) {//delete ile silme
                        if (window.e) // IE
                            e.returnValue = false;
                        else
                            e.preventDefault();
                        if ($(this).val() == null || $(this).val().trim() == "")
                            return;
                        else {
                            if (getSelectionStart(this) == 0 && getSelectionEnd(this) == $(this).val().length) {
                                $(this).val(options.defaultvalue);
                                setSelection(this, 1, 1);
                            }
                            else {
                                var oldvalue = $(this).val();
                                var indexNewValue = getSelectionStart(this);
                                var indexcentsSeparator = oldvalue.indexOf(options.centsSeparator);
                                if (options.centsLimit == 0) { //eğere küsürat 0 ise ve silme işlemi yapılmışsa..
                                    if (indexNewValue < oldvalue.length) {
                                        var newvalue = oldvalue.substring(0, indexNewValue) + oldvalue.substring(indexNewValue + 1);
                                        $(this).val(newvalue);
                                        setSelection(this, indexNewValue, indexNewValue);
                                    }
                                }
                                else if (indexNewValue == indexcentsSeparator) {
                                    setSelection(this, indexNewValue + 1, indexNewValue + 1);
                                }
                                else if (indexNewValue < indexcentsSeparator) {
                                    var strbefore = oldvalue.substring(0, indexcentsSeparator);
                                    if (strbefore.length == 1) {
                                        var newvalue = "0" + oldvalue.substring(indexcentsSeparator);
                                        $(this).val(newvalue);
                                        setSelection(this, 1, 1);
                                    }
                                    else {
                                        var newvalue = oldvalue.substring(0, indexNewValue) + oldvalue.substring(indexNewValue + 1);
                                        strbeforecentsSeparator = newvalue.substring(0, indexcentsSeparator);
                                        if (parseInt(strbeforecentsSeparator) == 0) {
                                            newvalue = "0" + oldvalue.substring(indexcentsSeparator);
                                        }
                                        $(this).val(newvalue);
                                        setSelection(this, indexNewValue, indexNewValue);
                                    }
                                }
                                else if (indexNewValue > indexcentsSeparator) {
                                    if (oldvalue.length != indexNewValue) {
                                        var newvalue = oldvalue.substring(0, indexNewValue) + oldvalue.substring(indexNewValue + 1) + "0";
                                        $(this).val(newvalue);
                                        setSelection(this, indexNewValue, indexNewValue);
                                    }
                                }
                            }
                        }
                    }
                }
            });

            function setSelection(element, selectionStart, selectionEnd) {

                input = element;

                if (input.createTextRange) {
                    var range = input.createTextRange();
                    range.collapse(true);
                    range.moveEnd('character', selectionEnd);
                    range.moveStart('character', selectionStart);
                    range.select();
                } else if (input.setSelectionRange) {
                    input.focus();
                    input.setSelectionRange(selectionStart, selectionEnd);
                }
                return this;
            }

            function getSelectionStart(element) {
                input = element;

                var pos = input.value.length;

                if (input.createTextRange) {
                    var r = document.selection.createRange().duplicate();
                    r.moveEnd('character', input.value.length);
                    if (r.text == '')
                        pos = input.value.length;
                    pos = input.value.lastIndexOf(r.text);
                } else if (typeof (input.selectionStart) != "undefined")
                    pos = input.selectionStart;

                return pos;
            }

            function getSelectionEnd(element) {
                input = element;

                var pos = input.value.length;

                if (input.createTextRange) {
                    var r = document.selection.createRange().duplicate();
                    r.moveStart('character', -input.value.length);
                    if (r.text == '')
                        pos = input.value.length;
                    pos = input.value.lastIndexOf(r.text);
                } else if (typeof (input.selectionEnd) != "undefined")
                    pos = input.selectionEnd;

                return pos;
            }

        };
    }
    else {
        //eklentimizi tanıtan fonksiyon options adında değişken yolluyoruz.
        $.fn.numeric_format = function (options) {
            var defaults = {
                defaultvalue: "",
                centsLimit: 0,
                thousandslimit: 7,
                centsSeparator: ',',
                thousandsSeparator: '.',
                clearOnEmpty: true
            };

            options = $.extend(defaults, options);

            var elementValue = $(this).val();
            if (elementValue == "0" && elementValue != options.defaultvalue && !options.clearOnEmpty) {
                $(this).val(options.defaultvalue);
            }

            $(this).bind("focusin", function () {
                if ($(this).prop("readonly") == false) {
                    if ($(this).val() == options.defaultvalue)
                        $(this).val("");
                }
            });

            $(this).bind("focusout", function () {
                if ($(this).prop("readonly") == false) {
                    if ($(this).val() == null || $(this).val().trim() == "") {
                        if (!options.clearOnEmpty)
                            $(this).val(options.defaultvalue);
                    }
                }
            });

            var previousValue, lastValue;
            var previousIndex, lastIndex;
            var kc, ck;

            $(this).bind("keydown", function (e) {
                if ($(this).prop("readonly") == false) {

                    previousValue = e.target.value;

                    ck = e.key;
                }
            });

            $(this).bind("input", function (e) {

                if ($(this).prop("readonly") == false) {
                    lastValue = e.target.value;
                    previousIndex = (getSelectionStart(this) - 1) == -1 ? 0 : (getSelectionStart(this) - 1);
                    lastIndex = getSelectionEnd(this);

                    if (ck == "Backspace" || ck == "Delete") {
                        ck = "";
                    }
                    else {
                        ck = lastValue.substring(previousIndex, lastIndex);
                    }

                    if (lastValue.length < previousValue.length && lastValue.length != 0 && lastValue.length != 1) {

                        var indexSeparator = lastValue.indexOf(options.centsSeparator);
                        var thousandsValue = lastValue.substring(0, indexSeparator);
                        var centsValue = lastValue.substring((indexSeparator + 1));

                        if (indexSeparator < 0) {
                            $(this).val(previousValue);
                            setSelection(this, lastIndex, lastIndex);
                        }
                        else {
                            if (thousandsValue.length == 0 || parseInt(thousandsValue) == 0) {
                                thousandsValue = "0";
                            }
                            if (centsValue.length < options.centsLimit) {
                                centsValue = centsValue.substring(0, (previousIndex - indexSeparator)) + "0" + centsValue.substring((previousIndex - indexSeparator));
                            }

                            var newValue = thousandsValue + options.centsSeparator + centsValue;
                            $(this).val(newValue);
                            if (thousandsValue == "0")
                                setSelection(this, 1, 1);
                            else
                                setSelection(this, lastIndex, lastIndex);
                        }
                    }
                    else {
                        if (isNaN(ck) && ck != "," && ck != ".") { //Sayı ve virgül ve nokta değil ise önceki değeri set ediyorum..
                            $(this).val(previousValue);
                            setSelection(this, previousIndex, previousIndex);
                        }
                        else {
                            if (previousIndex == 0 && lastIndex == lastValue.length) { //Mouse ile tümü seçilip yeni değer ataması yapılıyor
                                var newvalue = ck == "" ? "0" : ck;
                                if (newvalue == "," || newvalue == ".") //Eğer girilen değer virgül veya noktaysa 0 değer ataması yapılıyor..
                                    newvalue = "0";

                                if (options.centsLimit > 0) { //Tam sayı değerine varsa küsürat sayısı kadar sıfır atanıyor..
                                    newvalue += options.centsSeparator;
                                    for (var i = 0; i < options.centsLimit; i++) {
                                        newvalue += "0";
                                    }
                                }

                                $(this).val(newvalue)
                                if (newvalue.length > 0) { //Eğer yeni değer uzunluğu 0 dan büyükse
                                    if ((ck == "," || ck == ".") && options.centsLimit > 0) { //Küsürat var ise imlec virgül veya nokta sonrasına öteleniyor..
                                        var pos = newvalue.indexOf(options.centsSeparator) + 1;
                                        setSelection(this, pos, pos);
                                    }
                                    else //İmlec 1 sağa öteleniyor..
                                        setSelection(this, 1, 1);
                                }
                            }
                            else {
                                if (lastValue.length == 0) { //Text uzunluğu 0 ise ve virgül yada noktaya basarsa..    
                                    var newvalue = ck;
                                    if (newvalue == "," || newvalue == ".") // Eğer girilen değer virgül veya noktaysa 0 değer ataması yapılıyor..
                                        newvalue = "0";

                                    if (options.centsLimit > 0) { //Tam sayı değerine varsa küsürat sayısı kadar sıfır atanıyor..
                                        newvalue += options.centsSeparator;
                                        for (var i = 0; i < options.centsLimit; i++) {
                                            newvalue += "0";
                                        }
                                    }

                                    $(this).val(newvalue)
                                    if (newvalue.length > 0) { //Eğer yeni değer uzunluğu 0 dan büyükse
                                        if ((ck == "," || ck == ".") && options.centsLimit > 0) { //Küsürat var ise imlec virgül veya nokta sonrasına öteleniyor..
                                            var pos = newvalue.indexOf(options.centsSeparator) + 1;
                                            setSelection(this, pos, pos);
                                        }
                                        else //İmlec 1 sağa öteleniyor..
                                            setSelection(this, 1, 1);
                                    }
                                }
                                else if (lastValue.length > 0 && (ck == "," || ck == ".")) { //Text uzunluğu 0 dan büyük ve virgül veya noktaya basarsa..
                                    $(this).val(previousValue);
                                    if (options.centsLimit > 0) { //Küsüratlı ise if e giriyor değilse bişey yapmıyor..
                                        var pos = previousValue.indexOf(options.centsSeparator) + 1;
                                        setSelection(this, pos, pos); //İmlec virgül veya nokta sonrasına öteleniyor..
                                    }
                                }
                                else if (options.centsLimit == 0) {  //Küsürat uzunluğu 0 ise ve rakama basılmışsa..
                                    if (previousValue.length == 1 && previousValue.substring(0, 1) == "0") {
                                        $(this).val(lastValue.substring(1));
                                        setSelection(this, 1, 1);
                                    }
                                    else if (previousValue.length >= options.thousandslimit) { //Eğer tam kısmı istenen uzunluğa ulaşmamışsa..                               
                                        $(this).val(previousValue);
                                        setSelection(this, lastIndex, lastIndex);
                                    }
                                }
                                else if (lastValue.indexOf(options.centsSeparator) >= lastIndex) { //Küsürat uzunluğu 0 dan büyük ise ve virgül veya noktadan önce rakama basılmışsa..
                                    var indexcentsSeparator = previousValue.indexOf(options.centsSeparator);
                                    var lengthcentsSeparator = previousValue.substring(0, indexcentsSeparator).length;

                                    if (previousValue.substring(0, 1) == "0") { //Tam kısımdan sadece sıfır varsa rakam tam sayı kısmına atılıyor ve sonrasına virgül veya noktayla küsürat getiriliyor..
                                        var indexcentsSeparator = previousValue.indexOf(options.centsSeparator);
                                        var newvalue = ck + previousValue.substring(indexcentsSeparator);
                                        $(this).val(newvalue);
                                        setSelection(this, 1, 1);
                                    }
                                    else if (lengthcentsSeparator <= options.thousandslimit) { //Eğer tam kısmı istenen uzunluğa ulaşmamışsa..
                                        $(this).val(lastValue);
                                        setSelection(this, lastIndex, lastIndex);
                                    }
                                    else if (lengthcentsSeparator > options.thousandslimit) { //Eğer tam kısmı istenen uzunluğa ulaşmışsa..
                                        $(this).val(previousValue);
                                        setSelection(this, previousIndex, previousIndex);
                                    }
                                }
                                else if (lastValue.indexOf(options.centsSeparator) < previousIndex) { //Küsürat uzunluğu 0 dan büyük ise ve virgül veya noktadan sonra rakama basılmışsa..
                                    var newvalue = previousValue.substring(0, previousIndex) + ck + previousValue.substring(lastIndex);

                                    var indexcentsSeparator = previousValue.indexOf(options.centsSeparator);
                                    if ((previousIndex - indexcentsSeparator) <= options.centsLimit) {
                                        $(this).val(newvalue);
                                        setSelection(this, lastIndex, lastIndex);
                                    }
                                    else {
                                        $(this).val(previousValue);
                                        setSelection(this, previousIndex, previousIndex);
                                    }
                                }
                            }
                        }
                    }
                }
            });

            $(this).bind("keyup", function (e) {
                e.preventDefault();
            });

            function setSelection(element, selectionStart, selectionEnd) {

                input = element;

                if (input.createTextRange) {
                    var range = input.createTextRange();
                    range.collapse(true);
                    range.moveEnd('character', selectionEnd);
                    range.moveStart('character', selectionStart);
                    range.select();
                } else if (input.setSelectionRange) {
                    input.focus();
                    input.setSelectionRange(selectionStart, selectionEnd);
                }
                return this;
            }

            function getSelectionStart(element) {
                input = element;

                var pos = input.value.length;

                if (input.createTextRange) {
                    var r = document.selection.createRange().duplicate();
                    r.moveEnd('character', input.value.length);
                    if (r.text == '')
                        pos = input.value.length;
                    pos = input.value.lastIndexOf(r.text);
                } else if (typeof (input.selectionStart) != "undefined")
                    pos = input.selectionStart;

                return pos;
            }

            function getSelectionEnd(element) {
                input = element;

                var pos = input.value.length;

                if (input.createTextRange) {
                    var r = document.selection.createRange().duplicate();
                    r.moveStart('character', -input.value.length);
                    if (r.text == '')
                        pos = input.value.length;
                    pos = input.value.lastIndexOf(r.text);
                } else if (typeof (input.selectionEnd) != "undefined")
                    pos = input.selectionEnd;

                return pos;
            }

        };
    }
}(jQuery));