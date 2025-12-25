using StilPay.Entities;

namespace StilPay.UI.Admin.Models
{
    public class EditViewModel<T> where T : IEntity, new()
    {
        public T entity { get; set; }

        public EditViewModel()
        {
            entity = new T();
        }
    }

    public class EditViewModelWithoutInterface<T> where T : new()
    {
        public T entity { get; set; }

        public EditViewModelWithoutInterface()
        {
            entity = new T();
        }
    }
}
