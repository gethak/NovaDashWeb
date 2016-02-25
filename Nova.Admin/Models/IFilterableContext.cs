namespace Nova.Admin.Models
{
    public interface IFilterableContext
    {
        void EnableAllGlobalFilters();
        void DisableAllGlobalFilters();
    }
}