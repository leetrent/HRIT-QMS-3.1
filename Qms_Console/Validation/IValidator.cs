using System.Collections;


namespace QmsCore.Validation
{
    public interface IValidator
    {
        IList RecordsToDelete {get;set;}
        IList RecordsToInsert {get;set;}
        IList RecordsToUpdate {get;set;}
        void Validate(IList records, bool useSoftDelete);
        void Save();
        
    }

}//end namespace