using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region additional namespaces
using ChinookSystem.DAL;
using ChinookSystem.Entities;
using ChinookSystem.ViewModels;
#endregion


namespace ChinookSystem.BLL
{
    public class GenreServices
    {
        #region Constructor and Context Dependency
        private readonly ChinookContext _context;
        //obtain the context link from IServiceCollection when
        // this set of services is injected into the "outside user"
        internal GenreServices(ChinookContext context)
        {
            _context = context;
        }
        #endregion

        #region Services : Queries
        //obtain list of genres to be used in a select list

        public List<SelectionList> GetAllGenres()
        {
            IEnumerable<SelectionList> info = _context.Genres
                                                .Select(g => new SelectionList
                                                {
                                                    ValueId = g.GenreId,
                                                    DisplayText = g.Name

                                                });
                                            // .OrderBy(g => g.DisplayText); this sort is in sql
            return info.ToList();
            //return info.OrderBy(g => g.DisplayText).ToList(); this sort is in RAM

        }
        #endregion

    }
}
