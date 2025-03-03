function KRAScoreCalculator(managerRating, maxRating, weightage) {
    let res = ((managerRating / maxRating) * weightage);
    return res.toFixed(1);
}

function SubmitData(ButtonType) {

    var PerformanceEmployeeDataDTO = getPerformanceEmployeeData();
    var PerformanceEmployeeKRADatas = getPerformanceEmployeeKRAData();
    var PerformanceEmployeeTrainingDatas = getPerformanceEmployeeTrainingData();


    var inputDTO = {
        PerformanceEmployeeDataDTO: PerformanceEmployeeDataDTO,
        PerformanceEmployeeKRADatas: PerformanceEmployeeKRADatas,
        PerformanceEmployeeTrainingDatas: PerformanceEmployeeTrainingDatas,
        ButtonType: ButtonType
    };

    BlockUI();
    $.ajax({
        type: "POST",
        url: "/Performance/SavePerformanceEmployeeData",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            //$(currDiv).parent().parent().parent().remove();
            $successalert("", "Transaction Successful!");
            setTimeout(function () {
                location.reload();
            }, 2000);
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!'); UnblockUI();
            UnblockUI();
        }
    });
}


function getPerformanceEmployeeData() {
    var PerformanceEmployeeDataDTO = {};
    PerformanceEmployeeDataDTO.PerformanceEmployeeDataId = $.trim($("[name='PerformanceEmployeeData.PerformanceEmployeeDataId']").val()) == "" ? 0 : $("[name='PerformanceEmployeeData.PerformanceEmployeeDataId']").val();
    PerformanceEmployeeDataDTO.EmployeeCode = $("#EmployeeCode").val();
    PerformanceEmployeeDataDTO.EmployeeId = $("#EmployeeId").val();
    PerformanceEmployeeDataDTO.PerformanceSettingId = $("[name='PerformanceSetting.PerformanceSettingId']").val();

    PerformanceEmployeeDataDTO.KRAWeightageTotal = $.trim($("[name='tblWeightedAverageMethodKRA']").find("tfoot").find('[name="weightage"]').text()) == "" ? 0 : $("[name='tblWeightedAverageMethodKRA']").find("tfoot").find('[name="weightage"]').text();
    //PerformanceEmployeeDataDTO.KRAEmployeeRatingTotal = "";
    PerformanceEmployeeDataDTO.KRAManagersRatingTotal = $("[name='tblWeightedAverageMethodKRA']").find("tfoot").find('[name="managerrating"]').text();

    let KRAScoreTotal = $("[name='tblWeightedAverageMethodKRA']").find("tfoot").find('[name="score"]').text();
    KRAScoreTotal = $.trim(KRAScoreTotal) == "" ? 0 : KRAScoreTotal;
    PerformanceEmployeeDataDTO.KRAScoreTotal = KRAScoreTotal;


    PerformanceEmployeeDataDTO.BehaviouralSkillsWeightageTotal = $.trim($("[name='tblWeightedAverageMethodBS']").find("tfoot").find('[name="weightage"]').text()) == "" ? 0 : $("[name='tblWeightedAverageMethodBS']").find("tfoot").find('[name="weightage"]').text();
    //PerformanceEmployeeDataDTO.BehaviouralSkillsEmployeeRatingTotal = $("[name='tblWeightedAverageMethodBS']").find("tfoot").find('[name="weightage"]').text();
    PerformanceEmployeeDataDTO.BehaviouralSkillsManagersRatingTotal = $("[name='tblWeightedAverageMethodBS']").find("tfoot").find('[name="managerrating"]').text();

    let BehaviouralSkillsScoreTotal = $("[name='tblWeightedAverageMethodKRA']").find("tfoot").find('[name="score"]').text();
    BehaviouralSkillsScoreTotal = $.trim(BehaviouralSkillsScoreTotal) == "" ? 0 : BehaviouralSkillsScoreTotal;
    PerformanceEmployeeDataDTO.BehaviouralSkillsScoreTotal = BehaviouralSkillsScoreTotal;

    PerformanceEmployeeDataDTO.ClosingRemarksEmployee = $("[name='PerformanceEmployeeData.ClosingRemarksEmployee']").val() == undefined ? "" : $("[name='PerformanceEmployeeData.ClosingRemarksEmployee']").val();
    PerformanceEmployeeDataDTO.ClosingRemarksManager = $("[name='PerformanceEmployeeData.ClosingRemarksManager']").val() == undefined ? "" : $("[name='PerformanceEmployeeData.ClosingRemarksManager']").val();

    let RatingCalculationKRAWeightage = $("[name='tblRatingCalculationsAndSummary']").find("tbody").find('[name="WeightageKRA"]').text();
    let RatingCalculationKRAScore = $("[name='tblRatingCalculationsAndSummary']").find("tbody").find('[name="KRA"]').text();

    PerformanceEmployeeDataDTO.RatingCalculationKRAWeightage = isNaN(RatingCalculationKRAWeightage) ? 0 : RatingCalculationKRAWeightage;
    PerformanceEmployeeDataDTO.RatingCalculationKRAScore = RatingCalculationKRAScore;
    PerformanceEmployeeDataDTO.RatingCalculationKRAFinalScore = $("[name='tblRatingCalculationsAndSummary']").find("tbody").find('[name="TotalKRA"]').text();

    PerformanceEmployeeDataDTO.RatingCalculationBehaviouralSkillsWeightage = $("[name='tblRatingCalculationsAndSummary']").find("tbody").find('[name="WeightageBS"]').text();
    PerformanceEmployeeDataDTO.RatingCalculationBehaviouralSkillsScore = $("[name='tblRatingCalculationsAndSummary']").find("tbody").find('[name="BehavioralSkills"]').text();
    PerformanceEmployeeDataDTO.RatingCalculationBehaviouralSkillsFinalScore = $("[name='tblRatingCalculationsAndSummary']").find("tbody").find('[name="TotalBS"]').text();

    PerformanceEmployeeDataDTO.RatingCalculationFinalScore = $("[name='tblRatingCalculationsAndSummary']").find("tfoot").find('[name="TotalKRABSFinalScore"]').text();
    PerformanceEmployeeDataDTO.RatingCalculationFinalRating = $("[name='tblRatingCalculationsAndSummary']").find("tfoot").find('[name="finalrating"]').text();
    PerformanceEmployeeDataDTO.RatingCalculationFinalRatingId = $("[name='tblRatingCalculationsAndSummary']").find("tfoot").find('[name="finalrating"]').attr("point");

    PerformanceEmployeeDataDTO.HODFinalRating = $("[name='PerformanceEmployeeData.HODFinalRatingId'] option:selected").text();
    PerformanceEmployeeDataDTO.HODFinalRatingId = $("[name='PerformanceEmployeeData.HODFinalRatingId']").val();
    PerformanceEmployeeDataDTO.ClosingRemarksHOD = $("[name='PerformanceEmployeeData.ClosingRemarksHOD']").val();
    PerformanceEmployeeDataDTO.ViewType = $("[name='viewtype']").val();



    return PerformanceEmployeeDataDTO;
}

