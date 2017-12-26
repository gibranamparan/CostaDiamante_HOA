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

boolParse = function (myStr) {
    if(myStr)
        return myStr.toLowerCase() == 'true';
    else
        return false
}

sortBy = function (arr, field, direction) {
    var dir = direction == 'asc' ? 1 : direction == 'desc' ? -1 : 0;

    arr.sort(function (a, b) {
        return compareItems(dir, a[field], b[field])
    })
}

compareItems = function (dir, a, b) {
    if (typeof a === 'string')
        return dir * a.toLocaleLowerCase().localeCompare(b.toLocaleLowerCase())
    else
        return dir * (a - b)
}


function notifyError(message) {
    swal("Error!", message, "error")
}