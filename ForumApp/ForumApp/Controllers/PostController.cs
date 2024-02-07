﻿using ForumApp.Core.Contracts;
using ForumApp.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace ForumApp.Controllers
{
	public class PostController : Controller
	{
		private readonly IPostService postService;

        public PostController(IPostService _postService)
        {
			postService = _postService;
        }

		[HttpGet]
        public async Task<IActionResult> Index()
		{
			IEnumerable<PostModel> model = await postService.GetAllPostsAsync();
			
			return View(model);
		}

		[HttpGet]
		public IActionResult Add()
		{
			var model = new PostModel();	
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Add(PostModel model)
		{
			if (ModelState.IsValid == false)
			{
				return View();
			}

			await postService.AddAsync(model);

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			PostModel? model = await postService.GetByIdAsync(id);

			if (model == null)
			{

			}

			return View(model);
		}

	}
}
