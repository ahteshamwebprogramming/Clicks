

function addHouseRentDetailDiv() {

    let FY = $("#FY").val();
    let Regime = $("#Regime").val();

    var itDeclarationHouseRentDetailDTO = {};
    itDeclarationHouseRentDetailDTO.Regime = Regime;
    itDeclarationHouseRentDetailDTO.FY = FY;


    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/PayrollAccount/GetHouseRentDetailsPartialView',
        data: JSON.stringify(itDeclarationHouseRentDetailDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            let divCount = $("#particulars").find("div[name='RentedPremisesDetailForm']").find("div[name='HouseRentDetailsFormCollection']").find('div[name^=HouseRentDetailsForm]').length;
            let divName = (parseInt(divCount) + 1);

            $("#particulars").find("div[name='RentedPremisesDetailForm']").find("div[name='HouseRentDetailsFormCollection']").append('<div name="HouseRentDetailsForm' + divName + '" style="padding:15px;border:1px solid #f9e8e8;margin-top:5px;"><div>' + data + '</div></div>');

            $("#particulars").find("div[name='RentedPremisesDetailForm']").find("div[name='HouseRentDetailsFormCollection']").find("select").select2();
            $("#particulars").find("div[name='RentedPremisesDetailForm']").find("div[name='HouseRentDetailsFormCollection']").find("select").addClass("select2");
            UnblockUI();
        },
        error: function (result) {
            alert(result.responseText);
            UnblockUI();
        }
    });



    return;

    let divCount = $("#particulars").find("div[name='RentedPremisesDetailForm']").find("div[name='HouseRentDetailsFormCollection']").find('div[name^=HouseRentDetailsForm]').length;
    let divName = (parseInt(divCount) + 1);
    console.log(divName);
    $("#particulars").find("div[name='RentedPremisesDetailForm']").find("div[name='HouseRentDetailsFormCollection']").append('<div name="HouseRentDetailsForm' + divName + '" style="padding: 15px;border:1px solid #f9e8e8;margin-top:5px;"><div><div class="float-start"><h6>House Rent Details</h6></div><div class="float-end"><i class="fa-solid fa-xmark" onclick="removeHouseRentDetailDiv(this)"></i></div><div class="clearfix"></div><input type="hidden" value="0" name="ItDeclarationHouseRentDetailsId" /></div><div class="row"><div class="col-md-4 mb-3"><label class="form-label" for="username">Period From</label><div class= "input-group"><select class="form-select" name="MonthFrom"><option selected="" value="">--Select Month--</option><option value="1">January</option><option value="2">February</option><option value="3">March</option><option value="4">April</option><option value="5">May</option><option value="6">June</option><option value="7">July</option><option value="8">August</option><option value="9">September</option><option value="10">October</option><option value="11">November</option><option value="12">December</option></select><select class="form-select" name="YearFrom" ><option selected="" value="">-Select Year--</option><option value="2020">2020</option><option value="2021">2021</option>     <option value="2022">2022</option><option value="2023">2023</option><option value="2024">2024</option><option value="2025">2025</option><option value="2026">2026</option><option value="2027">2027</option><option value="2028">2028</option></select></div></div><div class="col-md-4 mb-3"><label class="form-label" for="username">Period To</label><div class="input-group"><select class="form-select" name="MonthTo"><option selected="" value="">--Select Month--</option><option value="1">January</option><option value="2">February</option><option value="3">March</option><option value="4">April</option><option value="5">May</option><option value="6">June</option><option value="7">July</option><option value="8">August</option><option value="9">September</option><option value="10">October</option><option value="11">November</option><option value="12">December</option></select><select class="form-select" name="YearTo"><option selected="" value="">-Select Year--</option><option value="2020">2020</option><option value="2021">2021</option><option value="2022">2022</option><option value="2023">2023</option><option value="2024">2024</option><option value="2025">2025</option><option value="2026">2026</option><option value="2027">2027</option><option value="2028">2028</option></select></div></div><div class="col-md-4 mb-3"><div class="col-md-12 mb-3"><label class="form-label" for="username">Amount /Month.</label><div class="input-group"><span class="input-group-text" id="basic-addon11">₹</span><input type="number" id="Amount" name="Amount" class="form-control mr-2" placeholder="" aria-label="" aria-describedby=""></div></div></div><div class="col-md-9 mb-1"><div class="mb-3"><label class="form-label" for="username">House Address</label><textarea class="form-control" id="HouseAddress" name="HouseAddress" rows="2"></textarea></div></div><div class="col-md-3 mb-1"><div class="row"><label class="form-label" for="username">Staying In</label><div class="col-md-12 mb-3"><select id="StayingIn" name="StayingIn" class="form-select" aria-label=""><option selected>Metro</option><option value="">Non Metro</option></select></div></div></div><div class="row"><div class="col-md-6 mb-3"><label class="form-label" for="username">Landloard Name</label><input type="text" id="LandlordName" name="LandlordName" class="form-control" /></div><div class="col-md-6 mb-3"><div class="col-md-12 mb-3"><label class="form-label" for="username">Landloard PanCard</label><div class="input-group"><input type="text" class="form-control mr-2" id="LandlordPancard" name="LandlordPancard" placeholder="" aria-label="" aria-describedby=""></div></div></div></div></div></div>');
    $("#particulars").find("div[name='RentedPremisesDetailForm']").find("div[name='HouseRentDetailsFormCollection']").find("select").select2();
    $("#particulars").find("div[name='RentedPremisesDetailForm']").find("div[name='HouseRentDetailsFormCollection']").find("select").addClass("select2");
}
function removeHouseRentDetailDiv(currDiv, ItDeclarationHouseRentDetailsId) {

    if (ItDeclarationHouseRentDetailsId == 0) {
        $(currDiv).parent().parent().parent().remove();

    }
    else {
        Swal.fire({ title: 'Are you sure?', text: "This will get deleted permanently!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, delete it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
            if (result.value) {
                BlockUI();
                var inputDTO = {
                    "ItDeclarationHouseRentDetailsId": ItDeclarationHouseRentDetailsId
                };
                $.ajax({
                    type: "POST",
                    url: "/PayrollAccount/DeleteHouseRentDetail",
                    contentType: 'application/json',
                    data: JSON.stringify(inputDTO),
                    success: function (data) {
                        $(currDiv).parent().parent().parent().remove();
                        $successalert("Deleted!", "Record has been deleted.");
                        UnblockUI();
                    },
                    error: function (error) {
                        $erroralert("Error!", error.responseText + '!'); UnblockUI();
                        UnblockUI();
                    }
                });
            }
        });
    }
}
function addLetOutPropertyDetailDiv() {
    $("#particulars").find("div[name='LetOutPropertyDetailForm']").find("div[name='LetOutPropertyDetailCollection']").append('<div name="LetOutPropertyDetailForm1" style="padding: 15px;border: 1px solid #f9e8e8;margin-top:5px;"><div><div class="float-start"><h6>Let Out Property Details</h6></div><div class="float-end"><i class="fa-solid fa-xmark" onclick="removeLetOutPropertyDetailDiv(this)"></i></div><div class="clearfix"></div></div><div class="row"><div class="col-md-9 mb-3"><label class="form-label" for="username">Annual Rent Received</label></div><div class="col-md-3 mb-3"><div class="input-group"><span class="input-group-text" id="basic-addon11">₹</span><input type="text" name="AnnualRentReceived" class="form-control mr-2" placeholder="" aria-label="" aria-describedby=""></div></div><div class="col-md-9 mb-3"><label class="form-label" for="username">Taxes Paid</label></div><div class="col-md-3 mb-3"><div class="input-group"><span class="input-group-text" id="basic-addon11">₹</span><input type="text" name="TaxesPaid" class="form-control mr-2" placeholder="" aria-label="" aria-describedby=""></div></div><div class="col-md-9 mb-3"><label class="form-label" for="username">Net Annual Value</label></div><div class="col-md-3 mb-3"><div class="input-group"><span class="input-group-text" id="basic-addon11">₹</span><input type="text" name="NetAnnualValue" class="form-control mr-2" placeholder="" aria-label="" aria-describedby="" disabled></div></div><div class="col-md-9 mb-3"><label class="form-label" for="username">Standard Deduction ( 30% of Net Annual Value)</label></div><div class="col-md-3 mb-3"><div class="input-group"><span class="input-group-text" id="basic-addon11">₹</span><input type="text" name="StandardDeduction" class="form-control mr-2" placeholder="" aria-label="" aria-describedby="" disabled></div></div><div class="col-md-9 mb-3"><label class="form-label" for="username">Net Income/Loss from Property</label></div><div class="col-md-3 mb-3"><div class="input-group"><span class="input-group-text" id="basic-addon11">₹</span><input type="text" name="NetIncomeLoss" class="form-control mr-2" placeholder="" aria-label="" aria-describedby="" disabled></div></div></div></div>');
}
function removeLetOutPropertyDetailDiv(currDiv, ItDeclarationLentOutPropertyDetailsId) {

    if (ItDeclarationLentOutPropertyDetailsId == 0) {
        $(currDiv).parent().parent().parent().remove();
    }
    else {
        Swal.fire({ title: 'Are you sure?', text: "This will get deleted permanently!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, delete it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
            if (result.value) {
                BlockUI();
                var inputDTO = {
                    "ItDeclarationLentOutPropertyDetailsId": ItDeclarationLentOutPropertyDetailsId
                };
                $.ajax({
                    type: "POST",
                    url: "/PayrollAccount/DeleteLentOutPropertyDetail",
                    contentType: 'application/json',
                    data: JSON.stringify(inputDTO),
                    success: function (data) {
                        $(currDiv).parent().parent().parent().remove();
                        $successalert("Deleted!", "Record has been deleted.");
                        UnblockUI();
                    },
                    error: function (error) {
                        $erroralert("Error!", error.responseText + '!'); UnblockUI();
                        UnblockUI();
                    }
                });
            }
        });
    }
}
function add80CInvestmentsForm() {
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/PayrollAccount/GetInvestments80CPartialView',
        //data: JSON.stringify({ ProjectId: project, ReportType: reportType }),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            let divCount = $('[name="Accordian80CInvestments"]').find("[name='80CInvestmentsFormCollection']").find('div[name^=80CInvestmentsForm]').length;
            let divName = (parseInt(divCount) + 1);
            $('[name="Accordian80CInvestments"]').find("[name='80CInvestmentsFormCollection']").append('<div class="row" name="80CInvestmentsForm' + divName + '">' + data + '</div>');
            $('[name="Accordian80CInvestments"]').find("[name='80CInvestmentsFormCollection']").find('div[name="80CInvestmentsForm' + divName + '"]').find("select").select2();
            $('[name="Accordian80CInvestments"]').find("[name='80CInvestmentsFormCollection']").find('div[name="80CInvestmentsForm' + divName + '"]').find("select").addClass("select2");
            UnblockUI();
        },
        error: function (result) {
            alert(result.responseText);
            UnblockUI();
        }
    });
    return;

    $("[name='Accordian80CInvestments']").find("div[name='80CInvestmentsFormCollection']").append('<div class="row" name="80CInvestmentsForm1"><div class="col-md-8 mb-3"><select class="form-select" name="InvestmentType" data-style="btn-default"><option selected>Select an Investment</option><optgroup label="80C" style="font-weight:800"><option>Life Insurance Premium</option><option>Public Provident Fund</option><option>Unit-Linked Insurance Plan</option><option>National Savings Certificates</option><option>ELSS Tax Saving Mutual Fund</option><option>Children Tution Fees</option><option>Sukanya Samriddhi Deposit Scheme</option><option>Employees Provident Fund (EPF)</option><option>Tax Saver Fixed Deposits</option><option>National Pension Scheme (NPS)</option><option>Senior Citizens Savings Scheme</option><option>5 Year fixed deposit in Scheduled Banks</option><option>Term Deposit in Post Office</option><option>Infrastructure Bonds</option><option>Stamp Duty and Registration Fee on Buying House Property</option><option>Interest on National Savings Certificates</option><option>NABARD Rural Bonds</option></optgroup><optgroup label="80CCC"><option>Contribution to annuity plan of LIC</option></optgroup><optgroup label="80CCD(1)"><option>National Pension Scheme</option></optgroup></select></div><div class="col-md-3 mb-3"><div class="input-group"><span class="input-group-text" id="basic-addon11">₹</span><input type="text" name="Value" class="form-control mr-2" placeholder="" aria-label="" aria-describedby=""></div></div><div class="col-md-1 mb-3"><div class="float-end"><i class="fa-solid fa-xmark" onclick="remove80CInvestmentsForm(this)"></i></div><div class="clearfix"></div></div></div>');
}
function remove80CInvestmentsForm(currDiv, ItDeclaration80CinvestmentsId) {

    if (ItDeclaration80CinvestmentsId == 0) {
        $(currDiv).parent().parent().parent().remove();
    }
    else {
        Swal.fire({ title: 'Are you sure?', text: "This will get deleted permanently!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, delete it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
            if (result.value) {
                BlockUI();
                var inputDTO = {
                    "ItDeclaration80CinvestmentsId": ItDeclaration80CinvestmentsId
                };
                $.ajax({
                    type: "POST",
                    url: "/PayrollAccount/DeleteInvestments80C",
                    contentType: 'application/json',
                    data: JSON.stringify(inputDTO),
                    success: function (data) {
                        $(currDiv).parent().parent().parent().remove();
                        $successalert("Deleted!", "Record has been deleted.");
                        UnblockUI();
                    },
                    error: function (error) {
                        $erroralert("Error!", error.responseText + '!'); UnblockUI();
                        UnblockUI();
                    }
                });
            }
        });
    }
}
function add80DInvestmentsForm() {
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/PayrollAccount/GetExemptions80DPartialView',
        //data: JSON.stringify({ ProjectId: project, ReportType: reportType }),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            let divCount = $('[name="Accordian80DInvestments"]').find("[name='80DInvestmentsFormCollection']").find('div[name^=80DInvestmentsForm]').length;
            let divName = (parseInt(divCount) + 1);
            $('[name="Accordian80DInvestments"]').find("[name='80DInvestmentsFormCollection']").append('<div class="row" name="80DInvestmentsForm' + divName + '">' + data + '</div>');
            $('[name="Accordian80DInvestments"]').find("[name='80DInvestmentsFormCollection']").find('div[name="80DInvestmentsForm' + divName + '"]').find("select").select2();
            $('[name="Accordian80DInvestments"]').find("[name='80DInvestmentsFormCollection']").find('div[name="80DInvestmentsForm' + divName + '"]').find("select").addClass("select2");
            UnblockUI();
        },
        error: function (result) {
            alert(result.responseText);
            UnblockUI();
        }
    });
    return;
    $("[name='Accordian80DInvestments']").find("div[name='80DInvestmentsFormCollection']").append('<div class="row" name="80DInvestmentsForm1"><div class="col-md-8 mb-3"><select class="form-select" name="ExemptionType" data-style="btn-default"><option selected>Select an Investment</option><optgroup label="80D" style="font-weight:800"><option>Medi Claim Policy For Self, Spouse, Children - 80D</option><option>Medi Claim Policy For Self, Spouse, Children for Senior Citizen - 80D</option><option>Medi Claim Policy For Parent - 80D</option><option>Medi Claim Policy For Parent for senior Citizen - 80D</option><option>Preventive health checkup - 80D</option><option>Preventive health checkup for parent - 80D</option><option>Medical Bills For Self, Spouse, Children for Senior Citizen - 80D</option><option>Medical Bills For Parent for Senior Citizen - 80D</option></optgroup></select></div><div class="col-md-3 mb-3"><div class="input-group"><span class="input-group-text" id="basic-addon11">₹</span><input type="text"  name="Value" class="form-control mr-2" placeholder="" aria-label="" aria-describedby=""></div></div><div class="col-md-1 mb-3"><div class="float-end"><i class="fa-solid fa-xmark" onclick="remove80DInvestmentsForm(this)"></i></div><div class="clearfix"></div></div></div>');
}
function remove80DInvestmentsForm(currDiv, ItDeclaration80DexemptionsId) {
    if (ItDeclaration80DexemptionsId == 0) {
        $(currDiv).parent().parent().parent().remove();
    }
    else {
        Swal.fire({ title: 'Are you sure?', text: "This will get deleted permanently!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, delete it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
            if (result.value) {
                BlockUI();
                var inputDTO = {
                    "ItDeclaration80DexemptionsId": ItDeclaration80DexemptionsId
                };
                $.ajax({
                    type: "POST",
                    url: "/PayrollAccount/DeleteExemptions80D",
                    contentType: 'application/json',
                    data: JSON.stringify(inputDTO),
                    success: function (data) {
                        $(currDiv).parent().parent().parent().remove();
                        $successalert("Deleted!", "Record has been deleted.");
                        UnblockUI();
                    },
                    error: function (error) {
                        $erroralert("Error!", error.responseText + '!'); UnblockUI();
                        UnblockUI();
                    }
                });
            }
        });
    }
}
function addOtherInvestmentsForm() {

    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/PayrollAccount/GetOtherInvestmentsExemptionsPartialView',
        //data: JSON.stringify({ ProjectId: project, ReportType: reportType }),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            let divCount = $('[name="AccordianOtherInvestments"]').find("[name='OtherInvestmentsFormCollection']").find('div[name^=OtherInvestmentsForm]').length;
            let divName = (parseInt(divCount) + 1);
            $('[name="AccordianOtherInvestments"]').find("[name='OtherInvestmentsFormCollection']").append('<div class="row" name="OtherInvestmentsForm' + divName + '">' + data + '</div>');
            $('[name="AccordianOtherInvestments"]').find("[name='OtherInvestmentsFormCollection']").find('div[name="OtherInvestmentsForm' + divName + '"]').find("select").select2();
            $('[name="AccordianOtherInvestments"]').find("[name='OtherInvestmentsFormCollection']").find('div[name="OtherInvestmentsForm' + divName + '"]').find("select").addClass("select2");
            UnblockUI();
        },
        error: function (result) {
            alert(result.responseText);
            UnblockUI();
        }
    });
    return;

    $("[name='AccordianOtherInvestments']").find("div[name='OtherInvestmentsFormCollection']").append('<div class="row" name="OtherInvestmentsForm1"><div class="col-md-8 mb-3"><select class="form-select"  name="OtherInvestmentExemptionId" data-style="btn-default"><option selected>Select an Investment</option><optgroup label="80CCD(1B)" style="font-weight:800"><option>Additional Excemption on Voluntary NPS</option></optgroup><optgroup label="80DD" style="font-weight:800"><option>Treatment of Dependent with Disablitiy</option><option>Treatment of Dependent with Severe Disablitiy</option></optgroup><optgroup label="80DDB" style="font-weight:800"><option>Medical Expenditure for Self or Dependent</option><option>Medical Expenditure for Self or Dependent for Senior Citizen</option><option>Medical Expenditure for Self or Dependent for Very Senior Citizen</option></optgroup><optgroup label="80E" style="font-weight:800">                <option>Interest Paid On Education Loan</option></optgroup><optgroup label="80EEA" style="font-weight:800"><option>Additional Interest on Housing Loan borrowed between 01-April-2019 to 31-March-2022</option></optgroup><optgroup label="80EEB" style="font-weight:800"><option>Interest on Elcetric Vehicle Loan borrowed between 01-April-2019 to 31-March-2023</option></optgroup><optgroup label="80G" style="font-weight:800"><option>Donation eligible for 100% Exemption</option><option>Donation eligible for 50% Exemption</option></optgroup><optgroup label="80TTA" style="font-weight:800"><option>Interest From Savings Account</option></optgroup><optgroup label="80U" style="font-weight:800"><option>Permanent Physical Disability(Self)</option><option>Permanent Physical Severe Disability(Self)</option></optgroup></select></div><div class="col-md-3 mb-3"><div class="input-group"><span class="input-group-text" id="basic-addon11">₹</span><input type="text"  name="Value" class="form-control mr-2" placeholder="" aria-label="" aria-describedby=""></div></div><div class="col-md-1 mb-3"><div class="float-end"><i class="fa-solid fa-xmark" onclick="removeOtherInvestmentsForm(this)"></i></div><div class="clearfix"></div></div></div>');
}
function removeOtherInvestmentsForm(currDiv, ItDeclarationOtherInvestmentExemptionId) {
    if (ItDeclarationOtherInvestmentExemptionId == 0) {
        $(currDiv).parent().parent().parent().remove();
    }
    else {
        Swal.fire({ title: 'Are you sure?', text: "This will get deleted permanently!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, delete it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
            if (result.value) {
                BlockUI();
                var inputDTO = {
                    "ItDeclarationOtherInvestmentExemptionId": ItDeclarationOtherInvestmentExemptionId
                };
                $.ajax({
                    type: "POST",
                    url: "/PayrollAccount/DeleteOtherInvestmentsExemptions",
                    contentType: 'application/json',
                    data: JSON.stringify(inputDTO),
                    success: function (data) {
                        $(currDiv).parent().parent().parent().remove();
                        $successalert("Deleted!", "Record has been deleted.");
                        UnblockUI();
                    },
                    error: function (error) {
                        $erroralert("Error!", error.responseText + '!'); UnblockUI();
                        UnblockUI();
                    }
                });
            }
        });
    }
}

