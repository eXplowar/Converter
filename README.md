# Converter - конвертирование HTML в PDF

Данный пример реализует следующие требования:

Пользователь отправляет HTML-документ через веб-страницу. Его обработкой занимается отдельный веб-сервис. В процессе обработки веб-приложение может упасть, но это не должно повлиять на дальнейшее получение пользователем готового PDF-документа.

Особенности и технологии данного решения:
- Данное решение использует YARP в качестве балансировщика нагрузки
- ApiServer реализован с использованием подхода Minimal APIs
- После получением API-сервером HTML-файла сервер создаёт Hangfire-задачу. Пользователю возвращается сообщение о начале конвертации
- По завершению работы Hangfire оповещает брокера сообщений RabbitMQ о завершении конвертации
- Все веб-приложения слушают Rabbit, то к которому подключен пользователь оповещает его через SignalR о готовности файла к загрузке
- Получив сообщение через SignalR, браузер выполняет rest-запрос к веб-приложению на скачиваение файла. Веб-приложение забирает его у ApiServer.

## Сборка

Возможен запуск из Visual Studio, для этого следует выбрать несколько запускаемых проектов:
- Converter.ApiServer - веб-сервис, реализующий основую бизнес задачу
- Converter.WebApp  - веб-приложение, через которое пользователь взаимодейстует с веб-сервисом
- Converter.Gateway - прокси-сервер (балансировщик нагрузки) перенаправляет пользователя на доступный ресурс

`Следует убедиться, что запускаемые проекты используют не Docker-профель, а Kestrel-профиль, имя которого соответствует имени проекта.`

Адрес и порт по умолчанию: http://localhost:3000/

### Запуск с использование Docker:

Следующие команды следует выполнять из `solution-директории`

Запуск RabbitMQ:
```sh
docker run -it --rm --name rabbit -p 5672:5672 -p 15672:15672 --network converter-network rabbitmq:3-management
```
Создание образа и запуск контейнера Converter.ApiServer:
```sh
docker build -f Converter.ApiServer/Dockerfile -t apiserver-image .
docker run --rm -d --hostname apiserver --network converter-network -p 1000:80 --name Converter.ApiServer apiserver-image
```
Создание образа и запуск двух контейнеров Converter.WebApp:
```sh
docker build -f Converter.WebApp/Dockerfile -t webapp-image .
docker run --rm -d --hostname webapp1 --network converter-network -p 2000:80 --name Converter.WebApp-1 webapp-image
docker run --rm -d --hostname webapp2 --network converter-network -p 2001:80 --name Converter.WebApp-2 webapp-image
```
Создание образа и запуск контейнера Converter.Gateway:
```sh
docker build -f Converter.Gateway/Dockerfile -t gateway-image .
docker run --rm -d --hostname gateway --network converter-network -p 3000:80 --name Converter.Gateway gateway-image
```

Адрес и порт по умолчанию: http://localhost:3000/

### Запуск с использование Docker Compose:
In progress..

## Roadmap
- Требуется доработка в переключении SignalR к живому веб-приложению
- Требуется доработка docker-compose
