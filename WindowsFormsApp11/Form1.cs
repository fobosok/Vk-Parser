using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;


namespace WindowsFormsApp11
{
	public partial class Form1 : Form
	{
		static VkApi vk = new VkApi();
		public static string path = @"D:\45k";


		public Form1()
		{
			InitializeComponent();

				vk.Authorize(new ApiAuthParams
				{
					ApplicationId = 6650452,
					Login = File.ReadAllText(@"D:\dwn\projects\vk parser\log-pass\Login.txt"),
					Password = File.ReadAllText(@"D:\dwn\projects\vk parser\log-pass\Password.txt"),
					Settings = Settings.All
				});
			//// Получить адрес сервера для загрузки.
			//var uploadServer = vk.Photo.GetWallUploadServer(169644929);
			//// Загрузить файл.
			//var wc = new WebClient();
			//var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, @"https://sun6-4.userapi.com/c635104/v635104458/33aeb/6Gmz2GS58tU.jpg"));
			//// Сохранить загруженный файл
			//var photos = vk.Photo.SaveWallPhoto(responseFile, null, 169644929);
			//vk.Wall.Post(new WallPostParams
			//{
			//	Attachments = photos,
			//	FromGroup = true,
			//	Message = "Test",
			//	OwnerId = -169644929
			//});
			search(35655);

		}
		public static string GetUrlOfBigPhoto(Photo photo)
		{
			if (photo == null)
				return null;
			if (photo.Photo2560 != null)
				return photo.Photo2560.AbsoluteUri;
			if (photo.Photo1280 != null)
				return photo.Photo1280.AbsoluteUri;
			if (photo.Photo807 != null)
				return photo.Photo807.AbsoluteUri;
			if (photo.Photo604 != null)
				return photo.Photo604.AbsoluteUri;
			if (photo.Photo130 != null)
				return photo.Photo130.AbsoluteUri;
			if (photo.Photo75 != null)
				return photo.Photo75.AbsoluteUri;
			if (photo.Sizes?.Count > 0)
			{
				var bigSize = photo.Sizes[0];
				for (int i = 0; i < photo.Sizes.Count; i++)
				{
					var photoSize = photo.Sizes[i];
					if (photoSize.Height > bigSize.Height && photoSize.Width > bigSize.Width)
						bigSize = photoSize;
				}
				return bigSize.Src.ToString();
			}
			return null;
		}
		public static void search(ulong counter)
		{
			var posts = vk.Wall.Get(new WallGetParams
			{
				OwnerId = -48006306,
				Offset = counter,
				Count = 100
			});
			int i = 1;
			foreach (var post in posts.WallPosts)
			{
				if (!post.MarkedAsAds && post.Attachment != null && post.Attachment.Type.Name == "Photo")
				{
					DirectoryInfo di = Directory.CreateDirectory(path + "\\post " + (counter));
					if (post.Text != string.Empty)
					{
						File.WriteAllText(path + "\\post " + (counter) + "\\text.txt", post.Text, Encoding.UTF8);
					}
					var attachList = post.Attachments.ToList();
					foreach (var attach in attachList)
					{
						if (attach.Type.Name == "Photo")
						{

							using (WebClient client = new WebClient())
							{
								client.DownloadFile(new Uri(GetUrlOfBigPhoto(attach.Instance as Photo)), $@"D:\45k\post {(counter)}\image{i}.jpg");
							}
							i++;
						}
					}
					counter++;
				}
				else
				{
					counter++;
				}
			}
			if (counter == posts.TotalCount)
			{
				return;
			}
			else
			{
				search(counter);
			}

		}
	}
}
