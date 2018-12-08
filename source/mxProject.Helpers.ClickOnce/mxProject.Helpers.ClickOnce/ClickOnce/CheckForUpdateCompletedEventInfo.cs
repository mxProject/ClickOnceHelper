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
    internal class CheckForUpdateCompletedEventInfo : IClickOnceUpdateInfo
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        internal CheckForUpdateCompletedEventInfo(CheckForUpdateCompletedEventArgs info)
        {
            m_Info = info;
        }

        private readonly CheckForUpdateCompletedEventArgs m_Info;

        /// <summary>
        /// 
        /// </summary>
        public Version AvailableVersion
        {
            get
            {
                if (!m_Info.UpdateAvailable) { return null; }
                return m_Info.AvailableVersion;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsUpdateRequired
        {
            get
            {
                if (!m_Info.UpdateAvailable) { return false; }
                return m_Info.IsUpdateRequired;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Version MinimumRequiredVersion
        {
            get
            {
                if (!m_Info.UpdateAvailable) { return null; }
                return m_Info.MinimumRequiredVersion;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UpdateAvailable
        {
            get { return m_Info.UpdateAvailable; }
        }

        /// <summary>
        /// 
        /// </summary>
        public long? UpdateSizeBytes
        {
            get
            {
                if (!m_Info.UpdateAvailable) { return null; }
                return m_Info.UpdateSizeBytes;
            }
        }

    }

}
