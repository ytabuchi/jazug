/* This class get a result from Computer Vision API
 * https://docs.microsoft.com/ja-jp/azure/cognitive-services/computer-vision/quickstarts/csharp#optical-character-recognition-ocr-with-computer-vision-api-using-ca-nameocr-a
 */
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AzureSample
{
    public class Ocr
    {
        const string uriBase = "https://southeastasia.api.cognitive.microsoft.com/vision/v1.0/ocr";

        public static async Task<string> DoOcrAsync(string imageUri)
        {
            using (var client = new HttpClient())
            {
                try
                {
					// ヘッダーでAPIキーを付与
					client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Secrets.ComputerVisionApiKey);
					var sendUri = $"{uriBase}?language=ja&detectOrientation=true";
					var content = new StringContent("{\"url\":\"" + imageUri + "\"}");
					content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

					var response = await client.PostAsync(sendUri, content);
					response.EnsureSuccessStatusCode();

                    var contentString = await response.Content.ReadAsStringAsync();
                    var ocrResultString = JsonConvert.DeserializeObject<OcrResult>(contentString);

                    var sb = new StringBuilder();
                    foreach (var line in ocrResultString.regions[0].lines)
                    {
                        foreach (var word in line.words)
                        {
                            sb.Append(word.text);
                        }
                    }

                    return sb.ToString();
				}
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.InnerException);
                }
                return "";
			}
        }

        /// <summary>
        /// Gets the text visible in the specified image file by using the Computer Vision REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file.</param>
        static async void MakeOCRRequest(string imageFilePath)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Secrets.ComputerVisionApiKey);

            // Request parameters.
            string requestParameters = "language=ja&detectOrientation=true";

            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                // Display the JSON response.
                //Console.WriteLine("\nResponse:\n");
                //Console.WriteLine(JsonPrettyPrint(contentString));
            }
        }


        /// <summary>
        /// Returns the contents of the specified file as a byte array.
        /// </summary>
        /// <param name="imageFilePath">The image file to read.</param>
        /// <returns>The byte array of the image data.</returns>
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }


        /// <summary>
        /// Formats the given JSON string by adding line breaks and indents.
        /// </summary>
        /// <param name="json">The raw JSON string to format.</param>
        /// <returns>The formatted JSON string.</returns>
        static string JsonPrettyPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            json = json.Replace(Environment.NewLine, "").Replace("\t", "");

            StringBuilder sb = new StringBuilder();
            bool quote = false;
            bool ignore = false;
            int offset = 0;
            int indentLength = 3;

            foreach (char ch in json)
            {
                switch (ch)
                {
                    case '"':
                        if (!ignore) quote = !quote;
                        break;
                    case '\'':
                        if (quote) ignore = !ignore;
                        break;
                }

                if (quote)
                    sb.Append(ch);
                else
                {
                    switch (ch)
                    {
                        case '{':
                        case '[':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', ++offset * indentLength));
                            break;
                        case '}':
                        case ']':
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', --offset * indentLength));
                            sb.Append(ch);
                            break;
                        case ',':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', offset * indentLength));
                            break;
                        case ':':
                            sb.Append(ch);
                            sb.Append(' ');
                            break;
                        default:
                            if (ch != ' ') sb.Append(ch);
                            break;
                    }
                }
            }

            return sb.ToString().Trim();
        }
    }

    public class OcrResult
    {
		public class Word
		{
			public string boundingBox { get; set; }
			public string text { get; set; }
		}

		public class Line
		{
			public string boundingBox { get; set; }
			public List<Word> words { get; set; }
		}

		public class Region
		{
			public string boundingBox { get; set; }
			public List<Line> lines { get; set; }
		}

		public string language { get; set; }
		public double textAngle { get; set; }
		public string orientation { get; set; }
		public List<Region> regions { get; set; }
    }
}