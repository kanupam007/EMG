
$(document).ready(function () {
    $('#tblProperyGroupMaster').DataTable({
        columns: [
        { title: "Plan Category Name " },
        { title: "Property Group " },
           { title: "Sequence" },
        { title: "Status" },
        { title: "Action", "orderable": false },
        ]
    });
    $('#tblProperyMaster').DataTable({
        columns: [
       { title: "Plan Category Name " },
       { title: "Property Group Name " },
       { title: "Property Name" },
        { title: "Display Name" },
        { title: "Sequence" },
        { title: "Status" },
       { title: "Action", "orderable": false },
        ]
    });
    $('#tblcompany').DataTable({
        columns: [
        { title: "First Name " },
        { title: "Last Name " },
        { title: "Email" },
        { title: "Work Phone" },
         { title: "Mobile Phone" },
        { title: "Action", "orderable": false },
        ]
    });
    $('#tblReasonMaster').DataTable({
        columns: [
        { title: "Reason" },
        { title: "Status" },
        { title: "Action", "orderable": false },
        ]

    });
    //$('#tblsubbrokeruser').DataTable({
    //    columns: [
    //    { title: "Broker Name" },
    //    { title: "Email" },
    //    { title: "Phone" },
    //    { title: "Action", "orderable": false },
    //    ]

    //});

    var curr = $('#CultureCode').val();
    var phoneFormat = getmaskingformat(curr);
    $("#Phone,#WorkPhone").mask(phoneFormat);
    $(".BWorkPhone,.BPhone").mask(phoneFormat);
    var ssnFormat = getmaskingformatSSN(curr);
    $("#SSN").mask(ssnFormat);
    $(".calender").mask('99/99/9999');
});
function DeletePropertyGroupMaster(id) {

    swal({
        title: "",
        text: "Are you sure to inactive this record?",
        //type: "warning",
        showCancelButton: true,
        confirmButtonClass: "#f27474",
        confirmButtonText: "Yes",
        cancelButtonText: "Cancel",
        closeOnConfirm: true,
        closeOnCancel: true
    },
function (isConfirm) {

    if (isConfirm) {
        $.ajax({
            type: "POST",
            url: "/Admin/DeletePropertyGroupMaster",
            data: "{'GroupId':'" + id + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                if (response == 1) {
                      location.reload(true);
                }
                else {
                    swal("some error occurrred");
                    return false;
                }
            }
        });
    }
})

    //if (confirm("Are you sure to inactive this record ? ")) {
    //    //var PlanId = $(elem).attr("data-Id");
    //    $.ajax({
    //        type: "POST",
    //        url: "/Admin/DeletePropertyGroupMaster",
    //        data: "{'GroupId':'" + id + "'}",
    //        contentType: "application/json; charset=utf-8",
    //        success: function (response) {

    //            if (response == 1) {
    //                location.reload(true);
    //            }
    //            else {
    //                swal("some error occurrred");
    //                return false;

    //            }

    //        }
    //    });
    //}
}
function GetDataPropertyGroupMaster() {
    var value = $('#value').prop('checked');
    if (value == true) {
        value = "1";
    }
    else {
        value = "0";
    }
    $.ajax({
        type: 'GET',
        contentType: false,
        // url: '@Url.Action("GetDeletedPropertyGroupMaster", "Admin")',
        url: "/Admin/GetDeletedPropertyGroupMaster",
        data: { value: value },
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            $('#tblProperyGroupMaster').html('');
            $('#tblProperyGroupMaster').html(data);
            $("#tblProperyGroupMaster").dataTable({
                "destroy": true,
                columns: [
                { title: "Plan Category Name " },
                { title: "Property Group " },
                   { title: "Sequence" },
                { title: "Status" },
                { title: "Action", "orderable": false },
                ]
                // ... skipped ...
            });

        }
    });
}
$(document).on('click', '#btnSavePropertyGroupMaster', function (e) {
    var isValid = true;

    if ($.trim($('#ddlPlanCategoryId').val()) == '') {
        isValid = false;
        $('#ddlPlanCategoryId').removeClass('custom-input-success').addClass('custom-input-error');
        $('#ddlPlanCategoryId').change(function () {
            $('#ddlPlanCategoryId').removeClass('custom-input-error').addClass('custom-input-success');
        });

    }
    if ($.trim($('#PropertyGroupName').val()) == '') {
        isValid = false;
        $('#PropertyGroupName').removeClass('custom-input-success').addClass('custom-input-error');
        $('#PropertyGroupName').keypress(function () {
            $('#PropertyGroupName').removeClass('custom-input-error').addClass('custom-input-success');
        });

    }
    if (isValid == false)
        e.preventDefault();
    else
        $("#formIdPropertyGroupMaster").submit();
});
$(document).on('click', '#btnSavePropertyMaster', function (e) {

    var isValid = true;
    if ($.trim($('#ddlPlanCategoryId').val()) == '') {
        isValid = false;
        $('#ddlPlanCategoryId').removeClass('custom-input-success').addClass('custom-input-error');
        $('#ddlPlanCategoryId').change(function () {
            $('#ddlPlanCategoryId').removeClass('custom-input-error').addClass('custom-input-success');
        });

    }
    if ($.trim($('#ddlGroupId').val()) == '') {
        isValid = false;
        $('#ddlGroupId').removeClass('custom-input-success').addClass('custom-input-error');
        $('#ddlGroupId').change(function () {
            $('#ddlGroupId').removeClass('custom-input-error').addClass('custom-input-success');
        });

    }
    if ($.trim($('#PropertyName').val()) == '') {
        isValid = false;
        $('#PropertyName').removeClass('custom-input-success').addClass('custom-input-error');
        $('#PropertyName').keypress(function () {
            $('#PropertyName').removeClass('custom-input-error').addClass('custom-input-success');
        });

    }
    if ($.trim($('#DisplayName').val()) == '') {
        isValid = false;
        $('#DisplayName').removeClass('custom-input-success').addClass('custom-input-error');
        $('#DisplayName').keypress(function () {
            $('#DisplayName').removeClass('custom-input-error').addClass('custom-input-success');
        });

    }
    if (isValid == false)
        e.preventDefault();
    else
        $("#formIdPropertyMaster").submit();
});
$(document).on('click', '#btnSaveReason', function (e) {
    var isValid = true;


    if ($.trim($('#ReasonName').val()) == '') {
        isValid = false;
        $('#ReasonName').removeClass('custom-input-success').addClass('custom-input-error');
        $('#ReasonName').keypress(function () {
            $('#ReasonName').removeClass('custom-input-error').addClass('custom-input-success');
        });

    }
    if (isValid == false)
        e.preventDefault();
    else
        $("#formIdReason").submit();
});
function DeletePropertyMaster(id) {


    swal({
        title: "",
        text: "Are you sure to inactive this record?",
        //type: "warning",
        showCancelButton: true,
        confirmButtonClass: "#f27474",
        confirmButtonText: "Yes",
        cancelButtonText: "Cancel",
        closeOnConfirm: true,
        closeOnCancel: true
    },
function (isConfirm) {

    if (isConfirm) {
        $.ajax({
            type: "POST",
            url: "/Admin/DeletePropertyMaster",
            data: "{'PropertyId':'" + id + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (response) {

                if (response == 1) {
                    location.reload(true);
                }
                else {
                    swal("some error occurrred");
                    return false;

                }

            }
        });
    }
})
    //if (confirm("Are you sure to inactive this record ? ")) {
    //    //var PlanId = $(elem).attr("data-Id");
    //    $.ajax({
    //        type: "POST",
    //        url: "/Admin/DeletePropertyMaster",
    //        data: "{'PropertyId':'" + id + "'}",
    //        contentType: "application/json; charset=utf-8",
    //        success: function (response) {

    //            if (response == 1) {
    //                location.reload(true);
    //            }
    //            else {
    //                swal("some error occurrred");
    //                return false;

    //            }

    //        }
    //    });
    //}
}
function GetDataPropertyMaster() {
    var value = $('#value').prop('checked');
    if (value == true) {
        value = "1";
    }
    else {
        value = "0";
    }
    $.ajax({
        type: 'GET',
        contentType: false,
        //url: '@Url.Action("GetDeletedPropertyMaster", "Admin")',
        url: "/Admin/GetDeletedPropertyMaster",
        data: { value: value },
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            $('#tblProperyMaster').html('');
            $('#tblProperyMaster').html(data);
            $("#tblProperyMaster").dataTable({
                "destroy": true,
                columns: [
                { title: "Plan Category Name " },
                { title: "Property Group Name " },
                { title: "Property Name" },
                { title: "Display Name" },
                 { title: "Sequence" },
                { title: "Status" },
                { title: "Action", "orderable": false },
                ]
                // ... skipped ...
            });

        }
    });
}


