SParktika
=====================

**1) SPraktika** - gui app

**2) Service_2GetWeather_and_CurrencyRates** - Windows Service 2 get currency rates with time-interval in *.xml file

**2) Service_2GetWeather_and_CurrencyRates** - Windows Service 2 get weather with time-interval in *.xml file

  #**Установка win service**:
  
  1)Командная строка разрабочика MSVS15
  
  2)Вход в директорию с службой
  
  3)installutil.exe Service_2Get_CurrencyRates.exe //[Service_2Get_Weather.exe]
  
  //Удаление installutil.exe /u Service_2Get_CurrencyRates.exe //[Service_2Get_Weather.exe]
  
  Конфиг появится в **D:\CurrencyInfoService** / **D:\WeatherInfoService**, там можно изменить интервал записи свежих данных и изменить директорию записи логов, настроить удаление логов по устариванию (По умолчанию хранятся логи последних пяти дней).


