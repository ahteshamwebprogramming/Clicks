var EmployeeByTenureChart;
var EmployeesBySalaryBandChart;
var EmployeesByQualificationChart;

function initPage() {


    $(".flatpickr-date1").flatpickr({
        plugins: [
            new monthSelectPlugin({
                shorthand: true, // Use shorthand month names (e.g., Jan, Feb, Mar)
                dateFormat: "M Y", // The format in which the selected date will be set
                altFormat: "F Y" // The format in which the date will be displayed
            })
        ]
    });

    $(".flatpickr-month-year").flatpickr({
        plugins: [
            new monthSelectPlugin({
                shorthand: true, // Use shorthand month names (e.g., Jan, Feb, Mar)
                dateFormat: "M Y", // The format in which the selected date will be set (e.g., Jan 2024)
                altFormat: "F Y" // The format in which the date will be displayed (e.g., January 2024)
            })
        ]
    });
}

function GetChartsWithFilteredData() {
    let FromDate = $("#Filters").find("[name='FromDate']").val();
    let ToDate = $("#Filters").find("[name='ToDate']").val();
    let Department = $("#Filters").find("[name='Department']").val();
    let WorkLocation = $("#Filters").find("[name='WorkLocation']").val();
}

function GetEmployeeList() {
    $.ajax({
        type: "POST",
        url: "/Admin/GetActiveEmployee",
        contentType: 'application/json',
        //data: JSON.stringify(inputDTO),
        success: function (data) {
            console.log(data.employeeList);
            console.log(data.employeeExperienceList);
            console.log(data.employeeExitList);
            EmployeeByGenderStatistics(data.employeeList);
            EmployeeByAgeGenderStats(data.employeeList);
            EmployeesByTenure(data.employeeList);
            //EmployeesBySalaryBand(data);
            DemograhicChart(data);
            EmployeeHeadCount(data);
            employeeTurnOverRateGraph(data);
            CurrentDateEmployeeStatsDTO(data.currentDateEmployeeStatsDTO);
        },
        error: function (error) {
            $erroralert("Chart Loading Error!", error.responseText + '!');
            UnblockUI();
        }
    });
}
function GetWageBillTrendChartData() {
    $.ajax({
        type: "POST",
        url: "/Admin/WageBillTrendChartData",
        contentType: 'application/json',
        success: function (data) {
            console.log(data);
            WageBillTrendChart(data);
        },
        error: function (error) {
            $erroralert("Chart Loading Error!", error.responseText + '!');
            UnblockUI();
        }
    });
}

function CurrentDateEmployeeStatsDTO(currentDateEmployeeStatsDTO) {
    $("#CurrentDateEmployeeStatsDTO").find("#TotalEmployee").text(currentDateEmployeeStatsDTO.totalEmployee)
    $("#CurrentDateEmployeeStatsDTO").find("#Present").text(currentDateEmployeeStatsDTO.present)
    $("#CurrentDateEmployeeStatsDTO").find("#LateComing").text(currentDateEmployeeStatsDTO.lateComing)
    $("#CurrentDateEmployeeStatsDTO").find("#TotalLeave").text(currentDateEmployeeStatsDTO.totalLeave)
    $("#CurrentDateEmployeeStatsDTO").find("#Absent").text(currentDateEmployeeStatsDTO.absent)

    let presentPercent = (currentDateEmployeeStatsDTO.present * 100 / currentDateEmployeeStatsDTO.totalEmployee).toFixed(0);
    let lateComingPercent = (currentDateEmployeeStatsDTO.lateComing * 100 / currentDateEmployeeStatsDTO.totalEmployee).toFixed(0);
    let totalLeavePercent = (currentDateEmployeeStatsDTO.totalLeave * 100 / currentDateEmployeeStatsDTO.totalEmployee).toFixed(0);
    let absentPercent = (currentDateEmployeeStatsDTO.absent * 100 / currentDateEmployeeStatsDTO.totalEmployee).toFixed(0);

    $("#CurrentDateEmployeeStatsDTO").find("#PresentProgressbar").attr("aria-valuenow", presentPercent)
    $("#CurrentDateEmployeeStatsDTO").find("#PresentProgressbar").css('width', presentPercent + '%')
    $("#CurrentDateEmployeeStatsDTO").find("#PresentPercent").text(presentPercent + '%')

    $("#CurrentDateEmployeeStatsDTO").find("#TotalLeaveProgressbar").attr("aria-valuenow", totalLeavePercent)
    $("#CurrentDateEmployeeStatsDTO").find("#TotalLeaveProgressbar").css('width', totalLeavePercent + '%')
    $("#CurrentDateEmployeeStatsDTO").find("#TotalLeavePercent").text(totalLeavePercent + '%')

    $("#CurrentDateEmployeeStatsDTO").find("#AbsentProgressbar").attr("aria-valuenow", absentPercent)
    $("#CurrentDateEmployeeStatsDTO").find("#AbsentProgressbar").css('width', absentPercent + '%')
    $("#CurrentDateEmployeeStatsDTO").find("#AbsentPercent").text(absentPercent + '%')

    $("#CurrentDateEmployeeStatsDTO").find("#LateComingProgressbar").attr("aria-valuenow", lateComingPercent)
    $("#CurrentDateEmployeeStatsDTO").find("#LateComingProgressbar").css('width', lateComingPercent + '%')
    $("#CurrentDateEmployeeStatsDTO").find("#LateComingPercent").text(lateComingPercent + '%')
}

