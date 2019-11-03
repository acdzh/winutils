import requests
import re
import msg
print("正在查询...")
headers = {
    'Host': '202.120.163.129:88',
    'Connection': 'keep-alive',
    'Cache-Control': 'max-age=0',
    'Upgrade-Insecure-Requests': '1',
    'DNT': '1',
    'Save-Data': 'on',
    'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.119 Safari/537.36',
    'Accept': 'text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8',
    'Referer': 'http://202.120.163.129:88/',
    'Accept-Encoding': 'gzip, deflate',
    'Accept-Language': 'zh,en;q=0.9,zh-CN;q=0.8',
    'Cookie': 'ASP.NET_SessionId=cqk1n0desop5pb5v4nwiuppy'
}
data = {'txtstart': '2018-01-01',
        'txtend': '2050-01-01',
        'btnser': '查询'
}

url = 'http://202.120.163.129:88/usedRecord1.aspx' 


r = requests.post(url, headers = headers, data = data)
print(r.text)
elec = re.search('剩余电量：<span class="number orange">(.*?)</span> 度', r.text)[1]
usge = re.findall('<tr class="contentLine">\s*<td>(.*?)</td>\s*<td>(.*?)</td>\s*<td>(.*?)</td>\s*<td>(.*?)</td>', r.text)
format_usge = []
s = "剩余电量 {} 度, 折合电费 {:.2f} 元.\n使用情况:".format(elec, 0.6170 * eval(elec))
ss = s
for i in usge:
    temp = ' ' * (6 - len(i[2])) + i[2]
    format_usge.append("\t{},{} 度, {:.2f} 元.".format(i[0], temp, eval(i[2]) * eval(i[3])))
for i in format_usge:
    ss += ('\n' + i)
print(ss)
i = input("是否发送至手机? (y/n) ")
if i == 'y':
    print(msg.send_msg('电费提醒', ss))