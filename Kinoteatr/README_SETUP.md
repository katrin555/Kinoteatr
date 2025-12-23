# Инструкция по настройке проекта

## Шаг 1: Настройка MySQL

### 1.1. Убедитесь, что MySQL установлен и запущен

### 1.2. Настройте строку подключения
Откройте файл `appsettings.json` и измените строку подключения:
```json
"DefaultConnection": "Server=localhost;Database=KinoteatrDB;User=root;Password=ВАШ_ПАРОЛЬ;"
```
Замените `ВАШ_ПАРОЛЬ` на ваш пароль MySQL (если пароля нет, оставьте `Password=;`)

### 1.3. Создание базы данных (выберите один вариант)

**Вариант А: Автоматическое создание (рекомендуется)**
- Ничего не делайте в MySQL
- База данных создастся автоматически при выполнении `dotnet ef database update`
- Убедитесь, что пользователь из строки подключения имеет права на создание баз данных

**Вариант Б: Ручное создание**
Если хотите создать базу вручную, откройте MySQL и выполните:
```sql
CREATE DATABASE KinoteatrDB CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

Если нужно дать права пользователю:
```sql
GRANT ALL PRIVILEGES ON KinoteatrDB.* TO 'root'@'localhost';
FLUSH PRIVILEGES;
```

## Шаг 2: Установка Entity Framework Core Tools

### Если инструмент не установлен:
```bash
dotnet tool install --global dotnet-ef
```

### Если возникает ошибка при установке, попробуйте:

**Вариант 1: Обновить существующую установку**
```bash
dotnet tool update --global dotnet-ef
```

**Вариант 2: Удалить и установить заново**
```bash
dotnet tool uninstall --global dotnet-ef
dotnet tool install --global dotnet-ef
```

**Вариант 3: Очистить кеш NuGet и установить**
```bash
dotnet nuget locals all --clear
dotnet tool install --global dotnet-ef
```

**Вариант 4: Установить конкретную версию**
```bash
dotnet tool install --global dotnet-ef --version 8.0.0
```

**Вариант 5: Локальная установка (рекомендуется при проблемах с глобальной)**
```bash
cd Kinoteatr
dotnet new tool-manifest
dotnet tool install dotnet-ef --version 8.0.0
```
**Важно:** При локальной установке используйте команды одним из способов:
- `dotnet tool run dotnet-ef migrations add InitialCreate`
- `dotnet dotnet-ef migrations add InitialCreate`
- Или просто `dotnet ef migrations add InitialCreate` (должно работать)

**Вариант 6: Установка с указанием источника NuGet**
```bash
dotnet tool install --global dotnet-ef --add-source https://api.nuget.org/v3/index.json
```

**Вариант 7: Использование через Visual Studio Package Manager Console**
Откройте Package Manager Console в Visual Studio и выполните:
```
dotnet tool install --global dotnet-ef
```

## Шаг 3: Восстановление пакетов

```bash
dotnet restore
```

## Шаг 4: Создание миграций базы данных

```bash
dotnet ef migrations add InitialCreate
```

## Шаг 5: Применение миграций к базе данных

```bash
dotnet ef database update
```

Эта команда создаст базу данных `KinoteatrDB` и все необходимые таблицы.

## Шаг 6: Запуск проекта

```bash
dotnet run
```

Или через Visual Studio: нажмите F5

## Примечания:

- При первом запуске автоматически создаются роли "Administrator" и "Viewer"
- Новые пользователи регистрируются с ролью "Viewer"
- Для создания администратора нужно вручную назначить роль через базу данных или добавить код инициализации