function EmployeeByGenderStatistics(data) {

    const genderCounts = data.reduce((counts, item) => {
        const gender = item.genderId;
        if (gender === 1) {
            counts.male = (counts.male || 0) + 1;
        } else if (gender === 2) {
            counts.female = (counts.female || 0) + 1;
        } else {
            counts.other = (counts.other || 0) + 1;
        }
        return counts;
    }, {});

    const maleCount = genderCounts.male || 0;
    const femaleCount = genderCounts.female || 0;
    const otherCount = genderCounts.other || 0;

    $("#divEmployeesByGenderStats").find("[name='maleCount']").text(maleCount)
    $("#divEmployeesByGenderStats").find("[name='femaleCount']").text(femaleCount)
    $("#divEmployeesByGenderStats").find("[name='otherCount']").text(otherCount)

    let cardColor, headingColor, legendColor, labelColor, shadeColor, borderColor;

    if (isDarkStyle) {
        cardColor = config.colors_dark.cardColor;
        headingColor = config.colors_dark.headingColor;
        legendColor = config.colors_dark.bodyColor;
        labelColor = config.colors_dark.textMuted;
        borderColor = config.colors_dark.borderColor;
    } else {
        cardColor = config.colors.cardColor;
        headingColor = config.colors.headingColor;
        legendColor = config.colors.bodyColor;
        labelColor = config.colors.textMuted;
        borderColor = config.colors.borderColor;
    }
    const chartOrderStatistics = document.querySelector('#EmployeesByGenderStats'),
        orderChartConfig = {
            chart: {
                height: 300,
                width: 300,
                type: 'donut'
            },
            labels: ['Male', 'Female', 'Other'],
            series: [maleCount, femaleCount, otherCount],
            colors: [config.colors.primary, '#FFC0CB', config.colors.secondary],
            stroke: {
                width: 5,
                //colors: [cardColor]
            },
            dataLabels: {
                enabled: false,
                formatter: function (val, opt) {
                    return parseInt(val) + '%';
                }
            },
            legend: {
                show: false
            },
            grid: {
                padding: {
                    top: 0,
                    bottom: 0,
                    right: 15
                }
            },
            plotOptions: {
                pie: {
                    donut: {
                        size: '70%',
                        labels: {
                            show: true,
                            value: {
                                fontSize: '1.5rem',
                                fontFamily: 'Public Sans',
                                color: headingColor,
                                offsetY: -15,
                                formatter: function (val, { series, seriesIndex, dataPointIndex, w }) {
                                    const percentage = (val / data.length * 100).toFixed(2);
                                    return percentage + '%';
                                }
                            },
                            name: {
                                offsetY: 20,
                                fontFamily: 'Public Sans'
                            },
                            total: {
                                show: true,
                                fontSize: '0.8125rem',
                                color: legendColor,
                                label: 'Total Employees',
                                formatter: function (w) {
                                    return data.length;
                                }
                            }
                        }
                    }
                }
            }
        };
    if (typeof chartOrderStatistics !== undefined && chartOrderStatistics !== null) {
        const statisticsChart = new ApexCharts(chartOrderStatistics, orderChartConfig);
        statisticsChart.render();
    }
}


function EmployeeByAgeGenderStats(data) {
    let cardColor, headingColor, legendColor, labelColor, shadeColor, borderColor;

    if (isDarkStyle) {
        cardColor = config.colors_dark.cardColor;
        headingColor = config.colors_dark.headingColor;
        legendColor = config.colors_dark.bodyColor;
        labelColor = config.colors_dark.textMuted;
        borderColor = config.colors_dark.borderColor;
    } else {
        cardColor = config.colors.cardColor;
        headingColor = config.colors.headingColor;
        legendColor = config.colors.bodyColor;
        labelColor = config.colors.textMuted;
        borderColor = config.colors.borderColor;
    }

    function calculateAge(dob) {
        if (!dob) {
            return null; // Return null if dob is null or undefined
        }

        const dateOfBirth = new Date(dob);
        if (isNaN(dateOfBirth.getTime())) {
            return null; // Return null if dob is an invalid date
        }

        const today = new Date();
        let age = today.getFullYear() - dateOfBirth.getFullYear();
        const month = today.getMonth() - dateOfBirth.getMonth();
        if (month < 0 || (month === 0 && today.getDate() < dateOfBirth.getDate())) {
            age--;
        }
        return age;
    }

    const ageGenderData = data.reduce((acc, employee) => {
        const age = calculateAge(employee.dob);
        const gender = employee.genderId;

        if (gender === null || gender === undefined) {
            //gender = 'other';
        }

        if (age === null) {
            return acc;
        }

        //const ageGroupStart = Math.floor(age / 5) * 5 + 1; // Group ages in intervals of 5 years starting from 1
        //const ageGroupEnd = ageGroupStart + 4; // End of age group

        //const ageGroupKey = `${ageGroupStart}-${ageGroupEnd}`;

        let ageGroupKey;
        if (age > 41) {
            ageGroupKey = '>41';
        }
        else if (age < 21) {
            ageGroupKey = '18-20';
        } else {
            const ageGroupStart = Math.floor(age / 5) * 5 + 1; // Group ages in intervals of 5 years starting from 1
            const ageGroupEnd = ageGroupStart + 4; // End of age group
            ageGroupKey = `${ageGroupStart}-${ageGroupEnd}`;
        }

        if (!acc[ageGroupKey]) {
            acc[ageGroupKey] = { male: 0, female: 0, other: 0 };
        }

        if (gender === 1) {
            acc[ageGroupKey].male++;
        } else if (gender === 2) {
            acc[ageGroupKey].female++;
        } else {
            acc[ageGroupKey].other++;
        }

        return acc;
    }, {});

    const categories = Object.keys(ageGenderData)
        .sort((a, b) => {
            if (a === '>41') return 1; // Ensure '42+' is the last category
            if (b === '>41') return -1;
            const aStart = parseInt(a.split('-')[0]);
            const bStart = parseInt(b.split('-')[0]);
            return aStart - bStart;
        });
    //const categories = Object.keys(ageGenderData).sort((a, b) => a - b); // Sort ages in ascending order
    const maleData = categories.map(ageGroupKey => ageGenderData[ageGroupKey].male);
    const femaleData = categories.map(ageGroupKey => ageGenderData[ageGroupKey].female);
    const otherData = categories.map(ageGroupKey => ageGenderData[ageGroupKey].other);


    const incomeChartEl = document.querySelector('#EmployeeByAgeGenderStats'),
        incomeChartConfig = {
            series: [
                {
                    name: "Male",
                    data: maleData
                }, {
                    name: "Female",
                    data: femaleData
                }, {
                    name: "Other",
                    data: otherData
                }
            ],
            chart: {
                height: 350,
                parentHeightOffset: 0,
                toolbar: {
                    show: false
                },
                type: 'area'
            },
            //colors: ['#0000FF', '#FFC0CB', '#808080'],
            colors: [config.colors.primary, '#FFC0CB', config.colors.secondary],
            dataLabels: {
                enabled: true
            },
            stroke: {
                width: 3,
                curve: 'straight'
            },
            legend: {
                show: true,
                position: "top",
                labels: {
                    useSeriesColors: true
                }
            },
            fill: {
                type: 'gradient',
                gradient: {
                    shade: shadeColor,
                    shadeIntensity: 0.6,
                    opacityFrom: 0.5,
                    opacityTo: 0.25,
                    stops: [0, 95, 100]
                }
            },
            grid: {
                borderColor: borderColor,
                strokeDashArray: 1,
                padding: {
                    top: 10,
                    bottom: -10,
                    left: 30,
                    right: 50
                }
            },
            xaxis: {
                categories: categories,
                axisBorder: {
                    show: false
                },
                axisTicks: {
                    show: false
                },
                labels: {
                    show: true,
                    style: {
                        fontSize: '12px',
                        colors: labelColor
                    }

                }
            },
            yaxis: {
                axisBorder: {
                    show: false
                },
                axisTicks: {
                    show: false
                },
                labels: {
                    show: false,
                    style: {
                        fontSize: '12px',
                        colors: labelColor
                    }, formatter: function (value) {
                        return parseInt(value); // Format the y-axis labels as integers                       
                    }
                }
            }
        };
    if (typeof incomeChartEl !== undefined && incomeChartEl !== null) {
        const incomeChart = new ApexCharts(incomeChartEl, incomeChartConfig);
        incomeChart.render();
    }
}



