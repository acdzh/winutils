import requests
import json
import sys
import smms
import os
import re

msg_host = "https://sc.ftqq.com/替换.send"

def send_msg(text, desp, md = 0):
    if text == "":
        if desp == "":
            return {'code': 3, 'msg': 'text and desp are all blank'}
        else:
            text = "文字消息... " + desp
    if md == 0:
        desp = desp.replace('\n', '\n\n\n')
    my_data = {"text" : text, "desp" : desp}
    try:
        js = json.loads(requests.post(msg_host, data = my_data).text)
        if js["errno"] == 0 and js["errmsg"] == "success":
            return {'code': 0, 'msg': 'success', 'data': my_data}
        else:
            return {'code': 1, 'msg': 'server error', 'j': js}
    except:
        return {'code': 2, 'msg': 'program error'}


def send_img(img):
    if img == "":
        return {'code': 4, 'msg': 'none img path'}
    name=str(os.path.basename(img)).replace("\n", "").replace(" ", "_")
    text = ("图片消息... " + name)
    
    js = smms.upload(img.replace("\n",""))
    if js['code'] == 'success':
        url = js['data']['url']
    else:
        return {'code': 5, 'msg': 'upload failed', 'j': js}

    desp = "![{}]({})".format(name, url)
    return send_msg(text, desp)
    
def send_url(url):
    if url == "":
        return {'code': 6, 'msg': 'none url'}
    text = "url消息... " + url
    desp = "[{}]({})".format(url, url)
    return send_msg(text, desp)

def send_file(filepath):
    if filepath == "":
       return {'code': 7, 'msg': 'none such file'}

    name = name=str(os.path.basename(filepath)).replace("\n", "").replace(" ", "_")
    text = ("文件消息... " + name)

    data = {'model': 0, 'action' : 'upload'}
    files = {'file': (name, open(filepath.replace("\n", ""), 'rb')),}
    js = json.loads(requests.post("http://tmp.link/openapi/v1", data = data, files = files).text)
    if js['status'] == 0:
        d_url = "http://tmp.link/d/" + js['data']['ukey']
        f_url = js['data']['url']
    else:
        return {'code': 8, 'msg': 'upload failed'}
    
    desp = "[查看文件]({})\n\n[直接下载文件]({})".format(f_url, d_url)
    return send_msg(text, desp)

def send(msg):
    img_type = ('png', 'jpg', 'peg', 'gif', 'bmp', 'tif')
    if os.path.exists(msg) and msg[-3 : ] in img_type:
        return send_img(msg)
    if re.match(r'^https?:/{2}\w.+$', msg) or re.match(r'^http?:/{2}\w.+$', msg):
        return send_url(msg)
    if os.path.exists(msg):
        return send_file(msg)
    return send_msg("", msg)

def test(msg):
    img_types = ('png', 'jpg', 'peg', 'gif', 'bmp', 'tif')
    if os.path.exists(msg) and (msg[-3 : ] in img_types):
        return "img"
    if re.match(r'^https?:/{2}\w.+$', msg) or re.match(r'^http?:/{2}\w.+$', msg):
        return "url"
    if os.path.exists(msg):
        return "file"
    return "msg"


if __name__ == "__main__":
    operaters = ('-m', '-p', '-u', '-f')
    s = sys.argv
    l = len(s)
    if l > 1:
        if s[1] not in operaters:
            if l == 2:
                print(send(s[1]))
            elif l == 3:
                print(send_msg(s[1], s[2]))
        else:
            if s[1] == '-m':
                if l == 3:
                    print(send_msg("", s[2]))
                else:
                    print(send_msg(s[2], s[3]))
            elif s[1] == '-p':
                print(send_img(s[2]))
            elif s[1] == '-u':
                print(send_url(s[2]))
            elif s[1] =='-f':
                print(send_file(s[2]))
            else:
                print("重新输入.")
    # else:
    #     while 1:
    #         i = input()
    #         send(i)




