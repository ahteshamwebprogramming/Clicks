var sEditFieldNames = '';
var croppedImageBase64;
var GridForm = {};
let pageName = "";

document.addEventListener('invalid', (function () {
    return function (e) {
        e.preventDefault();
        document.getElementById("name").focus();
    };
})(), true);

$(document).ready(function () {
    //$(".List").DataTable();



    //$("#academicTable").DataTable();

    ResetValidationErrors();

    $("#employeepassportdetailsForm").find("[name='PassportIssueDate']").flatpickr({
        dateFormat: "d-M-Y",
        maxDate: new Date()
    });

    $("#employeepassportdetailsForm").find("[name='PassportIssueDate']").change(function () {
        let PassportIssueDate = moment($(this).val(), 'DD-MMM-YYYY');
        let d = new Date(PassportIssueDate);
        $("#employeepassportdetailsForm").find("[name='PassportValidTillDate']").flatpickr({
            dateFormat: "d-M-Y",
            minDate: d
        });
    });
    $("#employeepassportdetailsForm").find("[name='PassportValidTillDate']").change(function () {
        let PassportIssueDate = moment($(this).val(), 'DD-MMM-YYYY');
        let d = new Date(PassportIssueDate);
        $("#employeepassportdetailsForm").find("[name='PassportIssueDate']").flatpickr({
            dateFormat: "d-M-Y",
            maxDate: d
        });
    });


    jQuery(".form-control").each(function (key) {
        var last_value = "";
        var current_value = "";
        jQuery(this).focus(function () {
            last_value = $(this).val() == undefined ? "" : $(this).val();
        }).change(function () {
            current_value = $(this).val() == undefined ? "" : $(this).val();
            if (last_value.toLowerCase() != current_value.toLowerCase()) {
                sEditFieldNames = (',' + sEditFieldNames).indexOf(this.id) == -1 ? sEditFieldNames += this.id + ',' : sEditFieldNames
            }
        })
    })

    $("#Relationship").change(function () {
        let legalAgeDate = new Date();
        legalAgeDate.setFullYear(legalAgeDate.getFullYear() - 18);
        let relation = $(this).val();
        if (relation == "Father" || relation == "Mother" || relation == "Wife") {
            $("#MemberDob").flatpickr({
                dateFormat: "d-M-Y",
                maxDate: legalAgeDate
            });
        }
        else if (relation == "Child") {
            $("#MemberDob").flatpickr({
                dateFormat: "d-M-Y",
                maxDate: new Date()
            });
        }
    });

})

var Opt;
function SetOpt(sOpt) {
    //alert(sOpt)
    Opt = sOpt;
}

function SetFormName() {
    var formCollection = {}
    formCollection["employeepersonalinfoForm"] = "Personal Information";
    formCollection["employeejobForm"] = "Job Information";
    formCollection["employeecontactdetailForm"] = "Contact Detail";
    formCollection["employeeBankForm"] = "Bank Detail"
    formCollection["employeepassportdetailsForm"] = "Passport Detail"
    return formCollection;
}

function ValidateEmployee(formID) {

    var isValidEmployeeData = true;

    if (formID == "finalSubmit") {
        var forms;
        var gridForm;
        if (pageName == "Employee") {
            forms = "employeepersonalinfoForm,employeejobForm,employeecontactdetailForm,employeeBankForm,employeepassportdetailsForm"
        }
        else if (pageName == "EJoining") {
            forms = "employeepersonalinfoForm,employeecontactdetailForm,employeeBankForm,employeepassportdetailsForm"
        }
        formName = SetFormName();
        var arrForms = forms.split(",");
        $.each(arrForms, function (i) {
            if (!ValidForm(arrForms[i])) {
                $erroralert("Transaction Failed!", "Required mandatory fields are missing in " + formName[arrForms[i]] + ". Please review all the sections to fill up and submit again.");
                //isSubmitForm = false;
                isValidEmployeeData = false;
                return false;
            }
        });
        //Check for grid having mandatory field
        //GridForm.FamilyGridCount = "@Model.EmployeeMaster.EmployeeFamilyDetails.Count";
        //GridForm.AcademicGridCount = "@Model.EmployeeMaster.EmployeeAcademicDetails.Count";
        //GridForm.ExperienceGridCount = "@Model.EmployeeMaster.EmployeeExperienceDetails.Count";
        //GridForm.CertificationGridCount = "@Model.EmployeeMaster.EmployeeCertificationDetails.Count";
        //GridForm.ReferenceGridCount = "@Model.EmployeeMaster.EmployeeReferenceDetails.Count";
        //GridForm.LanguageGridCount = "@Model.EmployeeMaster.EmployeeLanguageDetails.Count";
        //gridForm = "employeefamilyForm,employeeacdemicForm,employeeexperienceForm,employeecertificationForm,employeereferenceForm,employeelanguagesForm"
        var sGridError = "";
        if (!ValidForm("employeefamilyForm") && GridForm.FamilyGridCount == 0)
            sGridError = "Family<br>"
        if (!ValidForm("employeeacdemicForm") && GridForm.AcademicGridCount == 0)
            sGridError = sGridError == "" ? "Academic<br>" : sGridError + "Academic<br>"
        if (!ValidForm("employeeexperienceForm") && GridForm.ExperienceGridCount == 0)
            sGridError = sGridError == "" ? "Experience<br>" : sGridError + "Experience<br>"
        if (!ValidForm("employeecertificationForm") && GridForm.CertificationGridCount == 0)
            sGridError = sGridError == "" ? "Certification<br>" : sGridError + "Certification<br>"
        if (!ValidForm("employeereferenceForm") && GridForm.ReferenceGridCount == 0)
            sGridError = sGridError == "" ? "Reference<br>" : sGridError + "Reference<br>"
        if (!ValidForm("employeelanguagesForm") && GridForm.LanguageGridCount == 0)
            sGridError = sGridError == "" ? "Languages<br>" : sGridError + "Languages<br>"
        if (sGridError != "") {
            $htmlalert("Error", "Transaction Failed!", "Fill the following data to save employee information " + sGridError + ". Please review all the sections to fill up and submit again.");
            return false;
        }
    }

    //if (formID == "employeefamilyForm" || formID == "employeeacdemicForm" || formID == "employeeexperienceForm" || formID == "employeecertificationForm" || formID == "employeereferenceForm" || formID == "employeelanguagesForm") {
    //forms += "," + formID;
    if ((!ValidForm(formID)) && isValidEmployeeData) {
        isValidEmployeeData = false;
    }
    return isValidEmployeeData
    //}
}


