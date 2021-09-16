
$(document).ready(function () {
    //var curr = $('#CultureCode').val();  
    setNavigation();
    $('#tblBrokerCompany').DataTable({
        columns: [
        { title: "Company Name" },
        { title: "Employee" },
        { title: "OE" },
        { title: "LE" },
         { title: "Status" },
        { title: "Action", "orderable": false },
        ]

    });
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
//$('#select_all').click(function () {
$(document).on('click', '#select_all', function (evt) {
    $('input:checkbox').prop('checked', this.checked);
});
function SendMailonCompanyEmployees() {
   
    var CompanyID = $('#CompanyID').val();
    var selectedval = "";
    var arr = $('input[name="empChk"]:checked');
    arr.each(function () {
        if (selectedval == "") {
            selectedval = $(this).val();
        }
        else {
            selectedval = selectedval + "," + $(this).val();
        }

    });
    $("#LoadMain").addClass("loader");
    $("#LoadMain").css({ display: "block" });
    if (selectedval != "") {
        $.ajax({
            type: 'GET',
            contentType: false,
            url: "/Broker/SendMailonCompanyEmployees",
            data: { empID: selectedval },
            contentType: "application/json; charset=utf-8",
            success: function (data) {            
                if (data=="Success")
                {
                    $("#LoadMain").removeClass("loader");
                    $("#LoadMain").css({ display: "none" });
                    toastr["success"]("Mail sent successfully.", "Success !");
                }
                else
                {
                    $("#LoadMain").removeClass("loader");
                    $("#LoadMain").css({ display: "none" });
                    toastr["error"]("Some error occured during sending email.", "Error !");
                }
              
            }
        });
    }
    else {
        $("#LoadMain").removeClass("loader");
        $("#LoadMain").css({ display: "none" });
        toastr["error"]("Please select atleast one employee", "Error !");
    }

}
function setNavigation() { 
    var path = window.location.pathname;
    path = path.replace(/\/$/, "");
    path = decodeURIComponent(path);
    $(".treeview  a").each(function () {
        var href = $(this).attr('href');
        if (path.substring(0, href.length) === href) {
            $(this).closest('li').addClass('active');
            $(this).closest('.treeview').addClass('active');
        }
    });
}
function ChangePassword(e) {   
    var isValid = true;
    var currentPassword = $('#OldPswd').val();
    var newPassword = $('#NewPswd').val();
    var confirmPassword = $('#ConfrmPswd').val();
    if (currentPassword == "" || newPassword == "" || confirmPassword == "") {
        $('#OldPswd,#NewPswd,#ConfrmPswd').each(function (e) {
            if ($.trim($(this).val()) == '') {
                isValid = false;
                $(this).removeClass('custom-input-success').addClass('custom-input-error');
                $(this).keypress(function () {

                    $(this).removeClass('custom-input-error').addClass('custom-input-success');
                });
            }

        });
    }
    else {

        if (currentPassword != "") {

            var a = checkStrength($("#NewPswd").val());
            if (a == 'Weak' || a == 'Too short' || a == 'Good' || a == 'Please provide a password') {
                isValid = false
                if (a == 'Please provide a password') {
                    $('#result').text('');
                }
                else {
                    $('#result').text(a);
                }

            }
            else {
                isValid = true
                $('#result').text(a);
            }

            if (newPassword.length < 8) {

                isValid = false;
                $("#NewPswd").removeClass('custom-input-success').addClass('custom-input-error');
                $("#NewPswd").keyup(function () {
                    if ($.trim($("#NewPswd").val()).length >= 8 && $.trim($("#NewPswd").val()).length <= 20) {
                        $("#NewPswd").removeClass('custom-input-error').addClass('custom-input-success');
                        isValid = true;
                    }
                    else {
                        $("#NewPswd").addClass('custom-input-error');
                        isValid = false;
                    }
                    isValid = ($(newPassword).val().length > 8 && $(newPassword).val().length < 20 ? true : false);
                });
            }
            else if (newPassword != confirmPassword) {
                isValid = false;
                $("#ConfrmPswd").removeClass('custom-input-success').addClass('custom-input-error');
                $('#PswdNotMatch').text("Password did not match");
                $('#PswdNotMatch').css('color', 'red');

                $("#ConfrmPswd").keyup(function () {
                    if ($.trim($(this).val()).length >= 8 && $.trim($(this).val()).length <= 20) {

                        $("#ConfrmPswd").removeClass('custom-input-error').addClass('custom-input-success');

                        isValid = true;
                    }
                    else {
                        $("#ConfrmPswd").removeClass('custom-input-success').addClass('custom-input-error');

                        isValid = false;
                    }
                });
            }
            //else {
            //    isValid = true;
            //}
        }



        if (isValid == false)
            e.preventDefault();

        else {
            if (newPassword == confirmPassword) {

                $.ajax({
                    // url: '@Url.Action("ChangeAdminPassword", "Broker")',
                    url: '/Broker/ChangeAdminPassword',
                    type: "POST",
                    data: JSON.stringify({ OldPassword: currentPassword, NewPassword: newPassword }),
                    async: false,
                    contentType: "application/json",
                    dataType: "json",
                    success: function (result) {

                        if (result > 0) {
                            toastr["success"]("Password successfully changed", "Success !");
                            $('#OldPswd').val('');
                            $("#NewPswd").val('');
                            $("#ConfrmPswd").val('');
                            $('#result').text('');
                            $('#PswdNotMatch').text('');

                        }
                        if (result == 0) {
                            toastr["error"]("New and old password are same", "Error !");
                        }
                        if (result == -1) {
                            toastr["error"]("Incorrect old password.", "Error !");
                        }

                    },
                    error: function (result) {

                    }
                });
            }

        }

    }
}
