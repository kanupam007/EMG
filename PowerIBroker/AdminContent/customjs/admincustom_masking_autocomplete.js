
$(document).ready(function () {

    var curr = $('#CultureCode').val();
    var phoneFormat = getmaskingformat(curr);
    $("#Phone,#WorkPhone").mask(phoneFormat);
    $("#CContact,#Billing_Phone,#Broker_WorkPhone,#Broker_Phone").mask(phoneFormat);
    $(".WorkPhone,.Phone").mask(phoneFormat);
    var ssnFormat = getmaskingformatSSN(curr);

    $("#SSN").mask(ssnFormat);
    $(".calender").mask('99/99/9999');
});

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
$(document).on('click', '.btnCurrSave', function (e) {
    if ($("#frmAddProduct").valid()) {
        $('.phonemask').each(function () {
            $(this).val(RemoveCommas($(this).val()));
        })
    }
});

$(document).on('blur', '.phoneChk', function (evt) {
    var curr = $(this);
    var $elems = $('.phoneChk');
    var values = [];
    var isDuplicated = false;
    $elems.each(function () {
        if (!this.value) return true;
        if (values.indexOf(this.value) !== -1) {
            isDuplicated = true;
            $(curr).val("");
            //alert('Phone no should be different.');
            toastr['error']("Phone no should be different.");
            return false;
        }
        values.push(this.value);
    });
    return isDuplicated;
})
$(document).on('blur', '.emailChk', function (evt) {
    var $elems = $('.emailChk');
    var curr = $(this);
    var values = [];
    var isDuplicated = false;
    $elems.each(function () {
        if (!this.value) return true;
        if (values.indexOf(this.value) !== -1) {
            isDuplicated = true;
            $(curr).val("");
            //alert('Email should be different.');
            toastr['error']("Email should be different.");
            return false;
        }
        values.push(this.value);
    });
    return isDuplicated;
})

$(document).on('click', '.clsaddmoreadmin', function () {    
    var buttoncontext = this;
    var Cid = $(this).attr("data-id");
    jsonObj = [];
    $("#addCompany .companyDiv").each(function () {

        item = {}
        item["ID"] = $(this).find('.ID').val();
        item["FirstName"] = $(this).find('.FirstName').val();
        item["LastName"] = $(this).find('.LastName').val();
        item["Email"] = $(this).find('.Email').val();
        item["Phone"] = $(this).find('.Phone').val();
        item["WorkPhone"] = $(this).find('.WorkPhone').val();
        item["IsActive"] = $(this).find('.IsActive').attr('data-val');
        jsonObj.push(item);
    })

    $.ajax({
        type: "POST",
        url: "/Admin/_AddCompany",
        data: JSON.stringify({ jsonObj: jsonObj, CompanyId: Cid }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#addCompany').html("");
            $('#addCompany').html(response);

            var formid = $("#frmAddProduct");
            formid.unbind();
            formid.data("validator", null);
            $.validator.unobtrusive.parse($("#frmAddProduct"));
            var curr = $('#CultureCode').val();
            var phoneFormat = getmaskingformat(curr);
            $("#CContact,#Billing_Phone,#Broker_WorkPhone,#Broker_Phone").mask(phoneFormat);
            $(".WorkPhone,.Phone").mask(phoneFormat);

        }
    });



});

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

$(document).on('click', '.empStatus', function (evt) {

    $("#LoadMain").addClass("loader");
    $("#LoadMain").css({ display: "block" })
    $.ajax({
        type: "GET",
        url: "/Admin/_CompanyList",
        data: { empid: $(this).attr('data-id'), status: $(this).attr('data-isactive'), CID: $('.cID').val() },
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $("#LoadMain").removeClass("loader");
            $("#LoadMain").css({ display: "none" });
            $('#companyList').html("");
            $('#companyList').html(response);
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
            // location.reload();
        }
    });
})

