using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanProject.MainApplication.Model
{
    public class ModelBase : ObservableObject
    {
        #region ICloneable Members
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion
    }
}
