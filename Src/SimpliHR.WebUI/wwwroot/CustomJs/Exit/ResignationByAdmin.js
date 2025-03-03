function GetEmployeeDetails() {
    BlockUI();
    // if (!isValidateForm()) {
    //     UnblockUI();
    //     return;
    // }
    let $empForm = $("#EmployeeExitDetail");
    let inputDTO = {};
    inputDTO.EmployeeId = $empForm.find("[name='EmployeeId']").val();
    BlockUI();
    $.ajax({
        type: "POST",
        url: "/ExitManagement/EmployeeDetails_InitiateResignationAdmin",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();

            if (data.action == "Edit") {
                $erroralert("Transaction Failed!", 'The Resignation is already in progress!');
            }
            else {
                $empForm.find("[name='NoticePeriod']").val(data.resignationDetails.noticePeriod);
                // Convert LastWorkingDate using moment.js
                let lastWorkingDate = moment(data.resignationDetails.lastWorkingDate).format('DD-MMM-YYYY');
                $empForm.find("[name='LastWorkingDate']").val(lastWorkingDate);
                $empForm.find("[name='LastWorkingDateAdmin']").val(lastWorkingDate);

                // Convert ResignationDate using moment.js
                let resignationDate = moment(data.resignationDetails.resignationDate).format('DD-MMM-YYYY');
                //$empForm.find("[name='ResignationDate']").val(resignationDate);
                $empForm.find("[name='ResignationDateAdmin']").val(resignationDate);

                initialLastWorkingDate = $("#LastWorkingDateAdmin").val();
                initialResignationDate = $("#ResignationDateAdmin").val();

                initDates();

            }
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
            UnblockUI();
        }
    });
}

function SaveResignationDetails() {
    ////alert("ok")
    //var rowData = {};
    //var exitVM = {};
    //var isData = false;
    //var dataCollection = new Array();
    //exitVM.ResignationDetails = GetFormControls();
    //exitVM.ResignationDetails.EmployeeId = jQuery("#EmployeeId").val()
    //var url = '/ExitManagement/SaveEmployeeResignationDetailsByAdmin'
    //if (!ValidForm("EmployeeExitDetail")) {
    //    //alert("Please fill personal information and try again");
    //    return false;
    //}


    //let inputDTO = {};
    //inputDTO["EmployeeId"] = $("#EmployeeExitDetail").find("[name='EmployeeId']").val();
    //inputDTO["NoticePeriod"] = $("#EmployeeExitDetail").find("[name='NoticePeriod']").val();
    //inputDTO["ResignationDate"] = $("#EmployeeExitDetail").find("[name='ResignationDate']").val();
    //inputDTO["LastWorkingDate"] = $("#EmployeeExitDetail").find("[name='LastWorkingDate']").val();
    //inputDTO["ReasonForLeavingAdmin"] = $("#EmployeeExitDetail").find("[name='ReasonForLeaving']").val();
    //inputDTO["AdminRemarks"] = $("#EmployeeExitDetail").find("[name='EmployeeComments']").val();

    //inputDTO.append("NoticePeriodWaiveOffAdmin", data.find("[name='NoticePeriodWaiveOffAdmin']").is(':checked') ? true : false);
    //inputDTO.append("EligibleToHireAdmin", data.find("[name='EligibleToHireAdmin']").is(':checked') ? true : false);
    //inputDTO.append("ActivateExitInterview", data.find("[name='ActivateExitInterview']").is(':checked') ? true : false);
    //inputDTO.append("ClearanceByPass", data.find("[name='ClearanceByPass']").is(':checked') ? true : false);
    //inputDTO.append("LWDPolicy", data.find("[name='LWDPolicy']").val());
    let data = $("#EmployeeExitDetail");
    let inputDTO = new FormData();
    inputDTO.append("EmployeeId", data.find("[name='EmployeeId']").val());
    inputDTO.append("NoticePeriod", data.find("[name='NoticePeriod']").val());
    inputDTO.append("ResignationDateAdmin", moment(data.find("[name='ResignationDateAdmin']").val()).format('YYYY-MM-DD'));
    inputDTO.append("LastWorkingDateAdmin", moment(data.find("[name='LastWorkingDateAdmin']").val()).format('YYYY-MM-DD'));
    inputDTO.append("NoticePeriodWaiveOffAdmin", data.find("[name='NoticePeriodWaiveOffAdmin']").is(':checked') ? true : false);
    inputDTO.append("EligibleToHireAdmin", data.find("[name='EligibleToHireAdmin']").is(':checked') ? true : false);
    inputDTO.append("ActivateExitInterview", data.find("[name='ActivateExitInterview']").is(':checked') ? true : false);
    inputDTO.append("ClearanceByPass", data.find("[name='ClearanceByPass']").is(':checked') ? true : false);
    inputDTO.append("LWDPolicy", data.find("[name='LastWorkingDate']").val());
    inputDTO.append("AdminRemarks", data.find("[name='AdminRemarks']").val());
    inputDTO.append("DocumentFileAdmin", $("#DocumentFileAdmin")[0].files[0]);
    inputDTO.append("ReasonForLeavingAdmin", data.find("[name='ReasonForLeaving']").val());
    if (!ValidForm("EmployeeExitDetail")) {
        return false;
    }


    BlockUI();


    jQuery.ajax({
        type: "POST",
        url: '/ExitManagement/SaveEmployeeResignationDetailsByAdmin',
        data: inputDTO,
        cache: false,
        processData: false,
        contentType: false,
        success: function (data) {
            UnblockUI();
            Swal.fire({
                title: 'Transaction Successful!',
                text: 'Resignation Details Saved',
                icon: 'success',
                confirmButtonText: 'OK'
            }).then((result) => {
                if (result.isConfirmed) {
                    window.location.href = "/ExitManagement/ExitA";
                }
            });
        },
        error: function (result) {
            UnblockUI();
            var x = 1;
            jQuery("#FormName").val("");
        }
    });

}

function ResetValidationErrors() {

    $('input[required]').on('input change paste keyup', function () {

        //if ($(this).prop('type') == "email" && $(this).val().trim() !== '') {
        //    let emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        //    if (!emailRegex.test($(this).val())) {
        //        // Add error class and insert error label after the input field
        //        $(this).addClass("error");
        //        $(this).after('<label class="error">Please enter a valid email address</label>');
        //    } else {
        //        // Remove error label if email is valid
        //        $(this).next('label.error').remove();
        //    }
        //}

        if ($(this).val().trim() !== '' && $(this).hasClass("error")) {
            $(this).removeClass("error");
            $(this).next('label.error').remove();
        }
        if ($(this).val().trim() === '') {
            if ($(this).prop('required') && !$(this).hasClass("error")) {
                $(this).addClass("error");
                $(this).after('<label class="error">This field is required</label>');
            }
        } else {
            $(this).next('label.error').remove();
        }
    });

    $('select[required]').on('change', function () {
        // Remove existing error class and error label
        $(this).removeClass("error");
        $(this).parent().find('label.error').remove();
    });

}
