using System;

namespace Parser
{
    class Program {
        
        static void Main() {
            
            var link = "https://vk.com";//Ссылка на страницу, ссылки с которой будем искать.
            var pars = new HtmlParser();

            var depth = 2;                                                  // Задаем на сколько ссылок внутрь искать.
            var pages = pars.GetLinks(new[] {new WebPage(link)} , depth).Result;//Полуаем ссылки, на которые указывает наша страничка.
            
            foreach (var page in pages) {//Вывод.
                
                Console.WriteLine("Link: {0} - {1} symbols", page.Url, page.GetCode().Result.Length);
            }
        }
    }
}