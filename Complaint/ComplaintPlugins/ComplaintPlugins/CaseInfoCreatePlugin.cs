using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintPlugins
{

    public class CaseInfoCreatePlugin : IPlugin
    {//The instance member used in multiple violation patterns
        internal IOrganizationService Service { get; set; }
        internal IPluginExecutionContext Context { get; set; }
        internal ITracingService tracingService { get; set; }

        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            this.Context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            //The violation
            this.Service = factory.CreateOrganizationService(this.Context.UserId);

            const string target = "Target";
            const string entityName = "syed_caseinfo";
            if (Context.InputParameters.Contains(target) && Context.InputParameters[target] is Entity)
            {
                //obtain the target 
                Entity entity = (Entity)Context.InputParameters[target];
                if (entity.LogicalName != entityName)
                    return;

                try
                {
                    entity.Attributes["syed_casenumber"] = this.GetCaseNumber();
                    entity.Attributes["syed_name"] = entity.Attributes["syed_casenumber"];
                    entity.Attributes["syed_title"] = "Complaint " + entity.FormattedValues["syed_complainttype"].ToString() + " Case Number " + entity.Attributes["syed_casenumber"];

                    //int age = entity.GetAttributeValue<int>("syed_casenumber");
                    //tracingService.Trace("Aget = " + age);
                    //if (age > 60)
                    //{
                    //    Entity en = new Entity(entity.LogicalName, entity.Id);
                    //    en["new_color"] = new OptionSetValue(100000001);
                    //    Service.crUpdate(en);
                    //}
                    //else
                    //{
                    //    throw new InvalidPluginExecutionException("Age should be greater than 60 ");
                    //}
                    //Service.Create(entity);
                }
                catch (InvalidPluginExecutionException ex)
                {
                    throw ex;
                }
            }
        }

        private string GetCaseNumber()
        {
            string caseNumber = null;
            DateTime current = DateTime.Now;
            caseNumber = string.Format("CAS : {0}-{1}-{2}-{3}", current.Day, current.Month, current.Year, 
                (current.Hour.ToString() + current.Minute.ToString() + current.Second.ToString() + current.Millisecond.ToString()));
            return caseNumber;
        }
    }
}
