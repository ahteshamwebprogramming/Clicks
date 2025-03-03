function ViewEPFPartial() {
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/StatutoryComponent/StatutoryComponentsEPFView',
        //data: JSON.stringify({ ProjectId: project, ReportType: reportType }),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            $('#div_EPFView').html(data);
            $('#div_EPFView').show();
            $("#div_EPFEdit").hide();
            UnblockUI();
        },
        error: function (result) {
            alert(result);
            UnblockUI();
        }
    });
}
function ViewESIPartial() {


    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/StatutoryComponent/StatutoryComponentsESIView',
        //data: JSON.stringify({ ProjectId: project, ReportType: reportType }),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            $('#div_ESIView').html(data);
            $('#div_ESIView').show();
            $("#div_ESIEdit").hide();
            UnblockUI();
        },
        error: function (result) {
            alert(result);
            UnblockUI();
        }
    });
}
function ViewLabourWelfareFundPartial() {
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/StatutoryComponent/StatutoryComponentsLabourWelfareFundView',
        //data: JSON.stringify({ ProjectId: project, ReportType: reportType }),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            $('#div_LabourWelfareFundView').html(data);
            $('#div_LabourWelfareFundView').show();
            $("#div_LabourWelfareFundEdit").hide();
            UnblockUI();
        },
        error: function (result) {
            alert(result);
            UnblockUI();
        }
    });
}

//function ViewLabourWelfareFundPartial() {
//    $("#div_LabourWelfareFundEdit").hide();
//    $("#div_LabourWelfareFundView").show();
//}

function EditEPFPartial() {

    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/StatutoryComponent/StatutoryComponentsEPFEdit',
        //data: JSON.stringify({ ProjectId: project, ReportType: reportType }),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            $('#div_EPFEdit').html(data);
            $("#div_EPFEdit").show();
            $("#div_EPFView").hide();
            //if (jQuery("#DepartmentId").val() != "") {
            //    jQuery("#DepartmentId option[value='" + jQuery("#DepartmentId").val() + "']").prop("selected", true).change();
            //}
            //else {
            jQuery.each(jQuery("#MappingEmployeeIds").val().split(","), function (i, e) {
                jQuery("#EmployeeId option[value='" + e + "']").prop("selected", true);
            });
            //}
            UnblockUI();
        },
        error: function (result) {
            alert(result);
            UnblockUI();
        }
    });
    return;
    $("#div_EPFEdit").show();
    $("#div_EPFView").hide();
}
function EditESIPartial() {

    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/StatutoryComponent/StatutoryComponentsESIEdit',
        //data: JSON.stringify({ ProjectId: project, ReportType: reportType }),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            $('#div_ESIEdit').html(data);
            $("#div_ESIEdit").show();
            $("#div_ESIView").hide();
            UnblockUI();
        },
        error: function (result) {
            alert(result);
            UnblockUI();
        }
    });
}

function EditLabourWelfareFundPartial() {
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/StatutoryComponent/StatutoryComponentsLabourWelfareFundEdit',
        //data: JSON.stringify({ ProjectId: project, ReportType: reportType }),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            $('#div_LabourWelfareFundEdit').html(data);
            $("#div_LabourWelfareFundEdit").show();
            $("#div_LabourWelfareFundView").hide();

            initializeLabourWelfareFundEdit();

            UnblockUI();
        },
        error: function (result) {
            alert(result);
            UnblockUI();
        }
    });
    return;
    $("#div_EPFEdit").show();
    $("#div_EPFView").hide();
}