$(document).on('click', '.editCompany', function (evt) {
    var empid = $(this).attr('data-id');
    $("#LoadMain").addClass("loader");
    $("#LoadMain").css({ display: "block" });
    $.ajax({
        type: "POST",
        url: "/Admin/_EditCompanyAdmin",
        data: "{'EmpId':'" + empid + "'}",
        contentType: "application/json; charset=utf-8",
        success: function (response) {

            $("#LoadMain").removeClass("loader");
            $("#LoadMain").css({ display: "none" });
            $('#myModal').html("");
            $('#myModal').html(response);
            var curr = $('#CultureCode').val();
            var phoneFormat = getmaskingformat(curr);
            $(".eWorkPhone,.ePhone").mask(phoneFormat);
            $('#myModal').modal('show');
            var formid = $("#frmeditcompany");
            formid.unbind();
            formid.data("validator", null);
            $.validator.unobtrusive.parse($("#frmeditcompany"));
        },
        failure: function (msg) {
            alert(msg);
        }
    });

})
$(document).on('click', '.btnsaveComp', function (evt) {
    evt.preventDefault();
    var $form = $('#frmeditcompany');
    if ($form.valid()) {
        jsonObj = {}
        jsonObj["ID"] = $('.eID').val();
        jsonObj["FirstName"] = $('.eFirstName').val();
        jsonObj["LastName"] = $('.eLastName').val();
        jsonObj["Email"] = $('.eEmail').val();
        jsonObj["AdministratorType"] = $('.eAdministratorType').val();
        jsonObj["Phone"] = RemoveCommas($('.ePhone').val());
        jsonObj["WorkPhone"] = RemoveCommas($('.eWorkPhone').val());
        jsonObj["IsActive"] = $('.eIsActive').attr('data-val');
        //jsonObj.push(item);
        $.ajax({
            type: "POST",
            url: "/Admin/SaveCompanyAdmin",
            data: JSON.stringify({ jsonObj: jsonObj }),
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                if (response == true) {
                    $('#myModal').modal('hide');
                    $("#LoadMain").addClass("loader");
                    $("#LoadMain").css({ display: "block" });
                    $.ajax({
                        type: "POST",
                        url: "/Admin/SaveCompanyAdmin",
                        data: JSON.stringify({ jsonObj: jsonObj, Flag: 2 }),
                        contentType: "application/json; charset=utf-8",
                        contentType: "application/json; charset=utf-8",
                        success: function (response) {
                            $("#LoadMain").removeClass("loader");
                            $("#LoadMain").css({ display: "none" });

                            $('#companyList').html("");
                            $('#companyList').html(response);
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
                            // location.reload();
                        }
                    });
                }
                else {
                    //alert("Phone or Email already exist at " + jsonObj["Phone"]);
                    toastr['error']("Phone or Email already exist at " + jsonObj["Phone"]);
                }
            },
            failure: function (msg) {
                alert(msg);
            }
        });
    }
});

//function autocomplete() {
//    $(".FirstName,.LastName").autocomplete({
//        source: function (request, response) {
//            var companyid = $("#ID").val();
//            if (companyid != 0) {
//                $.ajax({
//                    type: "POST",
//                    url: "/Admin/SearchEmployee",
//                    data: "{'Name':'" + request.term + "','CompanyId':'" + companyid + "'}",
//                    contentType: "application/json; charset=utf-8",
//                    async: false,
//                    success: function (data) {
//                        response($.map(data, function (item) {
//                            return { label: item.Text, value: item.Text, val: item.Value };
//                        }))
//                        //response(data)
//                    },
//                    failure: function (msg) {
//                        alert(msg);
//                    }
//                });
//            }
//        },
//        select: function (event, ui) {
//            $("#hdnEmpId").val(ui.item.val);
//            var a = event.target.id.split("_")[1]
//            $("#Clients_" + a + "__FirstName").text(ui.item.label);
//            var empid = $("#hdnEmpId").val();
//            if (empid != 0 || empid != undefined) {
//                $.ajax({
//                    type: "POST",
//                    url: "/Admin/EmployeeDetail",
//                    data: "{'EmpId':'" + empid + "'}",
//                    contentType: "application/json; charset=utf-8",
//                    success: function (response) {
//                        $("#Clients_" + a + "__ID").val(response.ID)
//                        $("#Clients_" + a + "__FirstName").val(response.FirstName)
//                        $("#Clients_" + a + "__LastName").val(response.LastName)
//                        $("#Clients_" + a + "__Email").val(response.Email)
//                        $("#Clients_" + a + "__WorkPhone").val(response.WorkPhone)
//                        $("#Clients_" + a + "__Phone").val(response.Phone)
//                        //  $("#frmAddProduct").submit();
//                        var curr = $('#CultureCode').val();
//                        var phoneFormat = getmaskingformat(curr);
//                        $(".WorkPhone,.Phone").mask(phoneFormat);
//                        var curr = $("#Clients_" + a + "__Phone");
//                        var $elems = $('.phoneChk');
//                        var values = [];
//                        var isDuplicated = false;
//                        $elems.each(function () {
//                            if (!RemoveCommas(this.value)) return true;
//                            if (values.indexOf(RemoveCommas(this.value)) !== -1) {
//                                isDuplicated = true;
//                                $(curr).val("");
//                                //alert('Phone no should be different.');
//                                toastr['error']("Phone no should be different.");
//                                return false;
//                            }
//                            values.push(RemoveCommas(this.value));
//                        });
//                        if (isDuplicated1 == false) {
//                            var $elems1 = $('.emailChk');
//                            var curr1 = $("#Clients_" + a + "__Email");
//                            var values1 = [];
//                            var isDuplicated1 = false;
//                            $elems1.each(function () {
//                                if (!this.value) return true;
//                                if (values1.indexOf(this.value) !== -1) {
//                                    isDuplicated1 = true;
//                                    $(curr1).val("");
//                                    //alert('Email should be different.');
//                                    toastr['error']("Email should be different.");
//                                    return false;
//                                }
//                                values1.push(this.value);
//                            });
//                        }

