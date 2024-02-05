using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumApp.Infrastructure.Constrants
{
	/// <summary>
	/// Validation Constants
	/// </summary>
	
	public static class ValidationConstants
	{
		/// <summary>
		/// Maximal Post Title length
		/// </summary>
		public const int TitleMaxLenght = 50;
		/// <summary>
		/// Minimal Post Title length
		/// </summary>
		public const int TitleMinLenght = 10;
		/// <summary>
		/// Maximal Post Content length
		/// </summary>
		public const int ContentMaxLenght = 1500;
		/// <summary>
		/// Minimal Post Content length
		/// </summary>
		public const int ContentMinLenght = 30;

	}
}