function EmployeesByTenure(data) {
    let cardColor, headingColor, legendColor, labelColor, shadeColor, borderColor;

    if (isDarkStyle) {
        cardColor = config.colors_dark.cardColor;
        headingColor = config.colors_dark.headingColor;
        legendColor = config.colors_dark.bodyColor;
        labelColor = config.colors_dark.textMuted;
        borderColor = config.colors_dark.borderColor;
    }
    else {
        cardColor = config.colors.cardColor;
        headingColor = config.colors.headingColor;
        legendColor = config.colors.bodyColor;
        labelColor = config.colors.textMuted;
        borderColor = config.colors.borderColor;
    }


    var currentDate = new Date();

    // Function to calculate the years of experience
    function calculateExperience(doj) {
        var joinDate = new Date(doj);
        var diffTime = Math.abs(currentDate - joinDate);
        var diffYears = diffTime / (1000 * 60 * 60 * 24 * 365.25);
        return diffYears;
    }
    // Calculate experience for each employee and find the maximum experience
    var experienceYears = data.map(employee => calculateExperience(employee.doj));
    var maxExperience = Math.max(...experienceYears);

    // Define dynamic categories
    var categories = ["<1"];
    for (var i = 1; i <= maxExperience; i += 5) {
        if (i + 4 > maxExperience) {
            categories.push(`>${i}`);
        } else {
            categories.push(`${i}-${i + 4}`);
        }
    }
    // Initialize counters for each category
    var categoryCounts = new Array(categories.length).fill(0);

    // Categorize experience
    experienceYears.forEach(function (experience) {
        if (experience < 1) {
            categoryCounts[0]++;
        } else {
            for (var i = 1; i < categories.length; i++) {
                var range = categories[i].split('-');
                if (range.length === 2) {
                    var min = parseInt(range[0]);
                    var max = parseInt(range[1]);
                    if (experience >= min && experience <= max) {
                        categoryCounts[i]++;
                        break;
                    }
                } else if (categories[i].startsWith('>')) {
                    var min = parseInt(categories[i].substring(1));
                    if (experience >= min) {
                        categoryCounts[i]++;
                        break;
                    }
                }
            }
        }
    });



    const EmployeeByTenure = document.querySelector('#EmployeeByTenure'),
        EmployeeByTenureChartOptions = {
            series: [
                {
                    name: 'No Of Employees',
                    data: categoryCounts//[18, 7, 15, 29, 18, 12, 9]
                }
            ],
            chart: {
                height: 400,
                stacked: true,
                type: 'bar',
                toolbar: { show: false }
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: '33%',
                    borderRadius: 12,
                    startingShape: 'rounded',
                    endingShape: 'rounded'
                }
            },
            colors: [config.colors.primary, config.colors.info],
            dataLabels: {
                enabled: false
            },
            stroke: {
                curve: 'smooth',
                width: 6,
                lineCap: 'round',
                colors: [cardColor]
            },
            legend: {
                show: true,
                horizontalAlign: 'left',
                position: 'top',
                markers: {
                    height: 8,
                    width: 8,
                    radius: 12,
                    offsetX: -3
                },
                labels: {
                    colors: legendColor
                },
                itemMargin: {
                    horizontal: 10
                }
            },
            grid: {
                borderColor: borderColor,
                padding: {
                    top: 0,
                    bottom: -8,
                    left: 20,
                    right: 20
                }
            },
            xaxis: {
                categories: categories,
                labels: {
                    style: {
                        fontSize: '13px',
                    },
                    formatter: function (value) {
                        if (value == "<1") {
                            return value + " Year";
                        }
                        else {
                            return value + " Years";
                        }

                    }
                },
                axisTicks: {
                    show: false
                },
                axisBorder: {
                    show: false
                }
            },
            yaxis: {
                labels: {
                    style: {
                        fontSize: '13px',

                    }
                }
            },
            states: {
                hover: {
                    filter: {
                        type: 'none'
                    }
                },
                active: {
                    filter: {
                        type: 'none'
                    }
                }
            }
        };
    if (typeof EmployeeByTenure !== undefined && EmployeeByTenure !== null) {

        if (typeof EmployeesBySalaryBandChart !== 'undefined' && EmployeesBySalaryBandChart !== null) {
            EmployeesBySalaryBandChart.destroy();
        }
        if (typeof EmployeesByQualificationChart !== 'undefined' && EmployeesByQualificationChart !== null) {
            EmployeesByQualificationChart.destroy();
        }

        EmployeeByTenureChart = new ApexCharts(EmployeeByTenure, EmployeeByTenureChartOptions);
        EmployeeByTenureChart.render();
    }

    return;




}

