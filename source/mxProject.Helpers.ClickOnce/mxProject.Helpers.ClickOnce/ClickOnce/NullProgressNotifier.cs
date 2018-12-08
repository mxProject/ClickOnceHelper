using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxProject.ClickOnce
{

    /// <summary>
    /// 
    /// </summary>
    internal class NullProgressNotifier : IClickOnceProgressNotifier
    {

        /// <summary>
        /// 
        /// </summary>
        internal static readonly NullProgressNotifier Value = new NullProgressNotifier();

        private NullProgressNotifier() { }

        /// <summary>
        /// 
        /// </summary>
        void IClickOnceProgressNotifier.Start() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="progress"></param>
        void IClickOnceProgressNotifier.Progress(IClickOnceProgressInfo progress) { }

        /// <summary>
        /// 
        /// </summary>
        void IClickOnceProgressNotifier.Complete() { }

    }

}
