function ValidationBroker() {
    $('.filled').hide();
    $('.form-control').removeClass('parsley-error');
    var valid = true;
    if ($('#txtFirstName').val() == "") {
        valid = FocusError('txtFirstName');
    }
    if ($('#txtLastName').val() == "") {
        valid = FocusError('txtLastName');
    }
    if ($('#AdministratorType').val() == "") {
        valid = FocusError('AdministratorType');
    }
    if ($('#txtEmail').val() == "") {
        valid = FocusError('txtEmail');
    }
    if ($('#txtWorkPhone').val() == "") {
        valid = FocusError('txtWorkPhone');
    }
    if ($('#txtMobile').val() == "") {
        valid = FocusError('txtMobile');
    }
    return valid;
}
function ValidationManageBroker() {
    $('.filled').hide();
    $('.form-control').removeClass('parsley-error');
    var valid = true;
    if ($('#txtBroker').val() == "") {
        valid = FocusError('txtBroker');
    }
    if ($('#txtStreet').val() == "") {
        valid = FocusError('txtStreet');
    }
    if ($('#txtCity').val() == "") {
        valid = FocusError('txtCity');
    }
    if ($('#txtState').val() == "") {
        valid = FocusError('txtState');
    }
    if ($('#txtZip').val() == "") {
        valid = FocusError('txtZip');
    }
    if ($('#txtBrokerPhone').val() == "") {
        valid = FocusError('txtBrokerPhone');
    }
    return valid;
}
function FocusError(Id) {
    $('#' + Id).addClass('parsley-error');
    $('#parsley-' + Id).show();
    $('#' + Id).val('');
    return false;
}
$(document).on('click', '#btnsubmit', function () {
    if (ValidationBroker()) {
        var temptext = $('#btnsubmit').text();
        var controllername = $('#btnsubmit').attr('controllername');
        $('#btnsubmit').text('Processing...');
        $('#btnsubmit').prop('disabled', true);
        var JsonData = {
            BrokerId: parseInt($('#btnsubmit').attr('brokerid')),
            BrokerName: $('#txtBroker').val(),
            SubBrokerId: parseInt($('#btnsubmit').attr('subbrokerid')),
            FirstName: $('#txtFirstName').val(),
            LastName: $('#txtLastName').val(),
            Email: $('#txtEmail').val(),
            Phone: RemoveMask($('#txtMobile').val()),
            WorkPhone: RemoveMask($('#txtWorkPhone').val()),
            AdministratorType: parseInt($('#AdministratorType').val())
        }
        $.post('/' + controllername+'/SaveBroker', JsonData, function (Response) {
            $('#btnsubmit').text(temptext);
            $('#btnsubmit').prop('disabled', false);
            if (Response > 0) {
                ShowAlertLoad('Updated', 'Details Updated Successfully', 'success', (controllername.toLowerCase() == "broker-admin" ? location.href:"/GeneralAgent/EditBroker"));
            }
            else {
                ShowAlert('Updation failed', 'Details updation failed', 'error');
            }

        })

    }
})
$(document).on('click', '#btnmanagebroker', function () {
    if (ValidationManageBroker()) {
        var temptext = $('#btnmanagebroker').text();
        var controllername = $('#btnmanagebroker').attr('controllername');
        $('#btnmanagebroker').text('Processing...');
        $('#btnmanagebroker').prop('disabled', true);
        var JsonData = {
            BrokerId: parseInt($('#btnsubmit').attr('brokerid')),
            BrokerName: $('#txtBroker').val(),
            Street: $('#txtStreet').val(),
            ZipCode: $('#txtZip').val(),
            BrokerPhone: RemoveMask($('#txtBrokerPhone').val()),
            Country: 'United States',
            State: $('#txtState').val(),
            City: $('#txtCity').val()
        }
        $.post('/' + controllername+'/ManageBroker', JsonData, function (Response) {
            $('#btnmanagebroker').text(temptext);
            $('#btnmanagebroker').prop('disabled', false);
            if (Response > 0) {
                ShowAlert('Updated', 'Details Updated Successfully', 'success');
            }
            else {
                ShowAlert('Updation failed', 'Details updation failed', 'error');
            }

        })

    }
})
function RemoveMask(number) {
    return number.replace(/[^A-Z0-9]/ig, "");
}
function EditBroker(SubBrokerId, FirstName, LastName, Email, WorkPhone, Phone, AdministratorType) {
    $('#txtFirstName').val(FirstName);
    $('#txtLastName').val(LastName);
    $('#txtEmail').val(Email);
    $('#txtWorkPhone').val(WorkPhone);
    $('#txtMobile').val(Phone);
    $('#AdministratorType').val(AdministratorType);
    $('#btnsubmit').attr('subbrokerid', SubBrokerId);
    $('#btnsubmit').text('Update');
    $("html, body").animate({ scrollTop: 0 }, "slow");

}
function BrokerActivateSuspend(sender, id, controller) {
    var status = $(sender).attr('status').toLowerCase();
    newmsg = (status == "true" ? "Activate" : "Suspend");
    swal({
        title: "",
        text: "Do you want to " + $(sender).text() + " the broker user?",
        //type: "warning",
        showCancelButton: true,
        confirmButtonClass: "#f27474",
        confirmButtonText: $(sender).text(),
        cancelButtonText: "Cancel",
        closeOnConfirm: true,
        closeOnCancel: true
    },
        function (isConfirm) {

            if (isConfirm) {
                $.post('/' + controller+'/UpdateStatusSubBroker', { SubBrokerId: id, Status: status == "true" ? false : true, Type: 'Status' }, function (Response) {
                    if (Response) {
                        ShowAlert('Success', 'Status updated successfully', 'success');
                        $(sender).text(newmsg);
                        $(sender).attr('status', status == "true" ? false : true);
                        if (newmsg == "Activate") {
                            $(sender).removeClass("btn-outline-danger");
                            $(sender).addClass("btn-outline-primary");
                        }
                        else {
                            $(sender).addClass("btn-outline-danger");
                            $(sender).removeClass("btn-outline-primary");
                        }
                    }
                    else {
                        ShowAlert('Error', 'Status updation failed', 'error');
                    }
                })

            }

        });
}
function BrokerDelete(sender, id,controller) {
    swal({
        title: "",
        text: "Do you want to remove the broker user?",
        //type: "warning",
        showCancelButton: true,
        confirmButtonClass: "#f27474",
        confirmButtonText: "Remove",
        cancelButtonText: "Cancel",
        closeOnConfirm: true,
        closeOnCancel: true
    },
        function (isConfirm) {

            if (isConfirm) {
                $.post('/' + controller+'/UpdateStatusSubBroker', { SubBrokerId: id, Status: true, Type: 'Delete' }, function (Response) {
                    if (Response) {
                        ShowAlert('Success', 'Broker user deleted successfully', 'success');
                        $(sender).parent().parent().remove();
                    }
                    else {
                        ShowAlert('Error', 'Broker user deletion failed', 'error');
                    }
                })

            }

        });
}
function validateZipCode(elementValue) {
    var zipCodePattern = /^\d{5}$|^\d{5}-\d{4}$/;
    if (!zipCodePattern.test($('#' + elementValue).val())) {
        $('#' + elementValue).addClass('parsley-error');
        $('#parsley-Zip-' + elementValue).show();
    }
    else {
        $('#' + elementValue).removeClass('parsley-error');
        $('#parsley-Zip-' + elementValue).hide();
    }
}
//$(document).on('keyup', '#txtSearch', function () {
//    var SearchValue = $('#txtSearch').val().toLowerCase();
//    $('#SubBrokerData tr').hide();
//    $('#SubBrokerData tr').each(function () {
//        $row = $(this);
//        if (SearchValue != "") {
//            var FirstName = $row.find("td:eq(0)").text().toLowerCase();
//            var LastName = $row.find("td:eq(1)").text().toLowerCase();
//            var Email = $row.find("td:eq(2)").text().toLowerCase();
//            var Phone = $row.find("td:eq(3)").text().toLowerCase();
//            var Mobile = $row.find("td:eq(4)").text().toLowerCase();
//            if (FirstName.lastIndexOf(SearchValue, 0) == 0 || LastName.lastIndexOf(SearchValue, 0) == 0 || Email.lastIndexOf(SearchValue, 0) == 0 || Phone.lastIndexOf(SearchValue, 0) == 0 || Mobile.lastIndexOf(SearchValue, 0) == 0) {
//                $(this).show();
//            }
//        }
//        else {
//            $(this).show();
//        }
//    });
//})