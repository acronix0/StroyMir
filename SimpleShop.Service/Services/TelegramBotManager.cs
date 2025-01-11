using Microsoft.Extensions.Configuration;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using SimpleShop.Core.Dtos;
using SimpleShop.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;

namespace SimpleShop.Service.Services
{
    public class TelegramBotManager
    {
        private readonly TelegramBotClient _botClient;
        private readonly string _chatId;

        public TelegramBotManager(IConfiguration configuration)
        {
            // Получаем токен и ID группы из appsettings.json
            string botToken = configuration.GetSection("TGBot").GetValue<string>("token");
            _chatId = configuration.GetSection("TGBot").GetValue<string>("groupId");

            if (string.IsNullOrEmpty(botToken) || string.IsNullOrEmpty(_chatId))
            {
                throw new ArgumentException("Telegram Bot settings are missing in appsettings.json");
            }

            _botClient = new TelegramBotClient(botToken);
        }

        public async Task SendNewOrderAsync(OrderDto order, string orderBaseUrl)
        {
            // Формируем текст сообщения с деталями заказа
            var orderMessage = new StringBuilder();
            orderMessage.AppendLine($"🛒 *Новый заказ #{order.Id}*");
            orderMessage.AppendLine($"👤 *Имя заказчика:* {order.RecipientName}");
            orderMessage.AppendLine($"📞 *Телефон заказчика:* {order.RecipientPhone}");
            if (!string.IsNullOrEmpty(order.RecipientEmail))
            {
                orderMessage.AppendLine($"📧 *Email заказчика:* {order.RecipientEmail}");
            }
            orderMessage.AppendLine($"\n💰 *Общая стоимость:* {FormatPriceDecimal(order.TotalPrice)}");
            orderMessage.AppendLine($"\n🚚 *Тип доставки:* {order.DeliveryType}");
            if (!string.IsNullOrEmpty(order.Address))
            {
                orderMessage.AppendLine($"🏠 *Адрес доставки:* {order.Address}");
            }
            orderMessage.AppendLine($"\n📅 *Дата заказа:* {DateTime.Now:dd.MM.yyyy}");
            if (!string.IsNullOrEmpty(order.Comment))
            {
                orderMessage.AppendLine($"\n📝 *Комментарий:* {order.Comment}");
            }

            // Добавляем ссылку на заказ
            string orderUrl = $"{orderBaseUrl}/api/order/{order.Id}";
            orderMessage.AppendLine($"\n🔗 *Ссылка на заказ:* [Посмотреть заказ]({orderUrl})");

            // Отправляем сообщение в Telegram
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: orderMessage.ToString(),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
            );
        }


        // Вспомогательный метод для форматирования цен
        private string FormatPriceDecimal(decimal price)
        {
            price = Math.Round(price, 2, MidpointRounding.AwayFromZero);

            // Форматируем число с использованием инвариантной культуры
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:N2}", price)
                         .Replace(",", " ") // Разделитель тысяч — пробел
                         .Replace(".", ",") + " руб."; // Десятичный разделитель — запятая
        }


       
        public async Task SendOrdersReportToTelegramAsync(List<OrderDto> orders)
        {
            // Устанавливаем лицензию для EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Создаем поток для файла Excel
            using (var stream = new MemoryStream())
            {
                using (var package = new ExcelPackage(stream))
                {
                    foreach (var order in orders)
                    {
                        // Создаем новый лист для каждого заказа
                        var worksheet = package.Workbook.Worksheets.Add($"Заказ #{order.Id}");

                        // Заголовки заказа
                        worksheet.Cells[1, 1].Value = "Имя заказчика:";
                        worksheet.Cells[1, 2].Value = order.RecipientName;

                        worksheet.Cells[2, 1].Value = "Телефон заказчика:";
                        worksheet.Cells[2, 2].Value = order.RecipientPhone;

                        worksheet.Cells[3, 1].Value = "Email заказчика:";
                        worksheet.Cells[3, 2].Value = order.RecipientEmail;

                        worksheet.Cells[4, 1].Value = "Тип доставки:";
                        worksheet.Cells[4, 2].Value = order.DeliveryType;

                        worksheet.Cells[5, 1].Value = "Адрес доставки:";
                        worksheet.Cells[5, 2].Value = order.Address;

                        worksheet.Cells[6, 1].Value = "Дата заказа:";
                        worksheet.Cells[6, 2].Value = order.OrderDate.ToShortDateString();

                        if (!string.IsNullOrEmpty(order.Comment))
                        {
                            worksheet.Cells[7, 1].Value = "Комментарий:";
                            worksheet.Cells[7, 2].Value = order.Comment;
                        }

                        worksheet.Cells[8, 1].Value = "Общая стоимость:";
                        worksheet.Cells[8, 2].Value = FormatPriceDecimal(order.TotalPrice);

                        // Заголовки таблицы товаров
                        worksheet.Cells[10, 1].Value = "Артикул";
                        worksheet.Cells[10, 2].Value = "Название";
                        worksheet.Cells[10, 3].Value = "Количество";
                        worksheet.Cells[10, 4].Value = "Цена";

                        // Стили заголовков таблицы
                        using (var range = worksheet.Cells[10, 1, 10, 4])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        }

                        // Добавляем данные товаров
                        for (int i = 0; i < order.OrderProducts.Count; i++)
                        {
                            var product = order.OrderProducts[i];
                            worksheet.Cells[i + 11, 1].Value = product.ProductArticle;
                            worksheet.Cells[i + 11, 2].Value = product.ProductName;
                            worksheet.Cells[i + 11, 3].Value = product.Count;
                            worksheet.Cells[i + 11, 4].Value = FormatPriceDecimal(product.ProductPrice);
                        }

                        // Автоматическая ширина столбцов
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    }

                    // Сохраняем Excel-файл в поток
                    package.Save();
                }

                stream.Position = 0;

                // Отправляем файл в Telegram
                var inputFile = new InputOnlineFile(stream, "Отчет_заказы.xlsx");
                await _botClient.SendDocumentAsync(
                    chatId: _chatId,
                    document: inputFile,
                    caption: $"📋 *Отчет по заказам*",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                );
            }
        }
    }
}
