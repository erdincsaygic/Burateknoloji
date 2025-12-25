using StilPay.Entities;

namespace StilPay.UI.WebSite.Models
{
    public class EditViewModel<T> where T : IEntity, new()
    {
        public T entity { get; set; }

        public EditViewModel()
        {
            entity = new T();
        }
    }
}