function disbaleESI() {
    BlockUI();
    var inputData = {
        "StatutoryComponentsEsiid": $('#StatutoryComponentsEsiid').val()
    };
    $.ajax({
        type: "POST",
        url: "/StatutoryComponent/DisableESIData",
        contentType: 'application/json',
        data: JSON.stringify(inputData),
        success: function (data) {
            ViewESIPartial();
            $successalert("Success!", "ESI data has been enabled!")
        },
        error: function (error) {
            $erroralert("Error!", error.responseText + '!'); UnblockUI();
        }
    });
}
function enableESI() {
    BlockUI();
    var inputData = {
        "StatutoryComponentsEsiid": $('#StatutoryComponentsEsiid').val()
    };
    $.ajax({
        type: "POST",
        url: "/StatutoryComponent/EnableESIData",
        contentType: 'application/json',
        data: JSON.stringify(inputData),
        success: function (data) {
            ViewESIPartial();
            $successalert("Success!", "ESI data has been enabled!")
        },
        error: function (error) {
            $erroralert("Error!", error.responseText + '!'); UnblockUI();
        }
    });


    //$("#epfTab-info").find("[name='btnDisableEPF']").show();
    //$("#epfTab-info").find("[name='btnEnableEPF']").hide();
}


function disbaleEPF() {
    BlockUI();
    var statutoryComponent_EPFDTO = {
        "StatutoryComponentsId": $('#StatutoryComponentsId').val()
    };
    $.ajax({
        type: "POST",
        url: "/StatutoryComponent/DisableEPFData",
        contentType: 'application/json',
        data: JSON.stringify(statutoryComponent_EPFDTO),
        success: function (data) {
            ViewEPFPartial();
            $successalert("Success!", "EPF data has been enabled!")
        },
        error: function (error) {
            $erroralert("Error!", error.responseText + '!'); UnblockUI();
        }
    });
}
function enableEPF() {
    BlockUI();
    var statutoryComponent_EPFDTO = {
        "StatutoryComponentsId": $('#StatutoryComponentsId').val()
    };
    $.ajax({
        type: "POST",
        url: "/StatutoryComponent/EnableEPFData",
        contentType: 'application/json',
        data: JSON.stringify(statutoryComponent_EPFDTO),
        success: function (data) {
            ViewEPFPartial();
            $successalert("Success!", "EPF data has been enabled!")
        },
        error: function (error) {
            $erroralert("Error!", error.responseText + '!'); UnblockUI();
        }
    });


    //$("#epfTab-info").find("[name='btnDisableEPF']").show();
    //$("#epfTab-info").find("[name='btnEnableEPF']").hide();
}
function disbaleLabourWelfareFund() {
    BlockUI();
    var statutoryComponentsLabourWelfareFundDTO = {
        "StatutoryComponentsLabourWelfareFundId": $('#StatutoryComponentsLabourWelfareFundId').val()
    };
    $.ajax({
        type: "POST",
        url: "/StatutoryComponent/DisableLabourWelfareFundData",
        contentType: 'application/json',
        data: JSON.stringify(statutoryComponentsLabourWelfareFundDTO),
        success: function (data) {
            ViewLabourWelfareFundPartial();
            $successalert("Success!", "Labour Welfare Fund has been disabled!")
        },
        error: function (error) {
            $erroralert("Error!", error.responseText + '!'); UnblockUI();
        }
    });
}
function enableLabourWelfareFund() {
    BlockUI();
    var statutoryComponentsLabourWelfareFundDTO = {
        "StatutoryComponentsLabourWelfareFundId": $('#StatutoryComponentsLabourWelfareFundId').val()
    };
    $.ajax({
        type: "POST",
        url: "/StatutoryComponent/EnableLabourWelfareFundData",
        contentType: 'application/json',
        data: JSON.stringify(statutoryComponentsLabourWelfareFundDTO),
        success: function (data) {
            ViewLabourWelfareFundPartial();
            $successalert("Success!", "Labour Welfare Fund has been enabled!")
        },
        error: function (error) {
            UnblockUI();
            $erroralert("Error!", error.responseText + '!');
        }
    });
}

