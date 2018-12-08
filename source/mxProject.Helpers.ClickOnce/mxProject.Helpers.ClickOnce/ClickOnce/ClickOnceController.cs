using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Deployment.Application;
using System.ComponentModel;
using System.IO;

namespace mxProject.ClickOnce
{

    /// <summary>
    /// Control application update with ClickOnce.
    /// </summary>
    public class ClickOnceController
    {

        #region ctor

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="notifier">an object for notifying progress.</param>
        /// <param name="autoDownloadAssembly">Whether to automatically download download groups containing assemblies.</param>
        public ClickOnceController(IClickOnceProgressNotifier notifier, bool autoDownloadAssembly)
        {
            Notifier = notifier ?? NullProgressNotifier.Value;
            AutoDownloadAssembly = autoDownloadAssembly;

            if (autoDownloadAssembly)
            {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            }
        }

        #endregion

        /// <summary>
        /// Gets the object for notifying progress.
        /// </summary>
        public IClickOnceProgressNotifier Notifier { get; }

        #region IsNetworkDeployed

        /// <summary>
        /// Gets whether it is running as a ClickOnce application.
        /// </summary>
        public bool IsNetworkDeployed
        {
            get { return ApplicationDeployment.IsNetworkDeployed; }
        }

        #endregion

        #region Application information