function EmployeesBySalaryBand(data) {
    let cardColor, headingColor, legendColor, labelColor, shadeColor, borderColor;

    if (isDarkStyle) {
        cardColor = config.colors_dark.cardColor;
        headingColor = config.colors_dark.headingColor;
        legendColor = config.colors_dark.bodyColor;
        labelColor = config.colors_dark.textMuted;
        borderColor = config.colors_dark.borderColor;
    } else {
        cardColor = config.colors.cardColor;
        headingColor = config.colors.headingColor;
        legendColor = config.colors.bodyColor;
        labelColor = config.colors.textMuted;
        borderColor = config.colors.borderColor;
    }


    let employeeData = data.employeeList;

    // Convert CTC to numbers and group by salary ranges
    const salaryRanges = {
        'Upto 50K': 0,
        '50K - 100K': 0,
        '100K - 150K': 0,
        '150K - 200K': 0,
        'Above 200K': 0
    };

    employeeData.forEach(employee => {
        if (employee.ctc) {
            const yearlySalary = parseInt(employee.ctc, 10);
            const monthlySalary = yearlySalary / 12;
            if (monthlySalary <= 50000) {
                salaryRanges['Upto 50K'] += 1;
            } else if (monthlySalary <= 100000) {
                salaryRanges['50K - 100K'] += 1;
            } else if (monthlySalary <= 150000) {
                salaryRanges['100K - 150K'] += 1;
            } else if (monthlySalary <= 200000) {
                salaryRanges['150K - 200K'] += 1;
            } else {
                salaryRanges['Above 200K'] += 1;
            }
        }
    });


    const categories = Object.keys(salaryRanges);
    const categoryCounts = Object.values(salaryRanges);


    const EmployeesBySalaryBand = document.querySelector('#EmployeesBySalaryBand'),
        EmployeesBySalaryBandChartOptions = {
            series: [
                {
                    name: 'No Of Employees',
                    data: categoryCounts//[18, 7, 15, 29, 18, 12, 9]
                }
            ],
            chart: {
                height: 400,
                stacked: true,
                type: 'bar',
                toolbar: { show: false }
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: '33%',
                    borderRadius: 12,
                    startingShape: 'rounded',
                    endingShape: 'rounded'
                }
            },
            colors: [config.colors.primary, config.colors.info],
            dataLabels: {
                enabled: false
            },
            stroke: {
                curve: 'smooth',
                width: 6,
                lineCap: 'round',
                colors: [cardColor]
            },
            legend: {
                show: true,
                horizontalAlign: 'left',
                position: 'top',
                markers: {
                    height: 8,
                    width: 8,
                    radius: 12,
                    offsetX: -3
                },
                labels: {
                    colors: legendColor
                },
                itemMargin: {
                    horizontal: 10
                }
            },
            grid: {
                borderColor: borderColor,
                padding: {
                    top: 0,
                    bottom: -8,
                    left: 20,
                    right: 20
                }
            },
            xaxis: {
                categories: categories,
                labels: {
                    style: {
                        fontSize: '13px',
                    },
                    formatter: function (value) {
                        return value;
                    }
                },
                axisTicks: {
                    show: false
                },
                axisBorder: {
                    show: false
                }
            },
            yaxis: {
                labels: {
                    style: {
                        fontSize: '13px',

                    },
                    formatter: function (value) {
                        return Math.floor(value); // Ensures labels are not displayed in decimal
                    }
                }
            },
            states: {
                hover: {
                    filter: {
                        type: 'none'
                    }
                },
                active: {
                    filter: {
                        type: 'none'
                    }
                }
            }
        };
    if (typeof EmployeesBySalaryBand !== undefined && EmployeesBySalaryBand !== null) {
        if (typeof EmployeeByTenureChart !== 'undefined' && EmployeeByTenureChart !== null) {
            EmployeeByTenureChart.destroy();
        }
        if (typeof EmployeesByQualificationChart !== 'undefined' && EmployeesByQualificationChart !== null) {
            EmployeesByQualificationChart.destroy();
        }
        EmployeesBySalaryBandChart = new ApexCharts(EmployeesBySalaryBand, EmployeesBySalaryBandChartOptions);
        EmployeesBySalaryBandChart.render();
    }

    return;
}

function EmployeesByQualification(data) {
    let cardColor, headingColor, legendColor, labelColor, shadeColor, borderColor;

    if (isDarkStyle) {
        cardColor = config.colors_dark.cardColor;
        headingColor = config.colors_dark.headingColor;
        legendColor = config.colors_dark.bodyColor;
        labelColor = config.colors_dark.textMuted;
        borderColor = config.colors_dark.borderColor;
    } else {
        cardColor = config.colors.cardColor;
        headingColor = config.colors.headingColor;
        legendColor = config.colors.bodyColor;
        labelColor = config.colors.textMuted;
        borderColor = config.colors.borderColor;
    }


    let employees = data.employeeList;
    let academicDetails = data.employeeAcademicList;
    let academics = data.academicMasterList;

    const employeeAcademicData = academicDetails.map(detail => {
        const employee = employees.find(emp => emp.employeeId === detail.employeeId);
        const academic = academics.find(acad => acad.academicId === detail.academicId);
        return {
            employeeId: detail.employeeId,
            employeeName: employee ? employee.employeeName : 'Unknown',
            academicName: academic ? academic.academicName : 'Unknown'
        };
    });

    // Count the number of employees per qualification
    const qualificationCounts = {};
    employeeAcademicData.forEach(item => {
        if (!qualificationCounts[item.academicName]) {
            qualificationCounts[item.academicName] = 0;
        }
        qualificationCounts[item.academicName]++;
    });

    // Prepare data for the chart
    const categories = Object.keys(qualificationCounts);
    const dataCounts = Object.values(qualificationCounts);




    const EmployeesByQualification = document.querySelector('#EmployeesByQualification'),
        EmployeesByQualificationChartOptions = {
            series: [
                {
                    name: 'No Of Employees',
                    data: dataCounts//[18, 7, 15, 29, 18, 12, 9]
                }
            ],
            chart: {
                height: 400,
                stacked: true,
                type: 'bar',
                toolbar: { show: false }
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: '33%',
                    borderRadius: 12,
                    startingShape: 'rounded',
                    endingShape: 'rounded'
                }
            },
            colors: [config.colors.primary, config.colors.info],
            dataLabels: {
                enabled: false
            },
            stroke: {
                curve: 'smooth',
                width: 6,
                lineCap: 'round',
                colors: [cardColor]
            },
            legend: {
                show: true,
                horizontalAlign: 'left',
                position: 'top',
                markers: {
                    height: 8,
                    width: 8,
                    radius: 12,
                    offsetX: -3
                },
                labels: {
                    colors: legendColor
                },
                itemMargin: {
                    horizontal: 10
                }
            },
            grid: {
                borderColor: borderColor,
                padding: {
                    top: 0,
                    bottom: -8,
                    left: 20,
                    right: 20
                }
            },
            xaxis: {
                categories: categories,
                labels: {
                    style: {
                        fontSize: '13px',
                    },
                    formatter: function (value) {
                        if (value == "<1") {
                            return value + " Year";
                        }
                        else {
                            return value + " Years";
                        }

                    }
                },
                axisTicks: {
                    show: false
                },
                axisBorder: {
                    show: false
                }
            },
            yaxis: {
                labels: {
                    style: {
                        fontSize: '13px',

                    }
                }
            },
            states: {
                hover: {
                    filter: {
                        type: 'none'
                    }
                },
                active: {
                    filter: {
                        type: 'none'
                    }
                }
            }
        };
    if (typeof EmployeesByQualification !== undefined && EmployeesByQualification !== null) {
        if (typeof EmployeeByTenureChart !== 'undefined' && EmployeeByTenureChart !== null) {
            EmployeeByTenureChart.destroy();
        }
        if (typeof EmployeesBySalaryBandChart !== 'undefined' && EmployeesBySalaryBandChart !== null) {
            EmployeesBySalaryBandChart.destroy();
        }
        EmployeesByQualificationChart = new ApexCharts(EmployeesByQualification, EmployeesByQualificationChartOptions);
        EmployeesByQualificationChart.render();
    }

    return;
}


