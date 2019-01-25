using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmmuCsvCon
{
    class ComparingFile : CommonInterface
    {

        const string cTestScript = "Test Script";
        const string cScriptStep = "Script Step";
        const string cScriptStepId = "Script Step ID";
        const string cResultType = "Result Type";
        const string cStepResultValues = "Step Result Values";
        const string cSection = "Section";
        const string cSequence = "Sequence";
        const string cHRAE = "HRAE";
        const string cShowComments = "Show Comments?";
        const string cCanStepFail = "Can Step Fail?";

        public ComparingFile(CsvHelper.CsvReader csvReader)
        {
            TestScript = csvReader.GetField(cTestScript);
            ScriptStep = csvReader.GetField(cScriptStep);
            if (string.IsNullOrEmpty(csvReader.GetField(cScriptStepId)))
            {
                ScriptStepId = 0;
            }
            else
                ScriptStepId = csvReader.GetField<float>(cScriptStepId);
            ResultType = csvReader.GetField(cResultType);
            StepResultValues = csvReader.GetField(cStepResultValues);
            Section = csvReader.GetField(cSection);
            Sequence = csvReader.GetField(cSequence);
            HRAE = csvReader.GetField(cHRAE);
            ShowComments = csvReader.GetField(cShowComments);
            CanStepFail = csvReader.GetField(cCanStepFail);
        }

        public string TestScript { get; set; }
        public string ScriptStep { get; set; }
        public float ScriptStepId { get; set; }
        public string ResultType { get; set; }
        public string StepResultValues { get; set; }
        public string Section { get; set; }
        public string Sequence { get; set; }
        public string HRAE { get; set; }
        public string ShowComments { get; set; }
        public string CanStepFail { get; set; }
        public bool Found { get; set; }
        public bool ContentMatch { get; set; }
    }
}
