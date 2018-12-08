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
    internal class ClickOnceProgressInfo : IClickOnceProgressInfo
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventArgs"></param>
        internal ClickOnceProgressInfo(DeploymentProgressChangedEventArgs eventArgs)
        {
            m_Args = eventArgs;
        }

        private readonly DeploymentProgressChangedEventArgs m_Args;

        /// <summary>
        /// 
        /// </summary>
        public long BytesCompleted
        {
            get { return m_Args.BytesCompleted; }
        }

        /// <summary>
        /// 
        /// </summary>
        public long BytesTotal
        {
            get { return m_Args.BytesTotal; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string State
        {
            get { return m_Args.State.ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Group
        {
            get { return m_Args.Group; }
        }

    }

}
