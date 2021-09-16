function showvalidatealert(msg) {
    swal(msg);
}
function ShowAlert(ttl, txt, status) {
    swal({
        title: ttl,
        text: txt,
        type: status
    });
    if (status == 'success') {
        $(".sweet-alert div.sa-confirm-button-container>button").addClass("btn-success");
    }
    else if (status == 'warning') {
        $(".sweet-alert div.sa-confirm-button-container>button").addClass("btn-warning");
    }
    else if (status == 'error') {
        $(".sweet-alert div.sa-confirm-button-container>button").addClass("btn-danger");
    }
}
function ShowAlertLoad(ttl, txt, status, location) {
    swal({
        title: ttl,
        text: txt,
        type: status

    }, function () { window.location.href = location });
    if (status == 'success') {
        $(".sweet-alert div.sa-confirm-button-container>button").addClass("btn-success");
    }
    else if (status == 'warning') {
        $(".sweet-alert div.sa-confirm-button-container>button").addClass("btn-warning");
    }
    else if (status == 'error') {
        $(".sweet-alert div.sa-confirm-button-container>button").addClass("btn-danger");
    }
    else if (status == 'info') {
        $(".sweet-alert div.sa-confirm-button-container>button").addClass("btn-info");
    }
}
function isAlphabateKey(evt) {
    debugger;
    var charCode = (evt.which) ? evt.which : event.keyCode
    if ((charCode > 33 && charCode < 65) || (charCode > 90 && charCode < 97) || (charCode > 122 && charCode < 127) || charCode == 31) {
        return false;
    }
    else {
        return true;
    }
        
}
function isNumberKey(evt) {
    debugger;
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 47 && charCode < 58) {
        return true;
    }
    else {
        return false;
    }
}