function RemoveCommas(value) {

    return value.replace(/\D/g, '');
}
function getmaskingformat(curr) {

    var phoneFormat = "";
    switch (curr) {
        case "en-IN":
            phoneFormat = "(999) 999-9999";
            break;
        case "en-GB":
            phoneFormat = "(999) 999-9999";
            break;
        case "es-ES":
            phoneFormat = "(999) 999-9999";
            break;
        case "fr-CA":
            phoneFormat = "99 99 99 99 99";
            break;
        case "de-CH":
            phoneFormat = "999 999-9999";
            break;

        case "zh-CN":
            phoneFormat = "99-9999-9999";
            break;
        case "fr-FR":
            phoneFormat = "99 99 99 99 99";
            break;

        case "pl-PL":
            phoneFormat = "999-999-9999";
            break;
        case "zh-SG":
            phoneFormat = "99-9999-9999";
            break;

        case "th-TH":
            phoneFormat = "(999) 999-9999";
            break;

        default:
            phoneFormat = "(999) 999-9999";
            break;
    }
    return phoneFormat;


}

function getmaskingformatSSN(curr) {

    var ssnFormat = "";
    switch (curr) {
        case "en-IN":
            ssnFormat = "999-99-9999";
            break;
        case "es-ES":
            ssnFormat = "999-99-9999";
            break;
        case "en-GB":
            ssnFormat = "999-99-9999";
            break;
        case "fr-CA":
            ssnFormat = "999 999 999";
            break;
        case "de-CH":
            ssnFormat = "99 999 9999";
            break;
        case "zh-CN":
            ssnFormat = "999 999 999";
            break;
        case "fr-FR":
            ssnFormat = "999 999 999";
            break;
        case "pl-PL":
            ssnFormat = "99-999/9999";
            break;
        case "zh-SG":
            ssnFormat = "999 999 999";
            break;
        case "th-TH":
            ssnFormat = "999-99-9999";
            break;
        default:
            ssnFormat = "999-99-9999";
            break;
    }
    return ssnFormat;


}



