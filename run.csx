#r "System.Net.Http"
#r "System.Runtime"
#r "System.Threading.Tasks"

using System;
using CoreTweet;
using HtmlAgilityPack;

// myTimer: 0 */10 * * * *
public static async Task Run(TimerInfo myTimer, TraceWriter log)
{
    //log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
    
    var httpClient = new HttpClient();
    var html = await httpClient.GetStringAsync("http://tos.nexon.co.jp/players");
    
    //log.Info(html);
    
    var htmlDoc = new HtmlAgilityPack.HtmlDocument();
    htmlDoc.LoadHtml(html);
    
    var nodes = htmlDoc.DocumentNode.SelectNodes("//li[@class='wait'][1]/ul/li");

    string tweet = "#ToSJP サーバー待機人数\n";
    foreach (var node in nodes)
    {
        tweet += node.SelectSingleNode("dl/dt").InnerHtml;
        tweet += " ";
        var num = node.SelectSingleNode("dl/dd/span")?.InnerHtml;
        if (num != null)
        {
            tweet += num;
            tweet += "人\n";
        }
        else
        {
            tweet += node.SelectSingleNode("dl/dd").InnerHtml;
            tweet += "\n";
        }
    }
    
    //log.Info(tweet);

    var consumerKey = Environment.GetEnvironmentVariable("CONSUMER_KEY");
    var consumerSecret = Environment.GetEnvironmentVariable("CONSUMER_SECRET");
    var accessToken = Environment.GetEnvironmentVariable("ACCESS_TOKEN");
    var accessTokenSecret = Environment.GetEnvironmentVariable("ACCESS_TOKEN_SECRET");
    var twitterClient = Tokens.Create(consumerKey, consumerSecret, accessToken, accessTokenSecret);
    await twitterClient.Statuses.UpdateAsync(status => tweet);
}
