using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

public class MyBotService
{
    private readonly ITelegramBotClient _botClient;
    private Timer _timer;
    private readonly Random _random = new Random();

    public MyBotService(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task StartAsync()
    {
        var cts = new CancellationTokenSource();
        var options = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        _botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, options, cts.Token);        

        Console.WriteLine("Бот запущен. Нажмите Enter, чтобы завершить работу.");
        Console.ReadLine();

        cts.Cancel();
    }

    private async Task SendStartMessage(ITelegramBotClient botClient, long chatId)
    {
        var keyboard = new ReplyKeyboardMarkup(new[]
        {
            new[]
            {
                new KeyboardButton("Начать")
            }
        });

        keyboard.ResizeKeyboard = true;

        await botClient.SendTextMessageAsync(chatId, "Добро пожаловать!", replyMarkup: keyboard);
    }

    private async Task SendExitMessage(ITelegramBotClient botClient, long chatId)
    {
        var keyboard = new ReplyKeyboardMarkup(new[]
        {
            new[]
            {
                new KeyboardButton("Выход")
            }
        });

        keyboard.ResizeKeyboard = true;

        await botClient.SendTextMessageAsync(chatId, "Вы покидаете бот. Если захотите вернуться, нажмите 'Начать'.", replyMarkup: keyboard);

        await SendStartMessage(botClient, chatId);
    }
   

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message)
        {
            var message = update.Message;

            if (message.Type == MessageType.Text)
            {
                string text = message.Text;

                if (text == "/start" || text == "Начать")
                {
                    long chatId = message.Chat.Id;
                    string firstName = message.Chat.FirstName;
                    string userName = message.Chat.Username;
                    Console.WriteLine($"Подключился пользователь: {firstName} ({userName})");

                    string greetingMessage = $"Привет, {firstName}!";                    

                    await botClient.SendTextMessageAsync(chatId, greetingMessage);

                    var keyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Играть в кости"),
                            new KeyboardButton("Посмотреть клип")
                        },
                        new[]
                        {
                            new KeyboardButton("Послушать музыку"),
                            new KeyboardButton("Слушать радио")
                        },
                        new[]
                        {
                            new KeyboardButton("Выход")
                        }
                    });

                    keyboard.ResizeKeyboard = true;

                    await botClient.SendTextMessageAsync(chatId, "Что бы вы хотели сделать?", replyMarkup: keyboard);
                    
                }
                else if (text == "Играть в кости")
                {
                    string firstName = message.Chat.FirstName;
                    string userName = message.Chat.Username;
                    Console.WriteLine($"Пользователь {firstName} ({userName}) выбрал Играть в кости");

                    long chatId = message.Chat.Id;

                    int userRoll = RollDice();
                    int botRoll = RollDice();

                    string resultMessage = $"Ваш результат: {userRoll}\nРезультат бота: {botRoll}\n";

                    if (userRoll > botRoll)
                    {
                        resultMessage += $"Поздравляем! {firstName} победил!";
                    }
                    else if (userRoll < botRoll)
                    {
                        resultMessage += "Бот победил!";
                    }
                    else
                    {
                        resultMessage += "Ничья!";
                    }

                    await botClient.SendTextMessageAsync(chatId, resultMessage);
                }
                else if (text == "Послушать музыку")
                {
                    string firstName = message.Chat.FirstName;
                    string userName = message.Chat.Username;
                    Console.WriteLine($"Пользователь {firstName} ({userName}) выбрал Послушать музыку");

                    long chatId = message.Chat.Id;
                    await botClient.SendTextMessageAsync(chatId, "Вот ссылка для прослушивания музыки: [https://music.apple.com/ua/playlist/%D1%82%D0%BE%D0%BF-50-%D1%83%D0%BA%D1%80%D0%B0%D1%97%D0%BD%D0%B0-2023/pl.u-MDAWeRNsA5Jm5rp]");
                }
                else if (text == "Посмотреть клип")
                {
                    string firstName = message.Chat.FirstName;
                    string userName = message.Chat.Username;
                    Console.WriteLine($"Пользователь {firstName} ({userName}) выбрал Посмотреть клип");

                    long chatId = message.Chat.Id;
                    await botClient.SendTextMessageAsync(chatId, "Вот ссылка на сборник свежих клипов: [https://www.youtube.com/playlist?list=PLI_7Mg2Z_-4JdNda4LkbSLaF0M5GrnBRG]");
                }
                else if (text == "Слушать радио")
                {
                    string firstName = message.Chat.FirstName;
                    string userName = message.Chat.Username;
                    Console.WriteLine($"Пользователь {firstName} ({userName}) выбрал Слушать радио");

                    long chatId = message.Chat.Id;

                    var keyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Люкс FM"),
                            new KeyboardButton("ХІТ FM")
                        },
                        new[]
                        {
                            new KeyboardButton("Radio ROKS"),
                            new KeyboardButton("KISS FM")
                        },
                        new[]
                        {
                            new KeyboardButton("Назад")
                        }
                    });

                    keyboard.ResizeKeyboard = true;

                    await botClient.SendTextMessageAsync(chatId, "Выберите радиостанцию:", replyMarkup: keyboard);
                }
                else if (text == "Люкс FM" || text == "ХІТ FM" || text == "Radio ROKS" || text == "KISS FM")
                {
                    long chatId = message.Chat.Id;

                    List<(string name, string link)> radioStations = new List<(string name, string link)>
                        {
                            ("Люкс FM", "https://lviv.lux.fm/"),
                            ("ХІТ FM", "https://play.tavr.media/hitfm/ukr/"),
                            ("Radio ROKS", "https://play.tavr.media/radioroks/"),
                            ("KISS FM", "https://play.tavr.media/kissfm/ukr/")
                        };

                    var selectedStation = radioStations.FirstOrDefault(station => station.name == text);

                    if (selectedStation != default)
                    {
                        await botClient.SendTextMessageAsync(chatId, $"Вот ссылка на радиостанцию {selectedStation.name}: {selectedStation.link}");
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(chatId, "Неверный выбор радиостанции.");
                    }
                }
                else if (text == "Назад")
                {
                    long chatId = message.Chat.Id;

                    var keyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Играть в кости"),
                            new KeyboardButton("Посмотреть клип")
                        },
                        new[]
                        {
                            new KeyboardButton("Послушать музыку"),
                            new KeyboardButton("Слушать радио")
                        },
                        new[]
                        {
                            new KeyboardButton("Выход")
                        }
                    });

                    keyboard.ResizeKeyboard = true;

                    await botClient.SendTextMessageAsync(chatId, "Что бы вы хотели сделать?", replyMarkup: keyboard);
                }
                else if (text == "Выход")
                {
                    await SendExitMessage(botClient, message.Chat.Id);

                    string firstName = message.Chat.FirstName;
                    string userName = message.Chat.Username;
                    Console.WriteLine($"Пользователь {firstName} ({userName}) покинул бот.");
                }
            }
        }
    }

    private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Ошибка: {exception.Message}");
    }   

    private int RollDice()
    {
        return _random.Next(1, 7);
    }
}

public class Program
{
    private static async Task Main()
    {
        string token = "6868902132:AAGr6Fm3KsZ_Q2xqq00qw3OZS2CAWAbz_YA";

        var botClient = new TelegramBotClient(token);
        var botService = new MyBotService(botClient);

        await botService.StartAsync();
    }
}