$(document).on('click', '.btnCurrSave', function (e) {

    var $form = $('formId');
    //$('#formId').validate().form();
    $('.currInput').each(function () {
        $(this).val(RemoveCommas($(this).val()));
    })

    $('.phonemask').each(function () {
        $(this).val(RemoveCommas($(this).val()));
    })
    $('.ssnmask').each(function () {
        $(this).val(RemoveCommas($(this).val()));
    })
    //$("#formId").submit();
});
function isNumber(evt) {

    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 47 || charCode > 57)) {

        return false;
    }
    return true;
}

function Addmoresubbroker() {

    jsonObj = [];
    $("#addBroker .brokerDiv").each(function () {

        item = {}
        item["BrokerId"] = $(this).find('.ID').val();
        item["BrokerName"] = $(this).find('.BrokerName').val();
        item["FirstName"] = $(this).find('.BFirstName').val();
        item["LastName"] = $(this).find('.BLastName').val();
        item["Street"] = $(this).find('.BStreet').val();
        item["City"] = $(this).find('.BCity').val();
        item["State"] = $(this).find('.BState').val();
        item["ZipCode"] = $(this).find('.BZipCode').val();
        item["Country"] = $(this).find('.BCountry').val();
        item["Email"] = $(this).find('.BEmail').val();
        item["Phone"] = $(this).find('.BPhone').val();
        item["WorkPhone"] = $(this).find('.BWorkPhone').val();

        jsonObj.push(item);
    })

    $.ajax({
        type: "POST",
        url: "/Admin/_AddBroker",
        data: JSON.stringify({ jsonObj: jsonObj }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {

            $('#addBroker').html("");
            $('#addBroker').html(response);
            var formid = $("#frmAddBrokersub");
            formid.unbind();
            formid.data("validator", null);
            $.validator.unobtrusive.parse($("#frmAddBrokersub"));
            var curr = $('#CultureCode').val();
            var phoneFormat = getmaskingformat(curr);
            $(".BWorkPhone,.BPhone").mask(phoneFormat);

        }
    });



}
$(document).on('click', '.removeSec', function () {

    var _this = this;
    var buttoncontext = this;
    var dataindex = $(this).parent().parent().attr('data-index');
    var id = $(buttoncontext).attr("data-id");
    //if (parseInt(id) == 0 && parseInt(dataindex) == 0) {
    //    alert('can not deleted')
    //    return false;
    //}
    if (parseInt(id) == 0) {
        ($(_this)).parent().parent(".child_list_index").remove();
    }

})
// Start Get Data Broker User In Poup
$(document).on('click', '.editSubBroker', function (evt) {
    var subbrokerid = $(this).attr('data-id');
    $("#LoadMain").addClass("loader");
    $("#LoadMain").css({ display: "block" });
    $.ajax({
        type: "POST",
        url: "/Admin/_EditSubBroker",
        data: "{'Subbrokerid':'" + subbrokerid + "'}",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $("#LoadMain").removeClass("loader");
            $("#LoadMain").css({ display: "none" });
            $('#myModal').html("");
            $('#myModal').html(response);
            var curr = $('#CultureCode').val();
            var phoneFormat = getmaskingformat(curr);
            $(".SBPhone,.SBWorkPhone").mask(phoneFormat);
            $('#myModal').modal('show');
            var formid = $("#frmeditbroker");
            formid.unbind();
            formid.data("validator", null);
            $.validator.unobtrusive.parse($("#frmeditbroker"));
        },
        failure: function (msg) {
            alert(msg);
        }
    });

})
// End Get Data Broker User In Poup