        /// <summary>
        /// Get the current version installed. Returns null if this application is not a ClickOnce application.
        /// </summary>
        /// <returns></returns>
        public Version GetCurrentVersion()
        {
            if (IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the URL of the application distribution destination. Returns null if this application is not a ClickOnce application.
        /// </summary>
        /// <returns></returns>
        public Uri GetActivationUri()
        {
            if (IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.ActivationUri;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get the path of the data directory of the application. Returns null if this application is not a ClickOnce application.
        /// </summary>
        /// <returns></returns>
        public string GetDataDirectory()
        {
            if (IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.DataDirectory;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets whether or not it is the first rub since the current version was installed.
        /// </summary>
        /// <returns></returns>
        public bool IsFirstRun()
        {
            if (IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.IsFirstRun;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the date and time the version check was last executed. Returns null if this application is not a ClickOnce application.
        /// </summary>
        /// <returns></returns>
        public DateTime? GetLastUpdateCheckDateTime()
        {
            if (IsNetworkDeployed)
            {
                DateTime date = ApplicationDeployment.CurrentDeployment.TimeOfLastUpdateCheck;

                if (date.Kind == DateTimeKind.Local)
                {
                    return date;
                }
                else
                {
                    return date.ToLocalTime();
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetUpdatedApplicationFullName()
        {
            if (IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.UpdatedApplicationFullName;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Version GetUpdatedVersion()
        {
            if (IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.UpdatedVersion;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Uri GetUpdateLocation()
        {
            if (IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.UpdateLocation;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region CheckForUpdate

        /// <summary>
        /// Gets whether the updated version is published.
        /// </summary>
        /// <returns>Update information.</returns>
        /// <exception cref="ClickOnceException">
        /// An exception occurred during version check.
        /// </exception>
        public IClickOnceUpdateInfo CheckForUpdate()
        {
            try
            {
                UpdateCheckInfo info = ApplicationDeployment.CurrentDeployment.CheckForDetailedUpdate(false);
                return new ClickOnceUpdateInfo(info);
            }
            catch (Exception ex)
            {
                throw new ClickOnceException("An exception occurred during version check. " + ex.Message, ex);
            }
        }

        #endregion

        #region CheckForUpdateAsync

        /// <summary>
        /// Gets whether the updated version is published.
        /// </summary>
        /// <returns>Update information.</returns>
        /// <exception cref="ClickOnceException">
        /// An exception occurred during version check.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Version check was canceled.
        /// </exception>
        public async Task<IClickOnceUpdateInfo> CheckForUpdateAsync()
        {

            CountdownEvent countdown = null;
            CheckForUpdateCompletedEventInfo info = null;

            void progress(object sender, DeploymentProgressChangedEventArgs e)
            {
                Notifier.Progress(new ClickOnceProgressInfo(e));
            }

            void completed(object sender, CheckForUpdateCompletedEventArgs e)
            {
                Notifier.Complete();

                if (e.Error != null)
                {
                    throw new ClickOnceException("An exception occurred during version check. " + e.Error.Message, e.Error);
                }

                if (e.Cancelled)
                {
                    throw new OperationCanceledException("Version check was canceled.");
                }

                info = new CheckForUpdateCompletedEventInfo(e);
                countdown?.Signal();
            }

            try
            {

                ApplicationDeployment.CurrentDeployment.CheckForUpdateProgressChanged += progress;
                ApplicationDeployment.CurrentDeployment.CheckForUpdateCompleted += completed;

                Notifier.Start();

                countdown = new CountdownEvent(1);

                await Task.Run(() => ApplicationDeployment.CurrentDeployment.CheckForUpdateAsync()).ConfigureAwait(false);

                countdown.Wait();

            }
            finally
            {
                ApplicationDeployment.CurrentDeployment.CheckForUpdateProgressChanged -= progress;
                ApplicationDeployment.CurrentDeployment.CheckForUpdateCompleted -= completed;

                countdown?.Dispose();
            }

            return info;

        }

        #endregion

        #region Update

        /// <summary>
        /// Update application version.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ClickOnceException">
        /// An exception occurred during version update.
        /// </exception>
        public bool Update()
        {
            try
            {
                return ApplicationDeployment.CurrentDeployment.Update();
            }
            catch (Exception ex)
            {
                throw new ClickOnceException("An exception occurred during version update. " + ex.Message, ex);
            }
        }

        #endregion

        #region UpdateAsync

        /// <summary>
        /// Update application version.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ClickOnceException">
        /// An exception occurred during version update.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Version update was canceled.
        /// </exception>
        public async Task<bool> UpdateAsync()
        {

            CountdownEvent countdown = null;
            bool result = false;

            void progress(object sender, DeploymentProgressChangedEventArgs e)
            {
                Notifier.Progress(new ClickOnceProgressInfo(e));
            }

            void completed(object sender, AsyncCompletedEventArgs e)
            {
                Notifier.Complete();

                if (e.Error != null)
                {
                    throw new ClickOnceException("An exception occurred during version update. " + e.Error.Message, e.Error);
                }

                if (e.Cancelled)
                {
                    throw new OperationCanceledException("Version update was canceled.");
                }
                
                result = true;
                countdown?.Signal();
            }

            try
            {

                ApplicationDeployment.CurrentDeployment.UpdateProgressChanged += progress;
                ApplicationDeployment.CurrentDeployment.UpdateCompleted += completed;

                Notifier.Start();

                countdown = new CountdownEvent(1);

                await Task.Run(() => ApplicationDeployment.CurrentDeployment.UpdateAsync()).ConfigureAwait(false);

                countdown.Wait();

            }
            finally
            {
                ApplicationDeployment.CurrentDeployment.UpdateProgressChanged -= progress;
                ApplicationDeployment.CurrentDeployment.UpdateCompleted -= completed;

                countdown?.Dispose();
            }

            return result;

        }

        #endregion

        #region IsFileGroupDownloaded

        /// <summary>
        /// Gets whether or not the specified download group is downloaded.
        /// </summary>
        /// <param name="groupName">The download group name.</param>
        /// <returns></returns>
        public bool IsFileGroupDownloaded(string groupName)
        {
            return ApplicationDeployment.CurrentDeployment.IsFileGroupDownloaded(groupName);
        }

        #endregion

        #region DownloadFileGroup

        /// <summary>
        /// Download specified download group.
        /// </summary>
        /// <param name="groupName">The download group name.</param>
        /// <exception cref="ClickOnceException">
        /// An exception occurred during version update.
        /// </exception>
        public bool DownloadFileGroup(string groupName)
        {
            try
            {
                ApplicationDeployment.CurrentDeployment.DownloadFileGroup(groupName);
                return true;
            }
            catch (Exception ex)
            {
                throw new ClickOnceException(string.Format("An exception occurred while updating the version of group '{0}'. {1}", groupName, ex.Message), ex);
            }
        }

        #endregion

        #region DownloadFileGroupAsync

        /// <summary>
        /// Download specified download group.
        /// </summary>
        /// <param name="groupName">The download group name.</param>
        /// <returns></returns>
        /// <exception cref="ClickOnceException">
        /// An exception occurred during version update.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Version update was canceled.
        /// </exception>
        public async Task<bool> DownloadFileGroupAsync(string groupName)
        {

            CountdownEvent countdown = null;
            bool result = false;

            void progress(object sender, DeploymentProgressChangedEventArgs e)
            {
                Notifier.Progress(new ClickOnceProgressInfo(e));
            }

            void completed(object sender, AsyncCompletedEventArgs e)
            {
                Notifier.Complete();

                if (e.Error != null)
                {
                    throw new ClickOnceException(string.Format("An exception occurred while updating the version of group '{0}'. {1}", groupName, e.Error.Message), e.Error);
                }

                if (e.Cancelled)
                {
                    throw new OperationCanceledException("Version update was canceled.");
                }

                result = true;

                countdown?.Signal();
            }

            try
            {

                ApplicationDeployment.CurrentDeployment.DownloadFileGroupProgressChanged += progress;
                ApplicationDeployment.CurrentDeployment.DownloadFileGroupCompleted += completed;

                Notifier.Start();

                countdown = new CountdownEvent(1);

                await Task.Run(() => ApplicationDeployment.CurrentDeployment.DownloadFileGroupAsync(groupName)).ConfigureAwait(false);

                countdown.Wait();

            }
            finally
            {
                ApplicationDeployment.CurrentDeployment.DownloadFileGroupProgressChanged -= progress;
                ApplicationDeployment.CurrentDeployment.DownloadFileGroupCompleted -= completed;

                countdown?.Dispose();
            }

            return result;

        }

        #endregion

        #region download groups

        /// <summary>
        /// Gets a value indicating whether to automatically download the download group containing the assembly.
        /// </summary>
        public bool AutoDownloadAssembly { get; }

        private readonly Dictionary<string, string> m_AssembyAndGroups = new Dictionary<string, string>();
        private readonly Dictionary<string, string> m_FileAndGroups = new Dictionary<string, string>();

        /// <summary>
        /// Register the download group of the specified assembly.
        /// </summary>
        /// <param name="assemblyName">The assembly name. (excluding extension)</param>
        /// <param name="groupName">The download group name.</param>
        public void RegistAssemblyDownloadGroup(string assemblyName, string groupName)
        {
            m_AssembyAndGroups[NormalizeDownloadFileName(assemblyName)] = groupName;
        }

        /// <summary>
        /// Unregister the download group of the specified assembly.
        /// </summary>
        /// <param name="assemblyName">The assembly name. (excluding extension)</param>
        public bool UnregistAssemblyDownloadGroup(string assemblyName)
        {
            return m_AssembyAndGroups.Remove(NormalizeDownloadFileName(assemblyName));
        }

        /// <summary>
        /// Gets the download group of the specified assembly.
        /// </summary>
        /// <param name="assemblyName">The assembly name. (excluding extension)</param>
        /// <param name="groupName">The download group name.</param>
        /// <returns></returns>
        public bool TryGetAssemblyDownloadGroup(string assemblyName, out string groupName)
        {
            return m_AssembyAndGroups.TryGetValue(NormalizeDownloadFileName(assemblyName), out groupName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {

            string assemblyName = args.Name.Split(',')[0];

            if (TryGetAssemblyDownloadGroup(assemblyName, out string groupName))
            {
                if (DownloadFileGroup(groupName))
                {
                    return GetAssembly(assemblyName);
                }
            }

            return null;

        }

        /// <summary>
        /// Gets the specified assembly.
        /// </summary>
        /// <param name="assemblyName">The assembly name. (excluding extension)</param>
        /// <returns></returns>
        private Assembly GetAssembly(string assemblyName)
        {

            string appPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);

            string aseemblyPath = Path.Combine(appPath, assemblyName + ".dll");

            if (File.Exists(aseemblyPath))
            {
                return Assembly.LoadFile(aseemblyPath);
            }

            aseemblyPath = Path.Combine(appPath, assemblyName + ".exe");

            if (File.Exists(aseemblyPath))
            {
                return Assembly.LoadFile(aseemblyPath);
            }

            return null;

        }

        /// <summary>
        /// Register the download group of the specified file.
        /// </summary>
        /// <param name="fileName">The file name. (including extension)</param>
        /// <param name="groupName">The download group name.</param>
        public void RegistFileDownloadGroup(string fileName, string groupName)
        {
            m_FileAndGroups[NormalizeDownloadFileName(fileName)] = groupName;
        }

        /// <summary>
        /// Unregister the download group of the specified file.
        /// </summary>
        /// <param name="fileName">The file name. (including extension)</param>
        public bool UnregistFileDownloadGroup(string fileName)
        {
            return m_FileAndGroups.Remove(NormalizeDownloadFileName(fileName));
        }

        /// <summary>
        /// Gets the download group of the specified file.
        /// </summary>
        /// <param name="fileName">The file name. (including extension)</param>
        /// <param name="groupName">The download group name.</param>
        /// <returns></returns>
        public bool TryGetFileDownloadGroup(string fileName, out string groupName)
        {
            return m_FileAndGroups.TryGetValue(NormalizeDownloadFileName(fileName), out groupName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string NormalizeDownloadFileName(string fileName)
        {
            return fileName.ToLower().Trim();
        }

        #endregion

    }
}