function getPerformanceEmployeeKRAData() {
    let PerformanceEmployeeKRADatas = [];

    $("[name='tblWeightedAverageMethodKRA']").find("tbody").find("tr").each((i, v) => {
        let PerformanceEmployeeKRADataDTO = {};
        PerformanceEmployeeKRADataDTO.SNo = $(v).find("[name='sn']").text();
        PerformanceEmployeeKRADataDTO.KRA = $(v).find("[name='attribute']").text();
        PerformanceEmployeeKRADataDTO.Weightage = $.trim($(v).find("[name='weightage']").text()) == "" ? 0 : $(v).find("[name='weightage']").text();
        PerformanceEmployeeKRADataDTO.EmployeeRating = $(v).find("[name='employeerating']").find("select").val();
        PerformanceEmployeeKRADataDTO.EmployeeRemarks = $(v).find("[name='employeeremark']").text();
        PerformanceEmployeeKRADataDTO.ManagerRating = $(v).find("[name='managerrating']").find("select").val();
        PerformanceEmployeeKRADataDTO.ManagerRemarks = $(v).find("[name='managerremark']").text();
        PerformanceEmployeeKRADataDTO.WAScore = $.trim($(v).find("[name='score']").text()) == "" ? 0 : $.trim($(v).find("[name='score']").text());
        PerformanceEmployeeKRADataDTO.Source = "KRA";
        PerformanceEmployeeKRADatas.push(PerformanceEmployeeKRADataDTO);
    });
    $("[name='tblWeightedAverageMethodBS']").find("tbody").find("tr").each((i, v) => {
        let PerformanceEmployeeKRADataDTO = {};
        PerformanceEmployeeKRADataDTO.SNo = $(v).find("[name='sn']").text();
        PerformanceEmployeeKRADataDTO.KRA = $(v).find("[name='attribute']").text();
        PerformanceEmployeeKRADataDTO.Weightage = $.trim($(v).find("[name='weightage']").text()) == "" ? 0 : $(v).find("[name='weightage']").text();
        PerformanceEmployeeKRADataDTO.EmployeeRating = $(v).find("[name='employeerating']").find("select").val();
        PerformanceEmployeeKRADataDTO.EmployeeRemarks = $(v).find("[name='employeeremark']").text();
        PerformanceEmployeeKRADataDTO.ManagerRating = $(v).find("[name='managerrating']").find("select").val();
        PerformanceEmployeeKRADataDTO.ManagerRemarks = $(v).find("[name='managerremark']").text();
        PerformanceEmployeeKRADataDTO.WAScore = $.trim($(v).find("[name='score']").text()) == "" ? 0 : $.trim($(v).find("[name='score']").text());
        PerformanceEmployeeKRADataDTO.Source = "Behavioral";
        PerformanceEmployeeKRADatas.push(PerformanceEmployeeKRADataDTO);
    });
    return PerformanceEmployeeKRADatas;
}
function getPerformanceEmployeeTrainingData() {
    let PerformanceEmployeeTrainingDatas = [];

    $("[name='tblTrainingNeeds']").find("tbody").find("tr").each((i, v) => {
        let PerformanceEmployeeTrainingDataDTO = {};
        PerformanceEmployeeTrainingDataDTO.TrainingNeedsMasterId = $(v).attr("trainingneedsmasterid");
        PerformanceEmployeeTrainingDataDTO.TrainingType = $(v).find("[name='TrainingType']").find("select").val();
        PerformanceEmployeeTrainingDataDTO.TrainingUrgency = $(v).find("[name='TrainingUrgency']").find("select").val();
        PerformanceEmployeeTrainingDatas.push(PerformanceEmployeeTrainingDataDTO);
    });

    return PerformanceEmployeeTrainingDatas;
}


function KRAPartialView() {

    var inputDTO = {};
    inputDTO.PerformanceSettingId = $("[name='PerformanceSettingId']").val();;
    inputDTO.Source = $("[name='Source']").val();;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Performance/MISView_ListPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_ViewPartial').html(data);
            $(".List").dataTable({
                "order": []
            });
            UnblockUI();
        },
        error: function (result) {
            //alert(result);
            UnblockUI();
        }
    });
}