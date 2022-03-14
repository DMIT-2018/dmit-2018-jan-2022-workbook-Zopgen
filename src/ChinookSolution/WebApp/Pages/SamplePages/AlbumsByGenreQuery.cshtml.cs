#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#region additional namespaces
using ChinookSystem.BLL;
using ChinookSystem.ViewModels;
using WebApp.Helpers; //contains the class paginator
#endregion


namespace WebApp.Pages.SamplePages
{
    public class AlbumsByGenreQueryModel : PageModel
    {
        #region Private Variable and DI constructor

        private readonly ILogger<IndexModel> _logger;
        private readonly AlbumServices _albumServices;
        private readonly GenreServices _genreServices;


        public AlbumsByGenreQueryModel(ILogger<IndexModel> logger,
                            AlbumServices albumServices, 
                            GenreServices genreServices)
        {
            _logger = logger;
            _albumServices = albumServices;
            _genreServices = genreServices;
        }
        #endregion

        #region Feedback and ErrorHandling
        [TempData]
        public string FeedBack { get; set; }
        public bool HasFeedBack => !string.IsNullOrEmpty(FeedBack);

        [TempData]
        public string ErrorMsg { get; set; }
        public bool HasErrorMsg => !string.IsNullOrEmpty(ErrorMsg);
        #endregion

        [BindProperty]
        public List<SelectionList> GenreList { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? GenreId { get; set; }

        [BindProperty]
        public List<AlbumsListBy> AlbumsByGenre { get; set; }

        #region Paginator variables
        //desired page size
        private const int PAGE_SIZE = 5;
        //instance for paginator class
        public Paginator Pager { get; set; }
        #endregion

        //currentPage value will appear on your url as a Request parameter value
        //         url address..?currentPage=n
        public void OnGet(int? currentPage)
        {
            //OnGet is executed as the page first is processed (as it comes up)

            //consume a service" GetAllGenres in register services of _genreServices
            GenreList = _genreServices.GetAllGenres();
            //sort the List <T> using the method .Sort
            GenreList.Sort((x, y) => x.DisplayText.CompareTo(y.DisplayText));

            //remember that this method executes as the page first comes up BEFORE
            //      anything has happened on the page including the FIRST display
            //any code in this method must handle the npossibility of missing data for the query argument

            if (GenreId.HasValue && GenreId.Value > 0)
            {
                //installation of the paginator setup
                //determine the page number to use with the paginator
                int pageNumber = currentPage.HasValue ? currentPage.Value : 1;

                //use the page state to setup data needed for paging
                PageState current = new PageState(pageNumber, PAGE_SIZE);

                //total rows in the complete query collection
                int totalrows = 0;

                //for effieciency of data being transfered, we will pass
                //  the current page number and the desired page size back to the query
                //the returned collection will ONLY have the rows of the whole query
                //  collection that will actually be shown
                //the total number of records for the whole query collection will be
                //  returned as a out parameter, this value is needed by the paginator 
                //  to setup its display logic.
                AlbumsByGenre = _albumServices.AlbumsByGenre((int)GenreId,
                                    pageNumber, PAGE_SIZE, out totalrows);

                //once the query is complete, use the returned total rows in instanizating
                //  an instance of paginator
                Pager = new Paginator(totalrows, current);
            }
        }

        public IActionResult OnPost()
        {
            if(GenreId == 0)
            {
                //prompt line test
                FeedBack = "You did not select a genre";
            }
            else
            {
                FeedBack = $"You selected genre id of {GenreId}";
            }
            return RedirectToPage(new {GenreId = GenreId }); //cause a Get request which forces OnGet execution
        }
        public IActionResult OnPostNew()
        {
            return RedirectToPage("/SamplePages/CRUDAlbum");
        }
    }
}