function employeeTurnOverRateGraph1(data) {

    let cardColor, headingColor, legendColor, labelColor, shadeColor, borderColor;

    if (isDarkStyle) {
        cardColor = config.colors_dark.cardColor;
        headingColor = config.colors_dark.headingColor;
        legendColor = config.colors_dark.bodyColor;
        labelColor = config.colors_dark.textMuted;
        borderColor = config.colors_dark.borderColor;
    } else {
        cardColor = config.colors.cardColor;
        headingColor = config.colors.headingColor;
        legendColor = config.colors.bodyColor;
        labelColor = config.colors.textMuted;
        borderColor = config.colors.borderColor;
    }

    const employeeTurnOverRateChartEl = document.querySelector('#employeeTurnOverRateChart'),
        employeeTurnOverRateChartConfig = {
            chart: {
                height: 194,
                // width: 175,
                type: 'line',
                toolbar: {
                    show: false
                },
                dropShadow: {
                    enabled: true,
                    top: 10,
                    left: 5,
                    blur: 3,
                    color: config.colors.warning,
                    opacity: 0.15
                },
                sparkline: {
                    enabled: true
                }
            },
            grid: {
                show: false,
                padding: {
                    right: 8
                }
            },
            colors: [config.colors.warning],
            dataLabels: {
                enabled: false
            },
            stroke: {
                width: 5,
                curve: 'smooth'
            },
            series: [
                {
                    data: [110, 270, 145, 245, 205, 285]
                }
            ],
            xaxis: {
                show: false,
                lines: {
                    show: false
                },
                labels: {
                    show: false
                },
                axisBorder: {
                    show: false
                }
            },
            yaxis: {
                show: false
            }
        };
    if (typeof employeeTurnOverRateChartEl !== undefined && employeeTurnOverRateChartEl !== null) {
        const employeeTurnOverRateChart = new ApexCharts(employeeTurnOverRateChartEl, employeeTurnOverRateChartConfig);
        employeeTurnOverRateChart.render();
    }

}


function employeeTurnOverRateGraph(data) {
    let cardColor, headingColor, legendColor, labelColor, shadeColor, borderColor;

    if (isDarkStyle) {
        cardColor = config.colors_dark.cardColor;
        headingColor = config.colors_dark.headingColor;
        legendColor = config.colors_dark.bodyColor;
        labelColor = config.colors_dark.textMuted;
        borderColor = config.colors_dark.borderColor;
    } else {
        cardColor = config.colors.cardColor;
        headingColor = config.colors.headingColor;
        legendColor = config.colors.bodyColor;
        labelColor = config.colors.textMuted;
        borderColor = config.colors.borderColor;
    }

    // Parse date strings to Date objects
    const employees = data.employeeList.map(emp => ({ ...emp, doj: new Date(emp.doj) }));
    const exits = data.employeeExitList.map(exit => ({ ...exit, lastWorkingDateAdmin: new Date(exit.lastWorkingDateAdmin) }));

    const getLast6Months = () => {
        const months = [];
        const now = new Date();
        for (let i = 5; i >= 0; i--) {
            const date = new Date(now.getFullYear(), now.getMonth() - i, 1);
            months.push(date);
        }
        return months;
    };

    const processTurnoverRate = (employees, exits) => {
        const months = getLast6Months();
        const turnoverRates = [];

        months.forEach((month, index) => {
            const monthStart = new Date(month.getFullYear(), month.getMonth(), 1);
            const monthEnd = new Date(month.getFullYear(), month.getMonth() + 1, 0);

            // Employees joining before the end of the month
            const employeesAtMonthEnd = employees.filter(emp => emp.doj <= monthEnd).length;

            // Exits within the month
            const leaversInMonth = exits.filter(exit => exit.lastWorkingDateAdmin >= monthStart && exit.lastWorkingDateAdmin <= monthEnd).length;

            // Calculate turnover rate
            const turnoverRate = (leaversInMonth / employeesAtMonthEnd) * 100;
            turnoverRates.push(turnoverRate.toFixed(2));
        });

        return { turnoverRates, months };
    };


    const { turnoverRates, months } = processTurnoverRate(employees, exits);


    const latestTurnoverRate = turnoverRates[turnoverRates.length - 1];
    const previousTurnoverRate = turnoverRates[turnoverRates.length - 2];
    const turnoverRateDifference = (latestTurnoverRate - previousTurnoverRate).toFixed(2);
    const latestMonth = `${months[months.length - 1].toLocaleString('default', { month: 'short' })}-${months[months.length - 1].getFullYear()}`;

    $("turnOverRateCurrentMonth").text(latestMonth);
    $("#latestTurnOverRate").text(latestTurnoverRate);
    if (turnoverRateDifference > 0) {
        $("#diffTurnOverRate")[0].innerHTML = '<i class="bx bx-chevron-up"></i>+' + turnoverRateDifference + '% from last month';
        $("#diffTurnOverRate").addClass("text-success");
        $("#diffTurnOverRate").removeClass("text-error");
    }
    else if (turnoverRateDifference < 0) {
        $("#diffTurnOverRate")[0].innerHTML = '<i class="bx bx-chevron-down"></i>-' + turnoverRateDifference + '% from last month';
        $("#diffTurnOverRate").removeClass("text-success");
        $("#diffTurnOverRate").addClass("text-error");
    }
    else {
        $("#diffTurnOverRate")[0].innerHTML = '<i class="bx bx-chevron-up"></i>' + turnoverRateDifference + '% from last month';
        $("#diffTurnOverRate").addClass("text-success");
        $("#diffTurnOverRate").removeClass("text-error");
    }


    const employeeTurnOverRateChartEl = document.querySelector('#employeeTurnOverRateChart'),
        employeeTurnOverRateChartConfig = {
            chart: {
                height: 194,
                type: 'line',
                toolbar: {
                    show: false
                },
                dropShadow: {
                    enabled: true,
                    top: 10,
                    left: 5,
                    blur: 3,
                    color: config.colors.warning,
                    opacity: 0.15
                },
                sparkline: {
                    enabled: true
                }
            },
            grid: {
                show: false,
                padding: {
                    right: 8,
                    bottom: 5,
                }
            },
            colors: [config.colors.warning],
            dataLabels: {
                enabled: false
            },
            stroke: {
                width: 5,
                curve: 'smooth'
            },
            series: [
                {
                    name: "Turn Over Rate",
                    data: turnoverRates
                }
            ],
            xaxis: {
                categories: months.map(month => `${month.toLocaleString('default', { month: 'short' })}-${month.getFullYear()}`),
                labels: {
                    style: {
                        fontSize: '12px'
                    },
                },
                type: 'category'
            },
            yaxis: {
                labels: {
                    style: {
                        fontSize: '12px'
                    }
                }
            }
        };

    if (typeof employeeTurnOverRateChartEl !== undefined && employeeTurnOverRateChartEl !== null) {
        const employeeTurnOverRateChart = new ApexCharts(employeeTurnOverRateChartEl, employeeTurnOverRateChartConfig);
        employeeTurnOverRateChart.render();
    }
}


