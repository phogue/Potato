using System;

using Procon.Core.Interfaces.Packages;
using Procon.Core.Interfaces.RSS.Objects;

namespace Procon.UI.API.ViewModels
{
    // Wraps a Package
    public class PackageViewModel : ViewModel<Package>
    {
        // View Model Public Accessors/Mutators.
        public String       Uid
        {
            get { return Model.Uid; }
        }
        public Version      Version
        {
            get { return Model.Version; }
        }
        public DateTime     LastModified
        {
            get { return Model.LastModified; }
        }
        public PackageType  PackageType
        {
            get { return Model.PackageType; }
        }
        public PackageState State
        {
            get { return Model.State; }
        }

        public String Name
        {
            get { return Model.Name; }
        }
        public String Image
        {
            get { return Model.Image; }
        }
        public String ForumLink
        {
            get { return Model.ForumLink; }
        }
        public String Author
        {
            get { return Model.Author; }
        }
        public String Website
        {
            get { return Model.Website; }
        }
        public String Description
        {
            get { return Model.Description; }
        }
        public Int32  Downloads
        {
            get { return Model.Downloads; }
        }
        public Int32  FileSize
        {
            get { return Model.FileSize; }
        }

        
        // Constructor.
        public PackageViewModel(Package model) : base(model)
        {
            // Listen for changes within the model:
            Model.PackageStateChanged += Package_StateChanged;
        }

        // Wraps the PackageStateChanged event.
        private void Package_StateChanged(Package sender, PackageState newState)
        {
            OnPropertyChanged(this, "State");
        }
    }
}
