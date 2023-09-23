using DUTC.Models.DTO;

namespace DUTC.Repository.Interface
{
    public interface IGenreService
    {
        bool Add(Genre model);
        bool Update(Genre model);
        Genre GetById(int id);
        bool Delete(int id);
        GenreListVm List(string sortOrder="",string term="");
    }
}
