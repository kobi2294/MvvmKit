using MvvmKit;
using MvvmKitAppSample.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKitAppSample.Services
{
    public class ItemsService : BackgroundServiceBase
    {
        public interface IModel
        {
            IStateList<TodoItem> TodoItems { get; set; }
        }

        private ServiceStore<IModel> _store = new ServiceStore<IModel>();
        public IStateCollectionReader<TodoItem> TodoItems;

        public ItemsService()
        {
            TodoItems = _store.CreateReader(this, m => m.TodoItems);
        }

        protected async override Task OnShutDown()
        {
            await base.OnShutDown();

            Debug.WriteLine("Items Service started shutting down");
            await Task.Delay(3000);
            Debug.WriteLine("Items Service Completed shutting down");
        }


        public Task AddItem(string caption)
        {
            return Run(async () =>
            {
                await _store.Modify(model =>
                {
                    var item = new TodoItem
                    {
                        Uid = Guid.NewGuid().ToString(),
                        Caption = caption,
                        IsChecked = false
                    };

                    model.TodoItems.Add(item);

                });
            });
        }

        public Task Toggle(string guid)
        {
            return Run(async () =>
            {
                await _store.Modify(model =>
                {
                    var item = model.TodoItems.Find(x => x.Uid == guid);
                    item.IsChecked = !item.IsChecked;
                    model.TodoItems.SetWhere(x => x.Uid == guid, item);
                });
            });
        }

    }
}
