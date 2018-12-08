using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.IO;

using mxProject.ClickOnce;

namespace ClickOnceSampleApp
{

    /// <summary>
    /// 
    /// </summary>
    public partial class Form1 : Form, IClickOnceProgressNotifier
    {

        #region ctor

        /// <summary>
        /// 
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            Init();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {

            m_ApplicationTitle = this.Text;

            m_ClickOnce = new ClickOnceController(this, true);

            m_ClickOnce.RegistAssemblyDownloadGroup("ClassLibrary1", "Group1");
            m_ClickOnce.RegistAssemblyDownloadGroup("ClassLibrary2", "Group2");
            m_ClickOnce.RegistFileDownloadGroup("Group3.txt", "Group3");

        }

        #region event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            ShowApplicationInformation();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCheckForUpdate_Click(object sender, EventArgs e)
        {
            CheckForUpdate();
            ShowApplicationInformation();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnCheckForUpdateAsync_Click(object sender, EventArgs e)
        {
            await CheckForUpdateAsync().ConfigureAwait(false);
            ShowApplicationInformation();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (!UpdateApplication()) { return; }
            ShowApplicationInformation();
            if (NeedRestart()) { Restart(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnUpdateAsync_Click(object sender, EventArgs e)
        {
            if (!await UpdateApplicationAsync()) { return; }
            ShowApplicationInformation();
            if (NeedRestart()) { Restart(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDownloadFileGroup1_Click(object sender, EventArgs e)
        {
            if (!DownloadFileGroup("Group1")) { return; }
            ShowApplicationInformation();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnDownloadFileGroupAsync1_Click(object sender, EventArgs e)
        {
            if (!await DownloadFileGroupAsync("Group1")) { return; }
            ShowApplicationInformation();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDownloadFileGroup2_Click(object sender, EventArgs e)
        {
            if (!DownloadFileGroup("Group2")) { return; }
            ShowApplicationInformation();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnDownloadFileGroupAsync2_Click(object sender, EventArgs e)
        {
            if (!await DownloadFileGroupAsync("Group2")) { return; }
            ShowApplicationInformation();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDownloadFileGroup3_Click(object sender, EventArgs e)
        {
            if (!DownloadFileGroup("Group3")) { return; }
            ShowApplicationInformation();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnDownloadFileGroupAsync3_Click(object sender, EventArgs e)
        {
            if (!await DownloadFileGroupAsync("Group3")) { return; }
            ShowApplicationInformation();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void BtnExecuteClass1_Click(object sender, EventArgs e)
        {
            try
            {
                ClassLibrary1.Class1 obj = new ClassLibrary1.Class1();
                WriteLog(obj.GetName());
            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void BtnExecuteClass2_Click(object sender, EventArgs e)
        {
            try
            {
                ClassLibrary2.Class2 obj = new ClassLibrary2.Class2();
                WriteLog(obj.GetName());
            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnLoadFile3_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists("Group3.txt"))
                {
                    if (m_ClickOnce.TryGetFileDownloadGroup("Group3.txt", out string groupName))
                    {
                        await m_ClickOnce.DownloadFileGroupAsync(groupName).ConfigureAwait(false);
                    }
                }

                WriteLog(string.Format("TextFile3 = {0}", File.ReadAllText("Group3.txt")));
            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex);
            }
        }

        #endregion

        private string m_ApplicationTitle;
        private ClickOnceController m_ClickOnce;

        #region application information

        /// <summary>
        /// 
        /// </summary>
        private void ShowApplicationInformation()
        {

            if (!m_ClickOnce.IsNetworkDeployed) { return; }

            this.Text = string.Format("{0} {1}", m_ApplicationTitle, m_ClickOnce.GetCurrentVersion());

            WriteLog("===== Application Information =====");

            WriteLog(string.Format("UpdatedApplicationFullName = {0}", m_ClickOnce.GetUpdatedApplicationFullName()));
            WriteLog(string.Format("CurrentVersion = {0}", m_ClickOnce.GetCurrentVersion()));
            WriteLog(string.Format("UpdatedVersion = {0}", m_ClickOnce.GetUpdatedVersion()));
            WriteLog(string.Format("ActivationUri = {0}", m_ClickOnce.GetActivationUri()));
            WriteLog(string.Format("UpdateLocation = {0}", m_ClickOnce.GetUpdateLocation()));
            WriteLog(string.Format("DataDirectory = {0}", m_ClickOnce.GetDataDirectory()));
            WriteLog(string.Format("LastUpdateCheckDateTime = {0}", m_ClickOnce.GetLastUpdateCheckDateTime()));
            WriteLog(string.Format("Group1.IsFileGroupDownloaded = {0}", m_ClickOnce.IsFileGroupDownloaded("Group1")));
            WriteLog(string.Format("Group2.IsFileGroupDownloaded = {0}", m_ClickOnce.IsFileGroupDownloaded("Group2")));
            WriteLog(string.Format("Group3.IsFileGroupDownloaded = {0}", m_ClickOnce.IsFileGroupDownloaded("Group3")));
 
            WriteLog("");

        }

        #endregion

        #region version check

        /// <summary>
        /// 
        /// </summary>
        private void CheckForUpdate()
        {

            WriteLog("===== CheckForUpdate =====");

            try
            {
                pnlButtons.Enabled = false;

                IClickOnceUpdateInfo info = m_ClickOnce.CheckForUpdate();
                WriteLog(string.Format("AvailableVersion = {0}", info.AvailableVersion));
                WriteLog(string.Format("IsUpdateRequired = {0}", info.IsUpdateRequired));
                WriteLog(string.Format("MinimumRequiredVersion = {0}", info.MinimumRequiredVersion));
                WriteLog(string.Format("UpdateAvailable = {0}", info.UpdateAvailable));
                WriteLog(string.Format("UpdateSizeBytes = {0}", info.UpdateSizeBytes));
            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex);
            }
            finally
            {
                WriteLog("");
                pnlButtons.Enabled = true;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task CheckForUpdateAsync()
        {

            WriteLog("===== CheckForUpdateAsync =====");

            try
            {
                pnlButtons.Enabled = false;

                IClickOnceUpdateInfo info = await m_ClickOnce.CheckForUpdateAsync().ConfigureAwait(false);
                WriteLog(string.Format("AvailableVersion = {0}", info.AvailableVersion));
                WriteLog(string.Format("IsUpdateRequired = {0}", info.IsUpdateRequired));
                WriteLog(string.Format("MinimumRequiredVersion = {0}", info.MinimumRequiredVersion));
                WriteLog(string.Format("UpdateAvailable = {0}", info.UpdateAvailable));
                WriteLog(string.Format("UpdateSizeBytes = {0}", info.UpdateSizeBytes));
            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex);
            }
            finally
            {
                WriteLog("");
                pnlButtons.Enabled = true;
            }

        }

        #endregion

        #region update

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool UpdateApplication()
        {

            WriteLog("===== UpdateApplication =====");

            try
            {
                pnlButtons.Enabled = false;

                bool result = m_ClickOnce.Update();

                WriteLog(result ? "The version of this application has been updated." : "The version of this application was not updated.");

                return result;

            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex);
                return false;
            }
            finally
            {
                WriteLog("");
                pnlButtons.Enabled = true;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<bool> UpdateApplicationAsync()
        {

            WriteLog("===== UpdateApplication =====");

            try
            {
                pnlButtons.Enabled = false;

                bool result = await m_ClickOnce.UpdateAsync().ConfigureAwait(false);

                WriteLog(result ? "The version of this application has been updated." : "The version of this application was not updated.");

                return result;

            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex);
                return false;
            }
            finally
            {
                WriteLog("");
                pnlButtons.Enabled = true;
            }

        }

        #endregion

        #region DownloadFileGroup

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool DownloadFileGroup(string groupName)
        {

            WriteLog("===== UpdateApplication =====");

            try
            {
                pnlButtons.Enabled = false;

                bool result = m_ClickOnce.DownloadFileGroup(groupName);

                WriteLog(string.Format(result ? "The version of '{0}' has been updated." : "The version of '{0}' was not updated.", groupName));

                return result;

            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex);
                return false;
            }
            finally
            {
                WriteLog("");
                pnlButtons.Enabled = true;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<bool> DownloadFileGroupAsync(string groupName)
        {

            WriteLog("===== UpdateApplication =====");

            try
            {
                pnlButtons.Enabled = false;

                bool result = await m_ClickOnce.DownloadFileGroupAsync(groupName).ConfigureAwait(false);

                WriteLog(string.Format(result ? "The version of '{0}' has been updated." : "The version of '{0}' was not updated.", groupName));

                return result;

            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex);
                return false;
            }
            finally
            {
                WriteLog("");
                pnlButtons.Enabled = true;
            }

        }

        #endregion

        #region progress

        /// <summary>
        /// 
        /// </summary>
        void IClickOnceProgressNotifier.Start()
        {
            WriteLog("----- start -----");
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
            WriteLog("");
        }

        void IClickOnceProgressNotifier.Complete()
        {
            WriteLog("----- complete -----");
            WriteLog("");
        }

        #endregion

        #region restart

        /// <summary>
        /// Get whether this application needs to be restarted.
        /// </summary>
        /// <returns></returns>
        private bool NeedRestart()
        {
            if (MessageBox.Show(this, "Would you like to restart this application?", this.Text, MessageBoxButtons.OKCancel) != DialogResult.OK) { return false; }
            return true;
        }

        /// <summary>
        /// Restart this application.
        /// </summary>
        private void Restart()
        {
            Application.Restart();
        }

        #endregion

        #region logging

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        private void WriteLog(string log)
        {
            ShowLog(log);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        private void ShowLog(string log)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action<string>(ShowLog), new object[] { log });
                return;
            }

            if (string.IsNullOrEmpty(log))
            {
                txtLog.AppendText(Environment.NewLine);
            }
            else
            {
                txtLog.AppendText(DateTime.Now.ToString("HH:mm:ss.fff ") + log + Environment.NewLine);
            }

            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
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
            catch ( Exception ex)
            {
                return string.Format("({0}:{1})", ex.GetType().Name, ex.Message);
            }
        }

        #endregion

    }
}