function test() {
}

function SaveITDeclarations() {






    BlockUI();

    var ItDeclarationHouseRentDetailList = [];
    var ItDeclarationHomeLoanDetailDTO = {};
    var ItDeclarationLentOutPropertyDetailList = [];
    var ItDeclarationOtherSourceOfIncomeDTO = {};
    var ItDeclaration80CinvestmentList = [];
    var ItDeclaration80DexemptionList = [];
    var ItDeclarationOtherInvestmentExemptionList = [];
    var ItDeclarationPreviousEmployementDTO = {};
    //House Rent
    if ($("#particulars").find("[name='IsStayingInRentedPremises']").is(":checked")) {
        let divHouseRentDetails = $("#particulars").find("div[name='RentedPremisesDetailForm']").find("div[name='HouseRentDetailsFormCollection']").find('div[name^=HouseRentDetailsForm]');
        divHouseRentDetails.each((i, v) => {

            if (!validateHouseRentDetails(v)) {
                $erroralert("Validation Error", "There are some validation errors in Housing Rent Details Section");
                UnblockUI();
                return false;
            }
            else if (!ValidateFYDates(v)) {
                //alert("Date Validation failed");
                UnblockUI();
                return false;
            }
            //else {
            //    alert("success");
            //    UnblockUI();
            //}

            var ItDeclarationHouseRentDetailDTO = {};

            let ItDeclarationHouseRentDetailsId = $(v).find("[name='ItDeclarationHouseRentDetailsId']").val();
            ItDeclarationHouseRentDetailsId = $.trim(ItDeclarationHouseRentDetailsId) == "" ? 0 : isNaN($.trim(ItDeclarationHouseRentDetailsId)) ? 0 : $.trim(ItDeclarationHouseRentDetailsId);
            ItDeclarationHouseRentDetailDTO.ItDeclarationHouseRentDetailsId = ItDeclarationHouseRentDetailsId
            //ItDeclarationHouseRentDetailDTO.ItDeclarationHouseRentDetailsId = $(v).find("[name='ItDeclarationHouseRentDetailsId']").val();
            ItDeclarationHouseRentDetailDTO.MonthFrom = $(v).find("[name='MonthFrom']").val();
            ItDeclarationHouseRentDetailDTO.YearFrom = $(v).find("[name='YearFrom']").val();
            ItDeclarationHouseRentDetailDTO.MonthTo = $(v).find("[name='MonthTo']").val();
            ItDeclarationHouseRentDetailDTO.YearTo = $(v).find("[name='YearTo']").val();
            ItDeclarationHouseRentDetailDTO.Amount = $.trim($(v).find("[name='Amount']").val()) == "" ? null : isNaN($.trim($(v).find("[name='Amount']").val())) ? null : $.trim($(v).find("[name='Amount']").val());
            ItDeclarationHouseRentDetailDTO.HouseAddress = $(v).find("[name='HouseAddress']").val();
            ItDeclarationHouseRentDetailDTO.StayingIn = $(v).find("[name='StayingIn']").val();
            ItDeclarationHouseRentDetailDTO.LandlordName = $(v).find("[name='LandlordName']").val();
            ItDeclarationHouseRentDetailDTO.LandlordPancard = $(v).find("[name='LandlordPancard']").val();
            ItDeclarationHouseRentDetailList.push(ItDeclarationHouseRentDetailDTO);
        });
    }
    //Home Loan
    let divHomeLoanDetails = $("#particulars").find("div[name='EmployeeRepayingHomeLoneDetailForm']");

    let ItDeclarationHomeLoanDetailsId = $(divHomeLoanDetails).find("[name='ItDeclarationHomeLoanDetailsId']").val();
    ItDeclarationHomeLoanDetailsId = $.trim(ItDeclarationHomeLoanDetailsId) == "" ? 0 : isNaN($.trim(ItDeclarationHomeLoanDetailsId)) ? 0 : $.trim(ItDeclarationHomeLoanDetailsId);
    ItDeclarationHomeLoanDetailDTO.ItDeclarationHomeLoanDetailsId = ItDeclarationHomeLoanDetailsId;

    let PrincipalPaidOnHomeLoan = $(divHomeLoanDetails).find("[name='PrincipalPaidOnHomeLoan']").val();
    PrincipalPaidOnHomeLoan = $.trim(PrincipalPaidOnHomeLoan) == "" ? null : isNaN($.trim(PrincipalPaidOnHomeLoan)) ? null : $.trim(PrincipalPaidOnHomeLoan);
    ItDeclarationHomeLoanDetailDTO.PrincipalPaidOnHomeLoan = PrincipalPaidOnHomeLoan;

    let InterestPaidOnHomeLoan = $(divHomeLoanDetails).find("[name='InterestPaidOnHomeLoan']").val();
    InterestPaidOnHomeLoan = $.trim(InterestPaidOnHomeLoan) == "" ? null : isNaN($.trim(InterestPaidOnHomeLoan)) ? null : $.trim(InterestPaidOnHomeLoan);
    ItDeclarationHomeLoanDetailDTO.InterestPaidOnHomeLoan = InterestPaidOnHomeLoan;

    ItDeclarationHomeLoanDetailDTO.NameOfTheLender = $(divHomeLoanDetails).find("[name='NameOfTheLender']").val();
    ItDeclarationHomeLoanDetailDTO.LenderPancard = $(divHomeLoanDetails).find("[name='LenderPancard']").val();

    //Lent Out Property
    let divLentOutPropertyDetails = $("#particulars").find("div[name='LetOutPropertyDetailForm']").find("div[name='LetOutPropertyDetailCollection']").find('div[name^=LetOutPropertyDetailForm]');
    divLentOutPropertyDetails.each((i, v) => {
        var ItDeclarationLentOutPropertyDetailDTO = {};

        let ItDeclarationLentOutPropertyDetailsId = $(v).find("[name='ItDeclarationLentOutPropertyDetailsId']").val();
        ItDeclarationLentOutPropertyDetailsId = $.trim(ItDeclarationLentOutPropertyDetailsId) == "" ? 0 : isNaN($.trim(ItDeclarationLentOutPropertyDetailsId)) ? 0 : $.trim(ItDeclarationLentOutPropertyDetailsId);
        ItDeclarationLentOutPropertyDetailDTO.ItDeclarationLentOutPropertyDetailsId = ItDeclarationLentOutPropertyDetailsId

        let AnnualRentReceived = $(v).find("[name='AnnualRentReceived']").val();
        AnnualRentReceived = $.trim(AnnualRentReceived) == "" ? null : isNaN($.trim(AnnualRentReceived)) ? null : $.trim(AnnualRentReceived);
        ItDeclarationLentOutPropertyDetailDTO.AnnualRentReceived = AnnualRentReceived

        let TaxesPaid = $(v).find("[name='TaxesPaid']").val();
        TaxesPaid = $.trim(TaxesPaid) == "" ? null : isNaN($.trim(TaxesPaid)) ? null : $.trim(TaxesPaid);
        ItDeclarationLentOutPropertyDetailDTO.TaxesPaid = TaxesPaid

        let NetAnnualValue = $(v).find("[name='NetAnnualValue']").val();
        NetAnnualValue = $.trim(NetAnnualValue) == "" ? null : isNaN($.trim(NetAnnualValue)) ? null : $.trim(NetAnnualValue);
        ItDeclarationLentOutPropertyDetailDTO.NetAnnualValue = NetAnnualValue

        let StandardDeduction = $(v).find("[name='StandardDeduction']").val();
        StandardDeduction = $.trim(StandardDeduction) == "" ? null : isNaN($.trim(StandardDeduction)) ? null : $.trim(StandardDeduction);
        ItDeclarationLentOutPropertyDetailDTO.StandardDeduction = StandardDeduction

        let NetIncomeLoss = $(v).find("[name='NetIncomeLoss']").val();
        NetIncomeLoss = $.trim(NetIncomeLoss) == "" ? null : isNaN($.trim(NetIncomeLoss)) ? null : $.trim(NetIncomeLoss);
        ItDeclarationLentOutPropertyDetailDTO.NetIncomeLoss = NetIncomeLoss

        ItDeclarationLentOutPropertyDetailList.push(ItDeclarationLentOutPropertyDetailDTO);
    });

    //Other Source Of Income
    let ItDeclarationOtherSourceOfIncomeId = $("[name='ItDeclarationOtherSourceOfIncomeDTO']").find("[name='ItDeclarationOtherSourceOfIncomeId']").val();
    ItDeclarationOtherSourceOfIncomeId = $.trim(ItDeclarationOtherSourceOfIncomeId) == "" ? 0 : isNaN($.trim(ItDeclarationOtherSourceOfIncomeId)) ? 0 : $.trim(ItDeclarationOtherSourceOfIncomeId);
    ItDeclarationOtherSourceOfIncomeDTO.ItDeclarationOtherSourceOfIncomeId = ItDeclarationOtherSourceOfIncomeId;

    let IncomeFromOtherSource = $("[name='ItDeclarationOtherSourceOfIncomeDTO']").find("[name='IncomeFromOtherSource']").val();
    IncomeFromOtherSource = $.trim(IncomeFromOtherSource) == "" ? null : isNaN($.trim(IncomeFromOtherSource)) ? null : $.trim(IncomeFromOtherSource);
    ItDeclarationOtherSourceOfIncomeDTO.IncomeFromOtherSource = IncomeFromOtherSource;

    let InterestEarnedFromSavingsDeposit = $("[name='ItDeclarationOtherSourceOfIncomeDTO']").find("[name='InterestEarnedFromSavingsDeposit']").val();
    InterestEarnedFromSavingsDeposit = $.trim(InterestEarnedFromSavingsDeposit) == "" ? null : isNaN($.trim(InterestEarnedFromSavingsDeposit)) ? null : $.trim(InterestEarnedFromSavingsDeposit);
    ItDeclarationOtherSourceOfIncomeDTO.InterestEarnedFromSavingsDeposit = InterestEarnedFromSavingsDeposit;

    let InterestEarnedFromFixedDeposit = $("[name='ItDeclarationOtherSourceOfIncomeDTO']").find("[name='InterestEarnedFromFixedDeposit']").val();
    InterestEarnedFromFixedDeposit = $.trim(InterestEarnedFromFixedDeposit) == "" ? null : isNaN($.trim(InterestEarnedFromFixedDeposit)) ? null : $.trim(InterestEarnedFromFixedDeposit);
    ItDeclarationOtherSourceOfIncomeDTO.InterestEarnedFromFixedDeposit = InterestEarnedFromFixedDeposit;

    let InterestEarnedFromNationalSavingsCertificates = $("[name='ItDeclarationOtherSourceOfIncomeDTO']").find("[name='InterestEarnedFromNationalSavingsCertificates']").val();
    InterestEarnedFromNationalSavingsCertificates = $.trim(InterestEarnedFromNationalSavingsCertificates) == "" ? null : isNaN($.trim(InterestEarnedFromNationalSavingsCertificates)) ? null : $.trim(InterestEarnedFromNationalSavingsCertificates);
    ItDeclarationOtherSourceOfIncomeDTO.InterestEarnedFromNationalSavingsCertificates = InterestEarnedFromNationalSavingsCertificates;


    //80C Investment
    let div80CInvestmentDetails = $("div[name='Accordian80CInvestments']").find("div[name='80CInvestmentsFormCollection']").find('div[name^=80CInvestmentsForm]');
    div80CInvestmentDetails.each((i, v) => {
        var ItDeclaration80CinvestmentDTO = {};

        let ItDeclaration80CinvestmentsId = $(v).find("[name='ItDeclaration80CinvestmentsId']").val();
        ItDeclaration80CinvestmentsId = $.trim(ItDeclaration80CinvestmentsId) == "" ? 0 : isNaN($.trim(ItDeclaration80CinvestmentsId)) ? 0 : $.trim(ItDeclaration80CinvestmentsId);
        ItDeclaration80CinvestmentDTO.ItDeclaration80CinvestmentsId = ItDeclaration80CinvestmentsId;

        ItDeclaration80CinvestmentDTO.InvestmentType = $(v).find("[name='InvestmentType']").val();

        let Value = $(v).find("[name='Value']").val();
        Value = $.trim(Value) == "" ? null : isNaN($.trim(Value)) ? null : $.trim(Value);
        ItDeclaration80CinvestmentDTO.Value = Value;
        ItDeclaration80CinvestmentList.push(ItDeclaration80CinvestmentDTO);
    });
    //80D Exemptions
    let div80DInvestmentDetails = $("div[name='Accordian80DInvestments']").find("div[name='80DInvestmentsFormCollection']").find('div[name^=80DInvestmentsForm]');
    div80DInvestmentDetails.each((i, v) => {
        var ItDeclaration80DexemptionDTO = {};

        let ItDeclaration80DexemptionsId = $(v).find("[name='ItDeclaration80DexemptionsId']").val();
        ItDeclaration80DexemptionsId = $.trim(ItDeclaration80DexemptionsId) == "" ? 0 : isNaN($.trim(ItDeclaration80DexemptionsId)) ? 0 : $.trim(ItDeclaration80DexemptionsId);
        ItDeclaration80DexemptionDTO.ItDeclaration80DexemptionsId = ItDeclaration80DexemptionsId;

        ItDeclaration80DexemptionDTO.ExemptionType = $(v).find("[name='ExemptionType']").val();

        let Value = $(v).find("[name='Value']").val();
        Value = $.trim(Value) == "" ? null : isNaN($.trim(Value)) ? null : $.trim(Value);
        ItDeclaration80DexemptionDTO.Value = Value;
        ItDeclaration80DexemptionList.push(ItDeclaration80DexemptionDTO);
    });
    //Other Investment and Exemptions
    let divOtherInvestmentDetails = $("div[name='AccordianOtherInvestments']").find("div[name='OtherInvestmentsFormCollection']").find('div[name^=OtherInvestmentsForm]');
    divOtherInvestmentDetails.each((i, v) => {
        var ItDeclarationOtherInvestmentExemptionDTO = {};
        let ItDeclarationOtherInvestmentExemptionId = $(v).find("[name='ItDeclarationOtherInvestmentExemptionId']").val();
        ItDeclarationOtherInvestmentExemptionId = $.trim(ItDeclarationOtherInvestmentExemptionId) == "" ? 0 : isNaN($.trim(ItDeclarationOtherInvestmentExemptionId)) ? 0 : $.trim(ItDeclarationOtherInvestmentExemptionId);
        ItDeclarationOtherInvestmentExemptionDTO.ItDeclarationOtherInvestmentExemptionId = ItDeclarationOtherInvestmentExemptionId;

        ItDeclarationOtherInvestmentExemptionDTO.OtherInvestmentExemptionId = $(v).find("[name='OtherInvestmentExemptionId']").val();

        let Value = $(v).find("[name='Value']").val();
        Value = $.trim(Value) == "" ? null : isNaN($.trim(Value)) ? null : $.trim(Value);
        ItDeclarationOtherInvestmentExemptionDTO.Value = Value;
        ItDeclarationOtherInvestmentExemptionList.push(ItDeclarationOtherInvestmentExemptionDTO);
    });
    //Previous Employement

    let ItDeclarationPreviousEmployementId = $("[name='PreviousEmployementDetailForm']").find("[name='ItDeclarationPreviousEmployementId']").val();
    ItDeclarationPreviousEmployementId = $.trim(ItDeclarationPreviousEmployementId) == "" ? 0 : isNaN($.trim(ItDeclarationPreviousEmployementId)) ? 0 : $.trim(ItDeclarationPreviousEmployementId);
    ItDeclarationPreviousEmployementDTO.ItDeclarationPreviousEmployementId = ItDeclarationPreviousEmployementId;

    let IncomeAfterExemptions = $("[name='PreviousEmployementDetailForm']").find("[name='IncomeAfterExemptions']").val();
    IncomeAfterExemptions = $.trim(IncomeAfterExemptions) == "" ? null : isNaN($.trim(IncomeAfterExemptions)) ? null : $.trim(IncomeAfterExemptions);
    ItDeclarationPreviousEmployementDTO.IncomeAfterExemptions = IncomeAfterExemptions;

    let IncomeTax = $("[name='PreviousEmployementDetailForm']").find("[name='IncomeTax']").val();
    IncomeTax = $.trim(IncomeTax) == "" ? null : isNaN($.trim(IncomeTax)) ? null : $.trim(IncomeTax);
    ItDeclarationPreviousEmployementDTO.IncomeTax = IncomeTax;

    let ProfessionalTax = $("[name='PreviousEmployementDetailForm']").find("[name='ProfessionalTax']").val();
    ProfessionalTax = $.trim(ProfessionalTax) == "" ? null : isNaN($.trim(ProfessionalTax)) ? null : $.trim(ProfessionalTax);
    ItDeclarationPreviousEmployementDTO.ProfessionalTax = ProfessionalTax;

    let EmployeeProvidentFund = $("[name='PreviousEmployementDetailForm']").find("[name='EmployeeProvidentFund']").val();
    EmployeeProvidentFund = $.trim(EmployeeProvidentFund) == "" ? null : isNaN($.trim(EmployeeProvidentFund)) ? null : $.trim(EmployeeProvidentFund);
    ItDeclarationPreviousEmployementDTO.EmployeeProvidentFund = EmployeeProvidentFund;

    let LeaveEncashmentExemptions = $("[name='PreviousEmployementDetailForm']").find("[name='LeaveEncashmentExemptions']").val();
    LeaveEncashmentExemptions = $.trim(LeaveEncashmentExemptions) == "" ? null : isNaN($.trim(LeaveEncashmentExemptions)) ? null : $.trim(LeaveEncashmentExemptions);
    ItDeclarationPreviousEmployementDTO.LeaveEncashmentExemptions = LeaveEncashmentExemptions;

    var iTDeclarationsViewModel = {};
    iTDeclarationsViewModel.ItDeclarationHouseRentDetailList = ItDeclarationHouseRentDetailList;
    iTDeclarationsViewModel.ItDeclarationHomeLoanDetailDTO = ItDeclarationHomeLoanDetailDTO;
    iTDeclarationsViewModel.ItDeclarationLentOutPropertyDetailList = ItDeclarationLentOutPropertyDetailList;
    iTDeclarationsViewModel.ItDeclarationOtherSourceOfIncomeDTO = ItDeclarationOtherSourceOfIncomeDTO;
    iTDeclarationsViewModel.ItDeclaration80CinvestmentList = ItDeclaration80CinvestmentList;
    iTDeclarationsViewModel.ItDeclaration80DexemptionList = ItDeclaration80DexemptionList;
    iTDeclarationsViewModel.ItDeclarationOtherInvestmentExemptionList = ItDeclarationOtherInvestmentExemptionList;
    iTDeclarationsViewModel.ItDeclarationPreviousEmployementDTO = ItDeclarationPreviousEmployementDTO;
    iTDeclarationsViewModel.FY = $("#FY").val();
    iTDeclarationsViewModel.Regime = $("#Regime").val();

    //UnblockUI();
    //return;


    $.ajax({
        type: "POST",
        url: "/PayrollAccount/SaveITDeclarations",
        contentType: 'application/json',
        data: JSON.stringify(iTDeclarationsViewModel),
        success: function (data) {
            if ($.trim(data) == "") {
                $successalert("Success!", "Saved Successfully!");
                window.location.href = "/PayrollAccount/ITDeclarations/" + $("#FY").val() + "_" + $("#Regime").val();
            }
            else {
                $erroralert("Saved with some errors!", data + "!");
            }
            UnblockUI();
        },
        error: function (error) {
            $erroralert("Error!", error.responseText + '!'); UnblockUI();
        }
    });

}





