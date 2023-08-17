using CineInfo_API.Data;
using CineInfo_API.Models;

namespace CineInfo_API.Utilities; 
public class FindById<TModel> where TModel: class{
    private readonly CineInfoContext _dbContext;
    private readonly Type[] _supportedTypes = {
        typeof(Movie),
        typeof(Cinema)
    };

    public FindById(CineInfoContext dbContext) {
        _dbContext = dbContext;
    }

    public TModel? Find(int id) {
        if (_supportedTypes.Contains(typeof(TModel))) {
            TModel? data = _dbContext.Set<TModel>().Find(id);
            return data;
        } else {
            throw new ArgumentException($"Unsupported type: {typeof(TModel)}");
        }
    }
}
