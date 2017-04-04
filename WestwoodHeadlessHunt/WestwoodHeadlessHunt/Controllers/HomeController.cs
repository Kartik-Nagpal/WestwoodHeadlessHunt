using log4net;
using SaneWeb.Resources;
using SaneWeb.Resources.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WestwoodHeadlessHunt.Data;

namespace WestwoodHeadlessHunt.Controllers
{
    public static class HomeController
    {
        private static readonly String JS_BASE64_PREFIX_JPEG = "data:image/jpeg;base64,";

        private static readonly ILog Logger = LogManager.GetLogger(typeof(HomeController));

        [DataBoundView("/index.html")]
        public static Object Index(HttpListenerContext context)
        {
            return new Object();
        }

        [Controller("/headGallery/", APIType.POST, "application/json")]
        public static byte[] GetHeadGallery(HttpListenerContext context, String body)
        {
            try
            {
                HeadGalleryRequest request = Utility.deserializeJSONToObject<HeadGalleryRequest>(body);
                if (request.head == null)
                {
                    throw new Exception("Missing head value!");
                }
                if (!Directory.Exists(request.head.id.ToString()))
                {
                    Directory.CreateDirectory(request.head.id.ToString());
                }
                List<int> ids = new List<int>();
                int i = 0;
                int start = request.index;
                int end = request.index + request.amount;
                List<int> temp = new List<int>();
                foreach (String file in Directory.GetFiles(request.head.id.ToString()))
                {
                    temp.Add(int.Parse(Path.GetFileName(file)));
                }
                temp.Reverse();
                foreach (int n in temp)
                {
                    if (i >= start && i < end)
                    {
                        ids.Add(n);
                    }
                    i++;
                }
                ids.Sort();
                ids.Reverse();
                return Encoding.UTF8.GetBytes(Utility.serializeObjectToJSON(new HeadGalleryResponse
                {
                    ids = ids,
                    total = i
                }));
            }
            catch (Exception e)
            {
                Logger.Warn("Faulty request received! (" + e.Message + ")");
                context.Response.StatusCode = 400;
                return Encoding.UTF8.GetBytes(Utility.serializeObjectToJSON(new ErrorResponse
                {
                    error = e.Message
                }));
            }
        }

        [Controller("/image/", APIType.POST, "application/jpeg")]
        public static byte[] GetHeadImage(HttpListenerContext context, String body)
        {
            try
            {
                HeadImageRequest request = Utility.deserializeJSONToObject<HeadImageRequest>(body);
                if (request.head == null)
                {
                    throw new Exception("Missing head value!");
                }
                if (!Directory.Exists(request.head.id.ToString()))
                {
                    throw new Exception("Invalid request parameter (request.head.id)!");
                }
                String path = Path.Combine(request.head.id.ToString(), request.id.ToString());
                if (!File.Exists(path))
                {
                    throw new Exception("Invalid request parameter (request.id)!");
                }
                //Why do we do this? Because JavaScript is shit.
                return Encoding.UTF8.GetBytes(Convert.ToBase64String(File.ReadAllBytes(path)));
            }
            catch (Exception e)
            {
                Logger.Warn("Faulty request received! (" + e.Message + ")");
                context.Response.StatusCode = 400;
                return Encoding.UTF8.GetBytes(Utility.serializeObjectToJSON(new ErrorResponse
                {
                    error = e.Message
                }));
            }
        }

        [Controller("/uploadHead/", APIType.POST, "application/json")]
        public static byte[] UploadHeadImage(HttpListenerContext context, String body)
        {
            try
            {
                HeadUploadRequest request = Utility.deserializeJSONToObject<HeadUploadRequest>(body);
                if (request.head == null)
                {
                    throw new Exception("Missing head value!");
                }
                if (!Directory.Exists(request.head.id.ToString()))
                {
                    throw new Exception("Invalid request parameter (request.head.id)!");
                }
                if (!request.image.Contains(JS_BASE64_PREFIX_JPEG))
                {
                    throw new Exception("Invalid picture upload type! Make sure you're uploading a JPEG!");
                }
                byte[] data = Convert.FromBase64String(request.image.Replace(JS_BASE64_PREFIX_JPEG, String.Empty));
                if (data.Length > 1024 * 1024 * 10)
                {
                    throw new Exception("Image is too large!");
                }
                Image image = Image.FromStream(new MemoryStream(data));

                if (!ImageFormat.Jpeg.Equals(image.RawFormat))
                {
                    throw new Exception("Unsupported image format (" + image.RawFormat.ToString() + ")");
                }
                int id = 0;
                do
                {
                    id++;
                } while (File.Exists(Path.Combine(request.head.id.ToString(), id.ToString())));

                System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;

                EncoderParameters encoderParameters = new EncoderParameters(1);
                EncoderParameter encoderParameter = new EncoderParameter(encoder, 10L);
                encoderParameters.Param[0] = encoderParameter;

                image.Save(Path.Combine(request.head.id.ToString(), id.ToString()), GetEncoder(ImageFormat.Jpeg), encoderParameters);
                return new byte[] { };
            }
            catch (Exception e)
            {
                Logger.Warn("Faulty request received! (" + e.Message + ") " + e.StackTrace);
                context.Response.StatusCode = 400;
                return Encoding.UTF8.GetBytes(Utility.serializeObjectToJSON(new ErrorResponse
                {
                    error = e.Message
                }));
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
