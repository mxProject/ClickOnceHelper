# ClickOnceHelper
A helper class that controls version updates by ClickOnce.

# Dependency
.NET Framework 4.5

# Features
* Provides async/await asynchronous methods. ApplicationDeployment class provides an asynchronous method of the event callback method, but now I think that the async / await method is more familiar.

* According to the registered assembly name and download group pairs, download the assembly when the assembly is referenced.

# Usage

### CheckForUpdate / CheckForUpdateAsync

```C#
private ClickOnceController m_ClickOnce;

private void CheckForUpdate()
{
    try
    {
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
}

private async Task CheckForUpdateAsync()
{
    try
    {
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
}
```

### UpdateApplication / UpdateApplicationAsync

```C#

private ClickOnceController m_ClickOnce;

private bool UpdateApplication()
{
    try
    {
        bool result = m_ClickOnce.Update();
        WriteLog(result ? "The version of this application has been updated." : "The version of this application was not updated.");
        return result;
    }
    catch (Exception ex)
    {
        WriteExceptionLog(ex);
        return false;
    }
}

private async Task<bool> UpdateApplicationAsync()
{
    try
    {
        bool result = await m_ClickOnce.UpdateAsync().ConfigureAwait(false);
        WriteLog(result ? "The version of this application has been updated." : "The version of this application was not updated.");
        return result;
    }
    catch (Exception ex)
    {
        WriteExceptionLog(ex);
        return false;
    }
}
```

### DownloadFileGroup / DownloadFileGroupAsync

```C#

private ClickOnceController m_ClickOnce;

private bool DownloadFileGroup(string groupName)
{
    try
    {
        bool result = m_ClickOnce.DownloadFileGroup(groupName);
        WriteLog(string.Format(result ? "The version of '{0}' has been updated." : "The version of '{0}' was not updated.", groupName));
        return result;
    }
    catch (Exception ex)
    {
        WriteExceptionLog(ex);
        return false;
    }
}

private async Task<bool> DownloadFileGroupAsync(string groupName)
{
    try
    {
        bool result = await m_ClickOnce.DownloadFileGroupAsync(groupName).ConfigureAwait(false);
        WriteLog(string.Format(result ? "The version of '{0}' has been updated." : "The version of '{0}' was not updated.", groupName));
        return result;
    }
    catch (Exception ex)
    {
        WriteExceptionLog(ex);
        return false;
    }
}
```

# Licence
This software is released under the MIT License, see LICENSE.

# Authors
mxProject

