
## 相依性

此組件建立在以下組件的基礎 :
| 檔案名稱 | 描述 |
| ------ | ----------- |
| yt-dlp.exe   | youtube檔案下載功能主要組件，提供下載介面，相關資訊請參考 :https://github.com/yt-dlp/yt-dlp |
| ffmpeg.exe | yt-dlp.exe需要的組件 |
| ffplay.exe    | yt-dlp.exe需要的組件  |
| ffprobe.exe    | yt-dlp.exe需要的組件 |


## 特色
1. 從youtube下載檔案到本地資料夾
2. 從youtube已公開的歌單下載影片，並自動排除先前下載過的影片 
3. 管理清單下載動作，紀錄每次從youtube清單下載影片的下載動作
4. 使用雲端同步資料，從youtuber清單下載的新檔案曲會自動上傳至雲端
5. 透過先前紀錄，可將之前下載清單檔案後上傳雲端的檔案從雲端重新下載至本地
6. 從youtube下載至本地支援不同格式(mp4/ mp3/ opus .....)

## 命令列功能路由說明
1. 單一檔案youtube下載
    + 透過youtube連結下載單一檔案至本地資料夾
2. 清單檔案youtube下載
    + 透過youtube清單連結下載多個檔案至本地資料夾，排除先前下在過的
3. 先前下載動作查詢及雲端下載
    + 條列下載動作紀錄，選擇這些紀錄的其中一個，從雲端下載 這個紀錄下載的檔案
4. 清除使用者資料
    + 刪除tempData，這個動作之後需要重新輸入使用者資訊及雲端位置，並清除先前所有下載動作記錄，不能再透過這些紀錄從雲端重新下載檔案
5. 更新yt-dlp
    + 如果不能從youtube下載，請嘗試使用這個功能
  
## 下載單一影片 
進到youtube網站，點選任一影片進入觀看頁面，複製瀏覽器上方連結用來在下載器中下載該影片

## 下載清單影片
透過youtube網站使用者內建的功能可建立專屬於自己的清單，這些歌單可以從每個影片播放網頁中去新增這些播放的影片，但是清單必須先設定為公開


影片播放頁面中找到`儲存`並點選，顯示彈跳視窗，可以從視窗中建立新的清單，點選 `建立新的播放清單`
![downloader1](https://github.com/0Speechless0/0Speechless0-youtubeDownloader/assets/36149504/5783b0ab-22d6-4e49-a9be-eabb3f99f57d)

輸入清單名稱，記得設為`公開`
![downloader3](https://github.com/0Speechless0/0Speechless0-youtubeDownloader/assets/36149504/0ffc03b0-8e35-4e9d-ba06-5f7a93b1fbec)


也可選擇已存在的清單加入
![downloader2](https://github.com/0Speechless0/0Speechless0-youtubeDownloader/assets/36149504/43fc5ba5-b14e-4b83-ac84-933f53d8d6c3)


再來看到左編列表會依清單建立時間由晚到早往下排序，剛剛建立的清單會在最上面，在左列表點選剛剛建立的清單名稱，右側顯示加入清單的影及是否公開，複製此頁瀏覽器上方的連結，用在下載器中下載此清單中的影片，描述畫面如下
![downloader4_](https://github.com/0Speechless0/0Speechless0-youtubeDownloader/assets/36149504/bceac10c-d321-49d2-9f7b-0b9618d9be19)


## 重新下載清單影片

當網站上清單中的影片已被上傳至雲端，則不能再透過下載器的清單下載功能(2.)下載影片，我們需要透過(3.)從雲端下載影片，由於影片在第一次從網站下載成功後會自動上傳至雲端，故這裡不需要再從網站下載，進入下載器的功能(3.)，畫面顯示清單的下載紀錄，及其編號，選擇編號下載編號對應的紀錄影片


## 本地模式、線上模式

tempData是判斷本地模式與線上模式的依據，tempData紀錄雲端的位置和登入雲端的帳密，雲端必須支援wedDav協議，可以使用開源的雲端服務如 : nextcloud ，您必須先架設雲端服務服務，並建立對外網路提供連線，

+ nextcloud 下載連結 (for windows) : https://nextcloud.com/install/#instructions-server

或是用ubunutu 的iso 重灌電腦，22.04以上的版本安裝時可額外選擇安裝nextcloud服務
+ ubuntu : https://www.ubuntu-tw.org/modules/tinyd0/

隨身碟燒入教學請自行網路上搜尋教學，不做說明

當建立好雲端服務及使用者帳密，第一次進入下載器會先詢問雲端服務未置及使用者設置，輸入完後程式嘗試連上雲端，如果失敗，則進入本地模式，否則當帳密也驗證成功後進入線上模式

兩者模式的能力區別在能否同步雲端資料

### 流程圖
![ytdownloader-模式設定流程 drawio(1)](https://github.com/0Speechless0/0Speechless0-youtubeDownloader/assets/36149504/b45ecf38-594f-4060-8cad-7ad6a67d788b)


## tempData.bin

用於存放下載影片紀錄(單一下載的功能(1.) 除外)及雲端連結資訊，第一次輸入雲端未置和使用者資訊成功後會自動從雲端同步到本地，若雲端沒有tempData會在本地自動建立
並在往後的功能(2.)下載更新並同步雲端，其存放在編譯後檔案目錄下



### 流程圖(線上模式)

![ytdownloader-tempData更新流程 drawio (1)](https://github.com/0Speechless0/0Speechless0-youtubeDownloader/assets/36149504/0af2f4b3-c9e8-4fc1-a561-6dbf50d80f83)

如果雲端位置有改變，您必須將原雲端位置上的同步資料夾轉移至新的位置，裡面包含tempData，下載器運行功能(4.)刪除本地tempData後重啟，將會看到雲端位置的詢問，再次設置新的雲端

## 組件安裝

所需的組件會在程式啟動後檢查檔案若不存在後安裝，若下載器無法正常運行，嘗試使用功能(5.)，或嘗試自行安裝組件放置於編譯檔目錄中
1. yt-dlp : https://github.com/yt-dlp/yt-dlp
2. ffmpeg  for win 7 above : https://www.gyan.dev/ffmpeg/builds/
