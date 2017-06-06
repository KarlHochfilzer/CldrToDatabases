using CldrToDatabases.DBModels;

namespace CldrToDatabases.DataBuilder
{
    public interface IDataToDBs
    {
        bool UseDbContext();

        void Save();

        void CreateDB(DataDB dataDB);

        void SaveDBData(DataDB dataDB);

        void CompareDBDataToLists(DataDB dataDB);
    }
}