// Start Get Data Broker Company In Poup
$(document).on('click', '.editBroker', function (evt) {
    var brokerid = $(this).attr('data-id');
    $("#LoadMain").addClass("loader");
    $("#LoadMain").css({ display: "block" });
    $.ajax({
        type: "POST",
        url: "/Admin/_EditBroker",
        data: "{'Brokerid':'" + brokerid + "'}",
        contentType: "application/json; charset=utf-8",
        success: function (response) {

            $("#LoadMain").removeClass("loader");
            $("#LoadMain").css({ display: "none" });
            $('#myModal').html("");
            $('#myModal').html(response);
            var curr = $('#CultureCode').val();
            var phoneFormat = getmaskingformat(curr);
            $("#Phone,#WorkPhone").mask(phoneFormat);
            $('#myModal').modal('show');
            var formid = $("#frmeditbroker");
            formid.unbind();
            formid.data("validator", null);
            $.validator.unobtrusive.parse($("#frmeditbroker"));
        },
        failure: function (msg) {
            alert(msg);
        }
    });

})
// End Get Data Broker Company In Poup

// Start Broker Company Update
$(document).on('click', '.btnsaveBroker', function (evt) {

    evt.preventDefault();
    var $form = $('#frmeditbroker');
    if ($form.valid()) {

        jsonObj = {}
        jsonObj["BrokerID"] = $('.bID').val();
        jsonObj["BrokerName"] = $('.PBrokerName').val();
        //jsonObj["FirstName"] = $('.PFirstName').val();
        //jsonObj["LastName"] = $('.PLastName').val();
        jsonObj["Street"] = $('.PStreet').val();
        jsonObj["City"] = $('.PCity').val();
        jsonObj["State"] = $('.PState').val();
        jsonObj["ZipCode"] = $('.PZipCode').val();
        jsonObj["Country"] = $('.PCountry').val();
        //jsonObj["Email"] = $('.PEmail').val();
        //jsonObj["Phone"] = RemoveCommas($('.PPhone').val());
        jsonObj["WorkPhone"] = RemoveCommas($('.PWorkPhone').val());
        $.ajax({
            type: "POST",
            url: "/Admin/SaveBroker",
            data: JSON.stringify({ jsonObj: jsonObj }),
            contentType: "application/json; charset=utf-8",
            success: function (response) {

                if (response == true) {
                    $('#myModal').modal('hide');
                    //$("#LoadMain").addClass("loader");
                    //$("#LoadMain").css({ display: "block" });
                    toastr["success"]("Broker company updated successfully.", "Success !");
                    $.ajax({
                        type: "GET",
                        url: "/Admin/_ManageBroker",
                        contentType: "application/text; charset=utf-8",
                        success: function (response) {
                            //$("#LoadMain").removeClass("loader");
                            //$("#LoadMain").css({ display: "none" });
                            $('#Contentplaceholder').html("");
                            $('#Contentplaceholder').html(response);

                        }
                    });

                }
                else {

                    $("#LoadMain").removeClass("loader");
                    $("#LoadMain").css({ display: "none" });
                    var curr = $('#CultureCode').val();
                    var phoneFormat = getmaskingformat(curr);
                    $("#Phone,#WorkPhone").mask(phoneFormat);
                    // toastr["error"]("Phone or Email already exist at " + jsonObj["Phone"] + "", "Error !");
                }
            },
            failure: function (msg) {
                alert(msg);
            }
        });
    }
});
// Start Broker Company Update

// Start Broker Company Add
$(document).on('click', '.btnAddBroker', function (evt) {

    evt.preventDefault();
    var $form = $('#frmAddBroker');
    if ($form.valid()) {
        $("#LoadMain").addClass("loader");
        $("#LoadMain").css({ display: "block" });
        jsonObj = {}
        jsonObj["BrokerID"] = $('#BrokerId').val();
        jsonObj["BrokerName"] = $('#BrokerName').val();
        jsonObj["Street"] = $('#Street').val();
        jsonObj["City"] = $('#City').val();
        jsonObj["State"] = $('#State').val();
        jsonObj["ZipCode"] = $('#ZipCode').val();
        jsonObj["Country"] = $('#Country').val();
        //jsonObj["Email"] = $('#Email').val();
        //jsonObj["Phone"] = RemoveCommas($('#Phone').val());
        jsonObj["WorkPhone"] = RemoveCommas($('#WorkPhone').val());
        $.ajax({
            type: "POST",
            url: "/Admin/SaveBroker",
            data: JSON.stringify({ jsonObj: jsonObj }),
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                if (response == true) {
                    $("#LoadMain").removeClass("loader");
                    $("#LoadMain").css({ display: "none" });
                    $('#myModal').modal('hide');
                    $('#BrokerId').val('0');
                    $('#BrokerName').val('');
                    $('#Street').val('');
                    $('#City').val('');
                    $('#State').val('');
                    $('#ZipCode').val('');
                    $('#Country').val('');
                    //$('#Email').val('');
                    //$('#Phone').val('');
                    $('#WorkPhone').val('');
                    toastr["success"]("Broker company added successfully.", "Success !");
                    $.ajax({
                        type: "GET",
                        url: "/Admin/_ManageBroker",
                        contentType: "application/text; charset=utf-8",
                        success: function (response) {
                            $('#Contentplaceholder').html("");
                            $('#Contentplaceholder').html(response);
                        }
                    });

                    $.ajax({
                        type: "GET",
                        url: "/Admin/_DropdownBrokerComapny",
                        contentType: "application/text; charset=utf-8",
                        success: function (response) {
                            $('#dropdownbrokercompany').html("");
                            $('#dropdownbrokercompany').html(response);
                        }
                    });

                    $.ajax({
                        type: "GET",
                        url: "/Admin/_DropdownBrokerCompany",
                        contentType: "application/text; charset=utf-8",
                        success: function (response) {
                            $('#DropdownBrokerCompany').html("");
                            $('#DropdownBrokerCompany').html(response);

                        }
                    });

                }
                else {
                    $("#LoadMain").removeClass("loader");
                    $("#LoadMain").css({ display: "none" });
                    toastr["error"]("Phone or Email already exist at " + jsonObj["Phone"] + "", "Error !");
                }
            },
            failure: function (msg) {
                alert(msg);
            }
        });
    }
});
// End Broker Company Add


