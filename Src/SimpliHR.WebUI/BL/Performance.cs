using ExcelDataReader;
using Org.BouncyCastle.Bcpg;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Performace;
using System.Data;

namespace SimpliHR.WebUI.BL;

public class Performance
{

    public List<PerformanceKRAMasterDBDTO> GetKRADataFromCSVFile(Stream stream, int UserId, int UnitId, int PerformanceSettingId)
    {

        var data = new List<PerformanceKRAMasterDBDTO>();
        try
        {
            using (var reader = ExcelReaderFactory.CreateCsvReader(stream))
            {
                var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true // To set First Row As Column Names    
                    }
                });
                //Models.ManageVendor.MVendor mVendor = new Models.ManageVendor.MVendor();
                if (dataSet.Tables.Count > 0)
                {
                    var dataTable = dataSet.Tables[0];
                    if (!dataTable.Columns.Contains("Employee Code"))
                    {
                        throw new Exception("The CSV file does not contain the 'Employee Code' column.");
                    }
                    if (!dataTable.Columns.Contains("Key Result Areas"))
                    {
                        throw new Exception("The CSV file does not contain the 'Key Result Areas' column.");
                    }
                    if (!dataTable.Columns.Contains("WeightAge(%)"))
                    {
                        throw new Exception("The CSV file does not contain the 'WeightAge(%)' column.");
                    }

                    var emptyEmployeeCodeRows = dataTable.AsEnumerable().Where(row => string.IsNullOrWhiteSpace(row["Employee Code"].ToString())).ToList();
                    if (emptyEmployeeCodeRows.Any())
                    {
                        throw new Exception("The CSV file contains empty records in the 'Employee Code' column.");
                    }
                    var emptyKeyResultAreasRows = dataTable.AsEnumerable().Where(row => string.IsNullOrWhiteSpace(row["Key Result Areas"].ToString())).ToList();
                    if (emptyKeyResultAreasRows.Any())
                    {
                        throw new Exception("The CSV file contains empty records in the 'Key Result Areas' column.");
                    }
                    // Check for invalid "WeightAge(%)" records
                    var invalidWeightageRows = dataTable.AsEnumerable().Where(row => string.IsNullOrWhiteSpace(row["WeightAge(%)"].ToString()) || !double.TryParse(row["WeightAge(%)"].ToString(), out _)).ToList();
                    if (invalidWeightageRows.Any())
                    {
                        throw new Exception("The CSV file contains invalid records in the 'WeightAge(%)' column.");
                    }

                    foreach (DataRow objDataRow in dataTable.Rows)
                    {
                        if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString()))) continue;
                        data.Add(new PerformanceKRAMasterDBDTO()
                        {
                            IsActive = true,
                            CreatedDate = System.DateTime.Now,
                            EmployeeCode = objDataRow["Employee Code"].ToString(),
                            KRA = objDataRow["Key Result Areas"].ToString(),
                            Weightage = Convert.ToDouble(objDataRow["WeightAge(%)"].ToString()),
                            CreatedBy = UserId,
                            UnitId = UnitId,
                            Source = "KRA",
                            PerformanceSettingId = PerformanceSettingId
                        });
                    }
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
        return data;
    }

    public List<PerformanceKRAMasterDBDTO> GetBehavoralDataFromCSVFile(Stream stream, int UserId, int UnitId, int PerformanceSettingId)
    {

        var data = new List<PerformanceKRAMasterDBDTO>();
        try
        {
            using (var reader = ExcelReaderFactory.CreateCsvReader(stream))
            {
                var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true // To set First Row As Column Names    
                    }
                });
                //Models.ManageVendor.MVendor mVendor = new Models.ManageVendor.MVendor();
                if (dataSet.Tables.Count > 0)
                {
                    var dataTable = dataSet.Tables[0];

                    if (!dataTable.Columns.Contains("Employee Code"))
                    {
                        throw new Exception("The CSV file does not contain the 'Employee Code' column.");
                    }
                    if (!dataTable.Columns.Contains("Attribute"))
                    {
                        throw new Exception("The CSV file does not contain the 'Attribute' column.");
                    }
                    if (!dataTable.Columns.Contains("WeightAge(%)"))
                    {
                        throw new Exception("The CSV file does not contain the 'WeightAge(%)' column.");
                    }

                    var emptyEmployeeCodeRows = dataTable.AsEnumerable().Where(row => string.IsNullOrWhiteSpace(row["Employee Code"].ToString())).ToList();
                    if (emptyEmployeeCodeRows.Any())
                    {
                        throw new Exception("The CSV file contains empty records in the 'Employee Code' column.");
                    }
                    var emptyAttributeRows = dataTable.AsEnumerable().Where(row => string.IsNullOrWhiteSpace(row["Attribute"].ToString())).ToList();
                    if (emptyAttributeRows.Any())
                    {
                        throw new Exception("The CSV file contains empty records in the 'Attribute' column.");
                    }
                    // Check for invalid "WeightAge(%)" records
                    var invalidWeightageRows = dataTable.AsEnumerable().Where(row => string.IsNullOrWhiteSpace(row["WeightAge(%)"].ToString()) || !double.TryParse(row["WeightAge(%)"].ToString(), out _)).ToList();
                    if (invalidWeightageRows.Any())
                    {
                        throw new Exception("The CSV file contains invalid records in the 'WeightAge(%)' column.");
                    }

                    foreach (DataRow objDataRow in dataTable.Rows)
                    {
                        if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString()))) continue;
                        data.Add(new PerformanceKRAMasterDBDTO()
                        {
                            IsActive = true,
                            CreatedDate = System.DateTime.Now,
                            EmployeeCode = objDataRow["Employee Code"].ToString(),
                            KRA = objDataRow["Attribute"].ToString(),
                            Weightage = Convert.ToDouble(objDataRow["WeightAge(%)"].ToString()),
                            CreatedBy = UserId,
                            UnitId = UnitId,
                            Source = "Behavioral",
                            PerformanceSettingId = PerformanceSettingId
                        });
                    }
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
        return data;
    }

}