function ValidateAndSaveEmployee(formID, url, sTabName) {

    if (pageName == "EJoining" && formID == "finalSubmit") {

        if ($("#chkDeclare").prop('checked') == true) {

            if (jQuery("#entered-captcha").val() != '') {

                if (jQuery("#entered-captcha").val() != jQuery("#generated-captcha").val()) {
                    $erroralert("Transaction Failed!", "Invalid captcha, try again!!");
                    return false;
                }
            }
            else {
                $erroralert("Transaction Failed!", "Enter the captcha..");
                return false;
            }

        }
        else {
            $erroralert("Transaction Failed!", "Please accept the declaration checkbox");
            return false;
        }
    }


    var validParam = {};
    var referenceId;
    referenceId = GetFormRefrenceId(formID);
    validParam.sTabName = sTabName;
    validParam.sOpt = Opt;
    validParam.sEditFieldNames = sEditFieldNames;
    validParam.employeeId = jQuery("#EmployeeId").val();
    validParam.refrenceId = referenceId;
    validParam.formId = formID;
    validParam.pageName = pageName;
    if (formID.toLowerCase() == "employeecontactdetailform")
        validParam.prefrenceId = GetFormRefrenceId("pemployeecontactdetailform");
    //if (pageName == "EJoining") {
    //    validParam.employeeId = = jQuery("#EmployeeId").val();

    //}
    //else if (pageName == "Employee" || pageName == "EditEmployee") {
    //    validParam.employeeId = jQuery("#EmployeeId").val();
    //}

    if (formID.toUpperCase() != "EMPLOYEEPERSONALINFOFORM" && jQuery("#EmployeeId").val() == 0) {
        $erroralert("Transaction Failed!", "Fill the required data in Personal Information Tab");
        return false;
    }


    var isValidEmployeeData = ValidateEmployee(formID);
    //alert(isValidEmployeeData);
    //return false;
    if (!isValidEmployeeData)
        return false;

    jQuery.ajax({
        type: "POST",
        url: "/EmployeeUploadDoc/ValidateEmployeeAttachments",
        data: validParam,
        success: function (response) {


            if (response.displayMessage.toUpperCase() == "SUCCESS") {
                sEditFieldNames = "";
                SaveEmployeeInfo(formID, url);
            }
            else
                //$erroralert("Transaction Failed!", "Attachment Required for <br>" + response.displayMessage);
                $htmlalert('error', 'Transaction Failed!', "Attachment Required for <br>" + response.displayMessage);

        },
        failure: function (response) {
            $erroralert("Error!", response.responseText + '!');
        },
        error: function (response) {
            $erroralert("Error!", response.responseText + '!');
        }
    });

}

function SetTabColour(colour, tabId) {

    if (colour == 'green') {
        jQuery("#" + tabId).removeClass("bs-stepper-circle");
        jQuery("#" + tabId).addClass("bs-stepper-circle-green")
    }
    else {
        jQuery("#" + tabId).removeClass("bs-stepper-circle-green");
        jQuery("#" + tabId).addClass("bs-stepper-circle")
    }
}

