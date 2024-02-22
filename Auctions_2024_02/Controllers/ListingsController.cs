using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Auctions_2024_02.Data;
using Auctions_2024_02.Models;
using Auctions_2024_02.Data.Services;
using System.Security.Claims;

namespace Auctions_2024_02.Controllers
{
    public class ListingsController : Controller
    {
        private readonly  IListingService _listingService;
        private readonly IBidsService _bidsService;
        private readonly ICommentsService _commentsService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ListingsController(IListingService listingService, IBidsService bidsService, ICommentsService commentsService,IWebHostEnvironment webHostEnvironment)
        {
            _listingService = listingService;
            _bidsService = bidsService;
            _commentsService = commentsService; 
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index(int? pageNumber,string searchString)
        {
            var applicationDbContext = _listingService.GetAllListing();
            int pageSize = 3;
            if (!string.IsNullOrEmpty(searchString))
            {
                applicationDbContext= applicationDbContext.Where(a=>a.Title.Contains(searchString));
                return View(await PaginatedList<Listing>.CreateAsync(applicationDbContext.Where(l => l.IsSold == false), pageNumber ?? 1, pageSize));
            }
            
            return View(await PaginatedList<Listing>.CreateAsync(applicationDbContext.Where(l=>l.IsSold==false),pageNumber??1,pageSize));
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listing = await _listingService.GetById(id);
            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ListingVM listingVM)
        {
            if(listingVM.ImageFile != null)
            {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                string filename= listingVM.ImageFile.FileName;
                string filepath= Path.Combine(uploadDir, filename);
                using(var fileStream=new FileStream(filepath, FileMode.Create))
                {
                    listingVM.ImageFile.CopyTo(fileStream);
                }
                var listObj = new Listing { 
                Title=listingVM.Title,
                Description=listingVM.Description,
                Price=listingVM.Price,
                IdentityUserId=listingVM.IdentityUserId,
                ImagePath= filename,
                };
                await _listingService.Add(listObj);
                return RedirectToAction("Index");
            }
            return View(listingVM);
        }

        [HttpPost]
        public async   Task<IActionResult> AddBid([Bind("Id,Price,ListingId,IdentityUserId")] Bid bid)
        {
            if (ModelState.IsValid)
            {
                await _bidsService.Add(bid);
            }
            var listing=await _listingService.GetById(bid.ListingId);
            listing.Price = bid.Price;
            await _listingService.SaveChanges();
            return View("Details",listing);
        }

        public async Task<ActionResult> CloseBidding(int id)
        {
            var listing = await _listingService.GetById(id);
            listing.IsSold=true;
            await _listingService.SaveChanges();
            return View("Details", listing);

        }
        [HttpPost]
        public async Task<IActionResult> AddComment([Bind("Id,Content,ListingId,IdentityUserId")]Comment comment)
        {
            if (ModelState.IsValid)
            {
                await _commentsService.Add(comment);
            }
            var listing=await _listingService.GetById(comment.ListingId);
            return View("Details", listing);
        }

        public async Task<IActionResult> MyListing(int? pageNumber)
        {
            var applicationDbContext = _listingService.GetAllListing();
            int pageSize = 3;
            return View("Index",await PaginatedList<Listing>.CreateAsync(applicationDbContext.Where(l=>l.IdentityUserId==User.FindFirstValue(ClaimTypes.NameIdentifier)), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> MyBids(int? pageNumber)
        {
            var applicationDbContext = _bidsService.GetAll();
            int pageSize = 3;
            return View(await PaginatedList<Bid>.CreateAsync(applicationDbContext.Where(l => l.IdentityUserId == User.FindFirstValue(ClaimTypes.NameIdentifier)), pageNumber ?? 1, pageSize));
        }
    }
}
