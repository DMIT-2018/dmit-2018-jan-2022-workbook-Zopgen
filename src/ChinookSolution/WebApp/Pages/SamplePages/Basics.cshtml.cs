using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.SamplePages
{
    public class BasicsModel : PageModel
    {
        //basically this is an object, treat is as such

        //data fields
        public string MyName;

        //properties

        //the annotation [TempData] stores data until its's read in another
        //  immediate request
        //this annotation attribute has two methods called Keep(string) and
        //  Peek(string) (used on content page)
        //keep in a dictionary (name/value pair)
        //useful to redirect when data is required for more than a single request
        //Implemented byTempData providers using either cookies or session state
        //Temp data is not bound to any particular control like BindProperty
        [TempData]
        public string FeedBack { get; set; }   

        //the annotation BindProperty ties a property in the PageModel class
        //  directly to a conmtrol on the Contyent Page
        //data is transfered betweeen the two automatically
        //on the content page the control to use this property will have a helper tag
        //  called asp-for

        //to retain a value in the control ties to this property AND retained
        //  via the @page use the SupportGet attribute = true
        [BindProperty(SupportsGet = true)]
		public int? id { get; set; }


		//constructors

		//behaviours (aka methods)
		public void OnGet()
        {
            //executes in response to a get request from the browser
            //when the page is "first" accessed, the browser issues a Get request
            //when the page is refreshed, WITHOUT a Post request, the browser issues a Get request
            //when the page is retreived in response to a form's POST using RedirectToPage()
            //IF NO RedirectToPage() is used on the post there is NO Get request issued

            //Server-Side processing
            //contains no html

            Random rnd = new Random();
            int oddeven = rnd.Next(0, 25);
            if(oddeven % 2 == 0)
			{
                MyName = $"Nick is even{oddeven}";
			}
			else
			{
                MyName = null;
			}
        }

        //processing in response to  a request from a form on a webpage
        //this request is reffered to as a Post (method-"post")

        //General Post
        //a general post occurs when a asp-page-handler is NOT used
        //the return datatype can be void, however, you will normally encounter
        //  the datatype IActionResult
        //the IActionResul requires some type of request action
        //  on the return statement of the method OnPost()
        //typical actions:
        //  Page()
        //  :does NOT issue a OnGet request
        //  :remains on the current page
        //  :a good action fpor form processing involving validation
        //      and with the catch of a try/catch
        //RedirectToPage()
        //  :DOES issue a OnGet request
        //  :is used to retain input values via the @page and your bindproperty
        //      form controls on your form on the content page

        public IActionResult OnPost()
		{
            //this line of code is used to cause a delay in processing
            //  so we can see on the Network Activity some type of
            //  siumpulated processing
            Thread.Sleep(2000);

            //retreive data via the Request object
            //Request: web page to server
            //Response: server to web page
            string buttonvalue = Request.Form["theButton"];
            FeedBack = $"Button press is {buttonvalue} with numeric input of {id}";
            //return Page(); // does not issue OnGet()
            return RedirectToPage(new {id = id }); // request for OnGet()
		}
    }
}
