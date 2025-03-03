using Azure;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Exit;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics.Metrics;

namespace SimpliHR.WebUI.BL;

public class InterViewResponses
{

    public EmployeeExitManagementViewModel getInterViewResponses(EmployeeExitManagementViewModel outputDTO)
    {
        if (outputDTO.ExitInterviewForm != null)
        {
            List<InterviewResponses> interviewResponsesList = new List<InterviewResponses>();
            var jsonLinq = JObject.Parse(outputDTO.ExitInterviewForm.Component);
            var srcArray = jsonLinq.Descendants().Where(d => d is JArray).First();
            outputDTO.HeaderComponents = JsonConvert.DeserializeObject<List<component>>(srcArray.ToString());
            if (outputDTO.ExitResignationList != null)
            {
                foreach (var resignationDetail in outputDTO.ExitResignationList)
                {
                    List<component>? componentList = new List<component>();
                    //componentList = outputDTO.HeaderComponents;

                    ////////////                

                    componentList = JsonConvert.DeserializeObject<List<component>>(JObject.Parse(outputDTO.ExitInterviewForm.Component).Descendants().Where(d => d is JArray).First().ToString());

                    ////////////////


                    var interviewResponses = resignationDetail.ExitInterviewData;
                    if (interviewResponses != null)
                    {
                        var jsonLinq_interviewResponses = JObject.Parse(interviewResponses);
                        if (jsonLinq_interviewResponses != null)
                        {
                            foreach (var interviewResponse in jsonLinq_interviewResponses)
                            {
                                string questionType = outputDTO.HeaderComponents.Where(x => x.key == interviewResponse.Key).Select(x => x.type).FirstOrDefault();
                                string questionKey = interviewResponse.Key;
                                if (questionType == "survey")
                                {
                                    if (interviewResponse.Value.Count() > 0)
                                    {
                                        var jsonitem_responseValue = JObject.Parse(interviewResponse.Value.ToString());
                                        foreach (var item_responseValue in jsonitem_responseValue)
                                        {
                                            componentList.Where(x => x.key == questionKey).FirstOrDefault().questions.Where(x => x.value == item_responseValue.Key).FirstOrDefault().response = "Yes";
                                        }
                                    }
                                }
                                else if (questionType == "textarea")
                                {
                                    componentList.Where(x => x.key == questionKey).FirstOrDefault().response = interviewResponse.Value.ToString();
                                }
                                else if (questionType == "radio")
                                {
                                    //componentList.Where(x => x.key == questionKey).FirstOrDefault().response = componentList.Where(x => x.key == questionKey).FirstOrDefault().values.Where(x => x.value == interviewResponse.Value.ToString()).Select(x => x.label).FirstOrDefault().ToString();  //interviewResponse.Value.ToString();
                                    var x = componentList.Where(x => x.key == questionKey).FirstOrDefault().values.Where(x => x.value == interviewResponse.Value.ToString()).ToList();
                                    if (x.Count > 0)
                                    {
                                        componentList.Where(x => x.key == questionKey).FirstOrDefault().response = x.FirstOrDefault().label.ToString();
                                    }

                                }
                                else
                                {
                                    componentList.Where(x => x.key == questionKey).FirstOrDefault().response = interviewResponse.Value.ToString();
                                }
                            }
                            InterviewResponses interviewResponses1 = new InterviewResponses();
                            interviewResponses1.Responses = componentList;
                            interviewResponses1.ResignationDetails = resignationDetail;
                            interviewResponsesList.Add(interviewResponses1);
                        }
                    }
                }
            }
            outputDTO.ResponseComponent = interviewResponsesList;
        }
        return outputDTO;
    }