function SaveEPFData() {
    var statutoryComponent_EPFDTO = {
        "StatutoryComponentsId": $('#StatutoryComponentsId').val(),
        "Epfnumber": $('#Epfnumber').val(),
        "DeductionCycle": $('#DeductionCycle').val(),
        "EmployeeContributionRate": $('#EmployeeContributionRate').val(),
        "EmployerContributionRate": $('#EmployerContributionRate').val(),
        "IsCtcinclusionEmployers": $("#IsCtcinclusionEmployers").is(":checked"),
        "IsCtcinclusionEmployersEdli": $('#IsCtcinclusionEmployersEdli').is(":checked"),
        "IsCtcinclusionAdminCharges": $('#IsCtcinclusionAdminCharges').is(":checked"),
        "IsEmployeeLevelOverride": $('#IsEmployeeLevelOverride').is(":checked"),
        "IsProrateRestrictedPfwage": $('#IsProrateRestrictedPfwage').is(":checked"),
        "IsLopbasedComponentSalary": $('#IsLopbasedComponentSalary').is(":checked"),
        "DepartmentId": $('#DepartmentId').val() == "" ? 0 : $('#DepartmentId').val(),
        "UnitId": $('#UnitId').val(),
        "MappingEmployeeIds": $('#EmployeeId').val().toString(),
        "IsAbryScheme": $('#IsAbryScheme').is(":checked"),
    };
    $.ajax({
        type: "POST",
        url: "/StatutoryComponent/SaveEPFData",
        contentType: 'application/json',
        data: JSON.stringify(statutoryComponent_EPFDTO),
        //dataType: "json",
        success: function (data) {
            ViewEPFPartial();
            Swal.fire({
                title: 'Saved Successfully!',
                text: 'EPF data has been saved!',
                icon: 'success',
                customClass: {
                    confirmButton: 'btn btn-success'
                },
                buttonsStyling: false
            });
        },
        error: function (error) {
            Swal.fire({
                title: 'Error!',
                text: error.responseText + '!',
                icon: 'error',
                customClass: {
                    confirmButton: 'btn btn-primary'
                },
                buttonsStyling: false
            });
        }
    });
}

function SaveESIData() {

    let esiData = $("#div_ESIEdit");
    let esiDataObj = $(esiData);

    var inputData = {
        "StatutoryComponentsEsiid": esiDataObj.find('[name="StatutoryComponentsEsiid"]').val(),
        "Esinumber": esiDataObj.find('[name="Esinumber"]').val(),
        "DeductionCycle": esiDataObj.find('[name="DeductionCycle"]').val(),
        "EmployeesContribution": esiDataObj.find('[name="EmployeesContribution"]').val(),
        "EmployersContribution": esiDataObj.find('[name="EmployersContribution"]').val(),
        "IsEmployersContibutionInCtc": esiDataObj.find('[name="IsEmployersContibutionInCtc"]').is(":checked"),
        "Esilimit": esiDataObj.find('[name="Esilimit"]').text(),
    };
    BlockUI();
    $.ajax({
        type: "POST",
        url: "/StatutoryComponent/SaveESIData",
        contentType: 'application/json',
        data: JSON.stringify(inputData),
        //dataType: "json",
        success: function (data) {
            ViewESIPartial();
            Swal.fire({
                title: 'Saved Successfully!',
                text: 'ESI data has been saved!',
                icon: 'success',
                customClass: {
                    confirmButton: 'btn btn-success'
                },
                buttonsStyling: false
            });
            UnblockUI();
        },
        error: function (error) {
            UnblockUI();
            Swal.fire({
                title: 'Error!',
                text: error.responseText + '!',
                icon: 'error',
                customClass: {
                    confirmButton: 'btn btn-primary'
                },
                buttonsStyling: false
            });
        }
    });
}


function SaveLabourWelfareFundData() {
    var statutoryComponentsLabourWelfareFundDTO = {
        "StatutoryComponentsLabourWelfareFundId": $('#StatutoryComponentsLabourWelfareFundId').val(),
        "EmployeesContribution": $('#div_LabourWelfareFundEdit').find("[name='EmployeesContribution']").val(),
        "EmployersContribution": $('#div_LabourWelfareFundEdit').find("[name='EmployersContribution']").val(),
        "DeductionCycle": $('#div_LabourWelfareFundEdit').find("[name='DeductionCycle']").val(),
    };
    $.ajax({
        type: "POST",
        url: "/StatutoryComponent/SaveLabourWelfareFundData",
        contentType: 'application/json',
        data: JSON.stringify(statutoryComponentsLabourWelfareFundDTO),
        //dataType: "json",
        success: function (data) {
            ViewLabourWelfareFundPartial();
            Swal.fire({
                title: 'Saved Successfully!',
                text: 'Labour Welfare Fund has been saved!',
                icon: 'success',
                customClass: {
                    confirmButton: 'btn btn-success'
                },
                buttonsStyling: false
            });
        },
        error: function (error) {
            Swal.fire({
                title: 'Error!',
                text: error.responseText + '!',
                icon: 'error',
                customClass: {
                    confirmButton: 'btn btn-primary'
                },
                buttonsStyling: false
            });
        }
    });
}

