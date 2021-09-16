
function AddCompany(sender) {
    var html = '';
    var count = parseInt($(sender).attr('count')) + 1;
    $(sender).attr('count', count);
    html += '<div id="ClientAdmin' + count + '">';
    html += '<div class="row">';
    html += '<div class="col-lg-6">';
    html += '<label class="section-title">Client Administrators</label>';
    html += '</div>';
    html += '<div class="col-lg-6" style="text-align:right;">';
    html += '<div class="demo-btn-list">';
    html += '<a style="cursor:pointer;" class="btn btn-primary btn-icon rounded-circle" title="Remove Administrator"  onclick="RemoveCompany(' + count + ');" ><div><i class="fa fa-minus"></i></div></a>';
html += '</div>';
html += '</div>';
html += '</div>';
    html += '<div class="form-layout">';
    html += '<div class="row mg-b-25">';
    html += '<div class="col-lg-4">';
    html += '<div class="form-group">';
    html += '<label class="form-control-label">Firstname: <span class="tx-danger">*</span></label>';
    html += '<input class="form-control" type="text" name="firstname" id="txtAdFirstName' + count + '" maxlength="20" onkeypress="">';
    html += '<ul class="parsley-errors-list filled" id="parsley-txtAdFirstName' + count + '"><li class="parsley-required">This value is required.</li></ul>';
    html += '</div>';
    html += '</div>';
    html += '<div class="col-lg-4">';
    html += '<div class="form-group">';
    html += '<label class="form-control-label">Lastname: <span class="tx-danger">*</span></label>';
    html += '<input class="form-control" type="text" name="lastname" id="txtAdLastName' + count + '" onkeypress="">';
    html += '<ul class="parsley-errors-list filled" id="parsley-txtAdLastName' + count + '"><li class="parsley-required">This value is required.</li></ul>';
    html += '</div>';
    html += '</div>';
    html += '<div class="col-lg-4">';
    html += '<div class="form-group">';
    html += '<label class="form-control-label">Administrator Type: <span class="tx-danger">*</span></label>';
    html += '<select class="form-control administrator" id="ddlAdAdministratorType' + count + '">';
    html += '<option value="0">HR Administrator</option>';
    html += '<option value="1">HR Specialist</option>';
    html += '</select>';
    html += '<ul class="parsley-errors-list filled" id="parsley-ddlAdAdministratorType' + count + '"><li class="parsley-required">This value is required.</li></ul>';
    html += '</div>';
    html += '</div>';
    html += '</div>';
    html += '<div class="row mg-b-25">';

    html += '<div class="col-lg-4">';
    html += '<div class="form-group">';
    html += '<label class="form-control-label">Email address: <span class="tx-danger">*</span></label>';
    html += '<input class="form-control" type="text" name="email" id="txtAdEmail' + count + '">';
    html += '<ul class="parsley-errors-list filled" id="parsley-txtAdEmail' + count + '"><li class="parsley-required">This value is required.</li></ul>';
    html += '</div>';
    html += '</div>';
    html += '<div class="col-lg-4">';
    html += '<div class="form-group">';
    html += '<label class="form-control-label">Work Phone: <span class="tx-danger">*</span></label>';
    html += '<input class="form-control mask" type="text" name="lastname" id="txtAdWorkPhone' + count + '" maxlength="15" onkeypress="return isNumberKey(event);">';
    html += '<ul class="parsley-errors-list filled" id="parsley-txtAdWorkPhone' + count + '"><li class="parsley-required">This value is required.</li></ul>';
    html += '</div>';
    html += '</div>';
    html += '<div class="col-lg-4">';
    html += '<div class="form-group">';
    html += '<label class="form-control-label">Mobile: <span class="tx-danger">*</span></label>';
    html += '<input class="form-control mask" type="text" name="email" id="txtAdMobile' + count + '" maxlength="15" onkeypress="return isNumberKey(event);">';
    html += '<ul class="parsley-errors-list filled" id="parsley-txtAdMobile' + count + '"><li class="parsley-required">This value is required.</li></ul>';
    html += '</div>';
    html += '</div>';
    html += '</div>';
    html += '</div>';
    html += '</div>';
    $('#MainAdmin').append(html);
}
function RemoveCompany(id) {
    var count = parseInt($('#IconPlus').attr('count')) - 1;
    $('#IconPlus').attr('count', count);
    $('#ClientAdmin' + id).remove();
}
function SameAsCompany() {
    $('#txtBName').val($('#txtCompanyName').val());
    $('#txtBStreet').val($('#txtStreetName').val());
    $('#txtBState').val($('#txtStateName').val());
    $('#txtBCity').val($('#txtCityName').val());
    $('#txtBZip').val($('#txtZipCode').val());
    $('#txtBBillingPhone').val($('#txtWorkPhone').val());
    
}
function BillingReset() {
    $('#txtBName').val('');
    $('#txtBStreet').val('');
    $('#txtBState').val('');
    $('#txtBCity').val('');
    $('#txtBZip').val('');
    $('#txtBBillingPhone').val('');
    $('#txtBAttention').val('');
    $('#txtBFax').val('');
    $('.billing').prop('checked', false);
}
function GetPostJson() {
    var CompanyNew = {};
    CompanyNew.ID = parseInt($('#btnsubmit').attr('cid'));
    CompanyNew.IsActive = true;
    CompanyNew.CustomerCode = $('#txtCustomerNumber').val();
    CompanyNew.CCompanyName = $('#txtCompanyName').val();
    CompanyNew.CStreetName = $('#txtStreetName').val();
    CompanyNew.CCountry = 'United States';
    CompanyNew.CState = $('#txtStateName').val();
    CompanyNew.CContact = RemoveMask($('#txtWorkPhone').val()) ;
    CompanyNew.CCity = $('#txtCityName').val();
    CompanyNew.CZipCode = $('#txtZipCode').val();
    CompanyNew.CURL = $('#txtUrl').val();
    CompanyNew.CBrokerId = parseInt($('#btnsubmit').attr('brokerid'));
    CompanyNew.CEmail = '';
    CompanyNew.BCompanyName = $('#txtBName').val();
    CompanyNew.Battention = $('#txtBAttention').val();
    CompanyNew.BStreetName = $('#txtBStreet').val();
    CompanyNew.BCountry = 'United States';
    CompanyNew.BState = $('#txtBState').val();
    CompanyNew.BCity = $('#txtBCity').val();
    CompanyNew.BZipCode = $('#txtBZip').val();
    CompanyNew.BPhone = RemoveMask($('#txtBBillingPhone').val()) ;
    CompanyNew.BFax = $('#txtBFax').val();
    CompanyNew.BId = parseInt($('#btnsubmit').attr('bid'));
    CompanyNew.ClientDetails = new Array();
    CompanyNew.SubBrokerIds = new Array();
    if (parseInt($('#IconPlus').attr('count')) > 0) {
        for (var i = 1; i <= parseInt($('#IconPlus').attr('count')); i++) {
            var CompanyClientNew = {};
            CompanyClientNew.ID = 0;
            CompanyClientNew.FirstName = $('#txtAdFirstName' + i + '').val();
            CompanyClientNew.LastName = $('#txtAdLastName' + i + '').val();
            CompanyClientNew.Email = $('#txtAdEmail' + i + '').val();
            CompanyClientNew.AdministratorType = parseInt($('#ddlAdAdministratorType1' + i + '').val());
            CompanyClientNew.Phone = RemoveMask($('#txtAdMobile' + i + '').val());
            CompanyClientNew.WorkPhone = RemoveMask($('#txtAdWorkPhone' + i + '').val());
            CompanyClientNew.IsActive = true;
            CompanyNew.ClientDetails.push(CompanyClientNew);
        }
    }
    if ($('input[name="allow-access"]:checked').length > 0) {
        $('input[name="allow-access"]:checked').each(function (index, element) {
            CompanyNew.SubBrokerIds.push($(this).val());
        })
    }
    return CompanyNew;
}
function ValidationCompany() {
    $('.filled').hide();
    $('.form-control').removeClass('parsley-error');
    var valid = true;
    
    if ($('#txtCompanyName').val() == "") {
        valid = FocusError('txtCompanyName');
    }
    if ($('#txtStreetName').val() == "") {
        valid = FocusError('txtStreetName');
    }
    if ($('#txtStateName').val() == "") {
        valid = FocusError('txtStateName');
    }
    if ($('#txtCityName').val() == "") {
        valid = FocusError('txtCityName');
    }
    if ($('#txtZipCode').val() == "") {
        valid = FocusError('txtZipCode');
    }
    if ($('#txtWorkPhone').val() == "") {
        valid = FocusError('txtWorkPhone');
    }
    if ($('#txtBName').val() == "") {
        valid = FocusError('txtBName');
    }
    if ($('#txtBAttention').val() == "") {
        valid = FocusError('txtBAttention');
    }
    if ($('#txtBStreet').val() == "") {
        valid = FocusError('txtBStreet');
    }
    if ($('#txtBState').val() == "") {
        valid = FocusError('txtBState');
    }
    if ($('#txtBCity').val() == "") {
        valid = FocusError('txtBCity');
    }
    if ($('#txtBZip').val() == "") {
        valid = FocusError('txtBZip');
    }
    
    if (parseInt($('#IconPlus').attr('count')) > 1) {
        for (var i = 1; i <= parseInt($('#IconPlus').attr('count')); i++) {
            if ($('#txtAdFirstName'+i).val() == "") {
                valid = FocusError('txtAdFirstName' + i);
            }
            if ($('#txtAdLastName' + i).val() == "") {
                valid = FocusError('txtAdLastName' + i);
            }
            if ($('#txtAdEmail' + i).val() == "") {
                valid = FocusError('txtAdEmail' + i);
            }
            if ($('#txtAdMobile' + i).val() == "") {
                valid = FocusError('txtAdMobile' + i);
            }
            if ($('#txtAdWorkPhone' + i).val() == "") {
                valid = FocusError('txtAdWorkPhone' + i);
            }
        }
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
    var url = $(this).attr('url');
    var redirecturl = $(this).attr('redirecturl');
    if (ValidationCompany()) {
        var temptext = $('#btnsubmit').text();
        $('#btnsubmit').text('Processing...');
        $('#btnsubmit').prop('disabled',true);
        $.ajax({
            url: url,
            type: "POST",
            data: JSON.stringify(GetPostJson()),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                $('#btnsubmit').text(temptext);
                $('#btnsubmit').prop('disabled', false);
                if (response > 0) {
                    ShowAlertLoad('Updation successfully', 'Company details updated successfully', 'success', redirecturl);
                }
                else {
                    ShowAlert('Updation failed', 'Company details updation failed!please try after some time', 'error');
                }
                
            },
            error: function (err) {
                $('#btnsubmit').text(temptext);
                $('#btnsubmit').prop('disabled', false);
                ShowAlert('Updation failed', err.statusText, 'error');
               
            }
        });
    }
})
function EditClient(FirstName, LastName, Email, AdministratorType, Phone, WorkPhone, ID) {

    $('#txtAdFirstName1').val(FirstName);
    $('#txtAdLastName1').val(LastName);
    $('#ddlAdAdministratorType1').val(AdministratorType);
    $('#txtAdEmail1').val(Email);
    $('#txtAdWorkPhone1').val(WorkPhone);
    $('#txtAdMobile1').val(Phone);
    $('.btnclient').show();
    $('#btnclientsubmit').attr('iddata', ID);
}
function ResetClient() {

    $('#txtAdFirstName1').val('');
    $('#txtAdLastName1').val('');
    $('#ddlAdAdministratorType1').val('');
    $('#txtAdEmail1').val('');
    $('#txtAdWorkPhone1').val('');
    $('#txtAdMobile1').val('');
    $('.btnclient').hide();
    $('#btnclientsubmit').attr('iddata', 0);
}
$(document).on('click', '#btnclientsubmit', function () {
    var url = $(this).attr('url');
    var json = {
        ID: parseInt($(this).attr('iddata')),
        FirstName: $('#txtAdFirstName1').val(),
        LastName: $('#txtAdLastName1').val(),
        Email: $('#txtAdEmail1').val(),
        AdministratorType: parseInt($('#ddlAdAdministratorType1').val()),
        Phone: RemoveMask($('#txtAdMobile1').val()),
        WorkPhone: RemoveMask($('#txtAdWorkPhone1').val()),
        IsActive: true
    }
    var temptext = $('#btnclientsubmit').text();
    $('#btnclientsubmit').text('Processing...');
    $('#btnclientsubmit').prop('disabled', true);
    $.post(url, json, function (res) {
        $('#btnclientsubmit').text(temptext);
        $('#btnclientsubmit').prop('disabled', false);
        if (res > 0) {
            $('.btnclient').hide();
            $('#btnclientsubmit').attr('iddata', 0);
            ShowAlertLoad('Updation Successful', 'Details Updated Successfully', 'success', location.href);
        }
        else {
            ShowAlert('Updation failed', 'Details updation failed!please try after some time', 'error');
        }

    })
})
function AddMasking(number) {
    return number.replace(/^(\d{3})(\d{3})(\d{4}).*/, '($1) $2-$3');
}
function RemoveMask(number) {
    return  number.replace(/[^A-Z0-9]/ig, "");
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