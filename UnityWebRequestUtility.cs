using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Bubu.UnityUtils
{
    /// <summary> Use UnityWebRequest with async-await (using UniTask). </summary>
    public static class UnityWebRequestUtility
    {
        public static async UniTask<string> Get(string uri, Dictionary<string, string> headers = null,
            System.IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default)
        {
            string result = null;

            using (var request = UnityWebRequest.Get(uri))
            {
                SetRequestHeader(request, headers);

                try
                {
                    await request.SendWebRequest().ToUniTask(progress, timing, cancellationToken);

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        LogError(request);
                    }
                    else
                    {
                        Log(request);
                        result = request.downloadHandler.text;
                    }
                }
                catch (System.OperationCanceledException ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
            }

            return result;
        }

        public static async UniTask<byte[]> GetRawData(string uri, Dictionary<string, string> headers = null,
            System.IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default)
        {
            byte[] result = null;

            using (var request = UnityWebRequest.Get(uri))
            {
                SetRequestHeader(request, headers);

                try
                {
                    await request.SendWebRequest().ToUniTask(progress, timing, cancellationToken);

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        LogError(request);
                    }
                    else
                    {
                        Log(request);
                        result = request.downloadHandler.data;
                    }
                }
                catch (System.OperationCanceledException ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
            }

            return result;
        }

        public static async UniTask<Texture2D> GetTexture(string uri, Dictionary<string, string> headers = null,
            System.IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default)
        {
            Texture2D result = null;

            using (var request = UnityWebRequestTexture.GetTexture(uri))
            {
                SetRequestHeader(request, headers);

                try
                {
                    await request.SendWebRequest().ToUniTask(progress, timing, cancellationToken);

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        LogError(request);
                    }
                    else
                    {
                        Log(request);
                        result = DownloadHandlerTexture.GetContent(request);
                    }
                }
                catch (System.OperationCanceledException ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
            }

            return result;
        }

        public static async UniTask<AssetBundle> GetAssetBundle(string uri, Dictionary<string, string> headers = null,
            System.IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default)
        {
            AssetBundle result = null;

            using (var request = UnityWebRequestAssetBundle.GetAssetBundle(uri))
            {
                SetRequestHeader(request, headers);

                try
                {
                    await request.SendWebRequest().ToUniTask(progress, timing, cancellationToken);

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        LogError(request);
                    }
                    else
                    {
                        Log(request);
                        result = DownloadHandlerAssetBundle.GetContent(request);
                    }
                }
                catch (System.OperationCanceledException ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
            }

            return result;
        }

        /// <remarks>
        /// <para> The Content-Type header will be set to <paramref name="contentType"/>. </para>
        /// <para> Remark: Providing Content-Type header in <paramref name="headers"/> would be ignored. </para>
        /// </remarks>
        public static async UniTask<string> Post(string uri, string postData, string contentType, Dictionary<string, string> headers = null,
            System.IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default)
        {
            string result = null;

            // remark: Before 2022.3, Content-Type header has default value: application/x-www-form-urlencoded
            // remark: In 2022.3, use UnityWebRequest.Post(uri, postData, contentType)
            // reference: https://github.com/Unity-Technologies/UnityCsReference/blob/2021.3/Modules/UnityWebRequest/Public/WebRequestExtensions.cs
            // using (var request = UnityWebRequest.Post(uri, postData, contentType))
            // {
            //     if (headers?.ContainsKey(Header.ContentType) == true)
            //         headers.Remove(Header.ContentType);
            //     SetRequestHeader(request, headers);
            //     await request.SendWebRequest();
            //     Log(request);
            //     if (request.result != UnityWebRequest.Result.Success)
            //         LogError(request);
            //     else
            //         result = request.downloadHandler.text;
            // }
            // remarks: prevent from UnityWebRequest.Post internal implementation issue,
            // it would call string urlencoded = WWWTranscoder.DataEncode(postData, System.Text.Encoding.UTF8);,
            // changing postData is not expected.
            // source: https://github.com/Unity-Technologies/UnityCsReference/blob/2021.3/Modules/UnityWebRequest/Public/WebRequestExtensions.cs#171
            using (var request = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                if (string.IsNullOrEmpty(postData))
                {
                    request.SetRequestHeader(Header.ContentType, contentType);
                    // no data to send, nothing more to setup
                }
                else
                {
                    byte[] payload = System.Text.Encoding.UTF8.GetBytes(postData);
                    request.uploadHandler = new UploadHandlerRaw(payload);
                    request.uploadHandler.contentType = contentType;
                }

                if (headers?.ContainsKey(Header.ContentType) == true)
                    headers.Remove(Header.ContentType);

                SetRequestHeader(request, headers);

                try
                {
                    await request.SendWebRequest().ToUniTask(progress, timing, cancellationToken);

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        LogError(request);
                    }
                    else
                    {
                        Log(request);
                        result = request.downloadHandler.text;
                    }
                }
                catch (System.OperationCanceledException ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
            }

            return result;
        }

        /// <remarks>
        /// <para> The Content-Type header will be set to multipart/form-data, with an appropriate boundary specification. </para>
        /// <para> If not supplied, a boundary will be generated for you. </para>
        /// <para> Remark: Providing Content-Type header in <paramref name="headers"/> would be ignored. </para>
        /// </remarks>
        /// <param name="multipartFormSections">
        /// <para> The parameters of MultipartFormDataSection(string name, byte[] data, string contentType) are </para>
        /// <para> assigned to Content-Disposition header as follow: </para>
        /// <para> Content-Disposition: form-data; name="<paramref name="name"/>" </para>
        /// <para> ------ </para>
        /// <para> The parameters of MultipartFormFileSection(string name, byte[] data, string fileName, string contentType) are </para>
        /// <para> assigned to Content-Disposition header as follow: </para>
        /// <para> Content-Disposition: form-data; name="<paramref name="name"/>"; filename="<paramref name="fileName"/>" </para>
        /// </param>
        public static async UniTask<string> Post(string uri, List<IMultipartFormSection> multipartFormSections, byte[] boundary = null, Dictionary<string, string> headers = null,
            System.IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default)
        {
            string result = null;

            boundary ??= UnityWebRequest.GenerateBoundary();
            using (var request = UnityWebRequest.Post(uri, multipartFormSections, boundary))
            {
                if (headers?.ContainsKey(Header.ContentType) == true)
                    headers.Remove(Header.ContentType);

                SetRequestHeader(request, headers);

                try
                {
                    await request.SendWebRequest().ToUniTask(progress, timing, cancellationToken);

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        LogError(request);
                    }
                    else
                    {
                        Log(request);
                        result = request.downloadHandler.text;
                    }
                }
                catch (System.OperationCanceledException ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
            }

            return result;
        }

        /// <remarks>
        /// <para> The Content-Type header will be set to application/x-www-form-urlencoded. </para>
        /// <para> application/x-www-form-urlencoded: the keys and values are encoded in key-value tuples separated by '&amp;' with a '=' between the key and the value. Non-alphanumeric characters in both keys and values are URL encoded. </para>
        /// <para> Reference: https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/POST </para>
        /// <para> Remark: Providing Content-Type header in <paramref name="headers"/> would be ignored. </para>
        /// </remarks>
        public static async UniTask<string> Post(string uri, Dictionary<string, string> formFields, Dictionary<string, string> headers = null,
            System.IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default)
        {
            string result = null;

            using (var request = UnityWebRequest.Post(uri, formFields))
            {
                if (headers?.ContainsKey(Header.ContentType) == true)
                    headers.Remove(Header.ContentType);

                SetRequestHeader(request, headers);

                try
                {
                    await request.SendWebRequest().ToUniTask(progress, timing, cancellationToken);

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        LogError(request);
                    }
                    else
                    {
                        Log(request);
                        result = request.downloadHandler.text;
                    }
                }
                catch (System.OperationCanceledException ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
            }

            return result;
        }

        /// <remarks>
        /// <para> The Content-Type header will be set to application/octet-stream. </para>
        /// <para> application/octet-stream: This is the default for binary files. As it means unknown binary file, browsers usually don't execute it, or even ask if it should be executed. They treat it as if the Content-Disposition header was set to attachment, and propose a "Save As" dialog. </para>
        /// <para> Reference: https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types </para>
        /// <para> Remark: Providing Content-Type header in <paramref name="headers"/> would be ignored. </para>
        /// </remarks>
        public static async UniTask<string> Put(string uri, byte[] bodyData, Dictionary<string, string> headers = null,
            System.IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default)
        {
            string result = null;

            using (var request = UnityWebRequest.Put(uri, bodyData))
            {
                if (headers?.ContainsKey(Header.ContentType) == true)
                    headers.Remove(Header.ContentType);

                SetRequestHeader(request, headers);

                try
                {
                    await request.SendWebRequest().ToUniTask(progress, timing, cancellationToken);

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        LogError(request);
                    }
                    else
                    {
                        Log(request);
                        result = request.downloadHandler.text;
                    }
                }
                catch (System.OperationCanceledException ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
            }

            return result;
        }

        /// <remarks>
        /// <para> The Content-Type header will be set to application/octet-stream. </para>
        /// <para> application/octet-stream: This is the default for binary files. As it means unknown binary file, browsers usually don't execute it, or even ask if it should be executed. They treat it as if the Content-Disposition header was set to attachment, and propose a "Save As" dialog. </para>
        /// <para> Reference: https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types </para>
        /// <para> Remark: Providing Content-Type header in <paramref name="headers"/> would be ignored. </para>
        /// </remarks>
        public static async UniTask<string> Put(string uri, string bodyData, Dictionary<string, string> headers = null,
            System.IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default)
        {
            string result = null;

            using (var request = UnityWebRequest.Put(uri, bodyData))
            {
                if (headers?.ContainsKey(Header.ContentType) == true)
                    headers.Remove(Header.ContentType);

                SetRequestHeader(request, headers);

                try
                {
                    await request.SendWebRequest().ToUniTask(progress, timing, cancellationToken);

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        LogError(request);
                    }
                    else
                    {
                        Log(request);
                        result = request.downloadHandler.text;
                    }
                }
                catch (System.OperationCanceledException ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
            }

            return result;
        }

        /// <remarks>
        /// <para> The Content-Type header will be set to <paramref name="contentType"/>. </para>
        /// <para> Remark: Providing Content-Type header in <paramref name="headers"/> would be ignored. </para>
        /// </remarks>
        public static async UniTask<string> Put(string uri, byte[] bodyData, string contentType, Dictionary<string, string> headers = null,
            System.IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default)
        {
            string result = null;

            using (var request = UnityWebRequest.Put(uri, bodyData))
            {
                request.uploadHandler.contentType = contentType;

                SetRequestHeader(request, headers);

                try
                {
                    await request.SendWebRequest().ToUniTask(progress, timing, cancellationToken);

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        LogError(request);
                    }
                    else
                    {
                        Log(request);
                        result = request.downloadHandler.text;
                    }
                }
                catch (System.OperationCanceledException ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
            }

            return result;
        }

        /// <remarks>
        /// <para> The Content-Type header will be set to <paramref name="contentType"/>. </para>
        /// <para> Remark: Providing Content-Type header in <paramref name="headers"/> would be ignored. </para>
        /// </remarks>
        public static async UniTask<string> Put(string uri, string bodyData, string contentType, Dictionary<string, string> headers = null,
            System.IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default)
        {
            string result = null;

            using (var request = UnityWebRequest.Put(uri, bodyData))
            {
                request.uploadHandler.contentType = contentType;

                SetRequestHeader(request, headers);

                try
                {
                    await request.SendWebRequest().ToUniTask(progress, timing, cancellationToken);

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        LogError(request);
                    }
                    else
                    {
                        Log(request);
                        result = request.downloadHandler.text;
                    }
                }
                catch (System.OperationCanceledException ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
            }

            return result;
        }

        public static async UniTask Delete(string uri, Dictionary<string, string> headers = null,
            System.IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default)
        {
            using (var request = UnityWebRequest.Delete(uri))
            {
                SetRequestHeader(request, headers);

                try
                {
                    await request.SendWebRequest().ToUniTask(progress, timing, cancellationToken);

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        LogError(request);
                    }
                    else
                    {
                        Log(request);
                    }
                }
                catch (System.OperationCanceledException ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
            }
        }

        public static async UniTask Head(string uri, Dictionary<string, string> headers = null,
            System.IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default)
        {
            using (var request = UnityWebRequest.Head(uri))
            {
                SetRequestHeader(request, headers);

                try
                {
                    await request.SendWebRequest().ToUniTask(progress, timing, cancellationToken);

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        LogError(request);
                    }
                    else
                    {
                        Log(request);
                    }
                }
                catch (System.OperationCanceledException ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"{ex.Message}");
                    LogError(request);
                }
            }
        }

        private static void SetRequestHeader(UnityWebRequest request, Dictionary<string, string> headers)
        {
            if (headers != null)
                foreach (var header in headers)
                    request.SetRequestHeader(header.Key, header.Value);
        }

        private static Logger logger = new Logger()
        {
            LogLevel = LogLevel.Information
        };
        public static void SetLogLevel(LogLevel logLevel)
        {
            logger.LogLevel = logLevel;

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation($"{nameof(UnityWebRequestUtility)} LogLevel set to {logger.LogLevel}.");
        }

        private static void Log(UnityWebRequest request)
        {
            if (!logger.IsEnabled(LogLevel.Information)) return;

            StringBuilder stringBuilder = new StringBuilder()
                .Append($"{System.Environment.NewLine}Request URL: {request.url}")
                .Append($"{System.Environment.NewLine}Request Method: {request.method}")
                .Append($"{System.Environment.NewLine}Status Code: {request.responseCode}");

            if (logger.IsEnabled(LogLevel.Debug))
            {
                var responseHeaders = request.GetResponseHeaders();

                if (responseHeaders != null)
                {
                    stringBuilder
                        .Append(System.Environment.NewLine)
                        .Append($"{System.Environment.NewLine}Response Headers");

                    foreach (var responseHeader in responseHeaders)
                        stringBuilder.Append($"{System.Environment.NewLine}{responseHeader.Key}: {responseHeader.Value}");
                }

                logger.LogDebug(stringBuilder.ToString());
            }
            else if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(stringBuilder.ToString());
            }
        }

        private static void LogError(UnityWebRequest request)
        {
            if (!logger.IsEnabled(LogLevel.Information)) return;

            StringBuilder stringBuilder = new StringBuilder()
                .Append($"{System.Environment.NewLine}Request URL: {request.url}")
                .Append($"{System.Environment.NewLine}Request Method: {request.method}")
                .Append($"{System.Environment.NewLine}Status Code: {request.responseCode}")
                .Append(System.Environment.NewLine)
                .Append($"{System.Environment.NewLine}Error: {request.error}")
                .Append($"{System.Environment.NewLine}Result: {request.result}");

            if (logger.IsEnabled(LogLevel.Debug))
            {
                var responseHeaders = request.GetResponseHeaders();

                if (responseHeaders != null)
                {
                    stringBuilder
                        .Append(System.Environment.NewLine)
                        .Append($"{System.Environment.NewLine}Response Headers");

                    foreach (var responseHeader in responseHeaders)
                        stringBuilder.Append($"{System.Environment.NewLine}{responseHeader.Key}: {responseHeader.Value}");
                }

                logger.LogDebug(stringBuilder.ToString());
            }
            else if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(stringBuilder.ToString());
            }
        }

        // The fundamental difference between the POST and PUT methods is highlighted by the different intent for the 
        // enclosed representation. The target resource in a POST request is intended to handle the enclosed 
        // representation according to the resource's own semantics, whereas the enclosed representation in a PUT 
        // request is defined as replacing the state of the target resource. Hence, the intent of PUT is idempotent and 
        // visible to intermediaries, even though the exact effect is only known by the origin server.
        //
        // source: https://www.rfc-editor.org/rfc/rfc9110
    }

    /// <remarks> reference: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers </remarks>
    public static class Header
    {
        public const string Accept = "Accept";
        public const string Authorization = "Authorization";
        public const string ContentType = "Content-Type";
    }

    /// <remarks> reference: https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types </remarks>
    public static class MediaType
    {
        public static class Application
        {
            // 8.1.  Character Encoding
            // JSON text exchanged between systems that are not part of a closed
            // ecosystem MUST be encoded using UTF-8 [RFC3629].
            //
            // 11.  IANA Considerations
            // The media type for JSON text is application/json.
            //
            // Note:  No "charset" parameter is defined for this registration.
            //
            // source: https://www.rfc-editor.org/rfc/rfc8259
            public const string Json = "application/json";
            public const string OctetStream = "application/octet-stream";
            public const string XWwwFormUrlencoded = "application/x-www-form-urlencoded";
        }

        public static class Multipart
        {
            public const string FormData = "multipart/form-data";
        }

        public static class Text
        {
            public const string Html = "text/html";
            public const string Plain = "text/plain";
        }
    }
}