function SaveEmployeeInfo(formID, url) {
    let isSubmitForm = true;
    jQuery("#FormName").val(formID);

    if (pageName == "EditEmployee") {
        SetReferenceId(formID)
        if (!(jQuery("#TableReferenceId").val() == "" || jQuery("#TableReferenceId").val() == 0)) {
            SaveEmployeeEditInfo();
            return false;
        }
    }
    else {
        var isValidEmployeeData = ValidateEmployee(formID);
        if (!isValidEmployeeData)
            return false;
    }

    jQuery("#errorDiv").show();
    jQuery("#errorPara").html("");
    var moveNextForm = false;
    var keyAttr = tableID = "";
    var tableCols = [];
    var data = [];

    // var formData = jQuery("#employeepersonalinfoForm").serialize(); ;
    // if (formID == "employeeReferenceForm") {
    jQuery("#ReferenceOf").val(jQuery("#EmployeeId").val());
    //}
    if (formID == "employeecontactdetailForm" && jQuery("#sameAsCurrent").prop("checked") == true) {
        if (jQuery("#PermanentStateId").val() == "0") {
            jQuery("#PermanentStateId").val(jQuery("#StateId").val())
        }
        if (jQuery("#PermanentCityId").val() == "0") {
            jQuery("#PermanentCityId").val(jQuery("#CityId").val())
        }
    }

    if (formID == "employeecontactdetailForm") {
        formData = "employeeContactDataString=" + JSON.stringify(GetContactData());
    }
    else if (formID == "employeepassportdetailsForm" || formID == "employeejobForm" || formID == "employeepersonalinfoForm" || formID == "finalSubmit") {
        formData = jQuery("#employeepersonalinfoForm").serialize() + "&EmployeeId=" + jQuery("#EmployeeId").val() + "&FormName=" + jQuery("#FormName").val();
        formData = formData + "&" + jQuery("#employeejobForm").serialize()
        formData = formData + "&" + jQuery("#employeepassportdetailsForm").serialize()
        var infoFillingStatus = jQuery("#InfoFillingStatus").val();
        formData = formData + "&InfoFillingStatus=" + infoFillingStatus
        //formData = (formID == "finalSubmit" ? formData + "&InfoFillingStatus=" + infoFillingStatus : formData + "&InfoFillingStatus=" + infoFillingStatus);
    }
    else if (formID == "employeeuploaddocForm") {
        var formData = GetEmployeeDocsData();
    }
    else if (formID == "employeelanguagesForm") {
        let read = $("#employeelanguagesForm").find("[name='Read']").is(':checked');// == true : false;
        let write = $("#employeelanguagesForm").find("[name='Write']").is(':checked');// == true : false;
        let speak = $("#employeelanguagesForm").find("[name='Speak']").is(':checked');// == true : false;
        let languageId = $("#employeelanguagesForm").find("[name='LanguageId']").val();
        let EmployeeLanguageDetailId = $("#employeelanguagesForm").find("[name='EmployeeLanguageDetailId']").val();
        EmployeeLanguageDetailId = EmployeeLanguageDetailId == null ? 0 : EmployeeLanguageDetailId;
        EmployeeLanguageDetailId = EmployeeLanguageDetailId == "" ? 0 : EmployeeLanguageDetailId;
        //formData = jQuery("#" + formID).serialize() + "  &EmployeeId=" + jQuery("#EmployeeId").val() + "&FormName=" + jQuery("#FormName").val();;
        formData = "LanguageId=" + languageId + "&Read=" + read + "&Write=" + write + "&Speak=" + speak + "&EmployeeLanguageDetailId=" + EmployeeLanguageDetailId + "&EmployeeId=" + jQuery("#EmployeeId").val() + "&FormName=" + jQuery("#FormName").val();
    }

    else
        formData = jQuery("#" + formID).serialize() + "&EmployeeId=" + jQuery("#EmployeeId").val() + "&FormName=" + jQuery("#FormName").val();
    var x = 1
    //jQuery.blockUI();
    BlockUI();
    jQuery.ajax({
        type: "POST",
        //url: "/Employee/SaveEmployeePersonalInfo",
        url: url,
        data: formData,
        cache: false,
        contentType: "application/x-www-form-urlencoded; charset=utf-8",
        success: function (receivedData) {
            UnblockUI();
            if (formID == "finalSubmit") {

                if (receivedData.displayMessage.trim().toUpperCase() == "SUCCESS") {
                    $successalert("", "Transaction Successful!");
                    setTimeout(function () {
                        if (pageName == "EJoining") {
                            window.location.href = '/Account/Index'
                        }
                        else if (pageName == "Employee" || pageName == "EditEmployee") {
                            window.location.href = '/Employee/Employees'
                        }

                    }, 2000);
                }
                else if (receivedData.displayMessage.trim().toUpperCase() != "_BLANK") {
                    //jQuery("#errorDiv").show();
                    //jQuery("#errorPara").html(receivedData.displayMessage.trim().replace("\r\n", "</br>"));
                    $erroralert("Transaction Failed!", receivedData.displayMessage.trim().replace("\r\n", "</br>"));
                    moveNextForm = false;
                }
                //OpenWindows('/Employee/Employee');
            }
            else if (formID == "employeepersonalinfoForm") {
                if (receivedData.displayMessage.trim().toUpperCase() == "SUCCESS") {
                    SetTabColour("green", "PersonalInformation")
                }
                else
                    SetTabColour("default", "PersonalInformation")

                jQuery("#EmployeeId").val(receivedData.employeeId);
                jQuery("#EmployeeCode").val(receivedData.employeeCode);
                jQuery("#EncEmployeeId").val(receivedData.enycEmployeeId);
                moveNextForm = true;
                MoveNext(formID);
                return false;
            }
            else if (formID == "employeecontactdetailForm") {
                SetTabColour("default", "ContactDetails")
                if (receivedData.length > 0 && receivedData[0].displayMessage.toUpperCase() == "SUCCESS") {
                    jQuery("#TicketId").val(receivedData[0].ticketId);
                    var cAddress = $.grep(receivedData, function (v) {
                        return v.contactType.toUpperCase() === "CURRENT";
                    });
                    if (cAddress) {
                        jQuery("#EmployeeContactDetailId").val(cAddress[0].employeeContactDetailId);
                    }
                    var pAddress = $.grep(receivedData, function (v) {
                        return v.contactType.toUpperCase() === "PERMANENT";
                    });
                    if (pAddress) {
                        jQuery("#pEmployeeContactDetailId").val(pAddress[0].employeeContactDetailId);
                    }

                    SetTabColour("green", "ContactDetails")

                    moveNextForm = true;
                    MoveNext(formID)
                }
                else if (receivedData[0].displayMessage.toUpperCase() != "_BLANK") {
                    jQuery("#errorDiv").show();
                    //jQuery("#errorPara").html(receivedData.displayMessage.replace("\r\n", "</br>"));
                    $erroralert("Transaction Failed!", "Error in saving contact");
                    //jQuery("#errorPara").html("Error in saving contact");
                }
            }
            else if (formID == "employeeBankForm") {
                SetTabColour("default", "BankDetails")
                if (receivedData.displayMessage.toUpperCase() == "SUCCESS") {
                    jQuery("#BankDetailId").val(receivedData.bankDetailId);
                    SetTabColour("green", "BankDetails")
                    moveNextForm = true;
                    MoveNext(formID)
                }
                else if (receivedData.displayMessage.toUpperCase() != "_BLANK") {
                    jQuery("#errorDiv").show();
                    //jQuery("#errorPara").html(receivedData.displayMessage.replace("\r\n", "</br>"));
                    $erroralert("Transaction Failed!", receivedData.displayMessage.replace("\r\n", "</br>"));
                }
                return;
            }
            else if (formID == "employeejobForm" || formID == "employeepassportdetailsForm") {
                if (receivedData.toUpperCase() == "SUCCESS") {
                    if (formID == "employeejobForm")
                        SetTabColour("green", "JobInformation")
                    else if (formID == "employeepassportdetailsForm")
                        SetTabColour("green", "PassportDetails")
                    moveNextForm = true;
                    MoveNext(formID)
                }
                else if (receivedData.toUpperCase() != "_BLANK") {
                    jQuery("#errorDiv").show();
                    //jQuery("#errorPara").html(receivedData.replace("\r\n", "</br>"));
                    $erroralert("Transaction Failed!", receivedData.displayMessage.replace("\r\n", "</br>"));
                }
                if (receivedData.toUpperCase() != "SUCCESS")
                    if (formID == "employeejobForm")
                        SetTabColour("default", "JobInformation")
                    else if (formID == "employeepassportdetailsForm")
                        SetTabColour("default", "PassportDetails")


            }
            else if (formID == "employeefamilyForm") {
                if ((receivedData.displayMessage.toUpperCase() == "SUCCESS"))
                    SetTabColour("green", "FamilyDetails");
                else
                    SetTabColour("default", "FamilyDetails");

                data = receivedData.employeeFamilyDetails;
                GridForm.FamilyGridCount = data.length;
                tableID = "familyTable";
                keyAttr = "employeeFamilyDetailId"
                tableCols = ['memberName', 'memberDob,date', 'relationship', 'Action']
                moveNextForm = true;
            }
            else if (formID == "employeeacdemicForm") {
                if ((receivedData.displayMessage.toUpperCase() == "SUCCESS"))
                    SetTabColour("green", "AcademicDetails");
                else
                    SetTabColour("default", "AcademicDetails");
                data = receivedData.employeeAcademicDetails;
                GridForm.AcademicGridCount = data.length;

                tableID = "academicTable";
                keyAttr = "academicDetailId"
                tableCols = ['instituteName', 'academicName,data-selectedid,academicId', 'passingYear', 'percentage', 'Action']
                moveNextForm = true;
            }
            else if (formID == "employeeexperienceForm") {
                if ((receivedData.displayMessage.toUpperCase() == "SUCCESS"))
                    SetTabColour("green", "ExperiencesBackgroud");
                else
                    SetTabColour("default", "ExperiencesBackgroud");
                data = receivedData.employeeExperienceDetails;
                GridForm.ExperienceGridCount = data.length;

                tableID = "experienceTable";
                keyAttr = "experienceDetailId"
                tableCols = ['companyName', 'jobTitle,data-selectedid,experienceJobTitleId', 'joinDate,date', 'lastWorkingDate,date', 'Action']
                moveNextForm = true;
            }
            else if (formID == "employeecertificationForm") {
                if ((receivedData.displayMessage.toUpperCase() == "SUCCESS"))
                    SetTabColour("green", "CertificationDetails");
                else
                    SetTabColour("default", "CertificationDetails");

                data = receivedData.employeeCertificationDetails;
                GridForm.CertificationGridCount = data.length;

                tableID = "certificationTable";
                keyAttr = "certificationDetailId"
                tableCols = ['certificationName', 'yearOfCertification', 'duration', 'Action']
                moveNextForm = true;
            }
            else if (formID == "employeereferenceForm") {
                if ((receivedData.displayMessage.toUpperCase() == "SUCCESS"))
                    SetTabColour("green", "ReferenceDetails");
                else
                    SetTabColour("default", "ReferenceDetails");

                data = receivedData.employeeReferenceDetails;
                GridForm.ReferenceGridCount = data.length;

                tableID = "referenceTable";
                keyAttr = "employeeReferenceId"
                tableCols = ['personName', 'presentCompany', 'referenceDesignation', 'referenceContactNo', 'referenceMobileNo', 'howYouKnow', 'Action']
                moveNextForm = true;
            }
            else if (formID == "employeelanguagesForm") {
                if ((receivedData.displayMessage.toUpperCase() == "SUCCESS"))
                    SetTabColour("green", "Language");
                else
                    SetTabColour("default", "Language");
                jQuery("#LanguageId").val("");
                jQuery("#LanguageId").change();
                data = receivedData.employeeLanguageDetails;
                GridForm.LanguageGridCount = data.length;
                tableID = "languageTable";
                keyAttr = "employeeLanguageDetailId"
                tableCols = ['language,data-selectedid,languageId', 'read,bool', 'write,bool', 'speak,bool', 'Action']
                moveNextForm = true;
            }

            if (!IsBlank(receivedData.displayMessage)) {


                if (!(receivedData.displayMessage.toUpperCase() == "SUCCESS" || receivedData.displayMessage.toUpperCase() == "_BLANK")) {
                    jQuery("#errorDiv").show();
                    //jQuery("#errorPara").html(receivedData.displayMessage.replace("\r\n", "</br>"));
                    $erroralert("Transaction Failed!", receivedData.displayMessage.replace("\r\n", "</br>"));
                    moveNextForm = false;
                }
            }

            if (data != null && moveNextForm == true) {
                //$successalert("", "Transaction Successful!");
                if (!(formID == "employeepersonalinfoForm" || formID == "employeecontactdetailForm")) {
                    if (keyAttr != "")
                        jQuery("#" + CapitalizeSmallFirstLetter(keyAttr, "C")).val("0");
                    if (data.length > 0)
                        PopulateTable(tableID, data, tableCols, keyAttr)
                }
            }

            if ((!(formID == "employeepersonalinfoForm" || formID == "employeecontactdetailForm" || formID == "employeejobForm" || formID == "employeepassportdetailsForm" || formID == "employeebankForm")) && moveNextForm)
                ResetForm(formID);


            if ((formID == "employeefamilyForm" || formID == "employeeacdemicForm" || formID == "employeeexperienceForm" || formID == "employeecertificationForm" || formID == "employeereferenceForm") && moveNextForm == true)
                ResetForm(formID);


        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText + '!');
            var x = 1;
        }
    });
}
function MoveNext(formID) {
    btnNext = jQuery("#" + formID).find('.btn-next')
    btnNext.trigger("click")
}