//                        if (isDuplicated == true || isDuplicated1 == true) {
//                            $("#Clients_" + a + "__ID").val("0")
//                            $("#Clients_" + a + "__FirstName").val("")
//                            $("#Clients_" + a + "__LastName").val("")
//                            $("#Clients_" + a + "__Email").val("")
//                            $("#Clients_" + a + "__WorkPhone").val("")
//                            $("#Clients_" + a + "__Phone").val("")
//                            // $("#frmAddProduct").submit();
//                        }
//                    },
//                    failure: function (msg) {
//                        alert(msg);
//                    }
//                });
//            }
//            return false;
//        },
//        messages: {
//            noResults: "", results: ""
//        }

//    })
//        .data("ui-autocomplete")._renderItem = function (ul, item) {
//            var value;
//            var label;
//            if (item.label.toLowerCase() != "no record found") {
//                value = item.value;//item.label.split('__')[0];
//                label = item.label;//item.label.split('__')[1];
//                item.label = label;
//                item.value = value;
//            }
//            return $("<li data-Id='" + item.label + "'>")
//              .data("ui-autocomplete-item", item)
//              .append("<a>" + item.label + "</a>")
//              .appendTo(ul);
//        };
//}

//function autocompleteBroker() {
//    $("#hdnbrokerIdrefresh").val('');
//    $("#Broker_BrokerName").autocomplete({
//        source: function (request, response) {

//            var companyid = $("#ID").val();
//            //if (companyid != 0) {
//            $.ajax({
//                type: "POST",
//                url: "/Admin/SearchBroker",
//                //data: "{'Name':'" + request.term + "','CompanyId':'" + companyid + "'}",
//                data: "{'Name':'" + request.term + "'}",
//                contentType: "application/json; charset=utf-8",
//                async: false,
//                success: function (data) {

//                    response($.map(data, function (item) {
//                        return { label: item.Text, value: item.Text, val: item.Value };
//                    }))
//                    //response(data)
//                },
//                failure: function (msg) {
//                    alert(msg);
//                }
//            });


//            $("#Broker_BrokerName").val('');


//            // }
//        },
//        select: function (event, ui) {


//            $("#hdnbrokerId").val(ui.item.val);
//            $("#hdnbrokerIdrefresh").val(ui.item.val);

//            //var a = event.target.id.split("_")[1]
//            //$("#Clients_" + a + "__FirstName").text(ui.item.label);
//            var brokerId = $("#hdnbrokerId").val();
//            if (brokerId != 0 || brokerId != undefined) {
//                $.ajax({
//                    type: "POST",
//                    url: "/Admin/BrokerDetail",
//                    data: "{'BrokerId':'" + brokerId + "'}",
//                    contentType: "application/json; charset=utf-8",
//                    success: function (response) {

//                        $("#BrokerId").val(response.BrokerId)
//                        $("#Broker_FirstName").val(response.FirstName)
//                        $("#Broker_LastName").val(response.LastName)
//                        $("#Broker_BrokerName").val(response.BrokerName)
//                        $("#Broker_Street").val(response.Street)
//                        $("#Broker_City").val(response.City)
//                        $("#Broker_State").val(response.State)
//                        $("#Broker_Country").val(response.Country)
//                        $("#Broker_ZipCode").val(response.ZipCode)
//                        $("#Broker_Email").val(response.Email)
//                        $("#Broker_Phone").val(response.Phone)
//                        $("#Broker_WorkPhone").val(response.WorkPhone)
//                        //  $("#frmAddProduct").submit();
//                        var curr = $('#CultureCode').val();
//                        var phoneFormat = getmaskingformat(curr);
//                        $("#Broker_WorkPhone,#Broker_Phone").mask(phoneFormat);

