using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmmuCsvCon
{
    class CompareExcels
    {
        public void Compare()
        {
            string file = "";
            string finalFilesFolder = @"C:\Users\syamrull\Downloads\Script Step templates\final";
            string allSriptStepFileLoc = @"C:\Users\syamrull\Downloads\Script Step templates\final\Script Step Advanced Find View 1-25-2019 11-17-06 AM.xlsx";

            //C:\Users\syamrull\Downloads\Script Step templates\final\Test Script Advanced Find View 1-25-2019 8-58-19 AM.csv

            ExcelToCSVCoversion(allSriptStepFileLoc, out file);

            CsvHelper.CsvReader destinationReader = new CsvHelper.CsvReader(File.OpenText(file + ".csv"));
            destinationReader.Read();
            destinationReader.ReadHeader();
            List<ComparingFile> comparingFile = new List<ComparingFile>();

            while (destinationReader.Read())
            {
                var destinationReadersource = new ComparingFile(destinationReader);
                comparingFile.Add(destinationReadersource);
            }

            string[] files = System.IO.Directory.GetFiles(finalFilesFolder, "*.Final.xlsx");

            foreach (var fileName in files)
            {
                ExcelToCSVCoversion(fileName, out file);
                if (string.IsNullOrEmpty(file)) continue;
                CsvHelper.CsvReader csvReader = new CsvHelper.CsvReader(File.OpenText(file + ".csv"));
                csvReader.Read();
                csvReader.ReadHeader();
                List<SourceExcel> sourceExcels = new List<SourceExcel>();
                while (csvReader.Read())
                {
                    var source = new SourceExcel(csvReader);
                    sourceExcels.Add(source);
                }

                CompareExcel(sourceExcels, comparingFile);
            }

            Console.ReadLine();
        }

        static bool CompareExcel(List<SourceExcel> sourceExcels, List<ComparingFile> comparingFiles)
        {
            var left = sourceExcels.OrderBy(cc => cc.ScriptStepId).ToList();
            var list = left.Select(cc => cc.TestScript).Distinct();
            comparingFiles = (from m in comparingFiles
                             orderby m.TestScript, m.ScriptStepId
                             select m).ToList();

            var right = comparingFiles.Where(cc => list.Any(c => (c.Equals(cc.TestScript)))).ToList();

            for (int i = 0; i < left.Count; i++)
            {
                for(int j=0;j<right.Count; j++)
                {
                    if(left[i].ScriptStepId == right[j].ScriptStepId && !right[j].Found)
                    {
                        left[i].Found = right[j].Found = true;
                        left[i].ContentMatch = right[j].ContentMatch = (Compare(left[i], right[j]));
                        break;
                    }
                }
            }

            var leftNotFound = left.Where(cc => !cc.Found).ToList();
            var rightNotFound = right.Where(cc => !cc.Found).ToList();

            var leftContentMisMatch = left.Where(cc => !cc.ContentMatch).ToList();
            var rightContentMisMatch = right.Where(cc => !cc.ContentMatch).ToList();

            Console.WriteLine(string.Format("Tag : {0}  TotalFinalRecords : {1}  " +
                "FinalFileNotFound : {2}  FinalFileContentMismatch : {3}  " +
                "TotalProductionRecords : {4}  ProductionNotFound : {5}  ProductionFileContentMismatch : {6}", 
                left[0].TestScript, 
                left.Count, leftNotFound.Count, leftContentMisMatch.Count, 
                right.Count, rightNotFound.Count, rightContentMisMatch.Count));


            return true;
        }

        static bool Compare(SourceExcel sourceExcel,ComparingFile comparingFile)
        {
            bool result = true;
            if (!sourceExcel.HRAE.Equals(comparingFile.HRAE))
            {
                Console.WriteLine(string.Format("HRAE Final : {0} Production : {1}",sourceExcel.HRAE, comparingFile.HRAE));
                result = false;
            }
            if (!sourceExcel.ResultType.Equals(comparingFile.ResultType))
            {
                Console.WriteLine(string.Format("ResultType Final : {0} Production : {1}", sourceExcel.ResultType, comparingFile.ResultType));
                result = false;
            }
            if (!sourceExcel.ScriptStep.Equals(comparingFile.ScriptStep))
            {
                Console.WriteLine(string.Format("ScriptStep Final : {0} Production : {1}", sourceExcel.ScriptStep, comparingFile.ScriptStep));
                result = false;
            }
            if (!sourceExcel.ScriptStepId.Equals(comparingFile.ScriptStepId))
            {
                Console.WriteLine(string.Format("ScriptStepId Final : {0} Production : {1}", sourceExcel.ScriptStepId, comparingFile.ScriptStepId));
                result = false;
            }
            if (!sourceExcel.Section.Equals(comparingFile.Section))
            {
                Console.WriteLine(string.Format("Section Final : {0} Production : {1}", sourceExcel.Section, comparingFile.Section));
                result = false;
            }
            if (!sourceExcel.Sequence.Equals(comparingFile.Sequence))
            {
                Console.WriteLine(string.Format("HRAE Sequence : {0} Production : {1}", sourceExcel.Sequence, comparingFile.Sequence));
                result = false;
            }
            if (!sourceExcel.ShowComments.Equals(comparingFile.ShowComments))
            {
                Console.WriteLine(string.Format("ShowComments Final : {0} Production : {1}", sourceExcel.ShowComments, comparingFile.ShowComments));
                result = false;
            }
            if (!sourceExcel.StepResultValues.Equals(comparingFile.StepResultValues))
            {
                Console.WriteLine(string.Format("StepResultValues Final : {0} Production : {1}", sourceExcel.StepResultValues, comparingFile.StepResultValues));
                result = false;
            }
            if (!sourceExcel.TestScript.Equals(comparingFile.TestScript))
            {
                Console.WriteLine(string.Format("TestScript Final : {0} Production : {1}", sourceExcel.TestScript, comparingFile.TestScript));
                result = false;
            }
            return result;
        }

        static void ExcelToCSVCoversion(string sourceFile,out string destinationFile)
        {
            Microsoft.Office.Interop.Excel.Application rawData = new Microsoft.Office.Interop.Excel.Application();
            destinationFile = null;
            string targetFile = string.Empty;
            try
            {
                var tempFileSheet1 = (Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())) + "data1";
                var tempFileSheet2 = (Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())) + "data1";
                destinationFile = tempFileSheet1;
                Workbook workbook = rawData.Workbooks.Open(sourceFile);
                int i = 0;
                foreach (Worksheet sheet in workbook.Worksheets)
                {
                    if (i == 0)
                    {
                        sheet.SaveAs(tempFileSheet1, XlFileFormat.xlCSV);
                    }
                    else
                    {
                        if (i == 2)
                        {
                            try
                            {
                                sheet.SaveAs(tempFileSheet2, XlFileFormat.xlCSV);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    i++;

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                }
            }
            catch(Exception ex)
            {
                destinationFile = null;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                rawData.DisplayAlerts = false;
                rawData.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rawData);
            }


            //Console.WriteLine();
            //Console.WriteLine($"The excel file {sourceFile} has been converted into {targetFile} (CSV format).");
            //Console.WriteLine();
        }
    }
}
