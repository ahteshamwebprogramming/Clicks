function addRow(currObj) {

    let tr = $(currObj).closest("table").find("tbody").find("tr");
    let indx = tr.length + 1;

    $(currObj).closest("table").find("tbody").append("<tr><td name='Point'><a href='javascript:void(0)' class='edit_number'>" + indx + "</a></td><td><a name='Category' href='javascript:void(0)' class='edit_text'>Category " + indx + "</a></td><td><a name='Description' href='javascript:void(0)' class='edit_text'>Description " + indx + "</a></td><td><a name='ScoreFrom' href='javascript:void(0)' class='edit_number'>0</a> - <a name='ScoreTo' href='javascript:void(0)' class='edit_number'>0</a><i class='fa fa-remove removeTr'></i></td></tr>");

    $.fn.editable.defaults.mode = 'inline';
    $('.edit_text').editable({
        type: 'text',
        success: function (k, v) {
            console.log(v);
            // var href = $(this).attr("href");
            // $('.name-list '+href).html(v);
        }
    });
    $('.edit_number').editable({
        type: 'text',
        validate: function (value) {
            if (isNaN(value)) {
                return 'Please enter a valid integer.';
            }
        },
        success: function (response, newValue) {
            $(this).text(parseInt(newValue, 10));
        }
    });

    $("[name='tblWeightedAverageMethod']").find(".removeTr").on('click', function () {
        $(this).closest('tr').remove();
    });
    $("[name='tblSimpleAverageMethod']").find(".removeTr").on('click', function () {
        $(this).closest('tr').remove();
    });
}


function SaveClearanceMapping() {
    BlockUI();

    if (ValidateSkillSetMatrix() == false) {
        UnblockUI();
        $erroralert("Error in skill set matrix!", 'Please check the highlighted rows for error. The sum should be 100% for all rows in skill set matrix!');
        return false;
    }
    //alert("Success");
    //return;
    let formData = $("#PerformaceSettings");
    let PerformanceSettingSkillSetMatrixList = [];
    let PerformanceSettingMechanismList = [];

    $(formData).find("[name='SkillSetMatrix']").find('tbody').find('tr').each(function () {
        $tr = $(this);
        let PerformanceSettingSkillSetMatrixDTO = {
            "BandId": $tr.find("[name='BandId']").attr("value"),
            "KRAWeightage": $tr.find("[name='KRAWeightage']").text(),
            "SoftSkillsWeightage": $tr.find("[name='SoftSkillsWeightage']").text()
        };
        PerformanceSettingSkillSetMatrixList.push(PerformanceSettingSkillSetMatrixDTO);
    });

    let tblName = $(formData).find("[name='PerformanceSetting.Mechanism']").val() == 1 ? "tblSimpleAverageMethod" : $(formData).find("[name='PerformanceSetting.Mechanism']").val() == 2 ? "tblWeightedAverageMethod" : "";

    $(formData).find("[name='" + tblName + "']").find('tbody').find('tr').each(function () {
        $tr = $(this);
        let PerformanceSettingMechanismDTO = {
            "Point": $tr.find("[name='Point']").text(),
            "Category": $tr.find("[name='Category']").text(),
            "Description": $tr.find("[name='Description']").text(),
            "ScoreFrom": $tr.find("[name='ScoreFrom']").text(),
            "ScoreTo": $tr.find("[name='ScoreTo']").text(),
        };
        PerformanceSettingMechanismList.push(PerformanceSettingMechanismDTO);
    });




    var PerformanceSettingDTO = {
        "ReviewPeriodFrom": moment($(formData).find("[name='PerformanceSetting.ReviewPeriodFrom']").val()).format('YYYY-MM-DD'),
        "ReviewPeriodTo": moment($(formData).find("[name='PerformanceSetting.ReviewPeriodTo']").val()).format('YYYY-MM-DD'),
        "AssesmentPeriodicity": $(formData).find("[name='PerformanceSetting.AssesmentPeriodicity']").val(),
        "RollOut": $(formData).find("[name='PerformanceSetting.RollOut']").val(),
        "Mechanism": $(formData).find("[name='PerformanceSetting.Mechanism']").val(),
        "SettingByManager": $("#PerformanceSetting_SettingByManager").is(":checked") == true ? true : false,
        "AcceptanceofGoalsByEmployee": $("#PerformanceSetting_AcceptanceofGoalsByEmployee").is(":checked") == true ? true : false,
        "EmployeeRating": $("#PerformanceSetting_EmployeeRating").is(":checked") == true ? true : false,
        "EmployeeRemarks": $("#PerformanceSetting_EmployeeRemarks").is(":checked") == true ? true : false,
        "EmployeeClosingRemarks": $("#PerformanceSetting_EmployeeClosingRemarks").is(":checked") == true ? true : false,
        "ManagerRating": $("#PerformanceSetting_ManagerRating").is(":checked") == true ? true : false,
        "ManagerRemarks": $("#PerformanceSetting_ManagerRemarks").is(":checked") == true ? true : false,
        "ManagerClosingRemarks": $("#PerformanceSetting_ManagerClosingRemarks").is(":checked") == true ? true : false,
        "TNRByManager": $("#PerformanceSetting_TNRByManager").is(":checked") == true ? true : false,
        "HODReview": $("#PerformanceSetting_HODReview").is(":checked") == true ? true : false,
        "HODClosingRemarks": $("#PerformanceSetting_HODClosingRemarks").is(":checked") == true ? true : false,
        "PerformanceSettingId": $(formData).find("[name='PerformanceSetting.PerformanceSettingId']").val() == "" ? 0 : $(formData).find("[name='PerformanceSetting.PerformanceSettingId']").val(),
    }

    var inputDTO = {
        "PerformanceSetting": PerformanceSettingDTO,
        "PerformanceSettingSkillSetMatrixList": PerformanceSettingSkillSetMatrixList,
        "PerformanceSettingMechanismList": PerformanceSettingMechanismList
    };

    $.ajax({
        type: "POST",
        url: "/Performance/SavePMSSetting",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            //$(currDiv).parent().parent().parent().remove();
            $successalert("", "Transaction Successful!");
            setTimeout(function () {
                window.location.href = "/Performance/ListPMSSetting";
            }, 2000);
            //resetForm();
            //ViewExitClearanceMappingTable();

        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!'); UnblockUI();
            UnblockUI();
        }
    });
}


