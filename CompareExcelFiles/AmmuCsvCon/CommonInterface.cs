using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmmuCsvCon
{
    interface CommonInterface
    {
        string TestScript { get; set; }
        string ScriptStep{ get; set; }
         float ScriptStepId{ get; set; }
         string ResultType{ get; set; }
         string StepResultValues{ get; set; }
         string Section{ get; set; }
         string Sequence{ get; set; }
         string HRAE{ get; set; }
         string ShowComments{ get; set; }
         string CanStepFail{ get; set; }

        bool Found { get; set; }

        bool ContentMatch { get; set; }

    }
}
