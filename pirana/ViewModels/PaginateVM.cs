namespace pirana.ViewModels
{
    public class PaginateVM<T>
    {
        public int PageMaxCount {get;set;}
        public int CurrentPage {get;set;}
        public IEnumerable<T> Items {get;set;}
    }
}