function AssesmentPeriodicityChangeEvent() {
    $("[name='PerformanceSetting.AssesmentPeriodicity']").change(function () {
        ReviewPeriodChangeManager();
    });
    $("[name='PerformanceSetting.ReviewPeriodFrom']").change(function () {
        ReviewPeriodChangeManager();
    });
}

function ReviewPeriodChangeManager() {
    BlockUI();
    let durationType = 'D'; let duration = 0; let momentPeriodTo;

    let periodFrom = $("[name='PerformanceSetting.ReviewPeriodFrom']").val();
    let momentPeriodFrom = moment(periodFrom, "DD-MMM-YYYY");

    if ($("[name='PerformanceSetting.AssesmentPeriodicity']").val() == 1) {
        durationType = 'D'
        duration = 1;
        momentPeriodTo = moment(momentPeriodFrom).add(duration, durationType);
    }
    else if ($("[name='PerformanceSetting.AssesmentPeriodicity']").val() == 2) {
        durationType = 'M'
        duration = 1;
        momentPeriodTo = moment(momentPeriodFrom).add(duration, durationType).subtract(1, 'days');
        //momentPeriodTo = moment(momentPeriodTo).add(-1, 'D');
    }
    else if ($("[name='PerformanceSetting.AssesmentPeriodicity']").val() == 3) {
        durationType = 'M'
        duration = 3;
        momentPeriodTo = moment(momentPeriodFrom).add(duration, durationType).subtract(1, 'days');
    }
    else if ($("[name='PerformanceSetting.AssesmentPeriodicity']").val() == 4) {
        durationType = 'M'
        duration = 6;
        momentPeriodTo = moment(momentPeriodFrom).add(duration, durationType).subtract(1, 'days');
    }
    else if ($("[name='PerformanceSetting.AssesmentPeriodicity']").val() == 5) {
        durationType = 'Y'
        duration = 1;
        momentPeriodTo = moment(momentPeriodFrom).add(duration, durationType).subtract(1, 'days');
    }
    else {
        momentPeriodTo = moment(momentPeriodFrom).add(duration, durationType);
    }


    let periodTo = moment(momentPeriodTo).format("DD-MMM-YYYY");
    $("[name='PerformanceSetting.ReviewPeriodTo']").val(periodTo);
    UnblockUI();
}

function ValidateSkillSetMatrix() {
    let res = true;
    $("table[name='SkillSetMatrix']").find('tbody').find('tr').each(function () {
        $tr = $(this);
        let KRAWeightage = $tr.find('[name="KRAWeightage"]').text();
        KRAWeightage = isNaN(KRAWeightage) ? 0 : KRAWeightage;
        let SoftSkillsWeightage = $tr.find('[name="SoftSkillsWeightage"]').text();
        SoftSkillsWeightage = isNaN(SoftSkillsWeightage) ? 0 : SoftSkillsWeightage;

        let totalWeightage = parseFloat(KRAWeightage) + parseFloat(SoftSkillsWeightage);
        if (totalWeightage != 100) {
            $tr.find('td').css("color", "red");
            $tr.find('td').find("a").css("color", "red");
            res = false;
        }
    });

    return res;
}