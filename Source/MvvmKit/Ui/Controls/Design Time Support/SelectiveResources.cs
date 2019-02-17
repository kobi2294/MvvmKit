using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace MvvmKit
{
    /// <summary>
    /// Use this class to combine run time and design time resources.
    /// <see cref="DesignTimeResource"/> instances that would be added to the <see cref="ResourceDictionary.MergedDictionaries"/> 
    /// property, will only be loaded at design time. You can use it for design time resources, such as design time data or resources
    /// that are defined in a dynamically loaded assembly
    /// </summary>
    public class SelectiveResources : ResourceDictionary
    {
        private readonly DtCollection _noopMergedDictionaries = new DtCollection();

        private class DtCollection : ObservableCollection<ResourceDictionary>
        {
            protected override void InsertItem(int index, ResourceDictionary item)
            {
                // if the type is DesignTimeResource - we only add it in design time, 
                // otherwise, we always add it
                if ((item is DesignTimeResource) && (Exec.IsRunTime)) return;

                // otherwise, add it in any condition
                base.InsertItem(index, item);
            }
        }

        public SelectiveResources()
        {
            var fieldInfo = typeof(ResourceDictionary).GetField("_mergedDictionaries", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(this, _noopMergedDictionaries);
            }
        }
    }

}