// Start Broker User Update
$(document).on('click', '.btnsaveSubBroker', function (evt) {

    evt.preventDefault();
    var $form = $('#frmeditbroker');
    if ($form.valid()) {

        jsonObj = {}
        jsonObj["SubBrokerID"] = $('.sbID').val();
        jsonObj["BrokerID"] = $('.bID').val();
        //jsonObj["BrokerName"] = $('.BrokerName').val();
        jsonObj["ddlBrokerId"] = $('.SBddlBroker').val();
        jsonObj["FirstName"] = $('.SBFirstName').val();
        jsonObj["LastName"] = $('.SBLastName').val();
        //jsonObj["Street"] = $('.SBStreet').val();
        //jsonObj["City"] = $('.SBCity').val();
        //jsonObj["State"] = $('.SBState').val();
        //jsonObj["ZipCode"] = $('.SBZipCode').val();
        //jsonObj["Country"] = $('.SBCountry').val();
        jsonObj["Email"] = $('.SBEmail').val();
        jsonObj["Phone"] = RemoveCommas($('.SBPhone').val());
        jsonObj["WorkPhone"] = RemoveCommas($('.SBWorkPhone').val());
        $.ajax({
            type: "POST",
            url: "/Admin/SaveSubBroker",
            data: JSON.stringify({ jsonObj: jsonObj }),
            contentType: "application/json; charset=utf-8",
            success: function (response) {

                if (response == true) {
                    $('#myModal').modal('hide');

                    toastr["success"]("Broker user updated successfully.", "Success !");
                    var brokerid = $(".ddlBrokerIdchange").val();
                    var flag = 0;
                    $.ajax({
                        type: "GET",
                        url: "/Admin/_SubbrokerList",
                        data: { flag: flag, BrokerId: brokerid },
                        contentType: "application/text; charset=utf-8",
                        success: function (response) {
                            $('#subBrokerList').html("");
                            $('#subBrokerList').html(response);

                        }
                    });

                }
                else {
                    $("#LoadMain").removeClass("loader");
                    $("#LoadMain").css({ display: "none" });
                    var curr = $('#CultureCode').val();
                    var phoneFormat = getmaskingformat(curr);
                    $("#Phone,#WorkPhone").mask(phoneFormat);
                    toastr["error"]("Phone or Email already exist at " + jsonObj["Phone"] + "", "Error !");
                }
            },
            failure: function (msg) {
                alert(msg);
            }
        });
    }
});
// End Broker User Update

