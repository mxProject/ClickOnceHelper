using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxProject.ClickOnce;

namespace ClickOnceSampleConsoleApp
{

    internal class AppUpdator : IClickOnceProgressNotifier
    {

        /// <summary>
        /// 
        /// </summary>
        internal AppUpdator()
        {

            m_ClickOnce = new ClickOnceController(this, true);

            m_ClickOnce.RegistAssemblyDownloadGroup("ClassLibrary1", "Group1");
            m_ClickOnce.RegistAssemblyDownloadGroup("ClassLibrary2", "Group2");

        }

        private readonly ClickOnceController m_ClickOnce;

        #region update

        /// <summary>
        /// Update this application.
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> UpdateAsync()
        {

            if (!m_ClickOnce.IsNetworkDeployed)
            {
                WriteLog("This application is not a ClickOnce application.");
                return false;
            }

            ShowApplicationInformation(m_ClickOnce);

            IClickOnceUpdateInfo info = await m_ClickOnce.CheckForUpdateAsync().ConfigureAwait(false);

            if (!info.UpdateAvailable)
            {
                WriteLog("This application is the latest version.");
                return false;
            }

            if (!await m_ClickOnce.UpdateAsync().ConfigureAwait(false))
            {
                WriteLog("This application was not updated.");
                return false;
            }

            ShowApplicationInformation(m_ClickOnce);

            return true;

        }

        #endregion

        #region application information

        /// <summary>
        /// 
        /// </summary>
        private void ShowApplicationInformation(ClickOnceController clickOnce)
        {

            if (!clickOnce.IsNetworkDeployed) { return; }

            WriteLog("===== Application Information =====");

            WriteLog(string.Format("UpdatedApplicationFullName = {0}", clickOnce.GetUpdatedApplicationFullName()));
            WriteLog(string.Format("CurrentVersion = {0}", clickOnce.GetCurrentVersion()));
            WriteLog(string.Format("UpdatedVersion = {0}", clickOnce.GetUpdatedVersion()));
            WriteLog(string.Format("ActivationUri = {0}", clickOnce.GetActivationUri()));
            WriteLog(string.Format("UpdateLocation = {0}", clickOnce.GetUpdateLocation()));
            WriteLog(string.Format("DataDirectory = {0}", clickOnce.GetDataDirectory()));
            WriteLog(string.Format("LastUpdateCheckDateTime = {0}", clickOnce.GetLastUpdateCheckDateTime()));
            WriteLog(string.Format("Group1.IsFileGroupDownloaded = {0}", clickOnce.IsFileGroupDownloaded("Group1")));
            WriteLog(string.Format("Group2.IsFileGroupDownloaded = {0}", clickOnce.IsFileGroupDownloaded("Group2")));

            WriteLog("");

        }

        #endregion

        #region Notification

        /// <summary>
        /// 
        /// </summary>
        void IClickOnceProgressNotifier.Complete()
        {
            WriteLog("----- complate -----");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="progress"></param>
        void IClickOnceProgressNotifier.Progress(IClickOnceProgressInfo progress)
        {
            WriteLog("----- progress -----");
            WriteLog(string.Format("State = {0}", progress.State));
            WriteLog(string.Format("Group = {0}", progress.Group));
            WriteLog(string.Format("State = {0}/{1}", progress.BytesCompleted, progress.BytesTotal));
        }

        /// <summary>
        /// 
        /// </summary>
        void IClickOnceProgressNotifier.Start()
        {
            WriteLog("----- start -----");
        }

        #endregion

        #region logging

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        private void WriteLog(string log)
        {
            if (string.IsNullOrEmpty(log))
            {
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff ") + log);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        private void WriteExceptionLog(Exception ex)
        {
            WriteLog(string.Format("[{0}] {1}", ex.GetType().Name, ex.Message));
            WriteLog(ex.StackTrace);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getter"></param>
        /// <returns></returns>
        private string GetValueOrException(Func<object> getter)
        {
            try
            {
                object value = getter();
                return value == null ? "(null)" : value.ToString();
            }
            catch (Exception ex)
            {
                return string.Format("({0}:{1})", ex.GetType().Name, ex.Message);
            }
        }

        #endregion

    }

}
