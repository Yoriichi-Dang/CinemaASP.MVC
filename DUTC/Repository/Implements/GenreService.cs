using DUTC.Data;
using DUTC.Models.DTO;
using DUTC.Repository.Interface;
using Microsoft.IdentityModel.Tokens;

namespace DUTC.Repository.Implements
{
    public class GenreService : IGenreService
    {
        private readonly ApplicationDbContext ctx;
        public GenreService(ApplicationDbContext ctx)
        {
            this.ctx = ctx;
        }

        public bool Add(Genre model)
        {
            try
            {
                ctx.Genres.Add(model);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var data = this.GetById(id);
                if (data == null)
                    return false;
                ctx.Genres.Remove(data);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Genre GetById(int id)
        {
            return ctx.Genres.Find(id);
        }

        public GenreListVm List(string sortOrder = "", string term = "")
        {
            GenreListVm model=new GenreListVm();
            model.sortOrder=sortOrder;
            model.term=term;
            model.sortOrder = sortOrder;
            
            var data = ctx.Genres.AsQueryable();
            if (!term.IsNullOrEmpty())
            {
                data = data.Where(a => a.GenreName.StartsWith(term));
            }
            if (sortOrder.Contains("id"))
            {
                if (sortOrder.Contains("desc"))
                {
                    data=data.OrderByDescending(a => a.Id);
                }
                else
                {
                    data = data.OrderBy(a => a.Id);
                }
            }
            else
            {
                if (sortOrder.Contains("desc"))
                {
                    data = data.OrderByDescending(a => a.GenreName);
                }
                else
                {
                    data = data.OrderBy(a => a.GenreName);
                }
            }
            model.Genres = data;
            return model;
        }

        public bool Update(Genre model)
        {
            try
            {
                ctx.Genres.Update(model);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
