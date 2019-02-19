using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lot_art
{
    public class ImportImages
    {
            // Replace <Subscription Key> with your valid subscription key.
            const string subscriptionKey = "4c60ef7116e64a958bcdfea8b219e01a";

            // You must use the same Azure region in your REST API method as you used to
            // get your subscription keys. For example, if you got your subscription keys
            // from the West US region, replace "westcentralus" in the URL
            // below with "westus".
            //
            // Free trial subscription keys are generated in the "westus" region.
            // If you use a free trial subscription key, you shouldn't need to change
            // this region.
            const string uriBase =
                "https://westeurope.api.cognitive.microsoft.com/vision/v2.0/analyze";

        public static async void ImportToDatabase()
        {
            using (var db = new ApplicationDbContext())
            {
                foreach (var imgSrc in Directory.GetFiles("./../../../images/new").Where(s => s.EndsWith("jpg")))
                {
                    if (db.Images.Any(i => i.Url == imgSrc))
                        continue;
                    
                    var img = new Image() { Url = imgSrc };
                    var res = await MakeAnalysisRequest(imgSrc);
                    var insertValue = new HashSet<string>();
                    if(res?.tags == null)
                        continue;
                    db.Images.Add(img);
                    foreach (var tag in res.tags)
                    {
                        tag.name = tag.name.ToLower();
                        if (insertValue.Contains(tag.name))
                            continue;
                        insertValue.Add(tag.name);
                        var tagO = db.Tags.SingleOrDefault(t => t.Name == tag.name);
                        if (tagO == null)
                        {
                            tagO = new Tag() { Name = tag.name };
                            db.Tags.Add(tagO);
                        }
                        db.Scores.Add(new Score() { Image = img, Tag = tagO, Value = tag.confidence });
                    }
                    db.SaveChanges();
                }
            }
                
        }



            /// <summary>
            /// Gets the analysis of the specified image file by using
            /// the Computer Vision REST API.
            /// </summary>
            /// <param name="imageFilePath">The image file to analyze.</param>
        static async Task<ImageResult> MakeAnalysisRequest(string imageFilePath)
        {
            ImageResult res = null;
            try
            {
                HttpClient client = new HttpClient();

                // Request headers.
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);

                // Request parameters. A third optional parameter is "details".
                // The Analyze Image method returns information about the following
                // visual features:
                // Categories:  categorizes image content according to a
                //              taxonomy defined in documentation.
                // Description: describes the image content with a complete
                //              sentence in supported languages.
                // Color:       determines the accent color, dominant color, 
                //              and whether an image is black & white.
                string requestParameters =
                    "visualFeatures=Categories,Description,Color,Tags";

                // Assemble the URI for the REST API method.
                string uri = uriBase + "?" + requestParameters;

                HttpResponseMessage response;

                // Read the contents of the specified local image
                // into a byte array.
                byte[] byteData = GetImageAsByteArray(imageFilePath);

                // Add the byte array as an octet stream to the request body.
                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    // This example uses the "application/octet-stream" content type.
                    // The other content types you can use are "application/json"
                    // and "multipart/form-data".
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    // Asynchronously call the REST API method.
                    response = await client.PostAsync(uri, content);
                }

                // Asynchronously get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                res = JsonConvert.DeserializeObject<ImageResult>(contentString);

                
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
            return res;
        }

        /// <summary>
        /// Returns the contents of the specified file as a byte array.
        /// </summary>
        /// <param name="imageFilePath">The image file to read.</param>
        /// <returns>The byte array of the image data.</returns>
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            // Open a read-only file stream for the specified file.
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                // Read the file's contents into a byte array.
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }

    public class ImageResult
    {
        public List<TagResult> tags { get; set; }
    }

    public class TagResult
    {
        public string name { get; set; }
        public double confidence { get; set; }
    }
}
