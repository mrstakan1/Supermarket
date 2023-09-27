using System;
using System.Collections.Generic;

namespace NewSupermarket
{
    internal class Program
    {
        const ConsoleKey ShowQueueCommand = ConsoleKey.Q;
        const ConsoleKey ServePersonCommand = ConsoleKey.W;
        const ConsoleKey ExitCommand = ConsoleKey.Escape;

        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            Shop shop = new Shop();

            bool isWorking = true;

            while (isWorking)
            {
                ShowCommands();

                ConsoleKeyInfo userInput = Console.ReadKey(true);

                switch (userInput.Key)
                {
                    case ShowQueueCommand:
                        shop.ShowQueue();
                        break;

                    case ServePersonCommand:
                        shop.ServeClient();
                        break;

                    case ExitCommand:
                        isWorking = false;
                        break;
                }

                Console.ReadKey();
                Console.Clear();
            }
        }

        static void ShowCommands()
        {
            Console.WriteLine($"{ShowQueueCommand} - показать очередь");
            Console.WriteLine($"{ServePersonCommand} - обслужить человека в очереди");
            Console.WriteLine($"{ExitCommand} - закрыть программу");
        }
    }

    class Client
    {
        private Random _random = new Random();
        private List<Item> _cart = new List<Item>();

        public Client(List<Item> cart)
        {
            CreateBalance();
            _cart = new List<Item>(cart);
        }

        public int Balance { get; private set; }

        public void PayOff()
        {
            int purchaseAmount = GetCartPrice();
            Console.WriteLine($"У клиента {Balance} денег, стоимость корзины - {purchaseAmount}");

            if (Balance > purchaseAmount)
            {
                Console.WriteLine("Клиент обслужен!");
            }
            else
            {
                UnloadCart();
            }
        }

        public void ShowCart()
        {
            Console.WriteLine($"Баланс: {Balance}");
            Console.WriteLine("Корзина:");

            foreach (var item in _cart)
            {
                item.ShowInfo();
            }
        }

        private void UnloadCart()
        {
            int purchaseAmount = GetCartPrice();

            while (purchaseAmount > Balance && _cart.Count > 0)
            {
                int productIndex = _random.Next(_cart.Count);

                Console.WriteLine($"Клиент убрал из корзины {_cart[productIndex].Amount} штук товара {_cart[productIndex].Product.Title}");
                _cart.Remove(_cart[productIndex]);

                purchaseAmount = GetCartPrice();
            }

            if (_cart.Count == 0)
            {
                Console.WriteLine("Клиенту не хватило денег и он ушел...\n");
            }
            else
            {
                Console.WriteLine("Клиент обслужен!");
            }
        }

        private int GetCartPrice()
        {
            int cartPrice = 0;

            for (int i = 0; i < _cart.Count; i++)
            {
                cartPrice += _cart[i].Amount * _cart[i].Product.Price;
            }

            return cartPrice;
        }

        private void CreateBalance()
        {
            int minBalance = 10;
            int maxBalance = 120;

            Balance = _random.Next(minBalance, maxBalance);
        }
    }

    class Shop
    {
        private Random _random = new Random();
        private List<Item> _stock = new List<Item>();
        private Queue<Client> _clients = new Queue<Client>();

        public Shop()
        {
            GenerateStock();
            ShowAssortment();

            Console.WriteLine("В магазин зашли люди и набрали товаров...\n");
            CreateQueue();
        }

        public void ServeClient()
        {
            if (_clients.Count > 0)
            {
                _clients.Dequeue().PayOff();
            }
            else
            {
                Console.WriteLine("Очередь пуста...\n");
            }
        }

        public void ShowQueue()
        {
            int clientNumber = 1;
            Console.WriteLine($"В очереди {_clients.Count} человек(a) и они взяли следующие товары:\n");

            foreach (var person in _clients)
            {
                Console.WriteLine($"Клиент {clientNumber} ");
                person.ShowCart();
                clientNumber++;
                Console.WriteLine();
            }

            Console.WriteLine();
        }

        private void ShowAssortment()
        {
            Console.WriteLine("Изначальное содержимое магазина:");

            foreach (var product in _stock)
            {
                product.ShowInfo();
            }

            Console.WriteLine();
        }

        private void CreateQueue()
        {
            int minClients = 1;
            int maxClients = 4;
            int amountOfClients = _random.Next(minClients, maxClients);

            for (int i = 0; i < amountOfClients; i++)
            {
                Client client = CreateCart();
                _clients.Enqueue(client);
            }
        }

        private Client CreateCart()
        {
            List<Item> cart = new List<Item>();

            int minBoughtProducts = 1;
            int maxBoughtProducts = _stock.Count;
            int boughtProducts = _random.Next(minBoughtProducts, maxBoughtProducts);

            for (int i = 0; i < boughtProducts; i++)
            {
                int productIndex = _random.Next(_stock.Count);
                Item item = _stock[productIndex];

                int amount = GetAmount(item);

                if (amount != 0)
                {
                    item.DecreaseAmount(amount);

                    cart.Add(new Item(item.Product, amount));
                }
            }

            return new Client(cart);
        }

        private int GetAmount(Item item)
        {
            int amount;
            int minAmountOfProduct = 1;
            int maxAmountOfProduct = item.Amount;

            if (item.Amount > 0)
            {
                do
                {
                    amount = _random.Next(minAmountOfProduct, maxAmountOfProduct);
                }
                while (amount > item.Amount);

                return amount;
            }
            else
            {
                return 0;
            }
        }

        private void GenerateStock()
        {
            List<string> _productTitles = new List<string> { "Помидор", "Огурец", "Яблоко", "Шоколад", "Вода", "Lipton" };
            int minAmount = 2;
            int maxAmount = 10;
            int minPrice = 5;
            int maxPrice = 25;

            for (int i = 0; i < _productTitles.Count; i++)
            {
                int amount = _random.Next(minAmount, maxAmount);
                int price = _random.Next(minPrice, maxPrice);
                _stock.Add(new Item(new Product(price, _productTitles[i]), amount));
            }
        }
    }

    class Product
    {
        public Product(int price, string title)
        {
            Price = price;
            Title = title;
        }

        public int Price { get; private set; }
        public string Title { get; private set; }
    }

    class Item
    {
        public Item(Product product, int amount)
        {
            Product = product;
            Amount = amount;
        }

        public Product Product { get; private set; }
        public int Amount { get; private set; }

        public void ShowInfo()
        {
            Console.Write($"Товар {Product.Title}, цена {Product.Price} | Наличие: {Amount} шт.\n");
        }

        public void DecreaseAmount(int soldAmount)
        {
            Amount -= soldAmount;
        }

        public void SetAmount(int amount)
        {
            Amount = amount;
        }
    }
}