//Start Broker User Add
$(document).on('click', '.btnAddSubBroker', function (evt) {

    evt.preventDefault();
    var $form = $('#frmAddBrokerUser');
    if ($form.valid()) {
        $("#LoadMain").addClass("loader");
        $("#LoadMain").css({ display: "block" });
        jsonObj = {}
        jsonObj["BrokerID"] = $('.ddlBroker').val();
        jsonObj["ddlBrokerId"] = $('.ddlBroker').val();
        jsonObj["FirstName"] = $('.BFirstName').val();
        jsonObj["LastName"] = $('.BLastName').val();
        //jsonObj["Street"] = $('.BStreet').val();
        //jsonObj["City"] = $('.BCity').val();
        //jsonObj["State"] = $('.BState').val();
        //jsonObj["ZipCode"] = $('.BZipCode').val();
        //jsonObj["Country"] = $('.BCountry').val();
        jsonObj["Email"] = $('.BEmail').val();
        jsonObj["Phone"] = RemoveCommas($('.BPhone').val());
        jsonObj["WorkPhone"] = RemoveCommas($('.BWorkPhone').val());
        $.ajax({
            type: "POST",
            url: "/Admin/SaveSubBroker",
            data: JSON.stringify({ jsonObj: jsonObj }),
            contentType: "application/json; charset=utf-8",
            success: function (response) {

                if (response == true) {
                    $('#myModal').modal('hide');
                    $("#LoadMain").removeClass("loader");
                    $("#LoadMain").css({ display: "none" });
                    $('.ddlBroker').val('');
                    $('.BFirstName').val('');
                    $('.BLastName').val('');
                    //$('.BStreet').val('');
                    //$('.BCity').val('');
                    //$('.BState').val('');
                    //$('.BZipCode').val('');
                    //$('.BCountry').val('');
                    $('.BPhone').val('');
                    $('.BEmail').val('');
                    $('.BWorkPhone').val('');
                    toastr["success"]("Broker user added successfully.", "Success !");
                    var brokerid = $(".ddlBrokerIdchange").val();
                    var flag = 0;
                    $.ajax({
                        type: "GET",
                        url: "/Admin/_SubbrokerList",
                        //data: { flag: flag, BrokerId: brokerid },
                        contentType: "application/text; charset=utf-8",
                        success: function (response) {
                            $('#subBrokerList').html("");
                            $('#subBrokerList').html(response);
                        }
                    });

                    $.ajax({
                        type: "GET",
                        url: "/Admin/_DropdownBrokerComapny",
                        contentType: "application/text; charset=utf-8",
                        success: function (response) {
                            $('#dropdownbrokercompany').html("");
                            $('#dropdownbrokercompany').html(response);
                        }
                    });

                }
                else {
                    $("#LoadMain").removeClass("loader");
                    $("#LoadMain").css({ display: "none" });
                    toastr["error"]("Phone or Email already exist at " + jsonObj["Phone"] + "", "Error !");
                }
            },
            failure: function (msg) {
                alert(msg);
            }
        });
    }
});
//End Broker User Add
// Start Broker Status Active-Inactive 
$(document).on('click', '.BrokerStatus', function (evt) {

    var brokerid = $(this).attr('data-id');
    var status = $(this).attr('data-status');
    var msgconfirm;
    if (status == 'False') {
        msgconfirm = 'activate';
    }
    else {
        msgconfirm = 'suspend';
    }
    status == true ? "Active" : "In-Active"


    swal({
        title: "",
        text: "Do you want to " + msgconfirm + " the broker company?",
        //type: "warning",
        showCancelButton: true,
        confirmButtonClass: "#f27474",
        confirmButtonText: "Yes",
        cancelButtonText: "Cancel",
        closeOnConfirm: true,
        closeOnCancel: true
    },
function (isConfirm) {

    if (isConfirm) {

        $.ajax({
            type: "POST",
            url: "/Admin/UpdateStatusBroker",
            data: "{'Brokerid':'" + brokerid + "','Status':'" + status + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (response) {

                if (response == true) {
                    $('#myModal').modal('hide');
                    toastr["success"]("Broker company status updated successfully.", "Success !");
                    $.ajax({
                        type: "GET",
                        url: "/Admin/_ManageBroker",
                        contentType: "application/text; charset=utf-8",
                        success: function (response) {

                            $('#Contentplaceholder').html("");
                            $('#Contentplaceholder').html(response);

                        }
                    });

                    $.ajax({
                        type: "GET",
                        url: "/Admin/_DropdownBrokerComapny",
                        contentType: "application/text; charset=utf-8",
                        success: function (response) {
                            $('#dropdownbrokercompany').html("");
                            $('#dropdownbrokercompany').html(response);
                        }
                    });

                    $.ajax({
                        type: "GET",
                        url: "/Admin/_DropdownBrokerCompany",
                        contentType: "application/text; charset=utf-8",
                        success: function (response) {
                            $('#DropdownBrokerCompany').html("");
                            $('#DropdownBrokerCompany').html(response);

                        }
                    });

                }
                else {

                }
            },
            failure: function (msg) {
                alert(msg);
            }
        });
    }
    else {

    }
});


});
// End Broker Status Active-Inactive 