//                        var curr = $("#Broker_Phone");
//                        var $elems = $('.phoneChk');
//                        var values = [];
//                        var isDuplicated = false;
//                        $elems.each(function () {
//                            if (!RemoveCommas(this.value)) return true;
//                            if (values.indexOf(RemoveCommas(this.value)) !== -1) {
//                                isDuplicated = true;
//                                $(curr).val("");
//                                //alert('Phone no should be different.');
//                                toastr['error']("Phone no should be different.");
//                                return false;
//                            }
//                            values.push(RemoveCommas(this.value));
//                        });
//                        if (isDuplicated1 == false) {
//                            var $elems1 = $('.emailChk');
//                            var curr1 = $("#Broker_Email");
//                            var values1 = [];
//                            var isDuplicated1 = false;
//                            $elems1.each(function () {
//                                if (!this.value) return true;
//                                if (values1.indexOf(this.value) !== -1) {
//                                    isDuplicated1 = true;
//                                    $(curr1).val("");
//                                    //alert('Email should be different.');
//                                    toastr['error']("Email should be different.");
//                                    return false;
//                                }
//                                values1.push(this.value);
//                            });
//                        }

//                        if (isDuplicated == true || isDuplicated1 == true) {

//                            $("#BrokerId").val("0")
//                            $("#Broker_FirstName").val("")
//                            $("#Broker_LastName").val("")
//                            $("#Broker_BrokerName").val("")
//                            $("#Broker_Street").val("")
//                            $("#Broker_City").val("")
//                            $("#Broker_State").val("")
//                            $("#Broker_Country").val("")
//                            $("#Broker_ZipCode").val("")
//                            $("#Broker_Email").val("")
//                            $("#Broker_Phone").val("")
//                            $("#Broker_WorkPhone").val("")
//                            // $("#frmAddProduct").submit();

//                        }
//                    },
//                    failure: function (msg) {
//                        alert(msg);
//                    }
//                });
//            }
//            return false;
//        },
//        messages: {
//            noResults: "", results: ""
//        }

//    })
//        .data("ui-autocomplete")._renderItem = function (ul, item) {
//            var value;
//            var label;
//            if (item.label.toLowerCase() != "no record found") {
//                value = item.value;//item.label.split('__')[0];
//                label = item.label;//item.label.split('__')[1];
//                item.label = label;
//                item.value = value;
//            }
//            return $("<li data-Id='" + item.label + "'>")
//              .data("ui-autocomplete-item", item)
//              .append("<a>" + item.label + "</a>")
//              .appendTo(ul);
//        };
//}

//$(document).on('keyup', '.FirstName,.LastName', function () {
//    autocomplete();
//});
//$(document).on('keyup', '#Broker_BrokerName', function () {


//    autocompleteBroker();
//    //alert($("#hdnbrokerId").val());
//});

//$(document).on('blur', '#Broker_BrokerName', function () {

//    if ($("#hdnbrokerIdrefresh").val() == "") {
//        $("#Broker_BrokerName").val('');
//    }


//    //alert($("#hdnbrokerId").val());
//});


$(document).on('click', '.radAdd', function (e) {

    if ($(this).val() == "comAdd") {
        $('#Billing_CompanyName').val($('#CCompanyName').val());
        $('#Billing_StreetName').val($('#CStreetName').val());
        $('#Billing_City').val($('#CCityName').val());
        $('#Billing_State').val($('#CStateName').val());
        $('#Billing_ZipCode').val($('#CZipCode').val());
        $('#Billing_Country').val($('#CCountryName').val());
    }
    else {
        var brokerid = $(".ddlBrokerIdchangeAddCompany").val();

        if (brokerid > 0) {
            $.ajax({
                type: "POST",
                url: "/Admin/_EditBrokerCompany",
                data: "{'Brokerid':'" + brokerid + "'}",
                contentType: "application/json; charset=utf-8",
                success: function (response) {
                 
                    $('#Billing_CompanyName').val(response.BrokerName);
                    $('#Billing_StreetName').val(response.Street);
                    $('#Billing_City').val(response.City);
                    $('#Billing_State').val(response.State);
                    $('#Billing_ZipCode').val(response.ZipCode);
                    $('#Billing_Country').val(response.Country);
                },
                failure: function (msg) {
                    alert(msg);
                }
            });

        }
    }
});
$(document).on('click', '.resetFields', function (e) {
    $('.radAdd').prop('checked', false);
    $('#Billing_CompanyName').val("");
    $('#Billing_StreetName').val("");
    $('#Billing_City').val("");
    $('#Billing_State').val("");
    $('#Billing_ZipCode').val("");
    $('#Billing_Country').val("");
    $('#Billing_Phone').val("");
    $('#Billing_Fax').val("");

})

$(document).on('blur', '.ePhone,.eWorkPhone ', function () {
    //if ($('.ePhone').val() == "" || $('.ePhone').val() == "(___) ___-____") {
    //    $('.ePhone').addClass('input-validation-error');


    //}
    if ($('.eWorkPhone').val() == "" || $('.eWorkPhone').val() == "(___) ___-____") {
        $('.eWorkPhone').addClass('input-validation-error');
    }

});
//resetFields