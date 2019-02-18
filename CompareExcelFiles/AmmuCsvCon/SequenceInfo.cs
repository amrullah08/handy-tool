using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmmuCsvCon
{
    class SequenceInfo : SequenceInterface
    {
        public string Sequence { get; set; }
        public string Section { get; set; }

        string cSequence = "Name";
        string cSection = "Section";

        public SequenceInfo(CsvHelper.CsvReader csvReader)
        {
            Sequence = csvReader.GetField(cSequence);
            Section = csvReader.GetField(cSection);
        }
    }
}