function validateHouseRentDetails(currDiv) {
    resetValidationError();
    var isValid = true;
    if ($(currDiv).find('[name="Regime"]').val() != "Old") {
        return isValid;
    }


    var inputs = $(currDiv).find($('input.required'));
    var selects = $(currDiv).find($('select.required'));
    var textareas = $(currDiv).find($('textarea.required'));
    inputs.each(function () {
        if ($.trim($(this).val()) == "") {
            if (!$(this).hasClass("error")) {
                var name = $(this).attr('name');
                $(this).addClass("error");
                $(this).parent().parent().find("span.error").show();
            }
            isValid = false;
        }
    });
    selects.each(function () {
        if ($.trim($(this).val()) == "") {
            if (!$(this).hasClass("error")) {
                var name = $(this).attr('name');
                $(this).addClass("error");
                $(this).parent().parent().find("span.error").show();
            }
            isValid = false;
        }
    });
    textareas.each(function () {
        if ($.trim($(this).val()) == "") {
            if (!$(this).hasClass("error")) {
                var name = $(this).attr('name');
                $(this).addClass("error");
                $(this).parent().parent().find("span.error").show();
            }
            isValid = false;
        }
    });
    return isValid;
}


function resetValidationError() {

    $('.required').click(function () {
        $(this).parent().parent().find("span.error").hide();
        $(this).removeClass("error");
    });
    $('.required').change(function () {
        $(this).parent().parent().find("span.error").hide();

        if ($(this).hasClass("inptgrps")) {
            $(this).parent().find('select').removeClass("error");
        } else
            $(this).removeClass("error");
    });

}
function ValidateFYDates(currDiv) {
    var isValid = true;
    let startYear = $(currDiv).find('[name="FY"]').val().split("-")[0];
    let endYear = $(currDiv).find('[name="FY"]').val().split("-")[1];
    let startDate = moment(startYear + "-03-01");
    let endDate = moment(endYear + "-04-01");


    let dateSelectedStart = moment($(currDiv).find('[name="YearFrom"]').val() + "-" + $(currDiv).find('[name="MonthFrom"]').val() + "-01");
    let dateSelectedEnd = moment($(currDiv).find('[name="YearTo"]').val() + "-" + $(currDiv).find('[name="MonthTo"]').val() + "-01");

    //if (dateSelectedStart < startDate) {

    //    isValid = false;

    //}
    //else if (dateSelectedEnd > endDate) {

    //}

    if (!dateSelectedStart.isBetween(startDate, endDate, 'months', '[]')) {
        $erroralert("Error in Housing Rent Details", "start date do not lies between");
        isValid = false;
    }
    else if (!dateSelectedEnd.isBetween(startDate, endDate, 'months', '[]')) {
        $erroralert("Error in Housing Rent Details", "end date do not lies between");
        isValid = false;
    }
    else if (dateSelectedEnd < dateSelectedStart) {
        $erroralert("Error in Housing Rent Details", "end date can not be less then startdate");
        isValid = false;
    }
    return isValid;
}