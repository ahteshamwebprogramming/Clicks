//$(document).ready(function () {
//    $(".select2").select2();
//});


function Init(paramId, displayMessage, httpStatusCode) {
    jQuery(".List").dataTable();
    if (paramId != 0 || httpStatusCode != 200) {
        jQuery("#List").hide();
        jQuery("#Add").show();
    }

    if (!(displayMessage.trim() == "SUCCESS" || displayMessage.trim() == "" || displayMessage.trim().toLowerCase() == "_blank")) {
        if (httpStatusCode == 200) {
            $successalert("", displayMessage);
            ClearForm();
        }
        else {
            $erroralert("Transaction Failed!", displayMessage);
        }
        /*alert(displayMessage);*/
    }
}

function ListView() {
    $("#Add").hide();
    $("#List").show();
}
function AddView(paramId = 0) {
    if (paramId == 0) {
        ClearForm();
    }
    $("#Add").show();
    $("#List").hide();
}

function GetFormControls() {
    var formData = {};
    var $allCtrl = $('.dbcol');
    $allCtrl.each(function (i) {
        var curCtrl = jQuery(this);
        if (this.type !== undefined) {
            if (this.type.toLowerCase() == "checkbox") {
                if (this.checked)
                    formData[curCtrl.attr('name')] = true;
                else
                    formData[curCtrl.attr('name')] = false;
            }

        }
        if (this.type.toLowerCase() == "radio") {
            if (this.checked)
                formData[curCtrl.attr('name')] = curCtrl.val();
        }
        else if (formData[curCtrl.attr('name')] === undefined)
            formData[curCtrl.attr('name')] = curCtrl.val();


    })
    return formData;
}

function GetFormValues(formObject) {
    if (formObject == null || formObject === undefined)
        formObject = new FormData();

    var $allCtrl = $('.dbcol');
    $allCtrl.each(function (i) {
        var curCtrl = jQuery(this);
        if (this.type !== undefined) {
            if (this.type.toLowerCase() == "checkbox") {
                if (this.checked)
                    formObject.append(curCtrl.attr('name'), true);
                //formData[curCtrl.attr('name')] = true;
                else
                    formObject.append(curCtrl.attr('name'), false);
                //formData[curCtrl.attr('name')] = false;
            }

        }
        if (this.type.toLowerCase() == "radio") {
            if (this.checked)
                formObject.append(curCtrl.attr('name'), curCtrl.val());
            //formData[curCtrl.attr('name')] = curCtrl.val();
        }
        else if (formObject[curCtrl.attr('name')] === undefined)
            formObject.append(curCtrl.attr('name'), curCtrl.val());
        //formData[curCtrl.attr('name')] = curCtrl.val();

    })
    return formObject;

}

function AddDirtyClass(obj) {
    jQuery(obj).each(function (key) {
        jQuery(this).change(function () {
            jQuery(this).addClass('dirty');
        });
    });
}

function IsBlank(obj) {
    return (!obj);
};



function ClearForm() {

    var $allCtrl = $('.clearForm');
    $allCtrl.each(function (i) {
        var curCtrl = jQuery(this);
        //if (this.type.toLowerCase() != "radio") {

        if (this.type.toLowerCase() == "select") {
            curCtrl.val("").change();
            curCtrl.val("0").change();
        }
        else if (this.type.toLowerCase() == "select-one") {
            curCtrl.val("").change();
            curCtrl.val("0").change();
        }
        else
            curCtrl.val("");


        //}
    })
}

function IsNumeric(data) {
    // alert("IsNumeric")
    isNumeric = /^[-+]?(\d+|\d+\.\d*|\d*\.\d+)$/;
    return isNumeric.test(data) ? true : false;
}

function PopulateDropDown(dropDownId, list, defaultSelected, key, value, defaultSelect) {

    if (defaultSelect != undefined)
        defaultSelect = "Select"
    if (list != null && list.length > 0) {

        //    //jQuery(dropDownId).removeAttr("disabled");
        var objDDLToPopulate = "#" + dropDownId;

        //alert("Hello" + jQuery(objDDLToPopulate));
        jQuery(objDDLToPopulate).append(jQuery("<option>Select</option>").val("0").html(defaultSelect));
        jQuery.each(list, function () {
            if ((',' + defaultSelected + ',').indexOf(this[key])>-1)
                jQuery(objDDLToPopulate).append(jQuery("<option selected></option>").val(this[key]).html(this[value]));
            else
                jQuery(objDDLToPopulate).append(jQuery("<option></option>").val(this[key]).html(this[value]));
        });
    }
}

function EditRecord(masterName, actionURL, paramId, isActive) {
    // event.preventDefault();
    if (isActive.trim().toLowerCase() == "false") {
        alert("Cannot edit Inactive record. Contact administrator for details")
        return false;
    }
    window.location.href = actionURL + (paramId != "" ? "/" + paramId : "/")
    return false;
}

function DeleteRecord(masterName, masterValue, actionURL, paramId, isActive) {
    // event.preventDefault();
    if (isActive.trim().toLowerCase() == "false") {
        alert(masterName + " is already Inactive");
        return false;
    }

    Swal.fire({ title: 'Are you sure?', text: "This will get deleted permanently!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, delete it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            window.location.href = actionURL + (paramId != "" ? "/" + paramId : "/") //"/Role/DeleteRole/" + roleId;    
        }
    });

    //if (confirm("Sure want to delete " + masterName + " '" + masterValue + "'?")) {
    //window.location.href = actionURL + (paramId != "" ? "/" + paramId : "/") //"/Role/DeleteRole/" + roleId;
    //}

    return false;
}

// Set the date we're counting down to
var countDownDate = new Date("Jan 5, 2024 15:37:25").getTime();

// Update the count down every 1 second
function StartCounter(countDownDate) {

    // Get today's date and time
    var now = new Date().getTime();

    // Find the distance between now and the count down date
    var distance = now - countDownDate;

    // Time calculations for days, hours, minutes and seconds
    var days = Math.floor(distance / (1000 * 60 * 60 * 24));
    var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
    var seconds = Math.floor((distance % (1000 * 60)) / 1000);

    // Output the result in an element with id=showInElement
    var retValue = ((isNaN(days) || days == 0) ? "00" : days) + ":" + ((isNaN(hours) || hours == 0) ? "00" : hours) + ":" + ((isNaN(minutes) || minutes == 0) ? "00" : minutes) + ":" + ((isNaN(seconds) || seconds == 0) ? "00" : seconds);

    // If the count down is over, write some text 
    if (distance < 0) {
        retValue = "EXPIRED";
    }
    return retValue
}


//$(".file_remove").on("click", function (e) {
//    var btnUpload = $("#upload_file"),
//        btnOuter = $(".button_outer");
//    $("#uploaded_view").removeClass("show");
//    $("#uploaded_view").find("img").remove();
//    btnOuter.removeClass("file_uploading");
//    btnOuter.removeClass("file_uploaded");
//});

