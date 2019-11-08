using System;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace getElecBills {
    public class Usage {
        public string date;
        public double elec;
        public double cost;
        public Usage(string date_, string elec_, string unitPrice_){
            this.date = date_;
            this.elec = double.Parse(elec_);
            this.cost = this.elec * double.Parse(unitPrice_);
        }
    }

    public class BuyInfo {
        public string date;
        public string elec;
        public string cost;
        public string people;
        public BuyInfo(string date_, string elec_, string cost_, string people_){
            this.date = date_;
            this.elec = elec_;
            this.cost = cost_;
            this.people = people_;
        }
    }
    class main {
        static void Main(string[] args) {
            string url = "http://202.120.163.129:88";
            string user_agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.87 Safari/537.36";
            //url = "http://httpbin.org/post";
            string html;

            // 获取Cookies
            Console.WriteLine("正在登陆...");
            string param = getRoomParam();
            byte[] bs = Encoding.ASCII.GetBytes(param);
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(url);
            req.Method = "POST";
            req.AllowAutoRedirect = false;
            req.ContentType = "application/x-www-form-urlencoded";
            req.Accept = "*/*";
            req.Headers["Accept-Encoding"] = "gzip, deflate";
            req.UserAgent = user_agent;
            req.CookieContainer = new CookieContainer();
            req.ContentLength = bs.Length;
            using (Stream reqStream = req.GetRequestStream()) {
                reqStream.Write(bs, 0, bs.Length);
            }
            HttpWebResponse res = (HttpWebResponse) req.GetResponse();
            CookieContainer cookiesContainer = req.CookieContainer;

            //获取使用情况
            req = (HttpWebRequest) WebRequest.Create(url + "/usedRecord1.aspx");
            param = "__VIEWSTATE=%2FwEPDwULLTIwNzc4NTkwMDgPZBYCAgEPZBYCAgMPFgIeC18hSXRlbUNvdW50AgoWFGYPZBYCZg8VBAoyMDE5LTExLTA4AzQzNwQ2LjM5BjAuNjE3MGQCAQ9kFgJmDxUECjIwMTktMTEtMDcDNDM3BTEwLjEyBjAuNjE3MGQCAg9kFgJmDxUECjIwMTktMTEtMDYDNDM3BDguNjAGMC42MTcwZAIDD2QWAmYPFQQKMjAxOS0xMS0wNQM0MzcEOC4yOQYwLjYxNzBkAgQPZBYCZg8VBAoyMDE5LTExLTA0AzQzNwUxMC40NwYwLjYxNzBkAgUPZBYCZg8VBAoyMDE5LTExLTAzAzQzNwQ3Ljk5BjAuNjE3MGQCBg9kFgJmDxUECjIwMTktMTEtMDIDNDM3BTEwLjcwBjAuNjE3MGQCBw9kFgJmDxUECjIwMTktMTEtMDEDNDM3BTEyLjA2BjAuNjE3MGQCCA9kFgJmDxUECjIwMTktMTAtMzEDNDM3BDcuMzYGMC42MTcwZAIJD2QWAmYPFQQKMjAxOS0xMC0zMAM0MzcFMTEuMDEGMC42MTcwZGS5OM1%2F9%2BwxhkCQGHJRfIk4fnYv%2BvoWFoCqsYQF%2Fi4ycw%3D%3D&__VIEWSTATEGENERATOR=1F9D3B38&__EVENTVALIDATION=%2FwEdAARpZNeF5JscOFkNrsfcKuAK0FAlDrIj7n5hu5FXeSoIm6Yp7cPvy138AI60rKnW%2FP2nnFfL7Vy8mnWVKUTeRozhtzf4zHz%2FNJklrJ1ilqPEMXyVUKS5a5y9WzhcZqJ22Ls%3D&txtstart=2019-08-01&txtend=2020-08-01&btnser=%E6%9F%A5%E8%AF%A2";
            bs = Encoding.ASCII.GetBytes(param);
            req.Method = "POST";
            req.AllowAutoRedirect = false;
            req.ContentType = "application/x-www-form-urlencoded";
            req.Accept = "*/*";
            req.Headers["Accept-Encoding"] = "gzip, deflate";
            req.UserAgent = user_agent;
            req.CookieContainer = cookiesContainer;
            req.ContentLength = bs.Length;
            using (Stream reqStream = req.GetRequestStream()) {
                reqStream.Write(bs, 0, bs.Length);
            }
            res = (HttpWebResponse) req.GetResponse();
            using (StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("utf-8"))) {
                html = reader.ReadToEnd();
            }
            double elec = double.Parse(new Regex("剩余电量：<span class=\"number orange\">(.*?)</span> 度").Match(html).Groups[1].Value);
            double money = 0.6170 * elec;
            int pages = int.Parse(new Regex("共 ([0-9]+) 页").Match(html).Groups[1].Value);
            pages = pages > 5 ? 5 : pages;

            Console.WriteLine("正在获取使用情况(20190801-20200801)...");
            List<Usage> usages = new List<Usage>();
            for(int i = 1; i <= pages; i++){
                Console.WriteLine(String.Format("第 {0}/{1} 页...", i, pages));
                req = (HttpWebRequest) WebRequest.Create(String.Format("{0}/usedRecord1.aspx?p={1}", url, i));
                req.CookieContainer = cookiesContainer;
                res = (HttpWebResponse) req.GetResponse();
                using (StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("utf-8"))) {
                    html = reader.ReadToEnd();
                }
                MatchCollection parsedHtml = new Regex("<tr class=\"contentLine\">\\s*<td>(.*?)</td>\\s*<td>(.*?)</td>\\s*<td>(.*?)</td>\\s*<td>(.*?)</td>").Matches(html);
                foreach (Match match in parsedHtml) {
                    usages.Add(new Usage(match.Groups[1].Value, match.Groups[3].Value, match.Groups[4].Value));
                    
                }
            }

            // 获取充值情况
            req = (HttpWebRequest) WebRequest.Create(url + "/buyRecord1.aspx");
            param = "__VIEWSTATE=%2FwEPDwUKMjA1MjY3MjQwNw9kFgICAQ9kFgICAw8WAh4LXyFJdGVtQ291bnQCARYCZg9kFgJmDxUFEzIwMTkvMTAvMTggMjE6NDA6MDEDNDM3BjMyNC4xNQYyMDAuMDAJ5LiA5Y2h6YCaZGQMENSL0gzjUU4psOw8oS5FvFObUk6BnTKf%2F%2Fk4bVT4JA%3D%3D&__VIEWSTATEGENERATOR=441C415E&__EVENTVALIDATION=%2FwEdAAT0xrnv8XAy2BJjOcdVy4880FAlDrIj7n5hu5FXeSoIm6Yp7cPvy138AI60rKnW%2FP2nnFfL7Vy8mnWVKUTeRozhKAyXZScMTmXgVCFJma%2FBATs5DFom9E1ym8mqbLmb1QY%3D&txtstart=2019-08-01&txtend=2020-08-01&btnser=%E6%9F%A5%E8%AF%A2";
            bs = Encoding.ASCII.GetBytes(param);
            req.Method = "POST";
            req.AllowAutoRedirect = false;
            req.ContentType = "application/x-www-form-urlencoded";
            req.Accept = "*/*";
            req.Headers["Accept-Encoding"] = "gzip, deflate";
            req.UserAgent = user_agent;
            req.CookieContainer = cookiesContainer;
            req.ContentLength = bs.Length;
            using (Stream reqStream = req.GetRequestStream()) {
                reqStream.Write(bs, 0, bs.Length);
            }
            res = (HttpWebResponse) req.GetResponse();
            using (StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("utf-8"))) {
                html = reader.ReadToEnd();
            }
            pages = int.Parse(new Regex("共 ([0-9]+) 页").Match(html).Groups[1].Value);
            pages = pages > 5 ? 5 : pages;
            Console.WriteLine("正在获取充值记录(20190808-20200801)...");
            List<BuyInfo> buyInfos = new List<BuyInfo>();
            for(int i = 1; i <= pages; i++){
                Console.WriteLine(String.Format("第 {0}/{1} 页...", i, pages));
                req = (HttpWebRequest) WebRequest.Create(String.Format("{0}/buyRecord1.aspx?p={1}", url, i));
                req.CookieContainer = cookiesContainer;
                res = (HttpWebResponse) req.GetResponse();
                using (StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("utf-8"))) {
                    html = reader.ReadToEnd();
                }
                MatchCollection parsedHtml = new Regex(
                    "<tr class=\"contentLine\">\\s*<td>(.*?)</td>\\s*<td>.*?</td>\\s*<td>(.*?)</td>\\s*<td>(.*?)</td>\\s*<td>(.*?)</td>\\s*</tr>"
                ).Matches(html);
                foreach (Match match in parsedHtml) {
                    buyInfos.Add(new BuyInfo(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, match.Groups[4].Value));
                    
                }
            }
            

            Console.WriteLine(String.Format("\n剩余电量 {0} 度, 折合电费 {1} 元.", elec, money));
            Console.WriteLine("\n使用情况: ");
            foreach (Usage usage in usages) {
                Console.WriteLine(String.Format("{0,-8}, {1, 8}度, {2, 8}元", usage.date, usage.elec, usage.cost));
            }

            Console.WriteLine("\n充值记录: ");
            foreach (BuyInfo buyInfo in buyInfos) {
                Console.WriteLine(String.Format("{0, -20}, {1, 8}度, {2, 8}元, {3, 4}", buyInfo.date, buyInfo.elec, buyInfo.cost, buyInfo.people));
            }

            Console.WriteLine("\n发送至手机(y/n)? ");
            String key = Console.ReadKey().Key.ToString();
            if (key == "Y") {
                string ss = String.Format("\n剩余电量 {0} 度, 折合电费 {1} 元.\n\n\n\n使用情况: \n\n", elec, money);
                ss += "|时间|用量(度)|折合电价(元)|\n|----------|----------|----------|\n";
                foreach (Usage usage in usages) {
                    ss += String.Format("|{0, -20},\t|{1, 8},\t|{2, 8};|\n", usage.date, usage.elec, usage.cost);
                }
                ss += "\n\n\n\n充值记录:\n\n";
                ss += "|日期|充值电量(度)|充值金额(元)|充值人|\n|:------|:------|:------|:------|\n";
                foreach (BuyInfo buyInfo in buyInfos) {
                ss += String.Format("|{0, -20},|{1, 8},|{2, 8},|{3, 4};|\n", buyInfo.date, buyInfo.elec, buyInfo.cost, buyInfo.people);
                }
                Console.WriteLine(ss);
                sendMsg(DateTime.Now.ToString("电费查询 (yyyy-MM-dd-hh:mm:ss)"), ss);
            }
        }

        static void sendMsg(string text, string desp) {
            string configPath = String.Format("{0}\\.config\\ServerChan\\SCKEY", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            string configDirPath = String.Format("{0}\\.config\\ServerChan", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            string scKey;
            if(!File.Exists(configPath)) {
                Console.WriteLine("SCKEY未找到.");
                Console.WriteLine("输入SCKEY(可在http://sc.ftqq.com/?c=code获取):");
                scKey = Console.ReadLine();
                if (!Directory.Exists(configDirPath)){
                    Directory.CreateDirectory(configDirPath);
                }
                using (StreamWriter streamWriter = new StreamWriter(configPath)) {
                    streamWriter.Write(scKey);
                }
            } else {
                using (StreamReader streamReader = new StreamReader(configPath)) {
                    scKey = streamReader.ReadLine();
                }
            }
            Console.WriteLine("发送中...");
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(string.Format("https://sc.ftqq.com/{0}.send?text={1}&desp={2}", scKey, text, desp));
            req.GetResponse();
        }

        static string getRoomParam() {
            string roomPath = String.Format(
                "{0}\\.config\\get-elec-bills\\ROOM", 
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            );
            string roomDirPath = String.Format(
                "{0}\\.config\\get-elec-bills", 
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            );
            string room;
            if(!File.Exists(roomPath)) {
                Console.WriteLine("房间配置未找到.");
                Console.WriteLine("已初始化为默认房间.");
                Console.WriteLine(string.Format("请完成一次正常查询, 将 post data 填入 {0} 文件中.", roomPath));
                room = "__VIEWSTATE=%2FwEPDwULLTE0MTgxMTM1NTAPZBYCAgEPZBYIAgEPEGRkFgECB2QCAw8QDxYGHg1EYXRhVGV4dEZpZWxkBQhST09NTkFNRR4ORGF0YVZhbHVlRmllbGQFBnJvb21kbR4LXyFEYXRhQm91bmRnZBAVCAbmpbzmoIsN5Y%2BL5ZutMuWPt%2BalvA3lj4vlm60z5Y%2B35qW8DeWPi%2BWbrTTlj7fmpbwN5Y%2BL5ZutNeWPt%2BalvA3lj4vlm6025Y%2B35qW8CDE55Y%2B35qW8CDIw5Y%2B35qW8FQgAAjAyAjAzAjA0AjA1AjA2AjE5AjIwFCsDCGdnZ2dnZ2dnFgECB2QCBQ8QDxYGHwAFCFJPT01OQU1FHwEFBnJvb21kbR8CZ2QQFQgG5qW85bGCAjFGAjJGAjNGAjRGAjVGAjZGDjIw5Y%2B35qW85o6n5Yi2FQgABDIwMDEEMjAwMgQyMDAzBDIwMDQEMjAwNQQyMDA2BDIwMDcUKwMIZ2dnZ2dnZ2cWAQIEZAIHDxAPFgYfAAUIUk9PTU5BTUUfAQUGcm9vbWRtHwJnZBAVJwbmiL%2Fpl7QLMjDlj7fmpbw0MDELMjDlj7fmpbw0MDILMjDlj7fmpbw0MDMLMjDlj7fmpbw0MDQLMjDlj7fmpbw0MDULMjDlj7fmpbw0MDYLMjDlj7fmpbw0MDcLMjDlj7fmpbw0MDgLMjDlj7fmpbw0MDkLMjDlj7fmpbw0MTALMjDlj7fmpbw0MTELMjDlj7fmpbw0MTILMjDlj7fmpbw0MTMLMjDlj7fmpbw0MTQLMjDlj7fmpbw0MTULMjDlj7fmpbw0MTYLMjDlj7fmpbw0MTcLMjDlj7fmpbw0MTgLMjDlj7fmpbw0MTkLMjDlj7fmpbw0MjALMjDlj7fmpbw0MjELMjDlj7fmpbw0MjILMjDlj7fmpbw0MjMLMjDlj7fmpbw0MjQLMjDlj7fmpbw0MjULMjDlj7fmpbw0MjYLMjDlj7fmpbw0MjcLMjDlj7fmpbw0MjgLMjDlj7fmpbw0MjkLMjDlj7fmpbw0MzALMjDlj7fmpbw0MzELMjDlj7fmpbw0MzILMjDlj7fmpbw0MzMLMjDlj7fmpbw0MzQLMjDlj7fmpbw0MzULMjDlj7fmpbw0MzYLMjDlj7fmpbw0MzcLMjDlj7fmpbw0MzkVJwAGMjAwNDAxBjIwMDQwMgYyMDA0MDMGMjAwNDA0BjIwMDQwNQYyMDA0MDYGMjAwNDA3BjIwMDQwOAYyMDA0MDkGMjAwNDEwBjIwMDQxMQYyMDA0MTIGMjAwNDEzBjIwMDQxNAYyMDA0MTUGMjAwNDE2BjIwMDQxNwYyMDA0MTgGMjAwNDE5BjIwMDQyMAYyMDA0MjEGMjAwNDIyBjIwMDQyMwYyMDA0MjQGMjAwNDI1BjIwMDQyNgYyMDA0MjcGMjAwNDI4BjIwMDQyOQYyMDA0MzAGMjAwNDMxBjIwMDQzMgYyMDA0MzMGMjAwNDM0BjIwMDQzNQYyMDA0MzYGMjAwNDM3BjIwMDQzORQrAydnZ2dnZ2dnZ2dnZ2dnZ2dnZ2dnZ2dnZ2dnZ2dnZ2dnZ2dnZ2dnZ2dkZBgBBR5fX0NvbnRyb2xzUmVxdWlyZVBvc3RCYWNrS2V5X18WBAUEYnV5UgUFdXNlZFIFDEltYWdlQnV0dG9uMQUMSW1hZ2VCdXR0b24y6AbOOXnWTabp6hikFcpUpLJnIKmV2cglq5aXoN4aToM%3D&drlouming=9&drceng=20&dr_ceng=2004&drfangjian=200437&radio=usedR&ImageButton1.x=74&ImageButton1.y=13";
                if (!Directory.Exists(roomDirPath)){
                    Directory.CreateDirectory(roomDirPath);
                }
                using (StreamWriter streamWriter = new StreamWriter(roomPath)) {
                    streamWriter.Write(room);
                }
            } else {
                using (StreamReader streamReader = new StreamReader(roomPath)) {
                    room = streamReader.ReadLine();
                }
            }
            return room;
        }

    }

}