    public XLWorkbook CreateExcelFile(EmployeeExitManagementViewModel outputDTO)
    {
        var wb = new XLWorkbook();
        wb.AddWorksheet(1);
        var ws1 = wb.Worksheet(1);
        string sheetName = "Responses";
        ws1.Name = sheetName;

        int _col = 0;
        int _row = 0;
        _col += 1;
        _row += 1;


        ////////////////////////////

        ws1.Cell(_row, _col).Value = "Survey Name";
        ws1.Cell(_row, _col).Style.Fill.BackgroundColor = XLColor.Blue;
        ws1.Cell(_row, _col).Style.Font.FontColor = XLColor.White;
        _col += 1;
        ws1.Cell(_row, _col).Value = "Exit Interview";
        _col += 1;
        ws1.Cell(_row, _col).Value = "Filter (LWD)";
        ws1.Cell(_row, _col).Style.Font.FontColor = XLColor.Red;
        _col += 1;

        string lwdf = "";
        if (outputDTO.InterviewFilters.LastWorkingDateFrom != null)
        {
            lwdf = outputDTO.InterviewFilters.LastWorkingDateFrom.Value.ToString("dd-MMM-yyyy");
        }

        ws1.Cell(_row, _col).Value = "From : " + lwdf;
        ws1.Cell(_row, _col).Style.NumberFormat.Format = "dd-MMM-yyyy";
        ws1.Cell(_row, _col).Style.Font.FontColor = XLColor.Red;
        _col += 1;

        string lwdt = "";
        if (outputDTO.InterviewFilters.LastWorkingDateTo != null)
        {
            lwdt = outputDTO.InterviewFilters.LastWorkingDateTo.Value.ToString("dd-MMM-yyyy");
        }
        ws1.Cell(_row, _col).Value = "To : " + lwdt;
        ws1.Cell(_row, _col).Style.Font.FontColor = XLColor.Red;
        _col += 1;

        ws1.Columns(1, _col - 1).Width = 15;

        _row += 1;
        _row += 1;
        ///////////////////////////

        int tableStartRow = _row;
        _col = 1;

        ws1.Cell(_row, _col).Value = "Employee Code";
        ws1.Range(ws1.Cell(_row, _col), ws1.Cell(_row + 1, _col)).Merge();
        ws1.Cell(_row, _col).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        _col += 1;
        ws1.Cell(_row, _col).Value = "Employee Name";
        ws1.Range(ws1.Cell(_row, _col), ws1.Cell(_row + 1, _col)).Merge();
        ws1.Cell(_row, _col).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        _col += 1;
        ws1.Cell(_row, _col).Value = "Designation";
        ws1.Range(ws1.Cell(_row, _col), ws1.Cell(_row + 1, _col)).Merge();
        ws1.Cell(_row, _col).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        _col += 1;
        ws1.Cell(_row, _col).Value = "Department";
        ws1.Range(ws1.Cell(_row, _col), ws1.Cell(_row + 1, _col)).Merge();
        ws1.Cell(_row, _col).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        _col += 1;
        ws1.Cell(_row, _col).Value = "Location";
        ws1.Range(ws1.Cell(_row, _col), ws1.Cell(_row + 1, _col)).Merge();
        ws1.Cell(_row, _col).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        _col += 1;
        ws1.Cell(_row, _col).Value = "Date of Joining";
        ws1.Range(ws1.Cell(_row, _col), ws1.Cell(_row + 1, _col)).Merge();
        ws1.Cell(_row, _col).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        _col += 1;
        ws1.Cell(_row, _col).Value = "Last Working Date";
        ws1.Range(ws1.Cell(_row, _col), ws1.Cell(_row + 1, _col)).Merge();
        ws1.Cell(_row, _col).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        _col += 1;
        ws1.Cell(_row, _col).Value = "Survey Sent Date";
        ws1.Range(ws1.Cell(_row, _col), ws1.Cell(_row + 1, _col)).Merge();
        ws1.Cell(_row, _col).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        _col += 1;
        ws1.Cell(_row, _col).Value = "Response Date";
        ws1.Range(ws1.Cell(_row, _col), ws1.Cell(_row + 1, _col)).Merge();
        ws1.Cell(_row, _col).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        _col += 1;

        ws1.Columns(tableStartRow, _col - 1).Width = 15;
        ws1.Range(ws1.Cell(tableStartRow, 1), ws1.Cell(tableStartRow, _col - 1)).Style.Font.FontColor = XLColor.Red;

        if (outputDTO.HeaderComponents != null)
        {
            if (outputDTO.HeaderComponents.Count > 0)
            {
                foreach (var item in outputDTO.HeaderComponents)
                {
                    if (item.type != "button")
                    {
                        if (item.type == "survey")
                        {
                            ws1.Cell(_row, _col).Value = item.label;
                            ws1.Range(ws1.Cell(_row, _col), ws1.Cell(_row, _col + item.questions.Count - 1)).Merge();
                            ws1.Cell(_row, _col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            foreach (var qitem in item.questions)
                            {
                                ws1.Cell(_row + 1, _col).Value = qitem.label;
                                ws1.Column(_col).AdjustToContents();
                                _col += 1;
                            }
                        }
                        else
                        {
                            ws1.Cell(_row, _col).Value = item.label;
                            ws1.Column(_col).AdjustToContents();
                            ws1.Range(ws1.Cell(_row, _col), ws1.Cell(_row + 1, _col)).Merge();
                            ws1.Cell(_row, _col).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            _col += 1;
                        }
                    }

                }
            }
        }
        //ws1.Columns(10, _col).AdjustToContents();
        ws1.Range(ws1.Cell(tableStartRow, 1), ws1.Cell(tableStartRow + 1, _col - 1)).Style.Font.Bold = true;
        ws1.Range(ws1.Cell(tableStartRow, 1), ws1.Cell(tableStartRow + 1, _col - 1)).Style.Fill.BackgroundColor = XLColor.AliceBlue;
        ws1.Range(ws1.Cell(tableStartRow, 1), ws1.Cell(tableStartRow + 1, _col - 1)).Style.Alignment.WrapText = true;

        _row += 1;
        _row += 1;

        if (outputDTO.ResponseComponent != null)
        {
            foreach (var item in outputDTO.ResponseComponent)
            {
                if (item.ResignationDetails != null)
                {
                    _col = 1;

                    ws1.Cell(_row, _col).Value = item.ResignationDetails.EmployeeCode;
                    _col += 1;
                    ws1.Cell(_row, _col).Value = item.ResignationDetails.EmployeeName;
                    _col += 1;
                    ws1.Cell(_row, _col).Value = item.ResignationDetails.JobTitle;
                    _col += 1;
                    ws1.Cell(_row, _col).Value = item.ResignationDetails.DepartmentName;
                    _col += 1;
                    ws1.Cell(_row, _col).Value = item.ResignationDetails.Location;
                    //ws1.Cell(_row, _col).Value = "";
                    _col += 1;
                    if (item.ResignationDetails.DOJ != null)
                    {
                        ws1.Cell(_row, _col).Value = item.ResignationDetails.DOJ.Value.ToString("dd-MMM-yyyy");
                    }
                    else
                    {
                        ws1.Cell(_row, _col).Value = "";
                    }
                    ws1.Cell(_row, _col).Style.NumberFormat.Format = "dd-MMM-yyyy";
                    _col += 1;

                    if (item.ResignationDetails.LastWorkingDateAdmin != null)
                    {
                        ws1.Cell(_row, _col).Value = item.ResignationDetails.LastWorkingDateAdmin.Value.ToString("dd-MMM-yyyy");
                    }
                    else
                    {
                        ws1.Cell(_row, _col).Value = "";
                    }
                    ws1.Cell(_row, _col).Style.NumberFormat.Format = "dd-MMM-yyyy";
                    _col += 1;

                    #region Survey Sent Date
                    if (item.ResignationDetails.AdminApprovalDate != null)
                    {
                        ws1.Cell(_row, _col).Value = item.ResignationDetails.AdminApprovalDate.Value.ToString("dd-MMM-yyyy");
                    }
                    else
                    {
                        ws1.Cell(_row, _col).Value = "";
                    }
                    ws1.Cell(_row, _col).Style.NumberFormat.Format = "dd-MMM-yyyy";
                    _col += 1;
                    #endregion

                    #region Survey Resonse Date

                    if (item.ResignationDetails.ExitInterviewSubmissionDate != null)
                    {
                        ws1.Cell(_row, _col).Value = item.ResignationDetails.ExitInterviewSubmissionDate.Value.ToString("dd-MMM-yyyy");
                    }
                    else
                    {
                        ws1.Cell(_row, _col).Value = "";
                    }
                    ws1.Cell(_row, _col).Style.NumberFormat.Format = "dd-MMM-yyyy";
                    _col += 1;
                    #endregion




                    if (item.Responses != null)
                    {
                        foreach (var responses in item.Responses)
                        {
                            if (responses.type != "button")
                            {
                                if (responses.type == "survey")
                                {
                                    if (responses.questions != null)
                                    {
                                        if (responses.questions.Count > 0)
                                        {
                                            foreach (var question in responses.questions)
                                            {
                                                ws1.Cell(_row, _col).Value = question.response;
                                                _col += 1;
                                            }
                                        }
                                    }

                                    //ws1.Cell(_row, _col).Value = item.label;
                                    //ws1.Range(ws1.Cell(_row, _col), ws1.Cell(_row, _col + item.questions.Count - 1)).Merge();
                                    //foreach (var qitem in item.questions)
                                    //{
                                    //    ws1.Cell(_row + 1, _col).Value = qitem.label;
                                    //    _col += 1;
                                    //}
                                }
                                else
                                {
                                    ws1.Cell(_row, _col).Value = responses.response;
                                    _col += 1;
                                }
                            }
                        }
                    }


                    _row += 1;
                }
            }
        }
        ws1.Range(ws1.Cell(tableStartRow, 1), ws1.Cell(tableStartRow + 1, _col)).Style.Alignment.WrapText = true;

        ws1.Range(tableStartRow, 1, _row - 1, _col - 1).Style
          .Border.SetOutsideBorder(XLBorderStyleValues.Thick)
          .Border.SetInsideBorder(XLBorderStyleValues.Thin);

        return wb;
    }



}
