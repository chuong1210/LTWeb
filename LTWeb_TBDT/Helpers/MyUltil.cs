namespace LTWeb_TBDT.Helpers
{
	public static class MyUltil
	{
		public static string UploadImage(IFormFile urlImage, string folder)
		{
			try
			{
				var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "hinhs", folder, urlImage.FileName);
				using (var myfile = new FileStream(fullPath, FileMode.CreateNew))
				{
					urlImage.CopyTo(myfile);
				}
				return urlImage.FileName;
			}
			catch (Exception ex)
			{
				return string.Empty;
			}
		}
	}
}
