namespace HostfileManager.ViewModel
{
    using System;
    using System.ComponentModel;
    using HostfileManager.Model;

    /// <summary>
    /// The base class for all ViewModels.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        #region private fields
        /// <summary>
        /// A flag indicating wehther this object has been disposed.
        /// </summary>
        private bool disposed;
        #endregion

        #region constructor(s) and destructor(s)

        /// <summary>Finalizes an instance of the <see cref="ViewModelBase"/> class. </summary>
        ~ViewModelBase()
        {
            this.Dispose(false);
        }

        #endregion

        #region public events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when a the save-changes is executed.
        /// </summary>
        public event EventHandler<ViewModeEventArgs> Save;

        /// <summary>
        /// Occurs when Crtl+C was pressed.
        /// </summary>
        public event EventHandler<ViewModeEventArgs> Copy;

        /// <summary>
        /// Occurs when F was pressed.
        /// </summary>
        public event EventHandler<ViewModeEventArgs> Reload;

        #endregion

        #region public functions

        /// <summary>Save the changes to the view-model back to the disk.</summary>
        /// <param name="currentView">The current <see cref="ViewMode"/>.</param>
        /// <param name="targetPath">The target Path.</param>
        public void SaveChanges(ViewMode currentView, string targetPath)
        {
            if (this.Save == null)
            {
                return;
            }

            ViewModeEventArgs e = new ViewModeEventArgs(currentView, targetPath);
            this.Save(this, e);
        }

        /// <summary>
        /// Copy the current application state to the user's clipboard.
        /// </summary>
        /// <param name="currentView">The current <see cref="ViewMode"/>.</param>
        public void CopyToClipboard(ViewMode currentView)
        {
            if (this.Copy == null)
            {
                return;
            }

            ViewModeEventArgs e = new ViewModeEventArgs(currentView, null);
            this.Copy(this, e);
        }

        /// <summary>Discard all changes and reload the the current hosts file.</summary>
        /// <param name="currentView">The current view.</param>
        public void ReloadFromDisk(ViewMode currentView)
        {
            if (this.Reload == null)
            {
                return;
            }

            ViewModeEventArgs e = new ViewModeEventArgs(currentView, null);
            this.Reload(this, e);
        }

        #endregion

        #region IDisposable members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// <param name="disposing">A flag indicating whether this object is currently disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                // Free other state (managed objects).
            }

            // Free your own state (unmanaged objects).
            // Set large fields to null.
            this.disposed = true;
        }

        #endregion

        #region INotifyPropertyChanged members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler == null)
            {
                return;
            }

            var e = new PropertyChangedEventArgs(propertyName);
            handler(this, e);
        }

        #endregion
    }
}
