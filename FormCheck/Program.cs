using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        // Загрузка данных
        var products = LoadProductsFromFile("products.csv");
        var discountCards = LoadDiscountCardsFromFile("discountCards.csv");

        // Проверка на пустоту списков
        if (products.Count == 0 || discountCards.Count == 0)
        {
            Console.WriteLine("Ошибка: данные не загружены.");
            return;
        }

        // Формирование чека
        var receipt = GenerateReceipt(products, discountCards[0]);

        // Вывод чека в консоль
        Console.WriteLine($"Дата: {receipt.Date:dd.MM.yyyy}");
        Console.WriteLine($"Время: {receipt.Date:HH:mm:ss}");
        Console.WriteLine("Товары:");
        foreach (var item in receipt.Items)
        {
            Console.WriteLine($"{item.Product.Description} x {item.Quantity} = {item.Total:F2}$ (Скидка: {item.Discount:F2}$)");
        }
        Console.WriteLine($"Итоговая цена: {receipt.TotalPrice:F2}$");
        Console.WriteLine($"Итоговая скидка: {receipt.TotalDiscount:F2}$");
        Console.WriteLine($"Итоговая стоимость: {receipt.TotalWithDiscount:F2}$");

        // Сохранение чека в файл
        SaveReceiptToCsv(receipt, "result.csv");
        Console.WriteLine("Чек сохранен в result.csv");
    }

    public static List<Product> LoadProductsFromFile(string filePath)
    {
        var products = new List<Product>();

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Файл {filePath} не найден.");
            return products;
        }

        using (var reader = new StreamReader(filePath))
        {
            // Пропускаем заголовок
            reader.ReadLine();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');

                if (values.Length != 5)
                {
                    Console.WriteLine($"Некорректная строка: {line}");
                    continue;
                }

                try
                {
                    var product = new Product
                    {
                        Id = int.Parse(values[0]),
                        Description = values[1],
                        Price = decimal.Parse(values[2]),
                        QuantityInStock = int.Parse(values[3]),
                        IsWholesale = bool.Parse(values[4])
                    };

                    products.Add(product);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"Ошибка при обработке строки: {line}");
                    Console.WriteLine($"Сообщение об ошибке: {ex.Message}");
                }
            }
        }

        return products;
    }

    public static List<DiscountCard> LoadDiscountCardsFromFile(string filePath)
    {
        var discountCards = new List<DiscountCard>();

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Файл {filePath} не найден.");
            return discountCards;
        }

        using (var reader = new StreamReader(filePath))
        {
            // Пропускаем заголовок
            reader.ReadLine();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');

                if (values.Length != 3)
                {
                    Console.WriteLine($"Некорректная строка: {line}");
                    continue;
                }

                try
                {
                    var card = new DiscountCard
                    {
                        Id = int.Parse(values[0]),
                        Number = int.Parse(values[1]),
                        DiscountPercentage = decimal.Parse(values[2])
                    };

                    discountCards.Add(card);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"Ошибка при обработке строки: {line}");
                    Console.WriteLine($"Сообщение об ошибке: {ex.Message}");
                }
            }
        }

        return discountCards;
    }

    public static Receipt GenerateReceipt(List<Product> products, DiscountCard discountCard)
    {
        var receipt = new Receipt
        {
            Date = DateTime.Now,
            DiscountCard = discountCard
        };

        // Пример добавления товаров в чек
        receipt.Items.Add(new ReceiptItem
        {
            Product = products[0], // Milk
            Quantity = 5,
            Total = products[0].Price * 5,
            Discount = products[0].IsWholesale && 5 >= 5 ? (products[0].Price * 5) * 0.10m : 0
        });

        receipt.Items.Add(new ReceiptItem
        {
            Product = products[1], // Cream 400g
            Quantity = 2,
            Total = products[1].Price * 2,
            Discount = discountCard != null ? (products[1].Price * 2) * (discountCard.DiscountPercentage / 100) : 0
        });
        receipt.Items.Add(new ReceiptItem
        {
            Product = products[2], // Cream 400g
            Quantity = 2,
            Total = products[2].Price * 3,
            Discount = discountCard != null ? (products[2].Price * 2) * (discountCard.DiscountPercentage / 100) : 0
        });

        // Расчет итоговых значений
        receipt.TotalPrice = receipt.Items.Sum(item => item.Total);
        receipt.TotalDiscount = receipt.Items.Sum(item => item.Discount);
        receipt.TotalWithDiscount = receipt.TotalPrice - receipt.TotalDiscount;

        return receipt;
    }

    public static void SaveReceiptToCsv(Receipt receipt, string filePath)
    {
        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Date;Time;QTY;DESCRIPTION;PRICE;TOTAL;DISCOUNT;DISCOUNT CARD;DISCOUNT PERCENTAGE;TOTAL PRICE;TOTAL DISCOUNT;TOTAL WITH DISCOUNT");

            foreach (var item in receipt.Items)
            {
                writer.WriteLine($"{receipt.Date:dd.MM.yyyy};{receipt.Date:HH:mm:ss};{item.Quantity};{item.Product.Description};{item.Product.Price:F2}$;{item.Total:F2}$;{item.Discount:F2}$;{receipt.DiscountCard?.Number};{receipt.DiscountCard?.DiscountPercentage}%;{receipt.TotalPrice:F2}$;{receipt.TotalDiscount:F2}$;{receipt.TotalWithDiscount:F2}$");
            }
        }
    }
}