$(document).ready(function () {
    $('table.datatable').DataTable();
})

$(document).tooltip();

function changeIcon(bar) {
    $(bar).find("i").toggleClass("fa-window-minimize fa-window-maximize")
}

//Agrega funcion a JQuery para permitir solicitudes asincronas identificandose como usuario logeado
jQuery.postJSON = function (url, data, dataType, success, fail, always, antiForgeryToken) {
    if (dataType === void 0) { dataType = "json"; }
    if (typeof (data) === "object") { data = JSON.stringify(data); }
    var ajax = {
        url: url,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: dataType,
        data: data,
        success: success,
        fail: fail,
        complete: always
    };
    if (antiForgeryToken) {
        ajax.headers = {
            "__RequestVerificationToken": antiForgeryToken
        };
    };

    return jQuery.ajax(ajax);
};

$.fn.slideDownOrUp = function (show) {
    return show ? this.slideDown() : this.slideUp();
}

$.fn.fadeInOrOut = function (status) {
    return status ? this.fadeIn() : this.fadeOut();
}

Date.prototype.addDays = function (days) {
    var dat = new Date(this.valueOf());
    dat.setDate(dat.getDate() + days);
    return dat;
}

Date.prototype.dateISOFormat = function (days) {
    var dat = new Date(this.valueOf());
    return dat.toISOString().slice(0, 10);
}

boolParse = function (myStr){
    return myStr.toLowerCase()=='true';
}

sortBy = function (arr, field, direction) {
    if (direction=='asc')
        arr.sort(function (a, b) { return a[field] - b[field] })
    else if(direction == "desc")
        arr.sort(function (a, b) { return b[field] - a[field] })
}
