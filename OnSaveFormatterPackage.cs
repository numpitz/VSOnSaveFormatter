using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using static Nerdbank.Streams.MultiplexingStream;
using Task = System.Threading.Tasks.Task;

namespace OnSaveFormatter
{
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(OnSaveFormatterPackage.PackageGuidString)]
    [ProvideOptionPage(typeof(ExtensionPage), "OnSaveFormatter", "General", 0, 0, true)]
    public sealed class OnSaveFormatterPackage : AsyncPackage
    {
        public const string PackageGuidString = "115f9fe0-6c01-4fed-aad5-8a4b9e5acdbe";
        private DocumentSaveListener _documentSaveListener;
        private DTE2 _dte;
        private RunningDocumentTable _rdt;
        private uint _rdtCookie; // Cookie to manage event subscription
        private ExtensionPage _options;

        #region Package Members

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            _options = (ExtensionPage)GetDialogPage(typeof(ExtensionPage));
            _dte = (DTE2)await GetServiceAsync(typeof(DTE));

            var command = _dte?.Commands.Item("Edit.FormatDocument");
            if (command == null || !command.IsAvailable)
            {
                _options.IsFormatDocumentCommandAvailable = false;
                _dte = null;
                return;
            }

            _documentSaveListener = new DocumentSaveListener(_options);

            if (_dte != null)
            {
                _rdt = new RunningDocumentTable(this);
                Subscribe();
            }
        }

        private void Subscribe()
        {
            if (_rdt == null || _options == null)
                return;

            _documentSaveListener.BeforeSave -= OnBeforeDocumentSave;
            _documentSaveListener.BeforeSave += OnBeforeDocumentSave;
            _rdtCookie = _rdt.Advise(_documentSaveListener);
        }

        private void Unsubscribe()
        {
            if (_rdt != null && _rdtCookie != 0)
            {
                _rdt.Unadvise(_rdtCookie);
                _rdtCookie = 0;
                _rdt = null;
            }

            if (_documentSaveListener != null)
            {
                _documentSaveListener.BeforeSave -= OnBeforeDocumentSave;
                _documentSaveListener = null;
            }
        }
        private void OnBeforeDocumentSave(object sender, string documentPath)
        {
            if (!_options.EnableAutoFormatOnSave)
                return;

            ThreadHelper.ThrowIfNotOnUIThread();

            var command = _dte.Commands.Item("Edit.FormatDocument");
            if (command == null || !command.IsAvailable)
                return;

            string ext = System.IO.Path.GetExtension(documentPath).ToLower();

            bool isExtAllowed = _options.EnableAll;

            if(!isExtAllowed)
                switch (ext)
                {
                    case ".cs":   isExtAllowed = _options.EnableCS;   break;
                    case ".cpp":  isExtAllowed = _options.EnableCPP;  break;
                    case ".h":    isExtAllowed = _options.EnableCPP;  break;
                    case ".hpp":  isExtAllowed = _options.EnableCPP;  break;
                    case ".xml":  isExtAllowed = _options.EnableXML;  break;
                    case ".json": isExtAllowed = _options.EnableJSON; break;
                    case ".xaml": isExtAllowed = _options.EnableXAML; break;
                    case ".md":   isExtAllowed = _options.EnableMD;   break;
                    default:      isExtAllowed = false;               break;
                }

            if(!isExtAllowed)
                return;

            FormatDocument(documentPath);
        }

        private void FormatDocument(string documentPath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Document document = _dte.Documents.Item(documentPath);
            if (document == null)
                return;

            var originalActiveDocument = _dte.ActiveDocument;

            document.Activate();
            _dte.ExecuteCommand("Edit.FormatDocument");
            originalActiveDocument?.Activate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Unsubscribe();

            base.Dispose(disposing);
        }

        #endregion
    }
}
