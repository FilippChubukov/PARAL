using System;
using System.Net;
using System.Threading.Tasks;

namespace Parser {
    
    public class WebPage { 
        
        public string Url;
        
        public WebPage(string url) {
            
            Url = url;
        }
                
        public async Task<String> GetCode() {
            
            String html = "";
            
            using (var wclient = new WebClient()) {//Получаем из html ссылку строкой.

                try {

                    html = await wclient.DownloadStringTaskAsync(Url);//Загружаем ресурс как String из URL.
                }
                
                catch (Exception ex) {
                    
                    Console.Write("(!) Exception on {0} - {1}", Url, ex.Message);
                    Console.WriteLine();
                } 
                
            } return html;//Получаем ссылку строкой.
        }
    }
}