function EmployeesBySalaryBandRenderChart() {
    BlockUI();
    $.ajax({
        type: "POST",
        url: "/Admin/GetEmployeesBySalaryBandChartData",
        contentType: 'application/json',
        success: function (data) {
            UnblockUI();
            console.log(data.employeeList);
            //console.log(data.bandMasterList);
            EmployeesBySalaryBand(data);
        },
        error: function (error) {
            $erroralert("Chart Loading Error!", error.responseText + '!');
            UnblockUI();
        }
    });
}
function EmployeesByTenureRenderChart() {
    BlockUI();
    $.ajax({
        type: "POST",
        url: "/Admin/GetEmployeesByTenureChartData",
        contentType: 'application/json',
        success: function (data) {
            UnblockUI();
            console.log(data.employeeList);
            console.log(data.bandMasterList);
            EmployeesByTenure(data.employeeList);
        },
        error: function (error) {
            $erroralert("Chart Loading Error!", error.responseText + '!');
            UnblockUI();
        }
    });
}
function EmployeesByQualificationRenderChart() {
    BlockUI();
    $.ajax({
        type: "POST",
        url: "/Admin/GetEmployeesByQualificationChartData",
        contentType: 'application/json',
        success: function (data) {
            UnblockUI();
            console.log(data.employeeList);
            console.log(data.employeeAcademicList);
            console.log(data.academicMasterList);
            EmployeesByQualification(data);
        },
        error: function (error) {
            $erroralert("Chart Loading Error!", error.responseText + '!');
            UnblockUI();
        }
    });
}

function DemograhicChart(data) {
    let cardColor, headingColor, legendColor, labelColor, shadeColor, borderColor;

    if (isDarkStyle) {
        cardColor = config.colors_dark.cardColor;
        headingColor = config.colors_dark.headingColor;
        legendColor = config.colors_dark.bodyColor;
        labelColor = config.colors_dark.textMuted;
        borderColor = config.colors_dark.borderColor;
    } else {
        cardColor = config.colors.cardColor;
        headingColor = config.colors.headingColor;
        legendColor = config.colors.bodyColor;
        labelColor = config.colors.textMuted;
        borderColor = config.colors.borderColor;
    }

    let employees = data.employeeList;
    let experienceDetails = data.employeeExperienceList;

    const currentDate = new Date();

    // Function to calculate the age from dob
    function calculateAge(dob) {
        const birthDate = new Date(dob);
        let age = currentDate.getFullYear() - birthDate.getFullYear();
        const m = currentDate.getMonth() - birthDate.getMonth();
        if (m < 0 || (m === 0 && currentDate.getDate() < birthDate.getDate())) {
            age--;
        }
        const ageInMonths = age * 12 + m;
        return ageInMonths / 12;  // Return age in years including months
    }

    // Function to calculate experience in years and months
    function calculateExperience(startDate, endDate) {
        const start = new Date(startDate);
        const end = new Date(endDate || currentDate);
        const years = end.getFullYear() - start.getFullYear();
        const months = end.getMonth() - start.getMonth();
        const totalMonths = years * 12 + months;
        return totalMonths / 12;  // Return experience in years including months
    }

    // Calculate average age
    const totalAge = employees.reduce((sum, employee) => sum + calculateAge(employee.dob), 0);
    const averageAge = totalAge / employees.length;

    // Calculate average experience in the company
    const totalCompanyExperience = employees.reduce((sum, employee) => sum + calculateExperience(employee.doj), 0);
    const averageCompanyExperience = totalCompanyExperience / employees.length;

    // Calculate total experience including past experiences
    const totalExperience = employees.reduce((sum, employee) => {
        const previousExperiences = experienceDetails
            .filter(detail => detail.employeeId === employee.employeeId)
            .reduce((expSum, detail) => expSum + calculateExperience(detail.joinDate, detail.lastWorkingDate), 0);
        const currentExperience = calculateExperience(employee.doj);
        return sum + previousExperiences + currentExperience;
    }, 0);
    const averageTotalExperience = totalExperience / employees.length;

    // Prepare data for the chart
    const categories = ["Average Age", "Average Experience in Company", "Average Total Experience"];
    const dataCounts = [averageAge, averageCompanyExperience, averageTotalExperience];

    const demographicBarChartEl = document.querySelector('#demographicChart'),
        demographicBarChartConfig = {
            chart: {
                height: 400,
                type: 'bar',
                toolbar: {
                    show: false
                }
            },
            plotOptions: {
                bar: {
                    barHeight: '80%',
                    columnWidth: '40%',
                    startingShape: 'rounded',
                    endingShape: 'rounded',
                    borderRadius: 2,
                    distributed: true,
                    dataLabels: {
                        position: 'top'
                    }
                }
            },
            grid: {
                show: false,
                padding: {
                    top: 20,
                    bottom: -10,
                    left: -10,
                    right: 0
                }
            },
            colors: [
                config.colors.primary,
                config.colors.warning,
                config.colors.info
            ],
            dataLabels: {
                enabled: true,
                offsetY: -20,
                formatter: function (val, opts) {
                    const category = opts.w.config.xaxis.categories[opts.dataPointIndex];
                    return val.toFixed(1) + " Years";
                },
                style: {
                    fontSize: '12px',
                    colors: [labelColor]
                }
            },
            series: [
                {
                    name: "",
                    data: dataCounts
                }
            ],
            legend: {
                show: false
            },
            xaxis: {
                categories: ['Avg.Age', 'Avg.Co.Exp', 'Avg.Total.Exp'],
                axisBorder: {
                    show: false
                },
                axisTicks: {
                    show: false
                },
                labels: {
                    style: {
                        colors: labelColor,
                        fontSize: '12px'
                    }
                }
            },
            yaxis: {
                labels: {
                    show: false
                },
                formatter: function (value) {
                    return value.toFixed(1); // Format the y-axis labels as decimals with one decimal place
                }
            },
            tooltip: {
                y: {
                    formatter: function (val) {
                        return val.toFixed(1) + " years";
                    }
                },
                x: {
                    formatter: function (val) {
                        if (val == "Avg.Age")
                            return "Average Age";
                        if (val == "Avg.Co.Exp")
                            return "Average Company Experience";
                        if (val == "Avg.Total.Exp")
                            return "Average Total Experience";
                    }
                }
            }
        };
    if (typeof demographicBarChartEl !== undefined && demographicBarChartEl !== null) {
        const demographicBarChart = new ApexCharts(demographicBarChartEl, demographicBarChartConfig);
        demographicBarChart.render();
    }
}