$(document).on('click', '.SubBrokerStatus', function (evt) {
   
    var Subbrokerid = $(this).attr('data-id');
    var brokerid = $(this).attr('data-brokerid');
    var status = $(this).attr('data-status');
    var msgconfirm;
    if (status == 'False') {
        msgconfirm = 'activate';
    }
    else {
        msgconfirm = 'suspend';
    }
    status == true ? "Active" : "In-Active"
    swal({
        title: "",
        text: "Do you want to " + msgconfirm + " the broker user?",
        //type: "warning",
        showCancelButton: true,
        confirmButtonClass: "#f27474",
        confirmButtonText: "Yes",
        cancelButtonText: "Cancel",
        closeOnConfirm: true,
        closeOnCancel: true
    },
function (isConfirm) {

    if (isConfirm) {
        $.ajax({
            type: "POST",
            url: "/Admin/UpdateStatusSubBroker",
            data: "{'SubBrokerId':'" + Subbrokerid + "','Status':'" + status + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
             
                if (response == true) {
                    $('#myModal').modal('hide');
                    toastr["success"]("Broker user status updated successfully.", "Success !");
                    var flag = 0;
                    $.ajax({
                        type: "GET",
                        url: "/Admin/_SubbrokerList",
                        data: { flag: flag, BrokerId: brokerid },
                        contentType: "application/text; charset=utf-8",
                        success: function (response) {
                       
                            $('#subBrokerList').html("");
                            $('#subBrokerList').html(response);

                        }
                    });

                }
                else {

                }
            },
            failure: function (msg) {
                alert(msg);
            }
        });
    }
    else {

    }
});

});


// Start Broker User Deactivate 
$(document).on('click', '.SubBrokerDeactivate', function (evt) {

    var Subbrokerid = $(this).attr('data-id');

    swal({
        title: "",
        text: "Do you want to delete the broker user?",
        //type: "warning",
        showCancelButton: true,
        confirmButtonClass: "#f27474",
        confirmButtonText: "Yes",
        cancelButtonText: "Cancel",
        closeOnConfirm: true,
        closeOnCancel: true
    },
function (isConfirm) {
    if (isConfirm) {
        $.ajax({
            type: "POST",
            url: "/Admin/DeactivateSubBroker",
            data: "{'SubBrokerId':'" + Subbrokerid + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (response) {

                if (response == true) {
                    $('#myModal').modal('hide');
                    toastr["success"]("Broker user deleted successfully.", "Success !");
                    var brokerid = $(".ddlBrokerIdchange").val();
                    var flag = 0;
                    $.ajax({
                        type: "GET",
                        url: "/Admin/_SubbrokerList",
                        data: { flag: flag, BrokerId: brokerid },
                        contentType: "application/text; charset=utf-8",
                        success: function (response) {
                            $('#subBrokerList').html("");
                            $('#subBrokerList').html(response);

                        }
                    });

                }
                else {

                }
            },
            failure: function (msg) {
                alert(msg);
            }
        });
    }
    else {

    }
});

});
// End Broker User Deactivate 

// Start Broker Company Deactivate
$(document).on('click', '.BrokerDeactivate', function (evt) {
    var brokerid = $(this).attr('data-id');
    swal({
        title: "",
        text: "Do you want to delete the broker company?",
        //type: "warning",
        showCancelButton: true,
        confirmButtonClass: "#f27474",
        confirmButtonText: "Yes",
        cancelButtonText: "Cancel",
        closeOnConfirm: true,
        closeOnCancel: true
    },
function (isConfirm) {
    if (isConfirm) {
        $.ajax({
            type: "POST",
            url: "/Admin/DeactivateBroker",
            data: "{'Brokerid':'" + brokerid + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (response) {

                if (response == true) {
                    $('#myModal').modal('hide');
                    toastr["success"]("Broker company deleted successfully.", "Success !");
                    $.ajax({
                        type: "GET",
                        url: "/Admin/_ManageBroker",
                        contentType: "application/text; charset=utf-8",
                        success: function (response) {
                            $('#Contentplaceholder').html("");
                            $('#Contentplaceholder').html(response);
                        }
                    });

                    var flag = 0;
                    $.ajax({
                        type: "GET",
                        url: "/Admin/_SubbrokerList",
                        data: { flag: flag, BrokerId: brokerid },
                        contentType: "application/text; charset=utf-8",
                        success: function (response) {
                            $('#subBrokerList').html("");
                            $('#subBrokerList').html(response);

                        }
                    });

                    $.ajax({
                        type: "GET",
                        url: "/Admin/_DropdownBrokerComapny",
                        contentType: "application/text; charset=utf-8",
                        success: function (response) {
                            $('#dropdownbrokercompany').html("");
                            $('#dropdownbrokercompany').html(response);
                        }
                    });

                    $.ajax({
                        type: "GET",
                        url: "/Admin/_DropdownBrokerCompany",
                        contentType: "application/text; charset=utf-8",
                        success: function (response) {
                            $('#DropdownBrokerCompany').html("");
                            $('#DropdownBrokerCompany').html(response);

                        }
                    });
                }
                else {
                    toastr["error"]("Some error occured while deleting broker company.", "Error !");
                }
            },
            failure: function (msg) {
                alert(msg);
            }
        });

    }
    else {

    }
});

});
// End Broker Company Deactivate


