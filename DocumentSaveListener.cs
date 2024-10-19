using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using System;

namespace OnSaveFormatter
{
    internal class DocumentSaveListener : IVsRunningDocTableEvents3
    {
        private readonly ExtensionPage _options;
        public event EventHandler<string> BeforeSave; // Event for before save
        public DocumentSaveListener(ExtensionPage options)
        {
            _options = options;
        }

        public int OnBeforeSave(uint docCookie)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!_options.EnableAutoFormatOnSave)
                return VSConstants.S_OK;

            var rdt = new RunningDocumentTable(ServiceProvider.GlobalProvider);
            var documentInfo = rdt.GetDocumentInfo(docCookie);

            BeforeSave?.Invoke(this, documentInfo.Moniker);

            return VSConstants.S_OK;
        }

        // Other methods of IVsRunningDocTableEvents3 can be implemented as needed
        public int OnAfterSave(uint docCookie) => VSConstants.S_OK;
        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs) => VSConstants.S_OK;
        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame) => VSConstants.S_OK;
        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining) => VSConstants.S_OK;
        public int OnAfterSaveAll() => VSConstants.S_OK;
        public int OnAfterDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining) => VSConstants.S_OK;
        public int OnAfterAttributeChangeEx(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew) => VSConstants.S_OK;
        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame) => VSConstants.S_OK;

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining) => VSConstants.S_OK;
    }
}