function GetFormRefrenceId(formId) {

    var refId = 0;
    if (formId.toLowerCase() == "employeebankform")
        refId = jQuery("#BankDetailId").val();
    else if (formId.toLowerCase() == "employeefamilyform")
        refId = jQuery("#EmployeeFamilyDetailId").val();
    else if (formId.toLowerCase() == "employeeacdemicform")
        refId = jQuery("#AcademicDetailId").val();
    else if (formId.replace(" ", "").toLowerCase() == "employeeexperienceform")
        refId = jQuery("#ExperienceDetailId").val();
    else if (formId.toLowerCase() == "employeecertificationform")
        refId = jQuery("#CertificationDetailId").val();
    else if (formId.toLowerCase() == "employeereferenceform")
        refId = jQuery("#EmployeeReferenceId").val();
    else if (formId.toLowerCase() == "employeelanguagesform")
        refId = jQuery("#EmployeeLanguageDetailId").val();
    else if (formId.toLowerCase() == "employeecontactdetailform")
        refId = jQuery("#EmployeeContactDetailId").val();
    else if (formId.toLowerCase() == "pemployeecontactdetailform")
        refId = jQuery("#pEmployeeContactDetailId").val();

    //if (refId != 0)
    //    jQuery("#TableReferenceId").val(refId);
    return refId;
}

function PopulateTable(tableID, data, tableCols, keyAttr) {
    // alert("List");
    var tableCtrl = jQuery("#" + tableID)
    //var tBody = jQuery('<tbody>')
    //jQuery("#" + tableID + " tbody").remove();
    //jQuery("#" + tableID + " tr").each(function () {
    //    this.parentNode.removeChild(this);
    //});
    jQuery("#" + tableID + " tr").slice(1).remove()

    data.forEach(function (row) {
        var tr = $('<tr>');
        tableCols.forEach(function (attr) {
            if (attr.toUpperCase() == "ACTION")
                tr.append('<th><button  class="btn btn-primary me-1" onclick="SetFormDetail(\'' + tableID + '\',\'' + keyAttr + '\',\'' + row[keyAttr] +
                    '\')">Edit</button><Button style="" class="btn btn-primary" onclick = "DeleteRecordList(\'' + row[keyAttr] +
                    '\',\'' + tableID + '\')">Delete</Button></th>');
            //if (tableID == "familyTable") {
            //    tr.append('<th><button  class="btn btn-primary me-1" onclick="SetFormDetail(\'' + tableID + '\',\'' + keyAttr + '\',\'' + row[keyAttr] +
            //        '\')">Edit</button><Button style="" class="btn btn-primary" onclick = "DeleteRecordList(\'' + row[keyAttr] +
            //        '\',\'' + tableID + '\')">Delete</Button></th>');
            //}
            //else {
            //    tr.append('<th><button  class="btn btn-primary me-1" onclick="SetFormDetail(\'' + tableID + '\',\'' + keyAttr + '\',\'' + row[keyAttr] +
            //        '\')">Edit</button><Button style="display:none" class="btn btn-primary" onclick = "DeleteRecordList(\'' + row[keyAttr] +
            //        '\',\'' + tableID + '\')">Delete</Button></th>');
            //}
            else if (attr.indexOf(',') >= 1) {
                arrAttr = attr.split(',')
                if (arrAttr[1] == "date") {
                    tr.append('<td><span id="' + CapitalizeSmallFirstLetter(arrAttr[0], "C") + '_' + row[keyAttr] + '">' + (row[arrAttr[0]] == null ? "" : moment(row[arrAttr[0]], 'YYYY-MM-DD').format('DD-MMM-YYYY')) + '</span></td>');
                }
                else if (arrAttr[1] == "bool") {
                    tr.append('<td><span id="' + CapitalizeSmallFirstLetter(arrAttr[0], "C") + '_' + row[keyAttr] + '">' + (row[arrAttr[0]] == true ? "Yes" : "No") + '</span></td>');
                }
                else {
                    tr.append('<td><span id="' + CapitalizeSmallFirstLetter(arrAttr[2], "C") + '_' + row[keyAttr] + '" ' + arrAttr[1] + '="' + row[arrAttr[2]] + '">' + row[arrAttr[0]] + '</span></td>');
                }
            }
            else {
                tr.append('<td><span id="' + CapitalizeSmallFirstLetter(attr, "C") + '_' + row[keyAttr] + '">' + row[attr] + '</span></td>');
            }


            //tr.append();
        });
        tableCtrl.append(tr);
    });


    //if ($.fn.DataTable.isDataTable('#academicTable')) {
    //    $('#academicTable').DataTable().destroy();
    //}
    //$('#academicTable').DataTable()

}