// Start Get broker user on the behalf of broker company
$(document).on('change', '.ddlBrokerIdchange', function (evt) {

    var brokerid = $(".ddlBrokerIdchange").val();
    var flag = 0;
    $.ajax({
        type: "GET",
        url: "/Admin/_SubbrokerList",
        data: { flag: flag, BrokerId: brokerid },
        contentType: "application/text; charset=utf-8",
        success: function (response) {
            $('#subBrokerList').html("");
            $('#subBrokerList').html(response);

        }
    });


});
$(document).on('change', '.ddlBrokerIdchangeAddCompany', function (evt) {
    var brokerid = $(".ddlBrokerIdchangeAddCompany").val();

    var Companyid = $(".cID").val();

    if (Companyid < 1) {
        Companyid = 0;
    }

    if (brokerid > 0) {
        $(".rdBrokerAdd").css('display', 'block');
        //$(".resetFields").css('display', 'block');
    }
    else {

        $(".resetFields").trigger("click");
        $(".rdBrokerAdd").css('display', 'none');
        //$(".resetFields").css('display', 'none');

        //$('#Billing_CompanyName').val('');
        //$('#Billing_StreetName').val('');
        //$('#Billing_City').val('');
        //$('#Billing_State').val('');
        //$('#Billing_ZipCode').val('');
        //$('#Billing_Country').val('');
        //$('#Billing_Phone').val('');
        //$('#Billing_Fax').val('');
        
        
    }
    var flag = 0;
    $.ajax({
        type: "GET",
        url: "/Admin/_SubbrokerListCompany",
        data: { flag: flag, BrokerId: brokerid, Companyid: Companyid },
        contentType: "application/text; charset=utf-8",
        success: function (response) {
            $('#subBrokerList').html("");
            $('#subBrokerList').html(response);

        }
    });
    // }


});
$(document).on('change', '.ddlCompanychange', function (evt) {

    var companyid = $(".ddlCompanychange").val();

    $.ajax({
        type: "GET",
        url: "/Broker/_ManageEmployee",
        data: { CompanyId: companyid },
        contentType: "application/text; charset=utf-8",
        success: function (response) {
            $('#brokerEmployeeList').html("");
            $('#brokerEmployeeList').html(response);

        }
    });


});
// End Get broker user on the behalf of broker company
$(document).on('blur', '.newclsMPhone', function () {
    if ($('.newclsMPhone').val() == "" || $('.newclsMPhone').val() == "(___) ___-____") {
        $('.newclsMPhone').addClass('input-validation-error');
    }
});
$(document).on('blur', '.PPhone', function () {
    if ($('.PPhone').val() == "" || $('.PPhone').val() == "(___) ___-____") {
        $('.PPhone').addClass('input-validation-error');
    }
});
$(document).on('click', '#btnSaveImputedIncome', function (e) {

    var isValid = true;
    
    if ($.trim($('#AgeStart').val()) == '') {
        isValid = false;
        $('#AgeStart').removeClass('custom-input-success').addClass('custom-input-error');
        $('#AgeStart').keypress(function () {
            $('#AgeStart').removeClass('custom-input-error').addClass('custom-input-success');
        });

    }
    if ($.trim($('#AgeEnd').val()) == '') {
        isValid = false;
        $('#AgeEnd').removeClass('custom-input-success').addClass('custom-input-error');
        $('#AgeEnd').keypress(function () {
            $('#AgeEnd').removeClass('custom-input-error').addClass('custom-input-success');
        });

    }

    if ($.trim($('#EmployeeValue').val()) == '') {
        isValid = false;
        $('#EmployeeValue').removeClass('custom-input-success').addClass('custom-input-error');
        $('#EmployeeValue').keypress(function () {
            $('#EmployeeValue').removeClass('custom-input-error').addClass('custom-input-success');
        });

    }
    if (isValid == true)
    {
        $('#EmployeeValue').val(RemoveCurrency($('#EmployeeValue').val(),','));
    }


    if (isValid == false)
        e.preventDefault();
    else
        $("#formIdImputedIncome").submit();
});

function RemoveCurrency(value, curr) {
    if (curr == undefined || curr == null || curr == "") {
        curr = $('#CultureName').val();
    }
    return value.toString().replace(',', '').replace(curr, '').replace(',', '').replace('%', '');
}

function DeleteImputedIncome(id) {

    swal({
        title: "",
        text: "Are you sure to Delete this record?",
        //type: "warning",
        showCancelButton: true,
        confirmButtonClass: "#f27474",
        confirmButtonText: "Yes",
        cancelButtonText: "Cancel",
        closeOnConfirm: true,
        closeOnCancel: true
    },
function (isConfirm) {

    if (isConfirm) {
        $.ajax({
            type: "POST",
            url: "/Admin/DeleteImputedIncome",
            data: "{'AgeID':'" + id + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (response) {

                if (response == 1) {
                    location.reload(true);
                }
                else {
                    swal("some error occurrred");
                    return false;

                }

            }
        });
    }
})

}