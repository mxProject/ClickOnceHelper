using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxProject.ClickOnce
{

    //
    // 概要:
    //     Represents progress data reported in an asynchronous operation.
    public interface IClickOnceProgressInfo
    {
        //
        // 概要:
        //     Gets the number of bytes already downloaded by this operation.
        //
        // 戻り値:
        //     An System.Int64 representing the data already transferred, in bytes.
        long BytesCompleted { get; }

        //
        // 概要:
        //     Gets the total number of bytes in the download operation.
        //
        // 戻り値:
        //     An System.Int64 representing the total size of the download, in bytes.
        long BytesTotal { get; }

        //
        // 概要:
        //     Gets the action that the process is currently executing.
        //
        // 戻り値:
        //     A System.Deployment.Application.DeploymentProgressState value, stating what element
        //     or elements the operation is currently downloading.
        string State { get; }

        //
        // 概要:
        //     Gets the name of the file group being downloaded.
        //
        // 戻り値:
        //     A System.String containing the name of the file group, if the event occurred
        //     as the result of a call to Overload:System.Deployment.Application.ApplicationDeployment.DownloadFileGroupAsync;
        //     otherwise, a zero-length string.
        string Group { get; }

    }

}