function SetFormDetail(tableID, keyID, keyValue) {
    var tableCols = [];
    if (tableID == "familyTable") {
        tableCols = ['MemberName', 'MemberDob', 'Relationship'];
    }
    else if (tableID == "academicTable") {
        tableCols = ['InstituteName', 'AcademicId', 'PassingYear', 'Percentage']
    }
    else if (tableID == "experienceTable") {
        tableCols = ['CompanyName', 'ExperienceJobTitleId', 'JoinDate', 'LastWorkingDate']
    }
    else if (tableID == "certificationTable") {
        tableCols = ['CertificationName', 'YearOfCertification', 'Duration']
    }
    else if (tableID == "referenceTable") {
        tableCols = ['PersonName', 'PresentCompany', 'ReferenceDesignation', 'ReferenceContactNo', 'ReferenceMobileNo', 'HowYouKnow']
    }
    else if (tableID == "languageTable") {

        tableCols = ['LanguageId', 'Read', 'Write', 'Speak']
    }
    jQuery("#" + CapitalizeSmallFirstLetter(keyID, "C")).val(keyValue);
    tableCols.forEach(function (attr) {
        spanCtrl = jQuery("#" + attr + "_" + keyValue)
        sSpanVal = spanCtrl.text()
        var idVal = spanCtrl.data("selectedid");
        sSpanVal = idVal != undefined ? idVal : sSpanVal;
        //var isNotReadonly = (jQuery("#" + attr).attr('readonly') == undefined ? "" : jQuery("#" + attr).attr('readonly'));
        //if (isNotReadonly.toLowerCase() == "readonly")
        //    jQuery("#" + attr).removeAttr('readonly');
        if (attr.toUpperCase() == "DOB") {
            //  var fp = jQuery("#Dob").flatpickr({ dateFormat: "d-m-Y", setDate(new Date(sSpanVal)})

        }
        if (jQuery("#" + attr).attr("type") == "checkbox") {
            if (sSpanVal.toUpperCase() == "TRUE") {
                jQuery("#" + attr).prop('checked', true);
            }
            else if (sSpanVal.toUpperCase() == "YES") {
                jQuery("#" + attr).prop('checked', true);
            }
            else if (sSpanVal.toUpperCase() == "NO") {
                jQuery("#" + attr).prop('checked', false);
            }
            else if (sSpanVal.toUpperCase() == "FALSE") {
                jQuery("#" + attr).prop('checked', false);
            }
        }
        else {
            jQuery("#" + attr).val(sSpanVal)
            // if (jQuery("#" + attr).attr("type") == "select")
            jQuery("#" + attr).change();
            //document.getElementById(attr).value = sSpanVal;
        }

        //if (isNotReadonly.toLowerCase() == "readonly")
        //    jQuery("#" + attr).attr('readonly', true);

    });


}

