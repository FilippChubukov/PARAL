using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Parser {
    
    public class HtmlParser {
        
        public async Task<WebPage[]> GetLinks(WebPage[] pages, int depth) {
            
            
            while (depth != 0) {
                
                foreach (var page in pages) {
                    
                    var internalPages = await ParsePageAsync(page);//Получаем webpage'и.
                    pages = pages.Concat(internalPages).ToArray();//Строки ссылок сцепляем с массивом.
                }
                
                depth--;
            }
            return pages;
        }
        
        private async Task<WebPage[]> ParsePageAsync(WebPage page) {//Получаем webpage'и из той страницы которую парсим.
            
            var searchRegex = new Regex(@"<a.*? href=""(?<url>https?[\w\.:?&-_=#/]*)""+?"); // <a href="http://..."> Задаем как выглядит искомая ссылка.
            
            var html = await page.GetCode();//Получаем строку, в которой записана наша страничка.
            
            var links = searchRegex.Matches(html);//Ищем все ссылки на этой страничке.

            var pages = new WebPage[links.Count];//Массив для найденных ссылок.
            
            for (var i = 0; i < links.Count; i++) {//Записываем в этот массив найденные ссылки.
                
                pages[i] = new WebPage(links[i].Groups["url"].Value);
            }
            
            return pages;
        }
    }
}