using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfSampleProject.Models;

namespace WcfSampleProject
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IProviderService" in both code and config file together.
    [ServiceContract]
    public interface IProviderService
    {
        //[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        string DoSomeTasks(SampleModel data);
    }
}