function UploadEmployeeDocument(formID, url, actionId) {

    if (formID == "employeeportfolioForm") {

        if (typeof ($("#profileimagepreview").prop("src")) === "undefined") {

            if ($("#upload_file").val().length == 0) {
                $erroralert("Failed!", "Profile Picture is mandatory, if exists, refresh the page!");
                return false;
            }

        }

    }
    jQuery("#FormName").val(formID);
    if (jQuery(".dirty").length == 0) {
        if (!ValidForm(formID)) {

            if (formID == "employeeportfolioForm") {
                alert(jQuery("#profileimagepreview"));
                if (jQuery("#profileimagepreview").prop("src") != "" && jQuery("#profileimagepreview").prop("src") != undefined) {
                    MoveNext(formID);
                    $("#upload_file").removeClass("error");
                    $("#upload_file").parent().find("label.error").remove();
                    return false;
                }
                else
                    return false;
            }
            else {
                return false;
            }
        }
    }

    var formData = new FormData();
    if (formID == "employeeuploaddocForm")
        formData = GetEmployeeDocsData();
    if (formID == "employeeportfolioForm")
        formData = GetProfileImage();
    var x = 1
    BlockUI();
    jQuery.ajax({
        type: "POST",
        url: url,
        data: formData,
        cache: false,
        processData: false,
        contentType: false,
        dataType: "json",
        success: function (data) {
            UnblockUI();

            jQuery("#FormName").val("");
            if (formID == "employeeportfolioForm")
                SetTabColour("green", "ProfilePicture");
            if (actionId == "uploaddocs")
                GetEmployeeDocsData();
            else if (actionId == "uploaddocs_N_movenext")
                MoveNext(formID);
        },
        error: function (result) {
            UnblockUI();
            if (formID == "employeeportfolioForm")
                SetTabColour("green", "ProfilePicture");
            var x = 1;
            jQuery("#FormName").val("");
            MoveNext(formID);
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

function CopyClearDataForPermanent() {
    //jQuery("#sameAsCurrent").attr("checked") ? CopyCurrentToPermanent() : ClearFormData()
    if (jQuery("#sameAsCurrent").prop("checked") == true)
        CopyCurrentToPermanent();
    else
        ClearFormData();
}

function CopyCurrentToPermanent() {
    var formData = {};
    var allCtrl = jQuery("[id^=Permanent]")
    var copyFrom = "";
    var curCtrl = "";
    allCtrl.each(function (i) {
        curCtrl = jQuery(this);
        copyFrom = (curCtrl.attr('id')).replace("Permanent", "");
        if (curCtrl.prop("type").indexOf("select-") != -1) {
            if ((curCtrl.prop("id").indexOf("CountryId") != -1) || (curCtrl.prop("id").indexOf("StateId") != -1) || (curCtrl.prop("id").indexOf("CityId") != -1)) {
                //var selectedVal = jQuery("#" + copyFrom).val();
                //curCtrl.val(selectedVal);
                //curCtrl.text = jQuery("#" + copyFrom).find(':selected').text();
                curCtrl.find('option[value=' + jQuery("#" + copyFrom).val() + ']').remove();
                curCtrl.append(jQuery("<option selected></option>").val(jQuery("#" + copyFrom).val()).html(jQuery("#" + copyFrom).find(':selected').text()));
            }
            //else if ((curCtrl.prop("id").indexOf("StateId") != -1) || (curCtrl.prop("id").indexOf("CityId") != -1))
            //    curCtrl.append(jQuery("<option selected></option>").val(jQuery("#" + copyFrom).val()).html(jQuery("#" + copyFrom).find(':selected').text()));
        }
        else
            curCtrl.val(jQuery("#" + copyFrom).val())
    });
}

function ClearFormData() {
    var allCtrl = jQuery("[id^=Permanent]")
    allCtrl.each(function (i) {
        var curCtrl = jQuery(this);
        curCtrl.val('').change();
    });
}

function PopulateCascadingDDL(actionURL, parentDDL, parentDDLValue, childDDL, defaultSelected, keyText, valueText, cityDDL) {
    //var objDDL = countryDDL
    if (parentDDL != "")
        jQuery("#" + parentDDL).attr("disabled", true)

    if (parentDDL.toLowerCase() == "countryid" || parentDDL.toLowerCase() == "permanentcountryid" || parentDDL.toLowerCase() == "passportissuecountryid")
        sParam = { isActive: true, countryId: ((parentDDLValue == "" || parentDDLValue == 0) ? jQuery("#" + parentDDL).val() : parentDDLValue) };
    else if (parentDDL.toLowerCase() == "stateid" || parentDDL.toLowerCase() == "permanentstateid" || parentDDL.toLowerCase() == "passportissuestateid")
        sParam = { isActive: true, stateId: (parentDDLValue == 0 ? jQuery("#" + parentDDL).val() : parentDDLValue) };

    //if(jQuery('#' + childDDL).length>0)
    jQuery('#' + childDDL).empty();
    //jQuery("#" + childDDL).html("")
    //if (parentDDL.toLowerCase() == "permanentcountryid")
    //    jQuery("#" + cityDDL).html("")
    // jQuery(this).blockUI();
    jQuery.ajax({
        type: "GET",
        // url: "/WorkLocation/GetCounryStates",
        url: actionURL,
        data: sParam,
        success: function (response) {
            //PopulateDropDown("StateId", response, 'Model.StateId', 'stateId', 'stateName')
            if (parentDDL == "permanentstateid" && jQuery("#sameAsCurrent").prop("checked") == true)
                defaultSelected = jQuery("#CityId").val();

            PopulateDropDown(childDDL, response, defaultSelected, keyText, valueText)
            jQuery("#" + parentDDL).attr("disabled", false)
            jQuery("#" + childDDL).trigger("change");


            // jQuery(document).ajaxStop(jQuery.unblockUI);
        },
        failure: function (response) {
            $erroralert("Error!", response.responseText + '!');
        },
        error: function (response) {
            $erroralert("Error!", response.responseText + '!');
        }
    });
}
function DeleteRecordList(id, tableId) {
    Swal.fire({ title: 'Are you sure?', text: "This will get deleted permanently!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, delete it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            var del = false;
            var url = "", formData = "";
            //if (tableId == "languageTable") {
            //    del = true;
            //    formData = "EmployeeLanguageDetailId=" + id + "&EmployeeId=" + jQuery("#EmployeeId").val() + "";
            //    url = "/EmployeeLanguageDetail/DeleteEmployeeLanguage";
            //}
            //else
            if (tableId == "familyTable") {
                del = true;
                formData = "EmployeeFamilyDetailId=" + id + "&EmployeeId=" + jQuery("#EmployeeId").val() + "";
                url = "/EmployeeFamily/DeleteEmployeeFamilyDetail";
            }
            else if (tableId == "academicTable") {
                del = true;
                formData = "AcademicDetailId=" + id + "&EmployeeId=" + jQuery("#EmployeeId").val() + "";
                url = "/EmployeeAcademic/DeleteEmployeeAcademicDetail";
            }
            else if (tableId == "experienceTable") {
                del = true;
                formData = "ExperienceDetailId=" + id + "&EmployeeId=" + jQuery("#EmployeeId").val() + "";
                url = "/EmployeeExperience/DeleteEmployeeExperienceDetail";
            }
            else if (tableId == "certificationTable") {
                del = true;
                formData = "CertificationDetailId=" + id + "&EmployeeId=" + jQuery("#EmployeeId").val() + "";
                url = "/EmployeeCertification/DeleteEmployeeCertificationDetail";
            }
            else if (tableId == "referenceTable") {
                del = true;
                formData = "EmployeeReferenceId=" + id + "&ReferenceOf=" + jQuery("#EmployeeId").val() + "";
                url = "/EmployeeReference/DeleteEmployeeReferenceDetail";
            }
            else if (tableId == "languageTable") {
                del = true;
                formData = "EmployeeLanguageDetailId=" + id + "&EmployeeId=" + jQuery("#EmployeeId").val() + "";
                url = "/EmployeeLanguageDetail/DeleteEmployeeLanguageDetail";
            }
            //"experiencedetail", "certificationdetail", "referencedetail", "languagedetail" 

            if (del == true) {
                BlockUI();
                jQuery.ajax({
                    type: "POST",
                    //url: "/Employee/SaveEmployeePersonalInfo",
                    url: url,
                    data: formData,
                    cache: false,
                    contentType: "application/x-www-form-urlencoded; charset=utf-8",
                    success: function (receivedData) {
                        UnblockUI();
                        var tableName = "";
                        $successalert("", "Transaction Successful!");
                        let data, keyAttr, tableCols;
                        if (tableId == "languageTable") {
                            data = receivedData.employeeLanguageDetails;
                            GridForm.LanguageGridCount = data.length;
                            tableName = "Language";
                            keyAttr = "employeeLanguageDetailId"
                            tableCols = ['language,data-selectedid,languageId', 'read,bool', 'write,bool', 'speak,bool', 'Action']
                            if (keyAttr != "")
                                jQuery("#" + CapitalizeSmallFirstLetter(keyAttr, "C")).val("0");
                        }
                        else if (tableId == "familyTable") {
                            data = receivedData.employeeFamilyDetails;
                            GridForm.FamilyGridCount = data.length;
                            tableName = "FamilyDetails";
                            tableID = "familyTable";
                            keyAttr = "employeeFamilyDetailId"
                            tableCols = ['memberName', 'memberDob,date', 'relationship', 'Action']
                            if (keyAttr != "")
                                jQuery("#" + CapitalizeSmallFirstLetter(keyAttr, "C")).val("0");
                        }
                        else if (tableId == "academicTable") {
                            tableName = "AcademicDetails";
                            data = receivedData.employeeAcademicDetails;
                            GridForm.AcademicGridCount = data.length;
                        }
                        else if (tableId == "experienceTable") {
                            data = receivedData.employeeExperienceDetails;
                            tableName = "ExperiencesBackgroud";
                            GridForm.ExperienceGridCount = data.length;
                        }
                        else if (tableId == "certificationTable") {
                            data = receivedData.employeeCertificationDetails;
                            tableName = "CertificationDetails";
                            GridForm.CertificationGridCount = data.length;
                        }
                        else if (tableId == "referenceTable") {
                            data = receivedData.employeeReferenceDetails;
                            tableName = "ReferenceDetails";
                            GridForm.ReferenceGridCount = data.length;
                        }
                        if (tableName != "") {
                            if (data.length == 0)
                                SetTabColour("default", tableName);
                            else
                                SetTabColour("green", tableName);
                        }
                        PopulateTable(tableId, data, tableCols, keyAttr);


                    },
                    error: function (result) {
                        UnblockUI();
                        $erroralert("Transaction Failed!", result.responseText + '!');

                    }
                });
            }
        }
    });
}

function ViewAttachment(fldName, lblName, ScreenTab) {
    var refId = 0;
    if (ScreenTab.replace(" ", "").toLowerCase() == "bankdetails")
        refId = jQuery("#BankDetailId").val();
    else if (ScreenTab.replace(" ", "").toLowerCase() == "familydetails")
        refId = jQuery("#EmployeeFamilyDetailId").val();
    else if (ScreenTab.replace(" ", "").toLowerCase() == "academicdetails")
        refId = jQuery("#AcademicDetailId").val();
    else if (ScreenTab.replace(" ", "").toLowerCase() == "experiencesbackgroud")
        refId = jQuery("#ExperienceDetailId").val();
    else if (ScreenTab.replace(" ", "").toLowerCase() == "certificationdetails")
        refId = jQuery("#CertificationDetailId").val();
    else if (ScreenTab.replace(" ", "").toLowerCase() == "referencedetails")
        refId = jQuery("#EmployeeReferenceId").val();
    else if (ScreenTab.replace(" ", "").toLowerCase() == "language")
        refId = jQuery("#EmployeeLanguageDetailId").val();


    var w = window.open();
    w.document.title = "SimpliHR-View Attachment";

    let encEmployeeId = $("#EncEmployeeId").val();

    w.document.location.href = "ViewFile/" + fldName + (encEmployeeId != "" ? "&" + encEmployeeId + "" + (refId == 0 ? "" : "-=refid__" + refId) : "&%20"); //how to assign the url to the newly opened window
    $(w.document.body).html(stored);
    return false;
}


$(".file_remove").on("click", function (e) {
    var btnUpload = $("#upload_file"),
        btnOuter = $(".button_outer");
    $("#uploaded_view").removeClass("show");
    $("#uploaded_view").find("img").remove();
    btnOuter.removeClass("file_uploading");
    btnOuter.removeClass("file_uploaded");
    $("#upload_file").val('');
});

function GetEmployeeDocsData() {
    var ajaxData = new FormData();
    jQuery.each(jQuery(".employeeDocs"), function (i, obj) {
        var dataId = jQuery(this).data("dcumenttypes")
        jQuery.each(obj.files, function (j, file) {
            // var fileName = file.name;
            var fileExtension = file.name.replace(/^.*\./, '');
            var newName = dataId + "." + fileExtension
            ajaxData.append('UploadedFile', file, newName); // is the var i against the var j, because the i is incremental the j is ever 0
        });
    });
    ajaxData.append("EmployeeId", jQuery("#EmployeeId").val());
    ajaxData.append("ClientId", jQuery("#ClientId").val());
    ajaxData.append("FormName", jQuery("#FormName").val());
    ajaxData.append("EmailId", jQuery("#EmailId").val());
    ajaxData.append("FieldName", jQuery("#FieldName").val());
    ajaxData.append("EntrySource", "EmployeeScreen");
    //ajaxData.append("ReferenceId", jQuery("#ReferenceId").val());
    // ajaxData.append('UploadDcumentDetailId', jQuery("#UploadDcumentDetailId").val());
    return ajaxData;
}


function GetProfileImage() {
    let formData = new FormData();
    formData.append("ProfileImageFile", jQuery("#upload_file")[0].files[0]);
    formData.append("EmployeeId", jQuery("#EmployeeId").val());
    formData.append("ClientId", jQuery("#ClientId").val());
    formData.append("UnitId", jQuery("#UnitId").val());
    formData.append("EmployeeCode", jQuery("#EmployeeCode").val());
    formData.append("FormName", jQuery("#FormName").val());
    formData.append("CroppedImageBase64", croppedImageBase64);
    return formData;
}

function UploadAttachment(url) {
    if (jQuery(".dirty").length == 0) {
        return false;
    }
    var formData = new FormData();
    formData = GetEmployeeDocsData();
    var x = 1
    BlockUI();
    jQuery.ajax({
        type: "POST",
        url: url,
        data: formData,
        cache: false,
        processData: false,
        contentType: false,
        dataType: "json",
        success: function (data) {
            UnblockUI();
            if (data.displayMessage.toUpperCase() == "SUCCESS") {
                jQuery("#attachmentPopUp").modal("hide");
                GetEmployeeDocsData();
                jQuery("#FormName").val("");
                jQuery("#FileName").val("");
                $successalert("", "Transaction Successful!");
            }
            else {
                $erroralert("Transaction Failed!", 'Error');
            }
        },
        error: function (result) {
            UnblockUI();
            var x = 1;
            jQuery("#FormName").val("");
            //MoveNext(formID);
        }
    });
}

function ProfileImageCrop() {
    var cropper;
    $("#upload_file").change(function (e) {
        if (this.files == null || this.files == undefined) {
            $erroralert("", "Please upload image file");
        }
        if (!validateImageFileTypeByFile(this.files[0])) {
            this.value = "";
            $erroralert("Invalid Image", "Please select a valid image file (JPEG, PNG, GIF).");
            return false;
        }
        $("#btnModalImageCrop").click();
        var fileReader = new FileReader();
        fileReader.onload = function (event) {
            $('#croppedImage').attr('src', event.target.result);
            if (cropper) {
                croppedImageBase64 = "";
                cropper.destroy();
            }
            cropper = new Cropper($('#croppedImage')[0], {
                aspectRatio: 1 / 1,
                viewMode: 1,
                minContainerWidth: 600,
                minContainerHeight: 600,

            });
        };
        fileReader.readAsDataURL(e.target.files[0]);
        $("#profileimagepreview").hide();
    });
    $("#backDropModal").find(".btn-close").click(function () {

        var croppedCanvas = cropper.getCroppedCanvas();
        croppedImageBase64 = croppedCanvas.toDataURL();
        var btnUpload = $("#upload_file"),
            btnOuter = $(".button_outer");
        btnOuter.addClass("file_uploading");
        setTimeout(function () {
            btnOuter.addClass("file_uploaded");
        }, 3000);
        //var uploadedFile = URL.createObjectURL(e.target.files[0]);
        setTimeout(function () {
            $("#uploaded_view").append('<img src="' + croppedImageBase64 + '" />').addClass("show");
        }, 3500);
        //console.log(croppedImageBase64);
    });
    $("#cropButton").click(function () {

        $("#backDropModal").find(".btn-close").click();
    });
}

function ResetForm(formID) {
    jQuery("#" + formID)[0].reset();
    jQuery("#EmployeeFamilyDetailId").val("0");
    jQuery("#academicDetailId").val("0");
    jQuery("#ExperienceDetailId").val("0");
    jQuery("#CertificationDetailId").val("0");
    jQuery("#EmployeeReferenceId").val("0");
    jQuery("#EmployeeLanguageDetailId").val("0");
    jQuery("#FormName").val("");
    let selectElements = jQuery("#" + formID).find("select");
    selectElements.each((i, v) => {
        $(v).val(0);
        $(v).change();
    });
}

function SetDDLRequiredAttribute() {

    if ('@Model.EmployeeMastersKeyValues.EmployeeValidations.Where(x => x.FieldName == "PermanentCountryId" && x.IsMandatory == true).Count()' == '1') {
        jQuery("#PermanentCountryId").attr('required', 'required');
    }
    else
        jQuery("#PermanentCountryId").removeAttr('required');


    if ('@Model.EmployeeMastersKeyValues.EmployeeValidations.Where(x => x.FieldName == "PermanentStateId" && x.IsMandatory == true).Count()' == '1') {
        jQuery("#PermanentStateId").attr('required', 'required');
    }
    else
        jQuery("#PermanentStateId").removeAttr('required');

    if ('@Model.EmployeeMastersKeyValues.EmployeeValidations.Where(x => x.FieldName == "PermanentCityId" && x.IsMandatory == true).Count()' == '1') {
        jQuery("#PermanentCityId").attr('required', 'required');
    }
    else
        jQuery("#PermanentCityId").removeAttr('required');

    if ('@Model.EmployeeMastersKeyValues.EmployeeValidations.Where(x => x.FieldName == "MaritalStatusId" && x.IsMandatory == true).Count()' == '1') {
        jQuery("#MaritalStatusId").attr('required', 'required');
    }
    else
        jQuery("#MaritalStatusId").removeAttr('required');

    if ('@Model.EmployeeMastersKeyValues.EmployeeValidations.Where(x => x.FieldName == "ReligionId" && x.IsMandatory == true).Count()' == '1') {
        jQuery("#ReligionId").attr('required', 'required');
    }
    else
        jQuery("#ReligionId").removeAttr('required');

    if ('@Model.EmployeeMastersKeyValues.EmployeeValidations.Where(x => x.FieldName == "BloodGroupId" && x.IsMandatory == true).Count()' == '1') {
        jQuery("#BloodGroupId").attr('required', 'required');
    }
    else
        jQuery("#BloodGroupId").removeAttr('required');

    if ('@Model.EmployeeMastersKeyValues.EmployeeValidations.Where(x => x.FieldName == "BankId" && x.IsMandatory == true).Count()' == '1') {
        jQuery("#BankId").attr('required', 'required');
    }
    else
        jQuery("#BankId").removeAttr('required');


    if ('@Model.EmployeeMastersKeyValues.EmployeeValidations.Where(x => x.FieldName == "AcademicId" && x.IsMandatory == true).Count()' == '1') {
        jQuery("#AcademicId").attr('required', 'required');
    }
    else
        jQuery("#AcademicId").removeAttr('required');

    if ('@Model.EmployeeMastersKeyValues.EmployeeValidations.Where(x => x.FieldName == "ExperienceJobTitleId" && x.IsMandatory == true).Count()' == '1') {
        jQuery("#ExperienceJobTitleId").attr('required', 'required');
    }
    else
        jQuery("#ExperienceJobTitleId").removeAttr('required');

    if ('@Model.EmployeeMastersKeyValues.EmployeeValidations.Where(x => x.FieldName == "JobTitleId" && x.IsMandatory == true).Count()' == '1') {
        jQuery("#JobTitleId").attr('required', 'required');
    }
    else
        jQuery("#JobTitleId").removeAttr('required');

    if ('@Model.EmployeeMastersKeyValues.EmployeeValidations.Where(x => x.FieldName == "DepartmentId" && x.IsMandatory == true).Count()' == '1') {
        jQuery("#DepartmentId").attr('required', 'required');
    }
    else
        jQuery("#DepartmentId").removeAttr('required');

    if ('@Model.EmployeeMastersKeyValues.EmployeeValidations.Where(x => x.FieldName == "ManagerId" && x.IsMandatory == true).Count()' == '1') {
        jQuery("#ManagerId").attr('required', 'required');
    }
    else
        jQuery("#ManagerId").removeAttr('required');

    if ('@Model.EmployeeMastersKeyValues.EmployeeValidations.Where(x => x.FieldName == "HODId" && x.IsMandatory == true).Count()' == '1') {
        jQuery("#HODId").attr('required', 'required');
    }
    else
        jQuery("#HODId").removeAttr('required');

    if ('@Model.EmployeeMastersKeyValues.EmployeeValidations.Where(x => x.FieldName == "RoleId" && x.IsMandatory == true).Count()' == '1') {
        jQuery("#RoleId").attr('required', 'required');
    }
    else
        jQuery("#RoleId").removeAttr('required');

    if ('@Model.EmployeeMastersKeyValues.EmployeeValidations.Where(x => x.FieldName == "BandId" && x.IsMandatory == true).Count()' == '1') {
        jQuery("#BandId").attr('required', 'required');
    }
    else
        jQuery("#BandId").removeAttr('required');

    if ('@Model.EmployeeMastersKeyValues.EmployeeValidations.Where(x => x.FieldName == "YearOfCertification" && x.IsMandatory == true).Count()' == '1') {
        jQuery("#YearOfCertification").attr('required', 'required');
    }
    else
        jQuery("#YearOfCertification").removeAttr('required');

    if ('@Model.EmployeeMastersKeyValues.EmployeeValidations.Where(x => x.FieldName == "LanguageId" && x.IsMandatory == true).Count()' == '1') {
        jQuery("#LanguageId").attr('required', 'required');
    }
    else
        jQuery("#LanguageId").removeAttr('required');

    if ('@Model.EmployeeMastersKeyValues.EmployeeValidations.Where(x => x.FieldName == "WorkLocationId" && x.IsMandatory == true).Count()' == '1') {
        jQuery("#WorkLocationId").attr('required', 'required');
    }
    else
        jQuery("#WorkLocationId").removeAttr('required');
}

$('#Dob').change(function () {

    var Dob = new Date($("#Dob").val());
    if (Dob.getFullYear() < 1910) {
        $erroralert("Validation Failed!", "DOB year should be more than 1910");
        $("#Dob").val('');
        return false;
    }

});


$('#ContactNo').change(function () {


    var ContactNo = $('#ContactNo').val();

    if (ContactNo.length < 10) {

        $erroralert("Transaction Failed!", "Please enter at least 10 digits for a valid contact number");
        return false;
    }

});
$('#ContactNum').change(function () {


    var ContactNo = $('#ContactNum').val();

    if (ContactNo.length < 10) {

        $erroralert("Transaction Failed!", "Please enter at least 10 digits for a valid contact number");
        return false;
    }

});
$('#PermanentContactNum').change(function () {


    var ContactNo = $('#PermanentContactNum').val();

    if (ContactNo.length < 10) {

        $erroralert("Transaction Failed!", "Please enter at least 10 digits for a valid contact number");
        return false;
    }

});

$('#Pannumber').keyup(function () {
    this.value = this.value.toLocaleUpperCase();
});

$('#Pannumber').change(function () {
    var regex = /[A-Z]{5}[0-9]{4}[A-Z]{1}$/;
    var txtpan = $('#Pannumber').val();
    if (txtpan.length == 10) {
        if (!regex.test(txtpan)) {
            $('#Pannumber').val('');
            $erroralert("Transaction Failed!", "Not a valid PAN number");
        }

    }
    else {
        $('#Pannumber').val('');
        $erroralert("Transaction Failed!", "Please enter 10 digits for a valid PAN number");
    }


});


function checkTextEntry(e) {

    var pastedData = e.target.value;

    var specialChars = "<>!#$%^&*()_+[]{}?:;|'\"\\,./~`-="
    for (i = 0; i < specialChars.length; i++) {
        if (pastedData.indexOf(specialChars[i]) > -1) {
            $(e.target).val('');
            $erroralert("Validation Failed!", "Junk characters are not allowed.");
            //  alert('illegal characters are not allowed.');
            return false
        }
    }
    return true;
    //var k;
    //document.all ? k = e.keyCode : k = e.which;
    //return ((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57));
    //var inputData = e.target.value;
    //if (/^[a-zA-Z0-9- ]*$/.test(inputData) == false) {
    //    return false;
    //    //alert('Your search string contains illegal characters.');
    //}
}

//$('input:text').keyup(function (e) {

//    var pastedData = e.target.value;


//    var specialChars = "<>!#$%^&*()_+[]{}?:;|'\"\\,./~`-="
//    for (i = 0; i < specialChars.length; i++) {
//        if (pastedData.indexOf(specialChars[i]) > -1) {
//            $(e.target).val('');
//            alert('Your inserting illegal characters.');
//            return false
//        }
//    }
//    return true;
//});


















//Exit JS

function DownloadResignationDocumentM(fileId, filename) {
    // AJAX request to download the file
    $.ajax({
        url: '/ExitManagement/DownloadResignationDocumentM/' + fileId,
        method: 'GET',
        xhrFields: {
            responseType: 'blob'// Set the response type to blob
        },
        success: function (data) {
            // Create a temporary anchor element to trigger the download
            var a = document.createElement('a');
            var url = window.URL.createObjectURL(data);
            a.href = url;
            a.download = filename;
            // Set the filename for download
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
            //$successalert("Success!", "Downloaded succesfully.");
        },
        error: function (xhr, status, error) {
            $erroralert("Oops!", 'Error downloading file:' + xhr.responseText);
        }
    });
}
function DownloadResignationDocumentA(fileId, filename) {
    // AJAX request to download the file
    $.ajax({
        url: '/ExitManagement/DownloadResignationDocumentA/' + fileId,
        method: 'GET',
        xhrFields: {
            responseType: 'blob'// Set the response type to blob
        },
        success: function (data) {
            // Create a temporary anchor element to trigger the download
            var a = document.createElement('a');
            var url = window.URL.createObjectURL(data);
            a.href = url;
            a.download = filename;
            // Set the filename for download
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
            //$successalert("Success!", "Downloaded succesfully.");
        },
        error: function (xhr, status, error) {
            $erroralert("Oops!", 'Error downloading file:' + xhr.responseText);
        }
    });
}