function initializeLabourWelfareFundEdit() {
    $("#DeductionCycle").change(function () {
        alert("h");
    });
}
function initializeLabourWelfareFundEditxx() {
    alert("h");
}


//Department Employee JS
jQuery("#DepartmentId").change(function () {
    var select = document.getElementById('DepartmentId');
    var selected = [...select.options]
        .filter(option => option.selected)
        .map(option => option.value);
    jQuery("#DepartmentId").val(selected);
});

jQuery("#EmployeeId").change(function () {
    var select = document.getElementById('EmployeeId');
    var selected = [...select.options]
        .filter(option => option.selected)
        .map(option => option.value);
    // alert(selected);
    jQuery("#EmployeeId").val(selected);
});

function GetEmployeeOfDepartment(defaultServerValue) {
    //alert("@Model");
    var formParam;
    var unitId = jQuery("#UnitId").val();
    var departmentId = jQuery("#DepartmentId").val();
    if (departmentId == "")
        departmentId = 0;
    formParam = unitId + "&" + departmentId + "&true";

    var url = '/EmployeeAttendanceUI/EmployeeKeyValue/' + formParam;
    alert(url);
    jQuery("#EmployeeId").html("")

    BlockUI();
    jQuery.ajax({
        type: "GET",
        url: url,
        //data: formParam,
        success: function (response) {
            UnblockUI();
            PopulateDropDown('EmployeeId', response, defaultServerValue, 'employeeId', 'employeeName', 'All Employees')
            jQuery.each(jQuery("#MappingEmployeeIds").val().split(","), function (i, e) {
                jQuery("#EmployeeId option[value='" + e + "']").prop("selected", true);
            });


        },
        failure: function (response) {
            UnblockUI();
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });

}

function ShowEmployees(id) {
    var url = "/StatutoryComponent/GetEPFEmployees/" + id
    BlockUI();
    jQuery.ajax({
        type: "GET",
        url: url,
        //data: { id: formVM },
        //cache: false,
        //contentType: "application/x-www-form-urlencoded; charset=utf-8",
        success: function (data) {
            UnblockUI();
            if (data.displayMessage.toUpperCase() == "SUCCESS") {
                PopulateSalaryTemplateDetailTable(data.epfemployeeMappingList, "ListDetailTable");
                jQuery("#showEmployee").modal('show');
            }
            else {
                $erroralert("Error", data.displayMessage)
                //ShowServerMessage(data.displayMessage);
            }
        },
        error: function (result) {
            UnblockUI();
            var x = 1;
            jQuery("#FormName").val("");
        }
    });
}

function PopulateSalaryTemplateDetailTable(data, tableID) {
    var tableCtrl = jQuery("#" + tableID);
    jQuery("#" + tableID + " tr").slice(1).remove();
    data.forEach(function (row) {
        var tr = jQuery('<tr>');
        tr.append('<td><span>' + row["employeeName"] + '</span></td>');
        tableCtrl.append(tr);
    });

}


function SubmitTaxDetails() {
   
    //$.ajax({
    //    type: "POST",
    //    url: '/StatutoryComponent/SaveComponentsTaxLimit',
    //    //  contentType: 'application/json',
    //    data: { GratuityLimit: $("#GratuityLimit").val(), LeaveEncashmentLimit: $("#LeaveEncashmentLimit").val(), PFLimit: $("#PFLimit").val() },
    //    success: function (data) {
    //        alert(data);
    //        error: function (error) {
    //            $erroralert("Error!", error.responseText + '!'); UnblockUI();
    //            reject(error);
    //        }
    //    }
    //});

}