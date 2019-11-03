import requests
import json
import os
import sys


def upload_u(img):
    '''
        eg:
        {   'code': 'success',
            'data': {
                'delete': 'https://sm.ms/delete/hgm8rB4NPoxa12U',
                'filename': '6f6c9b73-2045-4405-8c28-08f008f413e2.jpg',
                'hash': 'hgm8rB4NPoxa12U',
                'height': 198,
                'ip': '180.160.38.133',
                'path': '/2019/02/25/5c73e42754372.jpg',
                'size': 11387,
                'storename': '5c73e42754372.jpg',
                'timestamp': 1551098919,
                'url': 'https://i.loli.net/2019/02/25/5c73e42754372.jpg',
                'width': 198
            }
        }
        {
            "code" : "error",
            "msg" : "No files were uploaded."
        }

        {
            'success': False, 
            'code': 'exception', 
            'message': 'Unavailable image format', 
            'RequestId': '2551D549-3A52-405C-9E4D-4909E68685D2'
        }
    '''
    try:
        return json.loads(
            requests.post(
                "https://sm.ms/api/v2/upload", 
                headers = {
                    'Authorization': '替换'
                },
                files = {
                    'smfile': (
                        os.path.splitext(os.path.basename(img))[0],
                        open(img.replace('\n', ''), 'rb')
                    )
                }
            ).text
        )
    except:
        return {'code': 'error', 'msg': 'Program error.'}

def upload(img):
    try:
        return json.loads(
            requests.post(
                "https://sm.ms/api/upload", 
                files = {
                    'smfile': (
                        os.path.splitext(os.path.basename(img))[0],
                        open(img.replace('\n', ''), 'rb')
                    )
                }
            ).text
        )
    except:
        return {'code': 'error', 'msg': 'Program error.'}


def history():
    return json.loads(requests.get("https://sm.ms/api/list").text)

def clear():
    return json.loads(requests.get("https://sm.ms/api/clear").text)
    
if __name__ == "__main__":
    imgs = sys.argv
    len = len(sys.argv)
    uptouser = False
    if len > 1:
      for i in imgs[1:]:
        if i == "-u" or i == "/u":
          uptouser = True
      for i in imgs[1:]:
        if uptouser:
            if i not in ("-u", "/u"):
              print("uploading to user: qwq...")
              j = upload_u(i)
              print("done.")
            else:
              continue
        else:
          print("uploading...")
          j = upload(i)
          print("done")
        if j['code'] == 'success':
            print(j['data']['url'])
            print('![{}]({})'.format(j['data']['filename'], j['data']['url']))
            print('<img src="{}">'.format(j['data']['url']))
            print(j['data']['delete'])
            print('\n')
        else:
            print('Error')
    # while 1:
    #     img = input()
    #     print(upload(i)['url'])
            