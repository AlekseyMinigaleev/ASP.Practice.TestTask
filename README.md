# Описание проекта
Сервис для определения местоположения по IP-адресу с использованием минимум 2 сервис-провайдеров и возвращением результатов в формате JSON. 
По умолчанию все данные сохраняются в базу данных Sqlite IPLocationService.db.
## Технические требования
1. Язык программирования: C#
2. Стек технологий: ASP.NET Core (.NET 6.0+)
3. Веб-сервер: IIS
4. по умолчанию добавлено 2 сервис-провайдера для определения местоположения по IP

## Запуск проекта
1. Склонируйте проект на локальный компьютер
2. Откройте проект в Visual Studio
3. Убедитесь, что у вас установлен .NET 6.0+
4. Если нужно добавить новый сервис провайдер, то необходимо добавить в конфигурационный файл IpLocationService\IpLocationService\ConfigurationFiles\Providers.json новый объект:
```
"ProviderName": {
  "Id": "", 
  "Ip": "",
  "City": "",
  "Region": "",
  "Country": "",
  "Timezone": "",
  "Token": "",
  "url": ""
}
```
где поля:
  "Id": "", 
  "Ip": "",
  "City": "",
  "Region": "",
  "Country": "",
  "Timezone": "",
содержат названия полей, котору будут в ответе от провайдера
"url": "", содрежит URL-адрес для запросов к провайдеру, содержащий token для авторизации на сайте провайдера.
5. Запустите приложение

## API методы
### Определение местоположения по IP
Метод для определения местоположения по IP-адресу с использованием указанного сервис-провайдера.

### Запрос
+ URL: http://localhost/Controller/Action
+ HTTP метод: POST/GET
+ Параметры:
  + IP - IP-адрес
  + PROVIDER - номер провайдера
### Ответ
+ Формат: JSON
+ Поля:
  + IP - IP-адрес
  + City - Город
  + Region - Регион
  + Country - Страна
  + Timezone - Временная зона
  + Пример запроса

### Пример запроса
POST http://localhost/Controller/Action
```
{
  "IP": "192.168.0.0",
  "PROVIDER": "1"
}
```
GET http://localhost/Controller/Action?IP=192.168.0.0&provider=2
### Пример ответа
```
{
  "IP": "192.168.0.0",
  "City": "Москва",
  "Region": "Москва",
  "Country": "Россия",
  "Timezone": "Europe/Moscow"
}
```

## Документация
Документация к коду находится в xml комментариях
