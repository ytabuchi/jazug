/* This class get a result from Computer Vision API
 * https://docs.microsoft.com/ja-jp/azure/cognitive-services/computer-vision/quickstarts/csharp#optical-character-recognition-ocr-with-computer-vision-api-using-ca-nameocr-a
 * Tryout page:
 * https://westus.dev.cognitive.microsoft.com/docs/services/56f91f2d778daf23d8ec6739/operations/56f91f2e778daf14a499e1fa
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

        public static async Task<string> DoOcrUriAsync(string imageUri)
        {
            using (var client = new HttpClient())
            {
				// ヘッダーでAPIキーを付与して日本語でComputer Vision APIに投げる
				client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Secrets.ComputerVisionApiKey);
				var sendUri = $"{uriBase}?language=ja&detectOrientation=true";

				try
                {
                    // 画像URLをJSONコンテントとしてPOSTする
                    var content = new StringContent("{\"url\":\"" + imageUri + "\"}");
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await client.PostAsync(sendUri, content);
                    response.EnsureSuccessStatusCode();

                    // 取得したJSONから読み取り結果をリターンする
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
					return "couldn't recognize.";
                }
            }
        }

        public static async Task<string> DoOcrStreamAsync(Stream imageStream)
        {
            using (var client = new HttpClient())
            {
                // ヘッダーとパラメーターを付与したリクエストを作成
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Secrets.ComputerVisionApiKey);
                var sendUri = $"{uriBase}?language=ja&detectOrientation=true";

                try
                {
                    // StreamをコンテントとしてPOSTする
                    var content = new StreamContent(imageStream);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    var response = await client.PostAsync(sendUri, content);
                    response.EnsureSuccessStatusCode();

                    // Get the JSON response.
                    string contentString = await response.Content.ReadAsStringAsync();

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
                    return "couldn't recognize.";
                }
            }
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