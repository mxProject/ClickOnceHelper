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
    public interface IClickOnceProgressNotifier
    {

        /// <summary>
        /// 
        /// </summary>
        void Start();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="progress"></param>
        void Progress(IClickOnceProgressInfo progress);

        /// <summary>
        /// 
        /// </summary>
        void Complete();

    }

}