function EmployeeHeadCount1(data) {
    let cardColor, headingColor, legendColor, labelColor, shadeColor, borderColor;

    if (isDarkStyle) {
        cardColor = config.colors_dark.cardColor;
        headingColor = config.colors_dark.headingColor;
        legendColor = config.colors_dark.bodyColor;
        labelColor = config.colors_dark.textMuted;
        borderColor = config.colors_dark.borderColor;
    } else {
        cardColor = config.colors.cardColor;
        headingColor = config.colors.headingColor;
        legendColor = config.colors.bodyColor;
        labelColor = config.colors.textMuted;
        borderColor = config.colors.borderColor;
    }

    let employees = data.employeeList;
    let exits = data.employeeExitList;



    const getLast12Months = () => {
        const months = [];
        const now = new Date();
        for (let i = 5; i >= 0; i--) {
            const date = new Date(now.getFullYear(), now.getMonth() - i, 1);
            months.push(date.toISOString().slice(0, 7)); // Format: YYYY-MM
        }
        return months;
    };

    const processData = (employees, exits) => {
        const months = getLast12Months();
        const joiners = Array(6).fill(0);
        const leavers = Array(6).fill(0);
        const total = Array(6).fill(0);

        employees.forEach(emp => {
            const joinMonth = emp.doj.slice(0, 7);
            const index = months.indexOf(joinMonth);
            if (index !== -1) {
                joiners[index]++;
            }
        });
        exits.forEach(exit => {
            const exitMonth = exit.lastWorkingDateAdmin.slice(0, 7);
            const index = months.indexOf(exitMonth);
            if (index !== -1) {
                leavers[index]++;
            }
        });
        let cumulativeTotal = employees.length;
        for (let i = 0; i < 6; i++) {
            cumulativeTotal += (joiners[i] - leavers[i]);
            total[i] = cumulativeTotal;
        }

        return { months, joiners, leavers, total };
    };

    const { months, joiners, leavers, total } = processData(employees, exits);




    const EmployeeHeadCountChartEl = document.querySelector('#EmployeeHeadCount'),
        EmployeeHeadCountChartConfig = {
            chart: {
                height: 300,
                type: 'line',
                stacked: false,
                toolbar: {
                    show: false
                }
            },
            dataLabels: {
                enabled: false
            },
            series: [
                {
                    name: 'New Joiners',
                    type: 'column',
                    data: joiners
                },
                {
                    name: 'Exits',
                    type: 'column',
                    data: leavers
                }
                //,
                //{
                //    name: 'Total Employees',
                //    type: 'line',
                //    data: total
                //}
            ],
            stroke: {
                width: [0, 0, 2]
            },
            xaxis: {
                categories: months,
                //axisBorder: {
                //    show: false
                //},
                axisTicks: {
                    show: false
                },
                labels: {
                    style: {
                        fontSize: '12px'
                    }
                }
            },
            yaxis: [
                {
                    title: {
                        text: 'New Joiners/Exits',
                    }
                }
                //,
                //{
                //    opposite: true,
                //    title: {
                //        text: 'Total Employees'
                //    }
                //}
            ],
            //plotOptions: {
            //    bar: {
            //        barHeight: '80%',
            //        columnWidth: '40%',
            //        startingShape: 'rounded',
            //        endingShape: 'rounded',
            //        borderRadius: 2,
            //        distributed: true
            //    }
            //},
            //grid: {
            //    show: false,
            //    padding: {
            //        top: 20,
            //        bottom: -10,
            //        left: -10,
            //        right: 0
            //    }
            //},            


            //legend: {
            //    show: false
            //},

            tooltip: {
                shared: true,
                intersect: false
            }

        };
    if (typeof EmployeeHeadCountChartEl !== undefined && EmployeeHeadCountChartEl !== null) {
        const EmployeeHeadCountChart = new ApexCharts(EmployeeHeadCountChartEl, EmployeeHeadCountChartConfig);
        EmployeeHeadCountChart.render();
    }

}

function EmployeeHeadCount(data) {
    let cardColor, headingColor, legendColor, labelColor, shadeColor, borderColor;

    if (isDarkStyle) {
        cardColor = config.colors_dark.cardColor;
        headingColor = config.colors_dark.headingColor;
        legendColor = config.colors_dark.bodyColor;
        labelColor = config.colors_dark.textMuted;
        borderColor = config.colors_dark.borderColor;
    } else {
        cardColor = config.colors.cardColor;
        headingColor = config.colors.headingColor;
        legendColor = config.colors.bodyColor;
        labelColor = config.colors.textMuted;
        borderColor = config.colors.borderColor;
    }

    let employees = data.employeeList;
    let exits = data.employeeExitList;

    const getLast12Months = () => {
        const months = [];
        const now = new Date();
        for (let i = 5; i >= 0; i--) {
            const date = new Date(now.getFullYear(), now.getMonth() - i, 1);
            const formattedMonth = date.toLocaleString('default', { month: 'short' });
            const year = date.getFullYear();
            months.push(`${formattedMonth}-${year}`); // Format: Jan-2024, Feb-2024, ...
        }
        return months;
    };

    const processData = (employees, exits) => {
        const months = getLast12Months();
        const joiners = Array(6).fill(0);
        const leavers = Array(6).fill(0);
        const total = Array(6).fill(0);

        employees.forEach(emp => {
            const joinDate = new Date(emp.doj); // Assuming emp.doj is a valid date string
            const joinMonth = `${joinDate.toLocaleString('default', { month: 'short' })}-${joinDate.getFullYear()}`;
            const index = months.indexOf(joinMonth);
            if (index !== -1) {
                joiners[index]++;
            }
        });
        exits.forEach(exit => {
            const exitDate = new Date(exit.lastWorkingDateAdmin); // Assuming exit.lastWorkingDateAdmin is a valid date string
            const exitMonth = `${exitDate.toLocaleString('default', { month: 'short' })}-${exitDate.getFullYear()}`;
            const index = months.indexOf(exitMonth);
            if (index !== -1) {
                leavers[index]++;
            }
        });
        let cumulativeTotal = employees.length;
        for (let i = 0; i < 6; i++) {
            cumulativeTotal += (joiners[i] - leavers[i]);
            total[i] = cumulativeTotal;
        }

        return { months, joiners, leavers, total };
    };

    const { months, joiners, leavers, total } = processData(employees, exits);

    const EmployeeHeadCountChartEl = document.querySelector('#EmployeeHeadCount'),
        EmployeeHeadCountChartConfig = {
            chart: {
                height: 300,
                type: 'line',
                stacked: false,
                toolbar: {
                    show: false
                }
            },
            dataLabels: {
                enabled: false
            },
            series: [
                {
                    name: 'New Joiners',
                    type: 'column',
                    data: joiners
                },
                {
                    name: 'Exits',
                    type: 'column',
                    data: leavers
                }
                //,
                //{
                //    name: 'Total Employees',
                //    type: 'line',
                //    data: total
                //}
            ],
            stroke: {
                width: [0, 0, 2]
            },
            xaxis: {
                categories: months,
                axisTicks: {
                    show: false
                },
                labels: {
                    style: {
                        fontSize: '12px'
                    }
                }
            },
            yaxis: [
                {
                    title: {
                        text: 'New Joiners/Exits',
                    }
                }
                //,
                //{
                //    opposite: true,
                //    title: {
                //        text: 'Total Employees'
                //    }
                //}
            ],
            tooltip: {
                shared: true,
                intersect: false
            }
        };

    if (typeof EmployeeHeadCountChartEl !== undefined && EmployeeHeadCountChartEl !== null) {
        const EmployeeHeadCountChart = new ApexCharts(EmployeeHeadCountChartEl, EmployeeHeadCountChartConfig);
        EmployeeHeadCountChart.render();
    }
}


