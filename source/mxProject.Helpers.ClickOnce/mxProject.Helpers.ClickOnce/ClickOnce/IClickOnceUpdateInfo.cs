using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Deployment.Application;

namespace mxProject.ClickOnce
{

    /// <summary>
    /// 
    /// </summary>
    public interface IClickOnceUpdateInfo 
    {

        /// <summary>
        /// 
        /// </summary>
        Version AvailableVersion
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        bool IsUpdateRequired
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        Version MinimumRequiredVersion
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        bool UpdateAvailable
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        long? UpdateSizeBytes
        {
            get;
        }

    }

}
