# 📺 Moyboy (RuTubeDownloader)
### CLI программа для скачивания видео с RuTube.

![SHAMANAAAAAAAAAAAAA](https://github.com/user-attachments/assets/17266482-ff26-41a6-994e-0e42a177ad81)

## 📃 Описание
Для корректной работы нужен установленный браузер Google Chrome.

Параметром или текстом по приглашению вводится ссылка на видео в формате `https://rutube.ru/video/ef2140ee69ec79c9e3e07a6835a5ff98/`, после чего можно выбрать необходимое качество, и начнётся скачивание.

## ❓ Другой способ, без этой программы
- Зайти в браузер, открыть нужное видео на RuTube, выбрать качество
- Открыть DevTools (Обычно клавиша F12)
- Перейти во вкладку Network или Сеть
  - ![network](https://github.com/user-attachments/assets/58e9bee9-cb48-41eb-a0b0-fd070cf023b1)

- Обновить страницу
- Воспользоваться поисковым фильтром, введя m3u
  - Chrome

    ![m3u](https://github.com/user-attachments/assets/3c285713-13d4-4471-9206-5bf898ad7bcc)
  - Firefox

    ![m3u_firefox](https://github.com/user-attachments/assets/aeee862d-440a-450c-bb18-4eb6d9bf9a99)
- Скопировать ссылку, окончивающуюся на .mp4.m3u8, отбросить лишнее

![изображение](https://github.com/user-attachments/assets/e43adc18-c8f7-4946-9412-1eaba197ab98)

- Загрузить, используя ffmpeg или аналоги, написав в консоль что-то вроде:

```bash
ffmpeg -i "https://salam-mskm9-5.rutube.ru/dive/river-3-318.rutube.ru/g0NEDP1kDDw1KWEhCU34fQ/hls-vod/5hMdDz44P_itrD9ReEVGAA/1727522698/2393/0x5000c500f2b53f87/2becba6a07074f7087110d8b0395a11f.mp4.m3u8" -c copy OUT_FILE_NAME.mp4
```

## ✍️ Как работает программа
После того, как загрузится самое главное (реклама) - начинается загрузка видео по путям из .m3u8-файла, который возвращается из GET-запроса. 

![get](https://github.com/user-attachments/assets/0a7d494c-bb25-4002-b1a6-88b3e0201c0d)

Чтобы он выполнился, подставив нужные ключи, используется Selenium. Он открывает Chrome по ссылке для получения актуального сформированного запроса. 

В результате возвращается список ссылок с доступными разрешениями видеофайла и серверами.

![get_response](https://github.com/user-attachments/assets/0db8a830-8abb-463e-80df-b57a4852baf8)

Их легко запарсить, игнорируя строки, начинающиеся с `#`, и удалив лишнюю информацию. 

Из получившегося списка можно использовать нужную для GET-запроса к выбарнному серверу с .m3u8-файлом нужного качества. 

Вернётся список адресов, где хранятся кусочки, которые нужно "склеить".

![response](https://github.com/user-attachments/assets/8d16c4fc-61bc-4216-9aa3-110eaa1e2afd)

После информация из кусочков записается в выходной файл формата .mp4.

![out_file](https://github.com/user-attachments/assets/aefaf5bc-7da2-4563-b87d-938fc35e6c19)