function WageBillTrendChart1(data) {

    var options = {
        chart: {
            height: 350,
            type: "line",
            stacked: false,
            toolbar: {
                show: false
            }
        },
        dataLabels: {
            enabled: true
        },
        colors: ["#FF1654", "#247BA0"],
        series: [
            {
                name: "Wage Bill pm",
                data: [1.4, 2, 2.5, 1.5, 2.5, 2.8, 3.8, 4.6]
            },
            {
                name: "Avg WB/Emp",
                data: [20, 29, 37, 36, 44, 45, 50, 58]
            }
        ],
        stroke: {
            width: [4, 4]
        },
        plotOptions: {
            bar: {
                columnWidth: "20%"
            }
        },
        xaxis: {
            categories: [2009, 2010, 2011, 2012, 2013, 2014, 2015, 2016]
        },
        yaxis: [
            {
                axisTicks: {
                    show: true
                },
                axisBorder: {
                    show: true,
                    color: "#FF1654"
                },
                labels: {
                    style: {
                        colors: "#FF1654"
                    }
                },
                title: {
                    text: "Wage Bill pm",
                    style: {
                        color: "#FF1654"
                    }
                }
            },
            {
                opposite: true,
                axisTicks: {
                    show: true
                },
                axisBorder: {
                    show: true,
                    color: "#247BA0"
                },
                labels: {
                    style: {
                        colors: "#247BA0"
                    }
                },
                title: {
                    text: "Avg WB/Emp",
                    style: {
                        color: "#247BA0"
                    }
                }
            }
        ],
        tooltip: {
            shared: false,
            intersect: true,
            x: {
                show: false
            }
        },
        legend: {
            horizontalAlign: "left",
            offsetX: 40
        }
    };

    var chart = new ApexCharts(document.querySelector("#WageBillTrendChart"), options);

    chart.render();
}

function WageBillTrendChart(data) {

    var seriesData = [
        {
            name: "Wage Bill pm",
            data: data.map(item => Math.round(item.wageBill))
        },
        {
            name: "Avg WB/Emp",
            data: data.map(item => Math.round(item.averageWageBill))
        }
    ];

    // Prepare x-axis categories from the input data
    var categories = data.map(item => {
        // Format month and year as "Month-Year"
        return `${getMonthName(item.month)}-${item.year}`;
    });

    function getMonthName(monthNumber) {
        // Array of month names
        var months = [
            "Jan", "Feb", "Mar", "Apr", "May", "Jun",
            "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        ];
        // Return the month name based on monthNumber (1-based index)
        return months[monthNumber - 1];
    }


    var options = {
        chart: {
            height: 350,
            type: "line",
            stacked: false,
            toolbar: {
                show: false
            }
        },
        dataLabels: {
            enabled: true
        },
        colors: ["#FF1654", "#247BA0"],
        series: seriesData,
        stroke: {
            width: [4, 4]
        },
        plotOptions: {
            bar: {
                columnWidth: "20%"
            }
        },
        xaxis: {
            categories: categories
        },
        yaxis: [
            {
                axisTicks: {
                    show: true
                },
                axisBorder: {
                    show: true,
                    color: "#FF1654"
                },
                labels: {
                    style: {
                        colors: "#FF1654"
                    }
                },
                title: {
                    text: "Wage Bill pm",
                    style: {
                        color: "#FF1654"
                    }
                }
            },
            {
                opposite: true,
                axisTicks: {
                    show: true
                },
                axisBorder: {
                    show: true,
                    color: "#247BA0"
                },
                labels: {
                    style: {
                        colors: "#247BA0"
                    }
                },
                title: {
                    text: "Avg WB/Emp",
                    style: {
                        color: "#247BA0"
                    }
                }
            }
        ],
        tooltip: {
            shared: false,
            intersect: true,
            x: {
                show: false
            }
        },
        legend: {
            horizontalAlign: "left",
            offsetX: 40
        }
    };

    var chart = new ApexCharts(document.querySelector("#WageBillTrendChart"), options);

    chart.render();
}




//Clock
function drawClock() {
    drawFace(ctx, radius);
    drawNumbers(ctx, radius);
    drawTime(ctx, radius);
}

function drawFace(ctx, radius) {
    const grad = ctx.createRadialGradient(0, 0, radius * 0.95, 0, 0, radius * 1.05);
    grad.addColorStop(0, '#333');
    grad.addColorStop(0.5, 'white');
    grad.addColorStop(1, '#333');
    ctx.beginPath();
    ctx.arc(0, 0, radius, 0, 2 * Math.PI);
    ctx.fillStyle = 'white';
    ctx.fill();
    ctx.strokeStyle = grad;
    ctx.lineWidth = radius * 0.1;
    ctx.stroke();
    ctx.beginPath();
    ctx.arc(0, 0, radius * 0.1, 0, 2 * Math.PI);
    ctx.fillStyle = '#333';
    ctx.fill();
}

function drawNumbers(ctx, radius) {
    ctx.font = radius * 0.15 + "px arial";
    ctx.textBaseline = "middle";
    ctx.textAlign = "center";
    for (let num = 1; num < 13; num++) {
        let ang = num * Math.PI / 6;
        ctx.rotate(ang);
        ctx.translate(0, -radius * 0.85);
        ctx.rotate(-ang);
        ctx.fillText(num.toString(), 0, 0);
        ctx.rotate(ang);
        ctx.translate(0, radius * 0.85);
        ctx.rotate(-ang);
    }
}

function drawTime(ctx, radius) {
    const now = new Date();
    let hour = now.getHours();
    let minute = now.getMinutes();
    let second = now.getSeconds();
    //hour
    hour = hour % 12;
    hour = (hour * Math.PI / 6) +
        (minute * Math.PI / (6 * 60)) +
        (second * Math.PI / (360 * 60));
    drawHand(ctx, hour, radius * 0.5, radius * 0.07);
    //minute
    minute = (minute * Math.PI / 30) +
        (second * Math.PI / (30 * 60));
    drawHand(ctx, minute, radius * 0.8, radius * 0.07);
    // second
    second = (second * Math.PI / 30);
    drawHand(ctx, second, radius * 0.9, radius * 0.02);
}

function drawHand(ctx, pos, length, width) {
    ctx.beginPath();
    ctx.lineWidth = width;
    ctx.lineCap = "round";
    ctx.moveTo(0, 0);
    ctx.rotate(pos);
    ctx.lineTo(0, -length);
    ctx.stroke();
    ctx.rotate(-pos);
}