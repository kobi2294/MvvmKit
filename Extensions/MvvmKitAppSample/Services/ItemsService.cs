using MvvmKit;
using MvvmKitAppSample.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKitAppSample.Services
{
    public class ItemsService : BackgroundServiceBase
    {
        private readonly ServiceCollectionField<TodoItem> _TodoItems = new ServiceCollectionField<TodoItem>();
        public ServiceCollectionPropertyReadonly<TodoItem> TodoItems { get => (_TodoItems, this); }

        public Task AddItem(string caption)
        {
            return Run(async () =>
            {
                var item = new TodoItem
                {
                    Uid = Guid.NewGuid().ToString(),
                    Caption = caption,
                    IsChecked = false
                };

                await _TodoItems.Add(item);
            });
        }

        public Task Toggle(string guid)
        {
            return Run(async () =>
            {
                var item = _TodoItems.Find(x => x.Uid == guid);
                item.IsChecked = !item.IsChecked;
                await _TodoItems.SetWhere(x => x.Uid == guid, item);
            });
        }

    }
}
