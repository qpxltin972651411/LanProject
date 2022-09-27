using LanProject.MainApplication.Model;
using LanProject.MainApplication.ValidationExceptionBehavior;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanProject.MainApplication.ViewModel
{
    public class SelfCompanyViewModel: BaseViewModel, IMySelfValidationException
    {
        public int MySelfErrorCount { get; set; }
        public SelfCompanyViewModel() : base() {}
        public int CalcErrorCount(CreateUnit source)
        {
            int result = MySelfErrorCount;
            if (!source.HaveTax) result -= 1;
            if (!source.HaveTel) result -= 2;
            if (!source.HaveFax) result -= 2;
            if (!source.HaveAddress) result -= 3;
            return result;
        }
    }
}
