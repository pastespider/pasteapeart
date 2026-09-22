namespace ExampleForm.Handler
{
    public class ArticleHandler
    {

        private IHttpClientFactory _httpClientFactory;
        public ArticleHandler(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public void Read(string url)
        {

        }

    }
}
